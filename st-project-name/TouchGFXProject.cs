using Newtonsoft.Json.Linq;

namespace st_project_name;

internal class TouchGFXProject : JsonConfigFile {

    private const string ApplicationKey = "Application";
    private const string NameKey = "Name";
    private const string ProjectFileKey = "ProjectFile";

    public string Name {
        get => Root[NameKey]?.Value<string>() ?? string.Empty;
        set {
            if (Root != null) {
                Root[NameKey] = value;
                IsChanged = true;
            }
        }
    }

    public string ProjectFile {
        get => Root?[ProjectFileKey]?.Value<string>() ?? string.Empty;
        set {
            if (Root != null) {
                Root[ProjectFileKey] = value;
                IsChanged = true;
            }
        }
    }

    public TouchGFXProject(string path) : base(path, ApplicationKey) { }

}
