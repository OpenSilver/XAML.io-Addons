// Checks the add-ons under addons/: the XML files against the schemas, the id, the slugs, the files that
// addon.xml names, the icon names, the gallery items, the skill names and the size of the images.
// Run from the repository root (.NET 10 SDK or later):
//     dotnet run tools/validate.cs
// XAML.io checks more when it loads an add-on, and only it shows how the controls render, so also use
// "Test my add-on..." in its More... panel.

using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

const long MaxImageBytes = 500 * 1024;
string[] imageExtensions = [".png", ".jpg", ".jpeg", ".webp", ".gif"];

var root = Directory.GetCurrentDirectory();
var addonsRoot = Path.Combine(root, "addons");
var schemasRoot = Path.Combine(root, "schemas");
var problems = new List<string>();
var isGitHubActions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";
var slugOwners = new Dictionary<string, string>();

if (!Directory.Exists(addonsRoot))
{
    Console.Error.WriteLine("Run this from the root of the repository: no 'addons' folder here.");
    return 2;
}

var folders = Directory.GetDirectories(addonsRoot).OrderBy(f => f, StringComparer.Ordinal).ToList();
foreach (var folder in folders)
{
    var id = Path.GetFileName(folder);
    if (!Regex.IsMatch(id, "^[a-z0-9][a-z0-9-]{0,62}$"))
    {
        Report(folder, 0, $"'{id}' is not a valid add-on id: use lowercase letters, digits and dashes.");
    }

    var manifestPath = Path.Combine(folder, "addon.xml");
    if (!File.Exists(manifestPath))
    {
        Report(folder, 0, "addon.xml is missing.");
        continue;
    }

    foreach (var xml in Directory.GetFiles(folder, "*.xml"))
    {
        Validate(xml);
    }

    XElement manifest;
    try
    {
        manifest = XDocument.Load(manifestPath, LoadOptions.SetLineInfo).Root!;
    }
    catch (XmlException)
    {
        continue;
    }

    if ((string?)manifest.Attribute("Id") != id)
    {
        Report(manifestPath, LineOf(manifest), $"Id must be '{id}', the name of the add-on folder.");
    }

    foreach (var slug in Split((string?)manifest.Attribute("Slugs")))
    {
        if (!slugOwners.TryAdd(slug, id))
        {
            Report(manifestPath, LineOf(manifest), $"Slug '{slug}' is used by both the '{slugOwners[slug]}' and '{id}' add-ons: give each add-on its own.");
        }
    }

    var referenced = new List<(string Path, XElement Element)>();
    if ((string?)manifest.Attribute("Banner") is { } banner)
    {
        referenced.Add((banner, manifest));
    }

    referenced.AddRange(manifest.Descendants()
        .Where(e => e.Name.LocalName is "Screenshot" or "Template")
        .Select(e => ((string?)e.Attribute("File") ?? "", e)));

    foreach (var skill in manifest.Descendants().Where(e => e.Name.LocalName == "Skill"))
    {
        var skillFolder = $"skills/{(string?)skill.Attribute("Name")}/";
        referenced.Add((skillFolder + "SKILL.md", skill));
        referenced.AddRange(Split((string?)skill.Attribute("Files")).Select(f => (skillFolder + f, skill)));
    }

    foreach (var (path, element) in referenced.Where(r => !File.Exists(Path.Combine(folder, r.Path))))
    {
        Report(manifestPath, LineOf(element), $"'{path}' was not found in the add-on folder.");
    }

    foreach (var skill in manifest.Descendants().Where(e => e.Name.LocalName == "Skill"))
    {
        var name = (string?)skill.Attribute("Name");
        var skillPath = Path.Combine(folder, "skills", name ?? "", "SKILL.md");
        if (File.Exists(skillPath) && FrontmatterName(skillPath) != name)
        {
            Report(skillPath, 0, $"The frontmatter of SKILL.md must have 'name: {name}', the Name of its <Skill> in addon.xml.");
        }
    }

    var icons = CheckIcons(Path.Combine(folder, "icons.xml"));
    var iconUsers = new List<(string Path, XElement Element)> { (manifestPath, manifest) };
    if (TryLoad(Path.Combine(folder, "gallery.xml")) is { } gallery)
    {
        var galleryPath = Path.Combine(folder, "gallery.xml");
        CheckItems(galleryPath, gallery);
        var sections = gallery.Elements().Where(e => e.Name.LocalName == "Section").ToList();
        var items = sections.SelectMany(s => s.Elements()).Where(e => e.Name.LocalName == "Group")
            .SelectMany(g => g.Elements()).Where(e => e.Name.LocalName == "Item");
        iconUsers.AddRange(sections.Concat(items).Select(e => (galleryPath, e)));
    }

    if (TryLoad(Path.Combine(folder, "designer.xml")) is { } designer)
    {
        iconUsers.AddRange(designer.Descendants().Where(e => e.Name.LocalName is "Panel" or "Option" or "Preset")
            .Select(e => (Path.Combine(folder, "designer.xml"), e)));
    }

    foreach (var (path, element) in iconUsers)
    {
        if ((string?)element.Attribute("Icon") is { } icon && !icons.Contains(icon))
        {
            Report(path, LineOf(element), $"No icon named '{icon}' in icons.xml.");
        }
    }

    foreach (var image in Directory.GetFiles(folder, "*", SearchOption.AllDirectories)
        .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant())))
    {
        var size = new FileInfo(image).Length;
        if (size > MaxImageBytes)
        {
            Report(image, 0, $"Images must be under {MaxImageBytes / 1024} KB; this one is {size / 1024} KB.");
        }
    }
}

