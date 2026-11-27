---
name: treegrid
description: >-
  Reference for TreeGridControl, the tree grid of the TreeGrid.OpenSilver package: a data grid
  whose rows expand and collapse, with typed columns, sorting, filtering, grouping, editing,
  check boxes and row drag and drop. Use it when writing or changing XAML that uses a
  TreeGridControl, or when the user wants hierarchical data (an org chart, a project plan,
  folders, a budget) shown as a table.
---

# TreeGrid

`TreeGridControl` shows a collection as rows and columns, where each row can hold child rows.
It comes from the `TreeGrid.OpenSilver` NuGet package.

## Before using it

1. The app project must reference the `TreeGrid.OpenSilver` package.
2. Declare the namespaces on the root element of the XAML file. The assembly is
   `TreeGrid.OpenSilver`, even though the namespaces start with `TreeGrid.Wpf`:

   | Prefix (convention) | Namespace | Types |
   |---|---|---|
   | `tg` | `clr-namespace:TreeGrid.Wpf;assembly=TreeGrid.OpenSilver` | `TreeGridControl` and its enums |
   | `cols` | `clr-namespace:TreeGrid.Wpf.Columns;assembly=TreeGrid.OpenSilver` | the column types |
   | `headers` | `clr-namespace:TreeGrid.Wpf.Headers;assembly=TreeGrid.OpenSilver` | `StackedHeaderRow`, `StackedColumn` |

## Minimal example

```xml
<tg:TreeGridControl ItemsSource="{Binding Employees}" ChildPropertyName="Reports"
                    AutoExpandLevels="1" ShowAlternatingRows="True">
  <tg:TreeGridControl.Columns>
    <cols:TreeGridTextColumn MappingName="Name" HeaderText="Name" Width="200" />
    <cols:TreeGridTextColumn MappingName="Title" HeaderText="Title" Width="160" />
    <cols:TreeGridNumericColumn MappingName="Salary" HeaderText="Salary" DisplayFormat="{}{0:C0}" />
    <cols:TreeGridBooleanIconColumn MappingName="Available" HeaderText="Available" />
  </tg:TreeGridControl.Columns>
</tg:TreeGridControl>
```

## The tree: two ways to describe it

| Data | Set | Example |
|---|---|---|
| Each item has a collection of child items | `ChildPropertyName` | `ChildPropertyName="Reports"` on employees that have `ObservableCollection<Employee> Reports` |
| A flat list where each item names its parent | `IdPropertyName` and `ParentIdPropertyName` | rows of a table with `Id` and `ParentId`; items whose parent key matches no item are roots |

- `AutoExpandLevels` sets the levels expanded when the items load: `0` (default) keeps everything
  collapsed, `1` opens the roots, `-1` opens the whole tree. Use `1` or `-1` so the tree is visible.
- The first column holds the expand arrows and the indent; `ExpanderColumnIndex` moves them.
- Use `ObservableCollection<T>` for children so added and removed items show up.
- Without `Columns`, `AutoGenerateColumns` (default true) creates a text column per property of the
  first item. Declare columns for anything users will see.

## Mistakes to avoid

- `DisplayFormat` is a `string.Format` pattern, so in XAML it needs the `{}` escape: `DisplayFormat="{}{0:C0}"`.
  `DateFormat` is a plain format: `DateFormat="MMM d, yyyy"`.
- Columns are not in the visual tree: `ElementName` and `RelativeSource` bindings on column properties
  do not work. Use a `StaticResource` or set them in code-behind.
- Most features are off by default: `AllowEditing`, `AllowFiltering`, `AllowGrouping`,
  `AllowRowDragDrop`, `AllowCheckBoxSelection`, `AllowDeleteRows`, `AllowPaste` and
  `ShowDefaultContextMenus` are all `False`. Sorting, column resizing and reordering, copying and the
  footer are on.
- Row height is the same for every row (`RowHeight`); rows cannot grow to fit their content.
- Grouping replaces the tree while it is active: the rows are grouped by the column values
  rather than nested by their parents.

## Look

- Theme: with the Modern theme, the grid takes the colors of its palette and follows its light and
  dark modes; don't add a theme for dark mode. Otherwise it uses its light theme. To keep a grid dark
  whatever the app's mode, add the dark theme to its resources (or to `Application.Resources` for
  every grid):

  ```xml
  <tg:TreeGridControl.Resources>
    <ResourceDictionary Source="/TreeGrid.OpenSilver;component/Themes/FluentDark.xaml" />
  </tg:TreeGridControl.Resources>
  ```

- Per-grid colors fall back to the theme when unset, and replace it in both light and dark mode when
  set, so prefer leaving them unset: `HeaderBackground`, `HeaderForeground` (the sort
  and filter icons follow it), `SelectedRowBackground`, `AlternatingRowBackground` with
  `ShowAlternatingRows="True"`, `HoverRowBackground` (no hover highlight until set), `RowBackground`,
  `CellForeground`, `GridLineBrush`.
- Layout: `GridLinesVisibility` (`None`, `Horizontal`, `Vertical`, `Both`), `RowHeight` (26),
  `HeaderRowHeight` (30), `CellFontSize`, `HeaderFontWeight`, `IndentPerLevel` (18),
  `FrozenColumnCount`.

Details and examples:

- [reference/columns.md](reference/columns.md): the column types and their properties, template
  columns, sizing, headers spanning several columns.
- [reference/features.md](reference/features.md): sorting, filtering, grouping, editing, selection,
  check boxes, drag and drop, clipboard, menus, footer, events and code-behind.
