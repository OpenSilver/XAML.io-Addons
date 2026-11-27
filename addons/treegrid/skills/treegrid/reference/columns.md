# TreeGrid columns

Columns go in `<tg:TreeGridControl.Columns>`, in display order. The types are in
`xmlns:cols="clr-namespace:TreeGrid.Wpf.Columns;assembly=TreeGrid.OpenSilver"`.

## Column types

| Type | Shows | Edits with |
|---|---|---|
| `TreeGridTextColumn` | the value as text | a text box |
| `TreeGridNumericColumn` | a right-aligned number | a text box, parsed with the culture, clamped to `MinValue`/`MaxValue` |
| `TreeGridDateTimeColumn` | a date, formatted with `DateFormat` (default `d`) | a date picker |
| `TreeGridComboBoxColumn` | the value as text | a combo box over its `ItemsSource` |
| `TreeGridCheckBoxColumn` | a check box for a `bool` (or `bool?` with `IsThreeState="True"`) | the check box, in place |
| `TreeGridBooleanIconColumn` | a green tick or a red cross, nothing for null | read-only |
| `TreeGridProgressBarColumn` | a bar from `Minimum` (0) to `Maximum` (100), with the percentage when `ShowPercentText` (default true) | read-only |
| `TreeGridHyperlinkColumn` | the value as a link; it navigates only when the value is an absolute URI | read-only |
| `TreeGridTemplateColumn` | your `CellTemplate` | your `EditTemplate`, if any |

## Properties of every column

| Property | Type, default | Notes |
|---|---|---|
| `MappingName` | `string` | the property of the items shown; dotted paths work (`Manager.Name`) |
| `HeaderText` | `string` | the header; the `MappingName` when unset |
| `Width` | `double`, auto | also `MinimumWidth` (20), `MaximumWidth` |
| `ColumnSizer` | `ColumnSizerMode`, `None` | `SizeToCells`, `SizeToHeader`, `AllCells`, `Star` (shares the free width), `LastColumnFill` |
| `TextAlignment` | `TextAlignment`, `Left` (`Right` for numeric columns) | |
| `DisplayFormat` | `string` | a `string.Format` pattern: `DisplayFormat="{}{0:C0}"`, `"{}{0:N1} kg"`, `"{}{0}%"` |
| `SortMemberPath` | `string` | sort by another property, such as a template column showing a label but sorting by a number |
| `CellForeground` | `Brush` | text color of the cells |
| `Padding` | `Thickness` | |
| `IsHidden` | `bool`, `False` | |
| `AllowSorting`, `AllowFiltering`, `AllowEditing`, `AllowResizing`, `AllowGrouping` | `bool`, `True` | per-column opt-outs; the grid-level switches must be on too |
| `HeaderTemplate` | `DataTemplate` | custom header content; its DataContext is the column (`{Binding HeaderText}`) |

## Numbers

| Property | Default | Notes |
|---|---|---|
| `NumberDecimalDigits` | `2` | used when `DisplayFormat` is not set |
| `UseGroupSeparator` | `True` | thousands separators |
| `MinValue`, `MaxValue` | none | clamp edited values; a `MinValue` of 0 or more also rejects negative input |
| `AllowDecimals` | from the property type | `bool?`; integer properties, or `NumberDecimalDigits="0"`, reject decimals |
| `AllowNegative` | from `MinValue` | `bool?` |

```xml
<cols:TreeGridNumericColumn MappingName="Budget" HeaderText="Budget" DisplayFormat="{}{0:C0}" Width="100" />
<cols:TreeGridNumericColumn MappingName="Hours" HeaderText="Hours" NumberDecimalDigits="1" />
```

## Yes/no icons

```xml
<cols:TreeGridBooleanIconColumn MappingName="Available" HeaderText="Available"
                                TrueText="Available" FalseText="Away" TrueBrush="#FF10B981" />
```

`TrueText`/`FalseText` (default Yes/No) are the tooltip and the text used by copy, filtering and
grouping. Also `FalseBrush`, `ShowBackgroundCircle` (true), `IconSize`, `TrueIcon`/`FalseIcon`
(`Geometry` on a 24x24 box), and `TrueTemplate`/`FalseTemplate`/`NullTemplate` to replace the visual.

## Combo box

`ItemsSource` cannot be bound with `ElementName` or `RelativeSource` (columns are not in the visual
tree): use a `StaticResource` or set it in code-behind.

```xml
<cols:TreeGridComboBoxColumn MappingName="Status" HeaderText="Status" ItemsSource="{StaticResource Statuses}" />
```

`DisplayMemberPath` and `SelectedValuePath` apply to the drop-down only: the cell shows the stored
value, so with `SelectedValuePath="Id"` it would show the key. Store the displayed value, or use a
template column to show something else.

## Template columns

The DataContext of the template is the item of the row. Set `MappingName` (or `SortMemberPath`) so
the column can sort and filter. The grid writes nothing back for template columns: an
`EditTemplate` edits through its own `Mode=TwoWay` bindings.

```xml
<!-- A colored status pill; StatusColor is a string such as "#FF10B981" -->
<cols:TreeGridTemplateColumn MappingName="Status" HeaderText="Status" Width="110">
  <cols:TreeGridTemplateColumn.CellTemplate>
    <DataTemplate>
      <Border Background="{Binding StatusColor}" CornerRadius="9" Padding="8,1"
              HorizontalAlignment="Left" VerticalAlignment="Center">
        <TextBlock Text="{Binding Status}" Foreground="White" FontSize="11" FontWeight="SemiBold" />
      </Border>
    </DataTemplate>
  </cols:TreeGridTemplateColumn.CellTemplate>
  <cols:TreeGridTemplateColumn.EditTemplate>
    <DataTemplate>
      <TextBox Text="{Binding Status, Mode=TwoWay}" />
    </DataTemplate>
  </cols:TreeGridTemplateColumn.EditTemplate>
</cols:TreeGridTemplateColumn>
```

`CellTemplateSelector` and `EditTemplateSelector` pick a template per row.

## Headers spanning several columns

A row above the headers groups adjacent columns. `ChildColumns` lists their `MappingName`s,
separated by commas; the spans follow resizing, reordering and freezing.

```xml
<tg:TreeGridControl.StackedHeaderRows>
  <headers:StackedHeaderRow>
    <headers:StackedHeaderRow.StackedColumns>
      <headers:StackedColumn HeaderText="Schedule" ChildColumns="Start,Finish" />
      <headers:StackedColumn HeaderText="Tracking" ChildColumns="Progress,Status" />
    </headers:StackedHeaderRow.StackedColumns>
  </headers:StackedHeaderRow>
</tg:TreeGridControl.StackedHeaderRows>
```

`headers` is `clr-namespace:TreeGrid.Wpf.Headers;assembly=TreeGrid.OpenSilver`. The height is
`StackedHeaderRowHeight` (28) on the grid.

## Frozen columns

`FrozenColumnCount="1"` keeps the first column in view while scrolling sideways;
`FooterColumnCount` does the same for the last columns.
