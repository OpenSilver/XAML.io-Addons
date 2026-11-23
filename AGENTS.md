# Instructions for AI agents

This repository holds add-ons for [XAML.io](https://xaml.io), the online IDE for OpenSilver apps. An add-on is a folder, `addons/<id>/`, of XML files, Markdown and images that put the controls of a NuGet package into the XAML.io designer. There is no code to build.

- [README.md](README.md): the workflow for authors.
- [AddonFormat.md](AddonFormat.md): every element and attribute. Read the sections of the files you write.
- [addons/controlskit/](addons/controlskit/): a complete add-on to imitate.

## Creating or updating an add-on

1. Work only in `addons/<id>/`. Do not edit `schemas/`, `AddonFormat.md`, `tools/`, `.github/` or other add-ons, and do not add an `index.xml`.
2. Collect the facts from the package, never from memory:
   - The latest version: `https://api.nuget.org/v3-flatcontainer/<id-in-lowercase>/index.json`. Download `https://api.nuget.org/v3-flatcontainer/<id>/<version>/<id>.<version>.nupkg` (lowercase): it is a zip.
   - It must be built for OpenSilver: its `.nuspec` depends on `OpenSilver`, directly or through another OpenSilver package. Packages for WPF, Avalonia or MAUI do not run in XAML.io: if so, stop and tell the user.
   - The DLLs under `lib/<framework>/` give the assembly names. The XML documentation file next to a DLL, when present, lists every public type (`T:`) and property (`P:`). Otherwise read the library's source (see its project URL) or decompile the DLL, for example with `ilspycmd`.
   - For each control: its namespace and assembly, the properties a user sets first, and their value types.
3. Write the files, starting from a copy of `addons/controlskit/`:
   - `addon.xml`: `Id` equal to the folder name; `Name`, `Publisher`, `Version` (of the add-on, start at `1.0`), `License`, `Website`, a one-sentence `Description`; `<Package>` with `VersionRange` and `RecommendedVersion`; `<Xmlns>`. Replace `Slugs="controlskit"` with the id, or remove it.
   - `gallery.xml`: one `<Section>` named after the library, with a short `Label` (it shows under the rail icon), `<Group>`s by kind of control, one `<Item>` per control.
   - `icons.xml`: an icon for the add-on, the section and each item.
   - Optional: `designer.xml` (toolbar buttons for the selected control), `skills/<name>/SKILL.md` (guidance for the XAML.io AI agent), `templates/`. Remove them and their elements in `addon.xml` unless the user asked for them.
4. Run `dotnet run tools/validate.cs` from the repository root and fix every problem until it prints `no problems found`.
5. In your summary, tell the user what you could not check: how each item renders, and the banner and screenshots, which only they can test in XAML.io (README, Step 3).

## Rules that are easy to get wrong

- Keep the first lines of each XML file, which point to `../../schemas/*.xsd`.
- Declare each namespace once, on `<Gallery>` (and on `<Designer>`), with the same prefix as in `<Xmlns>`.
- An `<Item>` holds exactly one XAML element. Write the snippet that a developer would write by hand: real property names only, sample content and colors so that it is visible when dropped, `DefaultWidth` and `DefaultHeight`. No `x:Name`, no event handlers, no `{StaticResource}` to resources of an app.
- Item names are unique in the add-on. To show an item in a second group, use `<ItemRef Name="..."/>`.
- Every `Icon="..."` names an icon of `icons.xml`. Icons are single-color SVGs with `viewBox="0 0 24 24"` and no `width` or `height`, colored with `fill` and `stroke` attributes (not `style`) and `fill="none"` for outlines: XAML.io repaints them with the theme color.
- Images (`Banner`, `<Screenshot>`) are PNG, JPEG or WebP under 500 KB. Do not invent or generate screenshots: leave them out and tell the user to add them.
- A skill's folder name, its `Name` in `addon.xml` and the `name:` of its `SKILL.md` frontmatter are equal, and `Files` lists every other file of the skill. Do not repeat the gallery snippets in a skill: XAML.io adds them.
- For a commercial library, add `<Pricing>` ([format](AddonFormat.md#pricing)), with the price and trial from the vendor's website.
