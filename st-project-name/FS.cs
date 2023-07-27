namespace st_project_name;

/// <summary>
/// Basic file system tools.
/// </summary>
internal static class FS {

    /// <summary>
    /// Gets a value indicating that the strting is likely to be a path string.
    /// </summary>
    /// <param name="value">Input string.</param>
    /// <returns>True if value contains a path separator.</returns>
    public static bool IsPath(string value) => value == "." || value == ".." || value.Contains('/') || value.Contains('\\');

    /// <summary>
    /// Gets the directory part of the path.
    /// </summary>
    /// <param name="path">Input path.</param>
    /// <returns>Directory name or "." if the path contains no directory part.</returns>
    public static string GetDirectory(string path) => Path.GetDirectoryName(path) ?? ".";

    /// <summary>
    /// Gets the file name part of the path.
    /// </summary>
    /// <param name="path">Input path.</param>
    /// <returns>File name (with extension).</returns>
    public static string GetFileName(string path) => Path.GetFileName(path);

    /// <summary>
    /// Gets the file name without the extension from the path.
    /// </summary>
    /// <param name="path">Input path.</param>
    /// <returns>File name without the extension.</returns>
    public static string GetName(string path) => Path.GetFileNameWithoutExtension(path);

    /// <summary>
    /// Renames a file.
    /// </summary>
    /// <param name="path">Existing file path.</param>
    /// <param name="newFileName">New file name.</param>
    /// <returns>New file path if successfull, null otherwise.</returns>
    public static string? Rename(string path, string newFileName) {
        var dir = GetDirectory(path);
        var newPath = Path.Combine(dir, newFileName);
        if (newPath == path || !File.Exists(path) || File.Exists(newPath)) return null;
        try {
            File.Move(path, newPath);
            return newPath;
        } catch {
            return null;
        }
    }

}