# Add-on file format

All files are UTF-8 XML. The loader matches the add-on elements and attributes by local name; `gallery.xml` and `designer.xml` declare the XAML presentation namespace as their default namespace so that the snippets they contain need no `xmlns`. The loader reports problems as `file(line): error|warning: message` in the browser console; an error skips the offending element (or the whole add-on when the manifest is unusable), and a warning falls back to the default value. Unknown attributes and elements are warnings, so typos are visible but do not break the gallery.

Paths below are relative to the `addons/` folder that holds the add-on folders. The examples that use ControlsKit are illustrative, not copies of the shipped files.

<!-- Also published verbatim in the XAML.io-Addons repository (scripts/addons/export-to-addons-repo.ps1): keep it free of links into this repository. -->


Each file has a schema (`addon.xsd`, `gallery.xsd`, `icons.xsd`, `designer.xsd`, and `index.xsd` in XAML.io), served at `https://xaml.io/schemas/<name>.xsd`. Reference it from the root element for completion and validation in the editor; the loader ignores `xsi:` attributes:

```xml
<Addon xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
       xsi:noNamespaceSchemaLocation="https://xaml.io/schemas/addon.xsd" Id="...">

<Gallery xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
         xsi:schemaLocation="http://schemas.microsoft.com/winfx/2006/xaml/presentation https://xaml.io/schemas/gallery.xsd">
```

`gallery.xsd` and `designer.xsd` target the XAML presentation namespace, because those files use it as their default namespace for the snippets. The schemas check structure, names and enumerations; the loader's checks (icons resolve, unique names, snippets parse) remain the reference. Relative paths work offline: `../../schemas/addon.xsd` from `addons/<id>/`, the same in XAML.io and in the XAML.io-Addons repository, which both have `schemas/` next to `addons/`.

## `index.xml`

Maintained by the XAML.io team in XAML.io; add-on authors do not write one. Lists the add-ons to load. Each `Id` is also the folder name, and `Files` lists the optional files the folder has (`gallery.xml`, `icons.xml`, `designer.xml`, separated by spaces).

```xml
<Addons>
  <Addon Id="opensilver" Files="gallery.xml icons.xml" />
  <Addon Id="componentone" Files="gallery.xml icons.xml designer.xml" />
  <Addon Id="controlskit" Files="gallery.xml icons.xml designer.xml" />
</Addons>
```

The add-ons panel lists the add-ons in this order. The IDE requests `addon.xml` and only the listed files, because the browser console reports a 404 for every missing file it requests, even though a missing optional file is not an error. Without `Files`, it tries all three.

A unit test fails if a folder is missing from the index or the other way round, or if `Files` doesn't match the folder's content.

## `<id>/addon.xml`

```xml
<Addon
    Id="controlskit"
    Name="ControlsKit"
    Publisher="Userware"
    Version="1.0"
    License="MIT"
    Icon="controlskit"
    Banner="assets/Banner.webp"
    Website="https://github.com/OpenSilver/OpenSilver.ControlsKit"
    Activation="Default"
    RailOrder="10"
    Slugs="controlskit"
    Description="One-line summary shown in listings."
    Highlights="Includes flexbox and masonry layouts and an animated navigation bar.">

  <Packages>
    <Package Id="OpenSilver.ControlsKit" RecommendedVersion="3.4.0-preview-2026-10-28-163447-eb332e2" Activation="Package" />
  </Packages>

  <Xmlns Prefix="controlskit" Uri="clr-namespace:OpenSilver.ControlsKit;assembly=OpenSilver.ControlsKit.Controls" />

  <Screenshots>
    <Screenshot File="assets/FlexPanel.png" />
  </Screenshots>

  <Templates>
    <Template Id="showcase" Name="ControlsKit showcase" File="templates/showcase.zip" StartFile="MainWindow.xaml"
              Description="Shown in tooltips and listings." />
  </Templates>

  <Skills>
    <Skill Name="controlskit" Files="reference/panels.md, reference/controls.md" />
  </Skills>
</Addon>
```

