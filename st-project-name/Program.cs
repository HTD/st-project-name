/// <summary>
/// This tool finds all project name references in STM32CubeIDE / TouchGFX project
/// and optionally replaces them with new name.
/// </summary>

using System.Text.RegularExpressions;
using st_project_name;

const string cubeDirectory = ".\\STM32CubeIDE";
const string errorPrefix = "ERROR:";
const string warningPrefix = "WARNING:";
const string ok = "OK.";

// Find project files:

var projectPath = Directory.Exists(cubeDirectory) ? Directory.EnumerateFiles(".\\STM32CubeIDE", "*.project", SearchOption.TopDirectoryOnly).FirstOrDefault() : null;
var iocPath = Directory.EnumerateFiles(".", "*.ioc", SearchOption.TopDirectoryOnly).FirstOrDefault();

if (projectPath == null || iocPath == null) {
    Console.Error.WriteLine($"{errorPrefix} Project not found in current directory.");
    return 1;
}

var touchGFXPath = Directory.EnumerateFiles(".", "*.touchgfx", SearchOption.AllDirectories).FirstOrDefault();
var partPath = Directory.EnumerateFiles(".", "*.touchgfx.part", SearchOption.AllDirectories).FirstOrDefault();
var targetConfigPath = Directory.EnumerateFiles(".", "target.config", SearchOption.AllDirectories).FirstOrDefault();

// Content analysis:

var iocFile = new IocFile(iocPath);
var cubeIDEProject = new CubeIDEProject(projectPath);
var linkLocationPath = new PathBuilder(cubeIDEProject.LinkLocationUri, ".*PROJECT_LOC$");
var touchGFXProject = touchGFXPath is null ? null : new TouchGFXProject(touchGFXPath);
var touchGFXProjectIocPath = touchGFXProject is null ? null : new PathBuilder(touchGFXProject.ProjectFile);
var touchGFXPart = partPath is null ? null : new TouchGFXProject(partPath);
var touchGFXPartIocPath = touchGFXPart is null ? null : new PathBuilder(touchGFXPart.ProjectFile);
var targetConfig = targetConfigPath is null ? null : new TargetConfig(targetConfigPath);
var touchGFXTargetIocPath = targetConfig is null ? null : new PathBuilder(targetConfig.ProjectFile);

if (args.Length == 1) {
    var newProjectName = args[0];
    if (!IsValidProjectName(newProjectName)) {
        Console.Error.WriteLine("ERROR: Valid project name expected!");
        return 2;
    }
    RenameProject(newProjectName);
} else {
    Console.WriteLine("PROJECT FILES SUMMARY:");
    Console.WriteLine();
    ShowProjectFilesSummary();
    Console.WriteLine();
    Console.WriteLine("PROJECT CONTENT SUMMARY:");
    Console.WriteLine();
    ShowContentSummary();
    CheckForIssues();
}

return 0;

void ShowProjectFilesSummary() {
    Console.WriteLine($".project path: \"{projectPath}\"");
    Console.WriteLine($".ioc path: \"{iocPath}\"");
    if (touchGFXPath != null) Console.WriteLine($".touchgfx path: \"{touchGFXPath}\"");
    if (partPath != null) Console.WriteLine($".touchgfx.part path: \"{partPath}\"");
    if (targetConfigPath != null) Console.WriteLine($"target.config path: \"{targetConfigPath}\"");
}

void ShowContentSummary() {
    if (iocFile is null || cubeIDEProject is null) return;
    Console.WriteLine($".ioc name: {iocFile.Name}");
    Console.WriteLine($"Project name: {cubeIDEProject.Name}");
    Console.WriteLine($"Project link name: {cubeIDEProject.LinkName}");
    Console.WriteLine($"Project link location: {cubeIDEProject.LinkLocationUri}");
    if (touchGFXProject != null) {
        Console.WriteLine($"TouchGFX application name: {touchGFXProject.Name}");
        Console.WriteLine($"TouchGFX project file: {touchGFXProject.ProjectFile}");
    }
    if (touchGFXPart != null) {
        Console.WriteLine($"TouchGFX .part application name: {touchGFXPart.Name}");
        Console.WriteLine($"TouchGFX .part project file: {touchGFXPart.ProjectFile}");
    }
    if (targetConfig != null) Console.WriteLine($"TouchGFX target config project file: {targetConfig.ProjectFile}");
}

void CheckForIssues() {
    if (cubeIDEProject is null || iocFile is null || linkLocationPath is null) return;
    var nameMatches = cubeIDEProject.Name == iocFile.Name;
    var linkNameMatches = cubeIDEProject.LinkName == iocFile.FileName;
    var linkLocationMatches = linkLocationPath.Target == iocFile.FileName;
    var touchGFXProjectNameMatches = touchGFXProject?.Name == iocFile.Name;
    var touchGFXProjectIocMatches = touchGFXProjectIocPath?.Target == iocFile.FileName;
    var touchGFXPartNameMatches = touchGFXPart?.Name == iocFile.Name;
    var touchGFXPartIocMatches = touchGFXPartIocPath?.Target == iocFile.FileName;
    var touchGFXTargetIocMatches = touchGFXTargetIocPath?.Target == iocFile.FileName;
    if (!nameMatches) Console.Error.WriteLine($"{warningPrefix} .ioc file name doesn't match the project name!");
    if (!linkNameMatches) Console.Error.WriteLine($"{warningPrefix} project link name doesn't match the .ioc file name!");
    if (!linkLocationMatches) Console.Error.WriteLine($"{warningPrefix} project link location doesn't match the .ioc file name!");
    if (touchGFXProject != null && !touchGFXProjectNameMatches) Console.Error.WriteLine($"{warningPrefix} TouchGFX main project name doesn't match .ioc name!");
    if (touchGFXProject != null && !touchGFXProjectIocMatches) Console.Error.WriteLine($"{warningPrefix} TouchGFX main project file doesn't match .ioc file name!");
    if (touchGFXPart != null && !touchGFXPartNameMatches) Console.Error.WriteLine($"{warningPrefix} TouchGFX part project name doesn't match .ioc name!");
    if (touchGFXPart != null && !touchGFXPartIocMatches) Console.Error.WriteLine($"{warningPrefix} TouchGFX part project file doesn't match .ioc file name!");
    if (targetConfig != null && !touchGFXTargetIocMatches) Console.Error.WriteLine($"{warningPrefix} TouchGFX target config project file doesn't match .ioc file name!");
}

