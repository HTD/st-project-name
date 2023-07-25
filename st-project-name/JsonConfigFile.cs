using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace st_project_name;

internal class JsonConfigFile {

    public JsonConfigFile(string path, string rootKey) {
        Document = Load(Path = path);
        Root = (JObject)(Document[rootKey] ?? throw new InvalidDataException());
    }

    public void Update() {
        if (Root != null && IsChanged) Save(Document, Path);
    }

    private static JObject Load(string path) {
        using StreamReader streamReader = File.OpenText(path);
        using JsonTextReader jsonTextReader = new JsonTextReader(streamReader);
        return JObject.Load(jsonTextReader);
    }

    private static void Save(JObject json, string path) {
        using StreamWriter streamWriter = File.CreateText(path);
        using JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter);
        jsonTextWriter.Formatting = Formatting.Indented;
        json.WriteTo(jsonTextWriter);
    }

    protected readonly string Path;
    protected readonly JObject Document;
    protected readonly JObject Root;
    protected bool IsChanged;

}