using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace st_project_name;

/// <summary>
/// Contains JSON project configuration data.
/// </summary>
internal class JsonConfigFile {

    /// <summary>
    /// Gets a value indicating that the source file exists.
    /// </summary>
    public bool Exists { get; }

    /// <summary>
    /// Gets the source file location.
    /// </summary>
    public string Path { get; private set; }

    /// <summary>
    /// Gets the source file name.
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    /// Creates JSON project configuration data from a local file.
    /// </summary>
    /// <param name="path">Source file path.</param>
    /// <param name="rootKey">The root element name.</param>
    public JsonConfigFile(string path, string rootKey) {
        Path = path;
        FileName = System.IO.Path.GetFileName(path);
        Exists = path.Length > 0 && File.Exists(Path);
        if (!Exists) return;
        Document = Load(Path);
        Root = (JObject?)Document[rootKey];
    }

    /// <summary>
    /// Renames the configuration file.
    /// </summary>
    /// <param name="newFileName">New file name (with extension).</param>
    /// <returns>True if the file was renamed successfully.</returns>
    public bool Rename(string newFileName) {
        if (!Exists || newFileName.Length < 1) return false;
        if (FS.Rename(Path, newFileName) is string newPath) {
            Path = newPath;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Saves the configuration changes if applicable.
    /// </summary>
    /// <returns>True if the file was updated successfully.</returns>
    public bool Update() {
        if (Document is null || Root is null || !IsChanged) return false;

        try {
            Save(Document, Path);
            return true;
        } catch {
            return false;
        }
    }

    /// <summary>
    /// Loads a JSON file.
    /// </summary>
    /// <param name="path">Source file path.</param>
    /// <returns>JSON object.</returns>
    private static JObject Load(string path) {
        using StreamReader streamReader = File.OpenText(path);
        using JsonTextReader jsonTextReader = new(streamReader);
        return JObject.Load(jsonTextReader);
    }

    /// <summary>
    /// Saves a JSON file.
    /// </summary>
    /// <param name="json">JSON object.</param>
    /// <param name="path">Target file path.</param>
    private static void Save(JObject json, string path) {
        using StreamWriter streamWriter = File.CreateText(path);
        using JsonTextWriter jsonTextWriter = new(streamWriter);
        jsonTextWriter.Formatting = Formatting.Indented;
        json.WriteTo(jsonTextWriter);
    }

    protected readonly JObject? Document;
    protected readonly JObject? Root;
    protected bool IsChanged;

}