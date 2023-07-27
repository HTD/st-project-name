using System.Reflection;
using Woof;
using Woof.ConsoleTools;
using Woof.Shell;

namespace st_project_name;

/// <summary>
/// Command line interface for the tool.
/// </summary>
internal static class CLI {

    const int E_OK = 0;
    const int E_SYNTAX_ERROR = 1;
    const int E_NO_ROOT_DIR = 2;
    const int E_NO_PROJECT_FILE = 3;
    const int E_NO_IOC_FILE = 4;
    const int E_INVALID_NAME = 5;

    /// <summary>
    /// Main entry point for the command.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Exit code.</returns>
    public static int Main(string[] args) {
        ConsoleEx.Init();
        ConsoleEx.BulletIndentation = 2;
        var commandLine = CommandLine.Default;
        commandLine.ParametersMin = 0;
        commandLine.ParametersMax = 2;
        commandLine.Map<Options>();
        commandLine.Delegates.Add(Options.Help, Help);
        commandLine.Delegates.Add(Options.Commit, Commit);
        commandLine.Parse(args);
        var errors = CommandLine.ValidationErrors;
        if (errors != null) return SyntaxError(errors);
        commandLine.RunDelegates();
        if (!commandLine.HasOption(Options.Help)) {
            LoadResult = Load();
            return LoadResult != 0
                ? LoadResult.Value
                : NewName is null ? Analysis() : Rename();
        }
        return 0;
    }

    /// <summary>
    /// Displays automatically generated help text.
    /// </summary>
    private static void Help() => Console.WriteLine(CommandLine.Help);

    /// <summary>
    /// Loads the project data.
    /// </summary>
    /// <returns>Exit code.</returns>
    private static int Load() {
        if (LoadResult != null) return LoadResult.Value;
        var commandLine = CommandLine.Default;
        switch (commandLine.Parameters.Count) {
            case 0:
                break;
            case 1:
                if (FS.IsPath(commandLine.Parameters[0])) ProjectRoot = commandLine.Parameters[0];
                else NewName = commandLine.Parameters[0];
                break;
            case 2:
                ProjectRoot = commandLine.Parameters[0];
                NewName = commandLine.Parameters[1];
                break;
        }
        if (!Directory.Exists(ProjectRoot)) {
            ConsoleEx.Error("The specified project root directory doesn't exist!");
            return E_NO_ROOT_DIR;
        }
        Project = new ProjectConfiguration(ProjectRoot);
        if (!Project.Project.Exists) {
            ConsoleEx.Error("The main project file not found.");
            return E_NO_PROJECT_FILE;
        }
        if (!Project.Ioc.Exists) {
            ConsoleEx.Error("The project .ioc file not found.");
            return E_NO_IOC_FILE;
        }
        return E_OK;
    }

