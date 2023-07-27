using Newtonsoft.Json.Linq;

namespace st_project_name;

/// <summary>
/// TouchGFX project configuration that allows changing relevant values.
/// </summary>
internal class TouchGFXProject : JsonConfigFile {

    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    public string Name {
        get => Root?[NameKey]?.Value<string>() ?? string.Empty;
        set {
            if (Root != null) {
                Root[NameKey] = value;
                IsChanged = true;
            }
        }
    }

    /// <summary>
    /// Gets or sets the project .ioc file location.
    /// </summary>
    public string ProjectFile {
        get => Root?[ProjectFileKey]?.Value<string>() ?? string.Empty;
        set {
            if (Root != null) {
                Root[ProjectFileKey] = value;
                IsChanged = true;
            }
        }
    }

    /// <summary>
    /// Creates new TouchGFX configuration.
    /// </summary>
    /// <param name="path">Source file path.</param>
    public TouchGFXProject(string path) : base(path, ApplicationKey) { }

    private const string ApplicationKey = "Application";
    private const string NameKey = "Name";
    private const string ProjectFileKey = "ProjectFile";

}