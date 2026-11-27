# TreeGrid features

Properties of `TreeGridControl`, with their defaults. Most features are off until enabled.

## Sorting

`AllowSorting` (true): a header click cycles ascending, descending, unsorted. `AllowMultiSort` (true):
Ctrl+click adds columns, numbered on the headers when `ShowSortNumbers` (true). Each level of the tree
is sorted on its own, so children stay under their parents.

```csharp
grid.SortColumn("Budget", ListSortDirection.Descending);
grid.SortColumn("Name", ListSortDirection.Ascending, addToExisting: true);
grid.ClearSorting();
```

## Filtering

`AllowFiltering="True"` adds a funnel to each header: a searchable list of values and two conditions
joined by And/Or, as in Excel.

| `FilterNodeMode` | Keeps |
|---|---|
| `MatchingNodesOnly` | the matches |
| `MatchingAndParentNodes` (default) | the matches and their parents |
| `MatchingAndChildNodes` | the matches and everything under them |
| `MatchingParentAndChildNodes` | both |

`ExpandNodesOnFiltering` (true) opens the parents of the matches. In code:
`grid.FilterPredicate = item => ((Task)item).Progress < 100;`, `ClearFilter("Owner")`, `ClearFilters()`.

## Grouping

`AllowGrouping="True"` lets people group the rows by column values; `ShowGroupDropArea="True"` adds the
area above the grid where they drop column headers. Without it, grouping is in the header menu
(`ShowDefaultContextMenus="True"`). While grouped, the tree is replaced by the groups; ungrouping restores it.

| Property | Default |
|---|---|
| `ShowGroupItemCount` | `True` |
| `AutoExpandGroups` | `True` |
| `HideGroupedColumns` | `True` |

```csharp
grid.GroupByColumn("Owner");
grid.ClearGrouping();
grid.QueryGroupCaption += (s, e) => e.Caption = $"{e.Group.Key}";
```

## Editing

```xml
<tg:TreeGridControl AllowEditing="True" EditTrigger="OnTap" ValidationMode="Cell" />
```

- `EditTrigger`: `OnDoubleTap` (default), `OnTap`, `OnKeyPress`, `None`; F2 always starts editing.
  Enter commits and moves down, Tab moves to the next editable cell, Escape cancels.
- Validation (`ValidationMode`: `Cell` by default, `Row`, `CellAndRow`, `None`) uses data annotations,
  `IDataErrorInfo`, `INotifyDataErrorInfo` and the `CellValidating`/`RowValidating` events. Errors show as
  a red cell border with a tooltip.
- `AllowBulkValueChange="True"`: Ctrl+Enter writes the value to every selected row of the column
  (asks first when they differ, unless `ShowBulkValueChangeConfirmation="False"`).
- `AllowDeleteRows="True"`: the Delete key removes the selected rows from their collections, after a
  confirmation (`ConfirmRowDelete`, true). The `RowsDeleting` event can cancel.

## Selection and check boxes

| `SelectionMode` | Behavior |
|---|---|
| `None` | nothing is selected |
| `Single` | one row |
| `Multiple` | each click toggles a row |
| `Extended` (default) | click selects, Ctrl toggles, Shift extends |

`SelectionUnit`: `Row` (default) or `Cell`. Read `SelectedItem`, `SelectedItems`; bind the selection to
the items with `SelectedMemberPath="IsSelected"`.

`AllowCheckBoxSelection="True"` puts a check box on each row. `CheckBoxCascadeMode`:
`SynchronizeWithParentAndChildren` (default: checking a parent checks its children, and a parent shows
partly checked), `SynchronizeChildrenOnly`, `None`. `CheckedMemberPath="Done"` reads and writes a `bool`
or `bool?` property of the items; `CheckedItems` lists the checked items. Add
`ObserveMemberPathChanges="True"` when the items change those properties themselves.

## Row drag and drop

`AllowRowDragDrop="True"`: people drop rows above, below or into other rows. With
`UpdateSourceOnRowDrop` (true) the grid moves the items between the child collections (or rewrites the
parent key of a flat list). Events: `RowDragStarting` (cancellable), `RowDropped`, `RowDropCompleted`.

## Clipboard and menus

- `AllowCopy` (true): Ctrl+C copies the selected rows as tab-separated text.
- `AllowPaste` (false) with `AllowEditing`: Ctrl+V pastes; `AllowExcelPaste` (true) fills a block of
  cells copied from Excel. In the browser, pasting is asynchronous: the result arrives with `PasteCompleted`.
- `ShowDefaultContextMenus="True"`: right-click menus on rows and headers to sort, filter, group, fit,
  freeze, hide columns (`AllowHidingColumns`, true), copy, expand and collapse.

## Footer

`ShowFooter` (true) shows the number of rows, and the selected, checked and grouped counts when there are
any. `FooterStatusText` replaces that text; `FooterContent` adds content on the right.

## Expanding in code

`AutoExpandLevels` sets what is open when the items load. Events `NodeExpanding` and `NodeCollapsing`
can cancel; `NodeExpanded` and `NodeCollapsed` follow.

## Other events

| Event | When |
|---|---|
| `SelectionChanged`, `CurrentCellChanged` | the selection or the current cell changes |
| `NodeChecked` | a check box changes |
| `SortColumnsChanged`, `FilterChanged` | sorting or filtering changes |
| `CellBeginEdit` (cancellable), `CellEndEdit` | editing a cell |
| `QueryCellStyle`, `QueryRowStyle` | per-cell and per-row colors, for conditional formatting |

```csharp
grid.QueryCellStyle += (s, e) =>
{
    if (e.Column.MappingName == "Variance" && ((BudgetLine)e.Record).Variance < 0)
        e.Foreground = new SolidColorBrush(Colors.Red);
};
```

## Loading children on demand

For large trees, children can be fetched the first time a row expands:

```csharp
grid.DataSource.HasChildNodesResolver = item => ((Folder)item).HasSubfolders;
grid.DataSource.RequestTreeItemsAsync = async (node, token) => await LoadSubfoldersAsync(node.Item, token);
```
