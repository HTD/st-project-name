using System.Xml.Linq;

namespace st_project_name;

/// <summary>
/// Contains STM32CubeIDE launch configuration.
/// </summary>
internal class LaunchConfiguration : XmlConfigFile {

    /// <summary>
    /// Gets or sets the build artifact file name.
    /// </summary>
    public string ProgramName {
        get => ProgramNameElement?.Attribute(ValueName)?.Value ?? string.Empty;
        set {
            if (ProgramNameElement is null) return;
            ProgramNameElement.SetAttributeValue(ValueName, value);
            IsChanged = true;
        }
    }

    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    public string ProjectAttr {
        get => ProjectAttrElement?.Attribute(ValueName)?.Value ?? string.Empty;
        set {
            if (ProjectAttrElement is null) return;
            ProjectAttrElement.SetAttributeValue(ValueName, value);
            IsChanged = true;
        }
    }

    /// <summary>
    /// Creates a STM32CubeIDE launch configuration metadata from a local file.
    /// </summary>
    /// <param name="path">Source path.</param>
    public LaunchConfiguration(string path) : base(path) {
        ProgramNameElement = Root?.Elements(StringAttributeName)
            .FirstOrDefault(e => e.Attribute(KeyName)?.Value.EndsWith(ProgramNameKey) == true);
        ProjectAttrElement = Root?.Elements(StringAttributeName)
            .FirstOrDefault(e => e.Attribute(KeyName)?.Value.EndsWith(ProjectAttrKey) == true);
    }

    private const string StringAttributeName = "stringAttribute";
    private const string KeyName = "key";
    private const string ValueName = "value";
    private const string ProgramNameKey = "PROGRAM_NAME";
    private const string ProjectAttrKey = "PROJECT_ATTR";

    private readonly XElement? ProgramNameElement;
    private readonly XElement? ProjectAttrElement;

}