bool IsValidProjectName(string projectName) => ValidProjectName().IsMatch(projectName);

void RenameProject(string newProjectName) {
    if (cubeIDEProject is null || linkLocationPath is null || iocFile is null) return;
    var newIoc = newProjectName + ".ioc";
    var newIocUri = linkLocationPath.RenameTarget(newProjectName).ToString();
    var newIocRel = touchGFXProjectIocPath?.RenameTarget(newProjectName).ToString();
    var isProjectChanged = false;
    if (newProjectName != iocFile.Name) {
        isProjectChanged = true;
        Console.Write("Renaming .ioc file...");
        iocFile.Rename(newProjectName);
        Console.WriteLine(ok);
    }
    if (newProjectName != cubeIDEProject.Name) {
        isProjectChanged = true;
        Console.Write("Changing the STM32CubeIDE project name...");
        cubeIDEProject.Name = newProjectName;
        Console.WriteLine(ok);

    }
    if (newIoc != cubeIDEProject.LinkName) {
        isProjectChanged = true;
        Console.Write("Changing the STM32CubeIDE project link name...");
        cubeIDEProject.LinkName = newIoc;
        Console.WriteLine(ok);
    }
    if (newIocUri != cubeIDEProject.LinkLocationUri) {
        isProjectChanged = true;
        Console.Write("Changing the STM32CubeIDE project link location URI...");
        cubeIDEProject.LinkLocationUri = newIocUri;
        Console.WriteLine(ok);
    }
    var isTouchGFXProjectChanged = false;
    var isTouchGFXPartChanged = false;
    var isTouchGFXTargetConfigChanged = false;
    if (newIocRel != null) {
        if (touchGFXProject != null) {
            if (newProjectName != touchGFXProject.Name) {
                isTouchGFXProjectChanged = true;
                Console.Write("Changing TouchGFX main project name...");
                touchGFXProject.Name = newProjectName;
                Console.WriteLine(ok);
            }
            if (newIocRel != touchGFXProject.ProjectFile) {
                isTouchGFXProjectChanged = true;
                Console.Write("Changing TouchGFX main project file...");
                touchGFXProject.ProjectFile = newIocRel;
                Console.WriteLine(ok);
            }
        }
        if (touchGFXPart != null) {
            if (newProjectName != touchGFXPart.Name) {
                isTouchGFXPartChanged = true;
                Console.Write("Changing TouchGFX part project name...");
                touchGFXPart.Name = newProjectName;
                Console.WriteLine(ok);
            }
            if (newIocRel != touchGFXPart.ProjectFile) {
                isTouchGFXPartChanged = true;
                Console.Write("Changing TouchGFX part project file...");
                touchGFXPart.ProjectFile = newIocRel;
                Console.WriteLine(ok);
            }
        }
        if (targetConfig != null) {
            if (newIocRel != targetConfig.ProjectFile) {
                isTouchGFXTargetConfigChanged = true;
                Console.Write("Changing TouchGFX target.config project file...");
                targetConfig.ProjectFile = newIocRel;
                Console.WriteLine(ok);
            }
        }
        if (isProjectChanged) {
            Console.Write("Saving changes in project file...");
            cubeIDEProject.Update();
            Console.WriteLine(ok);
        }
        if (touchGFXProject != null && isTouchGFXProjectChanged) {
            Console.Write("Saving changes in TouchGFX .project file...");
            touchGFXProject.Update();
            Console.WriteLine(ok);
        }
        if (touchGFXPart != null && isTouchGFXPartChanged) {
            Console.Write("Saving changes in TouchGFX .part file...");
            touchGFXPart.Update();
            Console.WriteLine(ok);
        }
        if (targetConfig != null && isTouchGFXTargetConfigChanged) {
            Console.Write("Saving changes in TouchGFX target.config file...");
            targetConfig.Update();
            Console.WriteLine(ok);
        }
        if (touchGFXPath != null) {
            var newTouchGFXPath = Path.Combine(Path.GetDirectoryName(touchGFXPath) ?? string.Empty, newProjectName + ".touchgfx");
            if (newTouchGFXPath != touchGFXPath) {
                Console.Write("Renaming .touchgfx file...");
                File.Move(touchGFXPath, newTouchGFXPath);
                Console.WriteLine(ok);
            }
        }
        if (partPath != null) {
            var newPartPath = Path.Combine(Path.GetDirectoryName(partPath) ?? string.Empty, newProjectName + ".touchgfx.part");
            if (newPartPath != partPath) {
                Console.Write("Renaming .touchgfx.part file...");
                File.Move(partPath, newPartPath);
                Console.WriteLine(ok);
            }
        }
        Console.WriteLine("DONE.");
    }
}

partial class Program {
    [GeneratedRegex("^[A-Za-z][0-9A-Za-z_]*$", RegexOptions.Compiled)]
    private static partial Regex ValidProjectName();
}