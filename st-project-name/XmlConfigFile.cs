using System.Xml.Linq;

namespace st_project_name;

/// <summary>
/// Contains XML project configuration data.
/// </summary>
internal class XmlConfigFile {

    /// <summary>
    /// Gets a value indicating that the source file exists.
    /// </summary>
    public bool Exists { get; }

    /// <summary>
    /// Creates XML project configuration data from a local file.
    /// </summary>
    /// <param name="path">Source file path.</param>
    public XmlConfigFile(string path) {
        Path = path;
        Exists = Path.Length > 0 && File.Exists(Path);
        if (!Exists) return;
        Document = XDocument.Load(Path = path);
        Root = Document.Root;
    }

    /// <summary>
    /// Saves the configuration changes if applicable.
    /// </summary>
    /// <returns>True if the file was updated successfully.</returns>
    public bool Update() {
        if (!Exists || !IsChanged || Document is null || Root is null) return false;
        try {
            Document.Save(Path);
            return true;
        } catch {
            return false;
        }
    }

    protected readonly string Path;
    protected readonly XDocument? Document;
    protected readonly XElement? Root;
    protected bool IsChanged;

}
