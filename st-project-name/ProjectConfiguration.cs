using System.Text.RegularExpressions;

namespace st_project_name;

/// <summary>
/// Contains STM32CubeIDE relevant project configuration data and allows performing the project rename operation.
/// </summary>
internal partial class ProjectConfiguration {

    /// <summary>
    /// Gets the device configuration data file metadata.
    /// </summary>
    public IocFile Ioc { get; }

    /// <summary>
    /// Gets the main project file metadata.
    /// </summary>
    public CubeIDEProject Project { get; }

    /// <summary>
    /// Gets the current project name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the list of changes made to the project.
    /// </summary>
    public List<string> Changes { get; } = new(16);

    /// <summary>
    /// Gets the list of issues detected in the project.
    /// </summary>
    public List<string> Issues { get; } = new(16);

    /// <summary>
    /// Gets the main TouchGFX project metadata.
    /// </summary>
    public TouchGFXProject TouchGFX { get; }

    /// <summary>
    /// Gets the TouchGFX .part project metadata.
    /// </summary>
    public TouchGFXProject TouchGFXPart { get; }

    /// <summary>
    /// Gets the TouchGFX target.config metadata.
    /// </summary>
    public TouchGFXTargetConfig TouchGFXTargetConfig { get; }

    /// <summary>
    /// Gets the path configuration for the current .ioc file path for STM32CubeIDE project.
    /// </summary>
    public PathConfiguration CubeIocPath { get; }

    /// <summary>
    /// Gets the path configuration for the current .ioc file path for the TouchGFX project.
    /// </summary>
    public PathConfiguration TouchGFXIocPath { get; }

    /// <summary>
    /// Creates the project configuration metadata.
    /// </summary>
    /// <param name="path">Project root directory.</param>
    public ProjectConfiguration(string path) {
        Files = new ProjectFiles(path);
        Ioc = new IocFile(Files.IOCFile);
        Project = new CubeIDEProject(Files.CubeIDEMain);
        Name = Project.Name;
        TouchGFX = new TouchGFXProject(Files.TouchGFXMain);
        TouchGFXPart = new TouchGFXProject(Files.TouchGFXPart);
        TouchGFXTargetConfig = new TouchGFXTargetConfig(Files.TouchGFXTargetConfig);
        CubeIocPath = new PathConfiguration(Project.LinkLocationUri, ".*PROJECT_LOC$");
        TouchGFXIocPath = new PathConfiguration(TouchGFX.ProjectFile);
        CheckIssues();
    }

    /// <summary>
    /// Checks for issues with the configuration data.
    /// </summary>
    private void CheckIssues() {
        var expectedIocPath = CubeIocPath.Copy().RenameTarget(Ioc.Name);
        var expectedIocPathString = expectedIocPath.ToString();
        if (Project.Exists) {
            if (Project.Name != Ioc.Name) Issues.Add("Project name doesn't match .ioc file name.");
            if (Project.LinkName != Ioc.FileName) Issues.Add("Project link name doesn't match .ioc file name.");
            if (Project.LinkLocationUri != expectedIocPathString) Issues.Add("Project link locationURI doesn't match .ioc file name.");
        }
        expectedIocPath = TouchGFXIocPath.Copy().RenameTarget(Ioc.Name);
        expectedIocPathString = expectedIocPath.ToString();
        if (TouchGFX.Exists) {
            if (TouchGFX.FileName != Ioc.Name + ".touchgfx") Issues.Add("TouchGFX project file name doesn't match .ioc file name.");
            if (TouchGFX.Name != Ioc.Name) Issues.Add("TouchGFX project Name doesn't match .ioc file name.");
            if (TouchGFX.ProjectFile != expectedIocPathString) Issues.Add("TouchGFX project ProjectFile doesn't match .ioc file name.");
        }
        if (TouchGFXPart.Exists) {
            if (TouchGFXPart.FileName != Ioc.Name + ".touchgfx.part") Issues.Add("TouchGFX project .part file name doesn't match .ioc file name.");
            if (TouchGFXPart.Name != Ioc.Name) Issues.Add("TouchGFX project .part Name doesn't match .ioc file name.");
            if (TouchGFXPart.ProjectFile != expectedIocPathString) Issues.Add("TouchGFX project .part ProjectFile doesn't match .ioc file name.");
        }
        if (TouchGFXTargetConfig.Exists) {
            if (TouchGFXPart.ProjectFile != expectedIocPathString) Issues.Add("TouchGFX target.config project_file doesn't match .ioc file name.");
        }
    }