foreach (var problem in problems)
{
    Console.WriteLine(problem);
}

Console.WriteLine(problems.Count == 0
    ? $"Checked {folders.Count} add-on(s): no problems found."
    : $"Checked {folders.Count} add-on(s): {problems.Count} problem(s) found.");
return problems.Count == 0 ? 0 : 1;

void Validate(string path)
{
    var schema = Path.Combine(schemasRoot, Path.ChangeExtension(Path.GetFileName(path), ".xsd"));
    if (!File.Exists(schema))
    {
        Report(path, 0, "Unexpected file: the XML files of an add-on are addon.xml, gallery.xml, icons.xml and designer.xml.");
        return;
    }

    var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
    settings.Schemas.Add(null, schema);
    settings.ValidationEventHandler += (_, e) =>
    {
        if (e.Severity == XmlSeverityType.Error)
        {
            Report(path, e.Exception.LineNumber, e.Message);
        }
    };

    try
    {
        using var reader = XmlReader.Create(path, settings);
        while (reader.Read())
        {
        }
    }
    catch (XmlException e)
    {
        Report(path, e.LineNumber, e.Message);
    }
}

void Report(string path, int line, string message)
{
    var file = Path.GetRelativePath(root, path).Replace('\\', '/');
    problems.Add(isGitHubActions
        ? $"::error file={file},line={Math.Max(line, 1)}::{message.Replace("\r", "").Replace("\n", " ")}"
        : $"{file}{(line > 0 ? $"({line})" : "")}: error: {message}");
}

HashSet<string> CheckIcons(string path)
{
    var names = new HashSet<string>(StringComparer.Ordinal);
    if (TryLoad(path) is not { } icons)
        return names;

    foreach (var icon in icons.Elements().Where(e => e.Name.LocalName == "Icon"))
    {
        var name = (string?)icon.Attribute("Name") ?? "";
        if (!names.Add(name))
        {
            Report(path, LineOf(icon), $"Two icons are named '{name}'.");
        }

        if (!icon.Elements().Any(e => e.Name.LocalName == "svg"))
        {
            Report(path, LineOf(icon), $"The icon '{name}' has no <svg> element.");
        }
    }

    return names;
}

void CheckItems(string path, XElement gallery)
{
    var groups = gallery.Elements().Where(e => e.Name.LocalName == "Section")
        .SelectMany(s => s.Elements()).Where(e => e.Name.LocalName == "Group").ToList();
    var items = new HashSet<string>(StringComparer.Ordinal);
    foreach (var item in groups.SelectMany(g => g.Elements()).Where(e => e.Name.LocalName == "Item"))
    {
        var name = (string?)item.Attribute("Name") ?? "";
        if (!items.Add(name))
        {
            Report(path, LineOf(item), $"Two items are named '{name}': define it once and use <ItemRef Name=\"{name}\" /> in other groups.");
        }

        if (item.Elements().Count() != 1)
        {
            Report(path, LineOf(item), $"The item '{name}' must hold exactly one XAML element: the snippet inserted when it is dropped.");
        }
    }

    foreach (var reference in groups.SelectMany(g => g.Elements()).Where(e => e.Name.LocalName == "ItemRef"))
    {
        var name = (string?)reference.Attribute("Name") ?? "";
        if (!items.Contains(name))
        {
            Report(path, LineOf(reference), $"No item named '{name}' in this gallery.");
        }
    }
}

static XElement? TryLoad(string path)
{
    try
    {
        return File.Exists(path) ? XDocument.Load(path, LoadOptions.SetLineInfo).Root : null;
    }
    catch (XmlException)
    {
        return null;
    }
}

static string? FrontmatterName(string path)
{
    var lines = File.ReadLines(path).ToList();
    if (lines.Count == 0 || lines[0].Trim() != "---")
        return null;

    return lines.Skip(1).TakeWhile(l => l.Trim() != "---")
        .Select(l => Regex.Match(l, @"^name:\s*['""]?([^'""]*)['""]?\s*$"))
        .FirstOrDefault(m => m.Success)?.Groups[1].Value;
}

static int LineOf(XElement element) => ((IXmlLineInfo)element).LineNumber;

static IEnumerable<string> Split(string? list) =>
    (list ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
