namespace st_project_name;

/// <summary>
/// Contains paths to the relevant STM32CubeIDE project files.
/// </summary>
internal class ProjectFiles {

    /// <summary>
    /// Gets the .ioc file path.
    /// </summary>
    public string IOCFile { get; }

    /// <summary>
    /// Gets the main STM32CubeIDE project file path.
    /// </summary>
    public string CubeIDEMain { get; }

    /// <summary>
    /// Gets the STM32CubeIDE launch configuration path.
    /// </summary>
    public string LaunchConfiguration { get; }

    /// <summary>
    /// Gets the main TouchGFX project file path.
    /// </summary>
    public string TouchGFXMain { get; }

    /// <summary>
    /// Gets the TouchGFX .part file path.
    /// </summary>
    public string TouchGFXPart { get; }

    /// <summary>
    /// Gets the TouchGFX target.config file path.
    /// </summary>
    public string TouchGFXTargetConfig { get; }

    /// <summary>
    /// Discovers relevant project files.
    /// </summary>
    /// <param name="path">Project root directory.</param>
    public ProjectFiles(string path) {
        IOCFile = Directory.EnumerateFiles(path, "*.ioc", SearchOption.TopDirectoryOnly).FirstOrDefault(string.Empty);
        string cubePath = Path.Combine(path, STM32CubeIDEDirectory);
        CubeIDEMain = Directory.Exists(cubePath)
            ? Directory.EnumerateFiles(cubePath, "*.project", SearchOption.TopDirectoryOnly).FirstOrDefault(string.Empty)
            : string.Empty;
        LaunchConfiguration = Directory.Exists(cubePath)
            ? Directory.EnumerateFiles(cubePath, "*.launch", SearchOption.TopDirectoryOnly).FirstOrDefault(string.Empty)
            : string.Empty;
        TouchGFXMain = Directory.EnumerateFiles(path, "*.touchgfx", SearchOption.AllDirectories).FirstOrDefault(string.Empty);
        TouchGFXPart = Directory.EnumerateFiles(path, "*.touchgfx.part", SearchOption.AllDirectories).FirstOrDefault(string.Empty);
        TouchGFXTargetConfig = Directory.EnumerateFiles(path, "target.config", SearchOption.AllDirectories).FirstOrDefault(string.Empty);
    }

    private const string STM32CubeIDEDirectory = "STM32CubeIDE";

}
