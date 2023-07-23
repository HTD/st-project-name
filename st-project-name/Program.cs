/// <summary>
/// This tool finds all project name references in STM32CubeIDE / TouchGFX project
/// and optionally replaces them with new name.
/// </summary>

using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

Directory.SetCurrentDirectory("D:\\Source\\Embedded\\Templates\\STM32H745I-DISCO");

const string cubeDirectory = ".\\STM32CubeIDE";
const string errorPrefix = "ERROR:";
const string warinngPrefix = "WARNING:";

// Find project files:

var projectPath = Directory.Exists(cubeDirectory) ? Directory.EnumerateFiles(".\\STM32CubeIDE", "*.project", SearchOption.TopDirectoryOnly).FirstOrDefault() : null;
var iocPath = Directory.EnumerateFiles(".", "*.ioc", SearchOption.TopDirectoryOnly).FirstOrDefault();

if (projectPath == null || iocPath == null) {
    Console.Error.WriteLine($"{errorPrefix} Project not found in current directory.");
    return 1;
}

var iocFileName = Path.GetFileNameWithoutExtension(iocPath);
var directoryName = new DirectoryInfo(Directory.GetCurrentDirectory()).Name;
var touchgfxPath = Directory.EnumerateFiles(".", "*.touchgfx", SearchOption.AllDirectories).FirstOrDefault();
var touchgfxFileName = touchgfxPath != null ? Path.GetFileNameWithoutExtension(touchgfxPath) : null;
var partPath = Directory.EnumerateFiles(".", "*.touchgfx.part", SearchOption.AllDirectories).FirstOrDefault();
var appConfigPath = Directory.EnumerateFiles(".", "application.config", SearchOption.AllDirectories).FirstOrDefault();
var targetConfigPath = Directory.EnumerateFiles(".", "target.config", SearchOption.AllDirectories).FirstOrDefault();

var iocFileNameMatches = iocFileName == directoryName;
var touchgfxProjectExists = touchgfxPath != null;
var touchgfxProjectNameMatches = touchgfxFileName == directoryName;

// Show project files summary:

Console.WriteLine($"Project directory name: \"{directoryName}\"");
Console.WriteLine($".project path: \"{projectPath}\"");
Console.WriteLine($".ioc path: \"{iocPath}\"");
if (touchgfxPath != null) Console.WriteLine($".touchgfx path: \"{touchgfxPath}\"");
if (partPath != null) Console.WriteLine($".touchgfx.part path: \"{partPath}\"");
if (appConfigPath != null) Console.WriteLine($"application.config path: \"{appConfigPath}\"");
if (targetConfigPath != null) Console.WriteLine($"target.config path: \"{targetConfigPath}\"");

if (!iocFileNameMatches) Console.Error.WriteLine(Environment.NewLine, $"{warinngPrefix} .ioc name doesn't match the project directory name.");
if (!touchgfxProjectExists) Console.Error.WriteLine(Environment.NewLine, $"{warinngPrefix} TouchGFX project NOT FOUND!");
else if (!touchgfxProjectNameMatches) Console.Error.WriteLine(Environment.NewLine, $"{warinngPrefix} TouchGFX project name doesn't match the project directory name.");
Console.WriteLine();

// Content analysis:

var projectXml = XDocument.Load(projectPath);
var projectNameElement = projectXml.Root?.Element("name");
var projectName = projectNameElement?.Value;
var projectLinks = projectXml.Root?.Element("linkedResources")?.Elements("link");
var iocLinkElement = projectLinks?.FirstOrDefault(e => e.Element("locationURI")?.Value?.EndsWith(".ioc") ?? false);
var iocLinkNameElement = iocLinkElement?.Element("name");
var iocLinkLocationUriElement = iocLinkElement?.Element("locationURI");

JToken? touchgfxApplicationNameProperty = null;
if (touchgfxPath != null) {
    var touchgfxProjectJson = JsonLoad(touchgfxPath);
    touchgfxApplicationNameProperty = touchgfxProjectJson["Application"]?["Name"];
}

// Content analysis summary:

if (projectName != null) {
    Console.WriteLine($"Current project name: \"{projectName}\"");
    if (projectName != directoryName) Console.Error.WriteLine($"{warinngPrefix} Current project name doesn't match the project directory name.");
    if (iocLinkElement != null) Console.WriteLine($".ioc link: \"{iocLinkNameElement?.Value}\" => \"{iocLinkLocationUriElement?.Value}\".");
    else Console.Error.WriteLine($"{warinngPrefix} .ioc link not found in the project file.");
}

if (touchgfxPath != null) {
    if (touchgfxApplicationNameProperty != null) Console.WriteLine($"TouchGFX application name: \"{touchgfxApplicationNameProperty.Value<string>()}\".");
}

static JObject JsonLoad(string path) {
    using StreamReader file = File.OpenText(path);
    using JsonTextReader json = new JsonTextReader(file);
    return JObject.Load(json);
}

return 0;