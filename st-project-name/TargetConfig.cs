using Newtonsoft.Json.Linq;

namespace st_project_name;
internal class TargetConfig : JsonConfigFile {

    private const string TargetConfigurationKey = "target_configuration";
    private const string ProjectFileKey = "project_file";

    public string ProjectFile {
        get => Root[ProjectFileKey]?.Value<string>() ?? string.Empty;
        set {
            Root[ProjectFileKey] = value;
            IsChanged = true;
        }
    }

    public TargetConfig(string path) : base(path, TargetConfigurationKey) { }

}
