using System.Xml.Linq;

namespace st_project_name;

/// <summary>
/// Contains main STM32CubeIDE project configuration.
/// </summary>
internal class CubeIDEProject : XmlConfigFile {

    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    public string Name {
        get => NameElement?.Value ?? string.Empty;
        set {
            if (NameElement != null) {
                NameElement.Value = value;
                IsChanged = true;
            }
        }
    }

    /// <summary>
    /// Gets or sets the project link name.
    /// </summary>
    public string LinkName {
        get => IocLinkNameElement?.Value ?? string.Empty;
        set {
            if (IocLinkNameElement != null) {
                IocLinkNameElement.Value = value;
                IsChanged = true;
            }
        }
    }

    /// <summary>
    /// Gets or sets the project link location URI.
    /// </summary>
    public string LinkLocationUri {
        get => IocLinkLocationUriElement?.Value ?? string.Empty;
        set {
            if (IocLinkLocationUriElement != null) {
                IocLinkLocationUriElement.Value = value;
                IsChanged = true;
            }
        }
    }

    /// <summary>
    /// Creates a STM32CubeIDE project metadata from a local file.
    /// </summary>
    /// <param name="path">Source path.</param>
    public CubeIDEProject(string path) : base(path) {
        NameElement = Root?.Element(NameKey);
        IocLinkElement = Root?.Element(LinkedResourcesKey)?
            .Elements(LinkKey).FirstOrDefault(e => e.Element(LocationUriKey)?.Value?.EndsWith(".ioc") ?? false);
        IocLinkNameElement = IocLinkElement?.Element(NameKey);
        IocLinkLocationUriElement = IocLinkElement?.Element(LocationUriKey);
    }

    private const string NameKey = "name";
    private const string LinkedResourcesKey = "linkedResources";
    private const string LinkKey = "link";
    private const string LocationUriKey = "locationURI";

    private readonly XElement? NameElement;
    private readonly XElement? IocLinkElement;
    private readonly XElement? IocLinkNameElement;
    private readonly XElement? IocLinkLocationUriElement;

}