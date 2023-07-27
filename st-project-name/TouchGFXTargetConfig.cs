using Newtonsoft.Json.Linq;

namespace st_project_name;

/// <summary>
/// TouchGFX project target configuration that allows changing the .ioc file location.
/// </summary>
internal class TouchGFXTargetConfig : JsonConfigFile {

    /// <summary>
    /// Gets or sets the .ioc file location.
    /// </summary>
    public string ProjectFile {
        get => Root?[ProjectFileKey]?.Value<string>() ?? string.Empty;
        set {
            if (Root is null) return;
            Root[ProjectFileKey] = value;
            IsChanged = true;
        }
    }

    /// <summary>
    /// Creates new TouchGFX target configuration.
    /// </summary>
    /// <param name="path">Source file path.</param>
    public TouchGFXTargetConfig(string path) : base(path, TargetConfigurationKey) { }

    private const string TargetConfigurationKey = "target_configuration";
    private const string ProjectFileKey = "project_file";

}