    /// <summary>
    /// Renames the project.
    /// </summary>
    /// <param name="newName">New project name.</param>
    /// <returns>True if the changes were made to the project.</returns>
    /// <exception cref="ArgumentException">Thrown when the new project name is invalid.</exception>
    public bool Rename(string newName) {
        if (newName.Length < 1 || !ValidProjectName().IsMatch(newName)) throw new ArgumentException("Invalid project name.");
        var newIocFileName = newName + ".ioc";
        if (Ioc.Exists && Ioc.Rename(newIocFileName)) Changes.Add(".ioc file rename");
        var newIocPath = CubeIocPath.Copy().RenameTarget(newName);
        var newIocPathString = newIocPath.ToString();
        if (Project.Exists) {
            if (Project.Name != newName) {
                Project.Name = newName;
                Changes.Add("Project name");
            }
            if (Project.LinkName != newIocFileName) {
                Project.LinkName = newIocFileName;
                Changes.Add("Project link name");
            }
            if (CubeIocPath.ToString() != newIocPathString) {
                Project.LinkLocationUri = newIocPathString;
                Changes.Add("Project link locationURI");
            }
            Project.Update();
        }
        var newTouchGFXFileName = newName + ".touchgfx";
        newIocPath = TouchGFXIocPath.Copy().RenameTarget(newName);
        newIocPathString = newIocPath.ToString();
        if (TouchGFX.Exists) {
            if (TouchGFX.FileName != newTouchGFXFileName) {
                if (TouchGFX.Rename(newTouchGFXFileName)) Changes.Add("TouchGFX project file rename");
            }
            if (TouchGFX.Name != newName) {
                TouchGFX.Name = newName;
                Changes.Add("TouchGFX Application.Name");
            }
            if (TouchGFX.ProjectFile != newIocPathString) {
                TouchGFX.ProjectFile = newIocPathString;
                Changes.Add("TouchGFX Application.ProjectFile");
            }
            TouchGFX.Update();
        }
        var newTouchGFXPartFileName = newTouchGFXFileName + ".part";
        if (TouchGFXPart.Exists) {
            if (TouchGFXPart.FileName != newTouchGFXPartFileName) {
                if (TouchGFXPart.Rename(newTouchGFXPartFileName)) Changes.Add("TouchGFX project.part file rename");
            }
            if (TouchGFXPart.Name != newName) {
                TouchGFXPart.Name = newName;
                Changes.Add("TouchGFX project.part Application.Name");
            }
            if (TouchGFXPart.ProjectFile != newIocPathString) {
                TouchGFXPart.ProjectFile = newIocPathString;
                Changes.Add("TouchGFX project.part Application.ProjectFile");
            }
            TouchGFXPart.Update();
        }
        if (TouchGFXTargetConfig.Exists && TouchGFXTargetConfig.ProjectFile != newIocPathString) {
            TouchGFXTargetConfig.ProjectFile = newIocPathString;
            Changes.Add("TouchGFX target.config target_configuration.project_file");
            TouchGFXTargetConfig.Update();
        }
        return Changes.Count > 0;
    }

    private readonly ProjectFiles Files;

    [GeneratedRegex("^[A-Za-z][0-9A-Za-z_]*$", RegexOptions.Compiled)]
    private static partial Regex ValidProjectName();

}
