using System.Xml.Linq;

namespace st_project_name;

internal class CubeIDEProject : XmlConfigFile {

    private const string NameKey = "name";
    private const string LinkedResourcesKey = "linkedResources";
    private const string LinkKey = "link";
    private const string LocationUriKey = "locationURI";

    public string Name {
        get => NameElement?.Value ?? string.Empty;
        set {
            if (NameElement != null) {
                NameElement.Value = value;
                IsChanged = true;
            }
        }
    }

    public string LinkName {
        get => IocLinkNameElement?.Value ?? string.Empty;
        set {
            if (IocLinkNameElement != null) {
                IocLinkNameElement.Value = value;
                IsChanged = true;
            }
        }
    }

    public string LinkLocationUri {
        get => IocLinkLocationUriElement?.Value ?? string.Empty;
        set {
            if (IocLinkLocationUriElement != null) {
                IocLinkLocationUriElement.Value = value;
                IsChanged = true;
            }
        }
    }

    public CubeIDEProject(string path) : base(path) {
        NameElement = Root.Element(NameKey);
        IocLinkElement = Root.Element(LinkedResourcesKey)?
            .Elements(LinkKey).FirstOrDefault(e => e.Element(LocationUriKey)?.Value?.EndsWith(".ioc") ?? false);
        IocLinkNameElement = IocLinkElement?.Element(NameKey);
        IocLinkLocationUriElement = IocLinkElement?.Element(LocationUriKey);
    }

    private readonly XElement? NameElement;
    private readonly XElement? IocLinkElement;
    private readonly XElement? IocLinkNameElement;
    private readonly XElement? IocLinkLocationUriElement;

}