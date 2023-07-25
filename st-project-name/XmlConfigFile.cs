using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace st_project_name;
internal class XmlConfigFile {

    public XmlConfigFile(string path) {
        Document = XDocument.Load(Path = path);
        Root = Document.Root ?? throw new InvalidDataException();
    }

    public void Update() {
        if (IsChanged) Document.Save(Path);
    }

    protected readonly string Path;
    protected readonly XDocument Document;
    protected readonly XElement Root;
    protected bool IsChanged;

}