    /// <summary>
    /// Tries to commit the current changes to Git repo if applicable.
    /// </summary>
    /// <param name="message">Optional commit message.</param>
    /// <returns>Exit code.</returns>
    static void Commit(string? message) {
        LoadResult = Load();
        if (LoadResult != E_OK) return;
        var cwd = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(ProjectRoot);
        if (message is null) message = "st-project-name run.";
        var cursor = new Cursor();
        try {
            var gitVersion = new ShellCommand("git -v").Exec();
            Console.WriteLine(gitVersion);
            var isRepo = Directory.Exists(".git");
            if (isRepo) Console.WriteLine("Git repository found.");
            else {
                cursor = ConsoleEx.Start("Initializing Git repository...");
                new ShellCommand("git init").ExecAndForget();
                ConsoleEx.Complete(cursor, true);
                const string gitignore = ".gitignore";
                if (!File.Exists(gitignore)) {
                    cursor = ConsoleEx.Start("Creating default .gitignore file...");
                    try {
                        var assembly = Assembly.GetExecutingAssembly();
                        var defaultGitIgnoreManifestName = assembly.GetManifestResourceNames().First(n => n.EndsWith(".gitignore"));
                        using var defaultGitIgnoreStream = assembly.GetManifestResourceStream(defaultGitIgnoreManifestName);
                        using var targetStream = new FileStream(gitignore, FileMode.Create, FileAccess.Write, FileShare.Read);
                        defaultGitIgnoreStream?.CopyTo(targetStream);
                        targetStream.Close();
                        ConsoleEx.Complete(cursor, true);
                    }
                    catch {
                        ConsoleEx.Complete(cursor, false);
                    }
                }
                cursor = ConsoleEx.Start("Adding files...");
                new ShellCommand("git add .").ExecAndForget();
                ConsoleEx.Complete(cursor, true);
                cursor = ConsoleEx.Start("Committing changes...");
                new ShellCommand($"git commit -m \"{message}\"").ExecAndForget();
                ConsoleEx.Complete(cursor, true);
            }
            var gitStatus = new ShellCommand("git status").Exec();
            var isClean = gitStatus.Contains("nothing to commit");
            var isUntracked = gitStatus.Contains("Untracked files");
            if (isUntracked) {
                cursor = ConsoleEx.Start("Adding untracked files...");
                new ShellCommand("git add .").ExecAndForget();
                ConsoleEx.Complete(cursor, true);
            }
            if (isClean) {
                Console.WriteLine("No changes to commit.");
                return;
            }
            cursor = ConsoleEx.Start("Committing changes...");
            new ShellCommand($"git commit -m \"{message}\"").ExecAndForget();
            ConsoleEx.Complete(cursor, true);
        }
        catch {
            ConsoleEx.Error("Git not found.");
        }
        finally {
            Directory.SetCurrentDirectory(cwd);
        }
    }

    /// <summary>
    /// Performs the project name analysis and reports the issues.
    /// </summary>
    /// <returns>Exit code.</returns>
    private static int Analysis() {
        if (Project is null) return E_NO_PROJECT_FILE;
        if (!Project.Ioc.Exists) return E_NO_IOC_FILE;
        Console.WriteLine($"Project name: `0f0`{Project.Project.Name}`");
        if (Project.TouchGFX.Exists) Console.WriteLine("`fff`TouchGFX` project present.");
        if (Project.Issues.Count < 1) {
            Console.WriteLine("`070`No project name / .ioc file issues.`");
        }
        else {
            Console.WriteLine("Project name issues:");
            foreach (var issue in Project.Issues) ConsoleEx.Item(issue);
        }
        return E_OK;
    }

    /// <summary>
    /// Renames the project.
    /// </summary>
    /// <returns>Exit code.</returns>
    private static int Rename() {
        if (Project is null) return E_NO_PROJECT_FILE;
        if (!Project.Ioc.Exists) return E_NO_IOC_FILE;
        if (NewName is null) return E_INVALID_NAME;
        Console.WriteLine("Renaming project...");
        try {
            Project.Rename(NewName);
        }
        catch (ArgumentException)  {
            ConsoleEx.Error("Invalid project name!");
            return E_INVALID_NAME;
        }
        if (Project.Changes.Count < 1) {
            Console.WriteLine("No changes made. Seems like already renamed.");
        }
        else {
            Console.WriteLine("Changes made:");
            foreach (var change in Project.Changes) ConsoleEx.Item(change);
        }
        return E_OK;
    }

    /// <summary>
    /// Displays the errors and usage in case of invalid command line syntax.
    /// </summary>
    /// <param name="errorText">Command line parsing errors text.</param>
    /// <returns>Exit code.</returns>
    static int SyntaxError(string? errorText) {
        Console.WriteLine(errorText);
        Console.WriteLine(CommandLine.Usage);
        return E_SYNTAX_ERROR;
    }

    /// <summary>
    /// Command line options.
    /// </summary>
    [Usage("{command} <--help|--commit> <project_root_path> <new_project_name>")]
    enum Options {

        [Option("?|h|help", null, "Displays this help message.")]
        Help,

        [Option("c|commit", "message", "Commits current changes to Git repo, initializes a new repo if not present and Git available.")]
        Commit

    }

    private static int? LoadResult;
    private static string ProjectRoot = ".";
    private static ProjectConfiguration? Project;
    private static string? NewName;

}