| Attribute / element | Required | Meaning |
|---|---|---|
| `Id` | Yes | Lowercase letters, digits and dashes, at most 63 characters; must equal the folder name exactly, otherwise the add-on is not loaded. |
| `Name` | No (defaults to `Id`) | Display name. |
| `PlaygroundName` | No (defaults to `Name`) | A longer name for the playground's banner ("You're in the {PlaygroundName} Playground") and welcome ("Welcome to the {PlaygroundName} Playground"), such as `ComponentOne for OpenSilver`. |
| `Publisher`, `Version`, `License`, `Website`, `Description` | No | Shown in the add-ons panel. `Website` must be an absolute `http` or `https` URL. `License` is an SPDX expression such as `MIT` or `MIT OR Apache-2.0`, or free text such as `Commercial`. |
| `Highlights` | No | A short line naming a few notable items ("Includes ..."), shown after the description in the add-on's details. Cards show only `Description`, so keep it to one sentence and the item lists here. The AI agent gets both texts. |
| `Icon` | No | Name of an icon in `icons.xml`. |
| `Banner` | No | Image at the top of the add-on's card and details, under the add-on folder (same rules as `<Screenshot File>`). Use 2:1 (e.g. 800x400), dark tones and no text (the name is shown under it), ideally under 100 KB; the card crops it slightly. Without it, the card shows the icon on a purple gradient. |
| `Activation` | No | `Default` (enabled by packages or explicitly) or `Always` (enabled in every project). |
| `RailOrder` | No | Integer; lower values come first among add-ons that are not `Always`. |
| `PreviewFlag` | No | Set by the XAML.io team. A query parameter name, such as `EnableC1Preview`, for an add-on not released yet: the built-in add-on is left out of the whole IDE (add-ons panel, rail, search, designer, AI agent, `xaml.io/<slug>` links) unless the page is opened with `?EnableC1Preview=true`. The flag is read from the address the page was opened with, so it also works with links such as `xaml.io/componentone?EnableC1Preview=true`. Remove the attribute to release the add-on; no rebuild is needed. Imported (local) add-ons ignore it. |
| `<Package>` | No | NuGet package linked to the add-on. `Activation="Package"` (default) enables the add-on when the project references the package, and enabling the add-on adds the package at `RecommendedVersion`. Without `RecommendedVersion`, enabling adds an `<XamlioAddon>` item instead of the package (warning). `Explicit` lists the package with the add-on only: referencing it does not enable the add-on, and enabling the add-on does not add it. |
| `<Xmlns>` | No | Preferred prefix for a namespace used by the add-on. Prefixes are validated today and will be used by C# snippets and the AI agent. |
| `<Screenshot File>` | No | Image under the add-on folder (`.png`, `.jpg`, `.jpeg` or `.webp`, animated allowed), as a relative path with forward slashes and at most 4 segments. Keep each file small: they are downloaded when the add-ons panel shows the add-on. |
| `Slugs` | No | Comma-separated vanity paths, so `xaml.io/<slug>` opens the add-on like `xaml.io/a/<id>`. Lowercase letters, digits and dashes; paths the IDE uses (`a`, `s`, `addons`, `schemas`, `launchers` and its other folders) are rejected. A slug claimed by two add-ons stays with the first one in `index.xml`. |
| `<Template>` | No | Starter solution opened by add-on links. `Id` and `File` (a `.zip` under the add-on folder) are required; `StartFile` is the project-relative file opened first (default: the main page). The first template is the default. See [Templates](#templates). |
| `<Skill>` | No | AI agent skill in `skills/<Name>/`. `Files` lists the files besides `SKILL.md`. See [Skills](#skills). |
| `<Pricing>` | No | For commercial libraries; omit it for free add-ons. See [Pricing](#pricing). |

### Templates

A template is a solution zip in the layout XAML.io stores: one folder per project with its `.csproj`, plus the `.sln`. The easiest way to make one is to build the project in XAML.io and use *Download as zip*; the `.Browser` and `.Simulator` host projects it contains are ignored. Keep only the packages the template needs beyond the defaults: the built-in OpenSilver packages are added when the template opens, at the IDE's current versions.

The add-on is enabled in the new project even if the template does not reference its package, but referencing it keeps the template usable on its own.

### Skills

A skill teaches the AI agent how to use the add-on's library. It uses the [Agent Skills](https://agentskills.io/specification) layout:

```
controlskit/skills/controlskit/
  SKILL.md                  frontmatter (name, description) + instructions; always loaded
  reference/panels.md       loaded by the agent when SKILL.md points to it
  reference/controls.md
```

```markdown
---
name: controlskit
description: OpenSilver.ControlsKit controls (FlexPanel, ...). Use when the page uses or should use them.
---

# ControlsKit
...see [reference/panels.md](reference/panels.md)...
```

- `Name`: lowercase letters, digits and dashes, starting with a letter or digit, at most 63 characters. It must equal the frontmatter `name` and be unique across add-ons (the first add-on in `index.xml` keeps it). The names of the agent's own skills (`opensilver`, `csharp`, `neon`) are reserved.
- `Files`: comma-separated paths relative to the skill folder, forward slashes, `.md` only, at most 39 (40 with `SKILL.md`, which is implicit). The server does not list folders, so a file that is not declared is never downloaded.
- Markdown files must be UTF-8 without a BOM (a BOM is stripped when loading). Keep skills small: the agent accepts at most 8 skills per run and 512 KB for all of them together, and drops the skills that do not fit.
- XAML.io generates `gallery.md` from the add-on's `Xamlio` gallery snippets, adds it to the first skill (when that skill lists fewer than 39 files and none of that name), and links it at the end of `SKILL.md`. An add-on with a gallery and no skill gets a generated skill named after its id with just that file. So do not repeat the gallery snippets in the skill.
- Write it for a model that knows OpenSilver/WPF but not this library: the package and `xmlns` to use, which control fits which need, property names and value types that differ from WPF, and pitfalls. Check every claim against the library source.
- Describe the library, not XAML.io or the agent's environment: no file paths, project names, package feeds or package versions. XAML.io tells the agent which project to change and which version to add (`RecommendedVersion`), and these can change without the add-on.

### Pricing

```xml
<Pricing Model="Paid" Price="From $495 per developer per year" TrialDays="30"
         PurchaseUrl="https://example.com/buy" TrialUrl="https://example.com/trial">
  Without a license key, the controls show a watermark.
</Pricing>
```

XAML.io sells nothing and checks no license: the library enforces its own licensing, so it behaves the same in XAML.io and in Visual Studio. Say in the text of the element how the library enforces it (trial watermark, key in code, and so on).

| Attribute | Meaning |
|---|---|
| `Model` | `Paid` (default): a license is needed. Enabling asks for confirmation with the price and the note, in the panel and when dropping an item (not when an add-on link enables it), and the AI agent enables the add-on only when the user asked for it by name or confirmed. `Freemium`: free with paid features; the pricing is shown without confirmation. `Free`: shown as free. |
| `Price` | Display text, such as "From $495 per developer per year". |
| `TrialDays` | Positive number of days of the free trial. |
| `PurchaseUrl` | Store page, shown as *Buy a license*. Expected for `Paid` and `Freemium` (a warning otherwise). |
| `TrialUrl` | Page where users get a trial key, shown as *Get a trial key*. |

URLs must be absolute `http` or `https`; anything else is an error and the link is left out.

### License badge

Each card of the add-ons panel shows a badge derived from `<Pricing>` and `License` (`AddonLicense.GetKind`):

| Badge | When |
|---|---|
| **Trial** | `Model="Paid"` with `TrialDays` or a `TrialUrl`. |
| **Paid** | `Model="Paid"` without a trial. |
| **Freemium** | `Model="Freemium"`. |
| **Open-Source** | No pricing (or `Model="Free"`) and a `License` made only of common OSI licenses (MIT, Apache-2.0, BSD, GPL family, MPL-2.0, MS-PL, ...). |
| **Free** | Any other add-on without pricing. |

## `<id>/gallery.xml`

```xml
<Gallery
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:controlskit="clr-namespace:OpenSilver.ControlsKit;assembly=OpenSilver.ControlsKit.Controls">

  <Section Name="ControlsKit" Label="ControlsKit" Icon="controlskit" Platforms="Xamlio">
    <Group Name="Panels" Label="Panels" Appearance="Details" IsHeaderVisible="true" MaxDisplayedItems="2">
      <Item Name="FlexPanel" Label="Flex Panel" Icon="flexpanel"
            Description="Shown as a tooltip and in Details groups."
            Keywords="flexbox,flex,stack"
            DefaultWidth="240" DefaultHeight="120">
        <controlskit:FlexPanel Orientation="Horizontal">
          <Border Width="48" Height="48" Background="#EF5350" />
        </controlskit:FlexPanel>
      </Item>
    </Group>
    <Group Name="Layout" Label="Layout">
      <ItemRef Name="FlexPanel" />
    </Group>
  </Section>
</Gallery>
```

Namespace declarations on `<Gallery>` (or any ancestor) are copied into each snippet that uses them, so snippets stay short. A prefix counts as used when it appears in an element or attribute name, or as `prefix:` inside an attribute value or text (for example `TargetType="controlskit:FlexPanel"` or `{x:Static ...}`).

### `<Section>` (a rail item)

| Attribute | Default | Meaning |
|---|---|---|
| `Name` | required | Identity. Sections with the same name across add-ons merge into one rail item (the built-in ones are `Controls` and `Layout`). `AI` and `Addons` are reserved by the IDE. |
| `Label` | `Name` | The text under the rail icon. The tile is narrow: a word such as "Controls" fills a line. Lines break between words and, in a longer word, before a capital letter (`ComponentOne` shows as "Component" / "One"); a part that still doesn't fit is condensed. Prefer one or two short words. |
| `ToolTip` | none | Shown when hovering the rail item, such as the full name when `Label` is an abbreviation (`Label="C1" ToolTip="ComponentOne"`). Leave it out when it would repeat the label: there is then no tooltip. |
| `Icon` | none | Icon name. When sections merge, the first add-on's label, tooltip and icon win. |
| `Platforms` | all | See below. |

### `<Group>`

| Attribute | Default | Meaning |
|---|---|---|
| `Name` | required | Identity within the section. |
| `Label` | `Name` | Group header text. |
| `Appearance` | `Normal` | `Normal`, `Compact` (small icons) or `Details` (icon plus description). `IconOnly` and `Button` are accepted and currently render like `Normal`. |
| `IsHeaderVisible` | `true` | Hides the header when `false`. |
| `MaxDisplayedItems` | `2` | Items shown before the group's "show more" overflow. |
| `Platforms` | inherited | See below. |

Group order in the file is the display order.

### `<Item>` and `<ItemRef>`

An `<Item>` contains exactly one XAML element: the snippet inserted on drop. `<ItemRef Name="..."/>` reuses an item defined elsewhere in the same add-on, so a control can appear in several groups with one definition. Defining the same item name twice is an error.

| Attribute | Default | Meaning |
|---|---|---|
| `Name` | required | Identity within the add-on; also the search key. |
| `Label` | `Name` | Text under the icon. |
| `Icon` | none | Icon name; a missing icon is a warning. |
| `Description` | none | Tooltip and `Details` text. |
| `Keywords` | none | Extra search terms, comma-separated. |
| `ItemType` | `UIElement` | `UIElement` or `UIElement3D`. |
| `DefaultWidth`, `DefaultHeight` | none | Size applied when dropped on a canvas-like target. |
| `ApplyDropProperties` | `true` | Adds the drop target's properties (`Grid.Row`, `Canvas.Left`, `Width`, ...) to the snippet root. Set to `false` for snippets that must stay untouched, such as a `MenuItem`. |
| `ApplyDefaultWidth` | `true` | Set to `false` to apply `DefaultHeight` only (e.g. `TextBlock`, which should size to its text). |
| `Platforms` | inherited | See below. |

### `Platforms`

A comma- or semicolon-separated list, case-insensitive: `OpenSilver` (VS extension), `Xamlio` (the Web IDE), `XRSharp`, `XRSharpPlayground`, `OpenSilverVsCode`, or `All`. When omitted, the element inherits its parent's value; when nothing in the chain sets it, the element is visible everywhere.

## `<id>/icons.xml`

```xml
<Icons>
  <Icon Name="flexpanel">
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">...</svg>
  </Icon>
</Icons>
```

Each icon holds one `<svg>` element; an icon without one is an error. Icons render at about 24 px, so use a `viewBox` (such as `0 0 24 24`) and avoid fixed `width`/`height`. Duplicate names are errors.

Icons are single-color: XAML.io repaints every element that has a `fill` or `stroke` attribute (other than `none`) with the color of the theme, and of the selection. So:

- Draw in any one color, such as `#000`, set with `fill` and `stroke` **attributes**, on the `<svg>` (inherited) or on the shapes. Colors set in `style` attributes or CSS are not repainted, so they stay as drawn on every background.
- Use `fill="none"` for outlines. A shape that gets no `fill` attribute, neither its own nor the `<svg>`'s, is drawn black and never repainted, which is invisible on the dark rail.
- Gradients, images, text and multiple colors are lost or unreadable at this size.

## `<id>/designer.xml`

Optional. Adds elements, side panels and property hints to the contextual bar that appears above the selected control, without shipping any code. Every element shows the current value of the selection, so the user sees what a click changed even when the control is off screen:

```xml
<Designer
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:c1="http://schemas.componentone.com/winfx/2006/xaml">
  <Control Type="c1:C1Chart" TopmostProperties="ChartType,Palette">
    <Panel Label="Chart type" Icon="columnchart" IconOf="ChartType" Description="How the values are drawn.">
      <Choice Property="ChartType" Label="Columns and bars">
        <Option Label="Columns" Value="Column" Icon="columnchart" Description="Compare values across categories" />
        <Option Label="Stacked" Value="ColumnStacked" Icon="stackedcolumnchart" />
      </Choice>
    </Panel>
    <Panel Label="Axes" Icon="axes" Target="View(c1:ChartView)">
      <Section Label="Vertical axis (Y)" Target="AxisY(c1:Axis)">
        <Text Property="Title" Label="Title" Placeholder="No title" />
        <Number Property="Min" Label="From">
          <Option Label="Automatic" />
          <Option Label="0" Value="0" />
        </Number>
        <Color Property="MajorGridStroke" Label="Grid lines" />
      </Section>
    </Panel>
    <Panel Label="Series" Icon="series">
      <Items Label="Series" Target="Data(c1:ChartData)" ItemLabel="Label">
        <Text Property="Label" Label="Name" />
        <Presets Label="Color" Appearance="Swatches">
          <Preset Label="Automatic"><Set Property="SymbolFill" /><Set Property="ConnectionStroke" /></Preset>
          <Preset Label="Indigo" Colors="#FF6366F1">
            <Set Property="SymbolFill" Value="#FF6366F1" /><Set Property="ConnectionStroke" Value="#FF6366F1" />
          </Preset>
        </Presets>
        <Add Label="Series"><c1:DataSeries Label="New" Values="10 20 30" /></Add>
      </Items>
    </Panel>
    <Panel Label="Legend" Icon="legend">
      <Toggle Label="Show the legend"><c1:C1ChartLegend /></Toggle>
      <Choice Property="Position" Label="Position" Target="c1:C1ChartLegend" When="has:c1:C1ChartLegend">
        <Option Label="Top" Value="TopCenter" Position="Top" />
        <Option Label="Right" Position="Right" />
      </Choice>
    </Panel>
  </Control>
  <Control Type="c1:C1FlexGrid">
    <Toggle Label="Editable" Property="IsReadOnly" Value="False" OffValue="True" />
  </Control>
</Designer>
```

### Controls and the bar

| Element / attribute | Meaning |
|---|---|
| `<Control Type>` | `prefix:TypeName` resolved with the file's namespace declarations. A `clr-namespace:` URI matches by CLR namespace (and assembly, when given); any other URI matches through the assembly's `XmlnsDefinition` attributes. The definition also applies to derived types. |
| `TopmostProperties` | Comma-separated properties listed first in the bar's *all properties* popup. |
| `HiddenProperties` | Comma-separated properties hidden from that popup. |
| `<Panel Label Icon? IconOf? ToolTip? Description?>` | A bar button that opens a side panel, like *Position* or *Animations*. `Icon` (from `icons.xml`) shows on the button; with `IconOf="Property"`, the button shows the icon of the current option of the panel's `<Choice>` of that property instead, so the bar tells which chart type is selected. The panel has `Label` as title, `Description` under it, then its settings and sections. It stays open, at the same scroll position, while the user edits and selects other elements of a type that has it. |
| `<Toggle>` (in the bar) | A switch with its label. Name it after the state shown when it is on (*Editable*, not *Read-only*). |
| `<Choice>` (in the bar) | A drop-down showing the current option. |

Keep the bar short: a toggle or a drop-down for the setting users change most, and one `<Panel>` per topic. `<SetProperty>` and `<SampleData>` directly under `<Control>` still work as plain bar buttons, but they don't show any state, so put them in a panel.

### Panel content

| Element / attribute | Meaning |
|---|---|
| `<Section Label>` | A titled group of settings that the user can fold. Sections start folded, so a panel shows its titles first. Sections cannot be nested. |
| `<Toggle Label Property Value? OffValue?>` | A switch. It is on when `Property` equals `Value` (default `True`); turning it off writes `OffValue` (default: the opposite boolean, otherwise the property is removed). |
| `<Toggle Label>` + XAML content | Without `Property`, a *presence* switch: on while an element of the content's type exists at the target, turning it on inserts the content and turning it off removes it (with the property elements that become empty). Use it for optional parts such as a legend, an action or a template. |
| `<Choice Property Appearance?>` + `<Option Value? Label? Icon? Description? Colors? Position? When?>` | One value among several, drawn by `Appearance`: `Tiles` (icon and label, `Description` as tooltip), `Swatches` (the `Colors` of each option, as a strip, or as a dot when there is one color), `Positions` (a frame with a mark at each option's `Position`, such as a legend position), `Pills` (a row of labels; long labels get fewer, wider pills) or `List` (a drop-down). `Auto`, the default, picks tiles when an option has an icon, then swatches when one has colors, then positions, then pills up to six options and a list beyond. Without `<Option>`, the names of the property's enum are listed. |
| `<Number Property Minimum? Maximum? Unit?>` + `<Option>` | A number: a box for any value (clamped to `Minimum`/`Maximum`, emptied to remove the property) and the options as quick steps. |
| `<Color Property>` + `<Option>` | A brush or color: swatches (a default palette without options), a *none* swatch that removes the property, and a box for `#RRGGBB`, `#AARRGGBB` or a color name. |
| `<Text Property Placeholder?>` | A text box; an empty text removes the property. |
| `<Presets Appearance?>` + `<Preset Label Icon? Colors? Description? When?>` + `<Set Property Value? Target?>` | Choices that each set several properties at once, such as a theme. A preset is current when all its values are set; a `<Set>` without `Value` removes the property. Appearances as for `<Choice>`, except `Positions`. |
| `<Items Label Target ItemLabel? ItemType?>` | One folding card per child element at `Target` (such as the series of a chart or the columns of a grid), titled with its `ItemLabel` property (or a pattern of properties such as `{From} to {To}`), with a button that removes it. `ItemType="prefix:Type"` lists only the children of that type or a derived one, such as the ranges among the marks and labels of a gauge. The settings inside apply to each item, with targets relative to it. `<Add Label Description? When?>` children are buttons that append their XAML content, with `Description` as tooltip. Items cannot be nested. |
| `<SetProperty Label Property Value>` | A link that sets `Property` to `Value`. The default tooltip is "Set {Property} to {Value}". |
| `<SampleData Label Property?>` | A link that inserts its XAML children: appended as children (content property) or, with `Property`, written as `<Type.Property>` (exactly one child). |

An `<Option>` without `Value` removes the property, such as *Automatic* or *Same as the chart*; it then needs a `Label`. While the property is not written, only that option is current, even when the default equals another option. The XAML content of toggles, presets and `<Add>` declares its namespaces on each snippet; they are hoisted to the page root after insertion.

### Common attributes

Every setting, section and panel takes `Label`, `ToolTip` and `Description` (help text under the label), plus:

- **`Target`**: where it reads and writes, from the selected element (from the item, inside `<Items>`). A `<Panel>`, `<Section>` or `<Presets>` passes its `Target` on: the settings it contains continue from there. Steps are separated by `/`:
  - `Name` is the property element `<Owner.Name>`, and `Name(prefix:Type)` the object it holds, such as `View(c1:ChartView)/AxisY(c1:Axis)`.
  - `prefix:Type` is the first child element of that type, such as `c1:C1ChartLegend`; `prefix:Owner.Name` is an attached property element, such as `c1:C1FlexGridFilterService.FlexGridFilter`.
  - `*` is every content child, and `.` alone the element itself (for `<Items>` listing its direct children).

  Missing elements are created on the first write and removed once empty.
- **`When`**: shows the element only while all its conditions hold, separated by `;`: `Property=A|B`, `Property!=A|B`, `has:path` and `!has:path` (a path exists), `is:prefix:Type` and `!is:prefix:Type` (the element is of that type or a derived one, such as for options that a definition on a base type shouldn't offer to one of its subclasses). They read the selected element (the item, inside `<Items>`), whatever the `Target`. Options, presets and `<Add>` buttons take `When` too.

`Property` is `Name` or `Owner.Name` (an attached property, such as `c1:PieOptions.InnerRadius` with its prefix), and is written on every selected element. A value equal to the property's default removes the attribute instead; values compare without case, and numbers by value (`1` = `1.0`, `-45,20` = `-45, 20`). When the selected elements disagree, nothing is shown as current.

### Merging and errors

The bar elements are inserted into the bar of the nearest built-in base type (for example the panel bar for a `Panel`), after its *Position* button and before the buttons of the host (*Animations* in XAML.io) and *...*, and wrap with them on narrow screens. When several definitions apply (the type and a base type), the most derived comes first. A type defined twice keeps its first definition: a warning within one add-on, silently across enabled add-ons. Unknown elements and attributes, an unknown icon, a nested `<Panel>`, `<Section>` or `<Items>`, an empty `<Panel>`, `<Section>` or `<Presets>` are warnings; an undeclared prefix, an invalid `Target` or `When`, a `Type` without a prefix when `<Designer>` has no default namespace, a missing required attribute or an empty `<SampleData>` is an error.

`designer.xml` cannot run code, so it is safe for add-ons from anyone. When a control needs more (custom editors, 3D manipulators), first-party add-ons can ship a designer satellite assembly instead (a library named `{Assembly}.XamlDesigner.dll` with an `EmbeddingFactory`); ask the XAML.io team.

Only definitions of enabled add-ons are used, and they refresh with the gallery.

## Project file

A project enables an add-on explicitly with an item in its `.csproj`:

```xml
<ItemGroup>
  <XamlioAddon Include="controlskit" Version="1.0.0" />
</ItemGroup>
```

- `Include` is the add-on id, case-insensitive. Unknown ids are kept and ignored, so a project still opens when an add-on is removed from the catalog.
- `Version` is optional: the add-on version the project was enabled with. It is informational for now.
- MSBuild ignores unknown items, so the project still builds outside XAML.io.
- XAML.io writes the items in their own `ItemGroup` after the project references, sorted by id, and only when there is at least one.

Add-ons that come with a package do not need this item: referencing the package is enough, and the *More...* panel adds the package instead of the item.
