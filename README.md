# XAML.io add-ons

Get your controls into the designer of [XAML.io](https://xaml.io), the online IDE for .NET apps in XAML and C#.

An **add-on** is a folder of a few XML files that describe your library. Once it ships, everyone using XAML.io finds:

- a card for your library in the *More...* panel of the designer, with your description, screenshots and links;
- your controls in the toolbox on the left, ready to drag onto the page (XAML.io adds your NuGet package to the project);
- optionally, buttons in the designer's toolbar to set the most useful properties of your controls;
- optionally, a skill that teaches the XAML.io AI agent to use your controls;
- a link such as `xaml.io/a/your-id` that opens XAML.io with your add-on, to put on your website.

No code, no build: you write XML, test it in XAML.io, then send a pull request to this repository. The [ControlsKit add-on](addons/controlskit/) is a complete example to copy.

## What you need

- Your controls in a NuGet package for [OpenSilver](https://opensilver.net), published on nuget.org.
- A GitHub account, and Git or [GitHub Desktop](https://desktop.github.com/).
- A text editor. [Visual Studio Code](https://code.visualstudio.com/) with the *XML* extension by Red Hat, or Visual Studio, completes the element and attribute names as you type, and flags typos. The first lines of the example's files point to the schemas in `schemas/` for this: keep them.
- A desktop browser such as Chrome, Edge or Firefox.

## Step 1: Copy the example

1. [Fork this repository](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo), then clone your fork to your computer.
2. Choose an **id** for your add-on: lowercase letters, digits and dashes, such as `acme-charts`.
3. Copy the folder `addons/controlskit` to `addons/acme-charts` (use your id).
4. In your new folder, delete what you will not need at first: `designer.xml`, `skills/` and `templates/`. In `addon.xml`, delete the `<Templates>` and `<Skills>` elements. You can add them back later.

Your folder now holds:

| File | What it is |
|---|---|
| `addon.xml` | The name, description, NuGet package and screenshots of your library. |
| `gallery.xml` | The items of the toolbox: each one is a XAML snippet that is inserted when the item is dropped on the page. |
| `icons.xml` | The SVG icons of your add-on and of its items. |
| `assets/` | The banner and screenshots shown in the *More...* panel. |

## Step 2: Describe your library and controls

**`addon.xml`**

- Set `Id` to your id, exactly the name of the folder.
- Fill in `Name`, `Publisher`, `Version`, `License`, `Website` and `Description` (one sentence, shown on the card).
- Optionally set `Slugs` to a short name such as your id: that is the vanity URL for your website and docs, for example `xaml.io/acme-charts`. Without it, people still reach your add-on at `xaml.io/a/acme-charts`. Delete the attribute if you do not want a vanity URL yet.
- In `<Packages>`, put the id of your NuGet package and the version to install:

  ```xml
  <Packages>
    <Package Id="Acme.Charts" RecommendedVersion="2.1.0" />
  </Packages>
  ```

- In `<Xmlns>`, put the XAML namespace of your controls and the prefix you would like to see in pages. The namespace is the one you would write in a page, in either form:
  - `clr-namespace:Acme.Charts;assembly=Acme.Charts.Controls`: the C# namespace of the controls, and the name of the DLL that holds them, without `.dll`. A `.nupkg` is a zip file: the DLL is in its `lib/` folder.
  - A URI such as `http://schemas.acme.com/charts`, if your assembly declares one with `[assembly: XmlnsDefinition(...)]`.

  Put the same declaration on `<Gallery>` in `gallery.xml`. Use the same prefix in both files.
- Replace `assets/Banner.webp` with your banner: 2:1 (for example 800 x 400), dark tones, no text, ideally under 100 KB. Replace the screenshots, and list them in `<Screenshots>`.

**`gallery.xml`**

Declare your namespace once on the `<Gallery>` element, then list your controls. A `<Section>` is an icon in the toolbox, with its `Label` under it, a `<Group>` a heading inside it, and an `<Item>` a control that can be dropped. The toolbox is narrow, so keep the section's `Label` to one or two short words, such as your product name:

```xml
<Section Name="AcmeCharts" Label="Acme Charts" Icon="acme">
  <Group Name="Charts" Label="Charts" Appearance="Details">
    <Item Name="BarChart" Label="Bar Chart" Icon="barchart" Keywords="bar,column,histogram"
          Description="Compares values with horizontal or vertical bars."
          DefaultWidth="320" DefaultHeight="200">
      <acme:BarChart Title="Sales" />
    </Item>
  </Group>
</Section>
```

Each `<Item>` holds exactly one XAML element: the snippet inserted on the page when the item is dropped. Write it as a developer would by hand:

- Use the real type and property names of the package version you install: check them in the library's source or documentation, not from memory.
- Give it a little sample content and colors, so that the control is visible as soon as it is dropped, and set `DefaultWidth` and `DefaultHeight` to a size where it looks right.
- Leave out `x:Name`, event handlers such as `Click="..."`, and `{StaticResource}` references to resources of your own app: the page has no code-behind or resources for them, and dropping a snippet twice would duplicate a name.
- Use your own namespace prefix, and the default namespace for the standard controls (`Border`, `TextBlock`, ...).

**`icons.xml`**

Put one `<Icon Name="...">` per name used in `Icon="..."`, each holding an `<svg>` with `viewBox="0 0 24 24"` and no `width` or `height`. Icons are shown at about 24 pixels and repainted in the color of the theme:

- Draw in one color, such as `#000`, with `fill` and `stroke` **attributes** (not `style` or CSS), and `fill="none"` for outlines.
- Keep them simple: no text, gradients or images.

Every element and attribute is described in [AddonFormat.md](AddonFormat.md), including the optional toolbar buttons (`designer.xml`), AI skills and templates.

## Step 3: Test it in XAML.io

1. Open [xaml.io](https://xaml.io) and wait for the designer to show a page, such as `MainWindow.xaml`. Use a new project (*New* in *My Solutions*) rather than one you care about: testing adds your package to it.
2. In the column of icons on the left of the designer, click **More...**, the last one.
3. At the bottom of the panel, below the add-ons, click *Want your controls featured here?*. In the window that opens, click **Test my add-on...**.
4. Pick your add-on folder, `addons/acme-charts`: the one that contains `addon.xml`.
5. If the browser asks whether to upload the files, confirm. The files are only read by the XAML.io page in your browser.
6. A message says whether the add-on loaded and lists any problems, with the file and the line. Your add-on is enabled for the project: its items appear in the toolbox, and the panel shows its details, marked *Local, this session only*.
7. Drag each of your items onto the page and check that it looks right. Click **Start** to build and run the app.
8. After editing your files, click **Load again...** in the details of your add-on and pick the same folder: the new version replaces the old one. Reloading the page unloads the add-on.

Your add-on is ready when the message says *No problems found in its files* and every item works on the page.

### If something goes wrong

| What you see | What to do |
|---|---|
| *The folder has no addon.xml* | Pick the folder that contains `addon.xml`, not a folder inside it. |
| *The folder holds 2 add-ons* | Pick your add-on folder, not the `addons` folder or the whole repository. |
| *addon.xml(12): error: ...* | The file and line of the problem. The parts with errors are left out until you fix them. [AddonFormat.md](AddonFormat.md) describes every element. |
| *Slug 'controlskit' is already used* | Change `Slugs` in `addon.xml` to your own short name. |
| The item is in the toolbox, but the page shows an error after the drop, or *Start* fails | Check the type and property names against your package. Also check that the package version is built for the OpenSilver version XAML.io uses (the latest): packages built for an older version often fail to build. |
| The NuGet package is not found | It must be published on nuget.org. Other feeds need the XAML.io team to allow them: say so in your pull request. |
| Nothing happens after picking the folder | Open the browser console (F12): the add-on messages start with `[Add-ons]`. |

## Step 4: Send a pull request

1. Commit your add-on folder, `addons/<your-id>/`, and nothing else.
2. Push to your fork and [open a pull request](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/creating-a-pull-request-from-a-fork) to this repository. Its description has a short checklist to tick.
3. A check named *Validate add-ons* runs on the pull request: it validates your XML files against the schemas and checks that the files they name exist. If it fails, its log lists the problems.
4. The XAML.io team tries your add-on, may ask for changes, then merges it. Your add-on ships with the next release of XAML.io.

To update your add-on later, send a new pull request that changes your folder, and increase `Version` in `addon.xml`.

### Before you send it

- *Test my add-on...* says *No problems found in its files*.
- Every item renders right after the drop, and the app builds and runs.
- `Publisher`, `License` and `Website` are filled in, and you have the right to publish the texts, icons and screenshots.
- Images are PNG, JPEG or WebP, each under 500 KB.

## Paid add-ons

Commercial libraries are welcome. Declare `<Pricing>` in `addon.xml` with the price and the trial, and links to buy a license and to get a trial key ([format](AddonFormat.md#pricing)). XAML.io then shows a *Trial* badge on the card, and states the price before a user enables the add-on. [Contact us](https://opensilver.net/contact/) if you have any questions or feedback.

## Check your files on your computer (optional)

The *Validate add-ons* check can also run on your computer, with the [.NET 10 SDK](https://dotnet.microsoft.com/download) or later. From the root of the repository:

```
dotnet run tools/validate.cs
```

It checks the files against the schemas, the id and slugs, the files that `addon.xml` names, the icon names, the gallery items and the skill names, and lists each problem with its file and line. *Test my add-on...* in XAML.io checks more, and only XAML.io shows how your controls render, so test there too.

## Using an AI coding agent

Agents such as GitHub Copilot, Cursor, Claude Code or Codex can write most of an add-on. [AGENTS.md](AGENTS.md) tells them how to work in this repository; most agents read it on their own. Ask, for example: *"Create an add-on for the NuGet package Acme.Charts, with an item for each chart control."* Then review the result, test it in XAML.io (Step 3) and add the banner and screenshots yourself.

## What is in this repository

```
addons/
  controlskit/        the example: ControlsKit, by Userware
  treegrid/           TreeGrid, ported from WPF: panels with column cards, property lists and type changes
  <your-id>/          your add-on
schemas/              the format of the add-on files, for completion in your editor and for the check
AddonFormat.md        every element and attribute of the add-on files
AGENTS.md             instructions for AI coding agents
tools/validate.cs     the check run on pull requests
```

The schemas, `AddonFormat.md`, ControlsKit and TreeGrid are copied from XAML.io, which loads the same files: please report problems with them in an issue rather than in a pull request.

## Questions

Open an [issue](../../issues). For a library that needs more than XML can describe, such as a custom editor in the designer, tell us about it there.

You may also [contact us](https://opensilver.net/contact/) directly.

## License

The documentation, schemas and tools of this repository are under the [MIT license](LICENSE). Each add-on keeps the license named by `License` in its `addon.xml`. By sending a pull request, you confirm that you may publish the files of your add-on and allow XAML.io to show them.
