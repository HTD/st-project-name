namespace st_project_name;

/// <summary>
/// Contains the properties of the STM32CubeIDE .ioc file.
/// </summary>
internal class IocFile {

    /// <summary>
    /// Gets a value indicating the file exists.
    /// </summary>
    public bool Exists { get; }

    /// <summary>
    /// Gets the path to the .ioc file.
    /// </summary>
    public string Path { get; private set; }

    /// <summary>
    /// Gets the name (without the extension) of the .ioc file.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the file name of the .ioc file.
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    /// Creates the .ioc file metadata.
    /// </summary>
    /// <param name="path">A path to the STM32CubeIDE .ioc file.</param>
    public IocFile(string path) {
        Exists = path.Length > 0 && File.Exists(path);
        if (Exists) {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(Path);
            FileName = System.IO.Path.GetFileName(Path);
        } else {
            Path = string.Empty;
            Name = string.Empty;
            FileName = string.Empty;
        }
    }

    /// <summary>
    /// Renames the .ioc file.
    /// </summary>
    /// <param name="newName">New name without the extension.</param>
    /// <returns>True if the file was renamed successfully.</returns>
    public bool Rename(string newFileName) {
        if (!Exists) return false;
        if (FS.Rename(Path, newFileName) is string newPath) {
            Path = newPath;
            Name = FS.GetName(Path);
            FileName = FS.GetFileName(Path);
            return true;
        }
        return false;
    }

}