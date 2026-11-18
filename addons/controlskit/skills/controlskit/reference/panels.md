# ControlsKit panels

All three panels are in `xmlns:controlskit="clr-namespace:OpenSilver.ControlsKit;assembly=OpenSilver.ControlsKit.Controls"`.

## FlexPanel

Lays children out in a row (`Orientation="Horizontal"`, the default) or a column, like CSS flexbox.
Children that do not fit on the line wrap to a new line.

| Property | Type, default | Values |
|---|---|---|
| `Orientation` | `Orientation`, `Horizontal` | `Horizontal`, `Vertical` |
| `JustifyContent` | `JustifyContent`, `Start` | `Start`, `End`, `Center`, `SpaceBetween`, `SpaceAround`, `SpaceEvenly` (main axis) |
| `AlignItems` | `AlignItems`, `Start` | `Start`, `Center`, `End` (cross axis, within a line), `Stretch` (the panel's full cross size, not the line's); ignored when both `JustifyContent` and `AlignContent` are `Space*` values |
| `AlignContent` | `AlignContent`, `Start` | `Start`, `Center`, `End`, `SpaceBetween`, `SpaceAround`, `SpaceEvenly` (how wrapped lines are distributed) |
| `Spacing` | `double`, `0` | gap between items; ignored by the `Space*` values of `JustifyContent`, which compute their own gaps |
| `controlskit:FlexPanel.Grow` | attached `double`, `0` | on a child: share of the free space on its line, like `flex-grow`; ignored when `JustifyContent` is a `Space*` value |

```xml
<!-- Toolbar: title on the left, actions on the right, vertically centered -->
<controlskit:FlexPanel JustifyContent="SpaceBetween" AlignItems="Center" Margin="16,8">
  <TextBlock Text="Orders" FontSize="20" FontWeight="SemiBold" />
  <StackPanel Orientation="Horizontal">
    <Button Content="Export" Margin="0,0,8,0" />
    <Button Content="New order" />
  </StackPanel>
</controlskit:FlexPanel>

<!-- Search box that takes the remaining width -->
<controlskit:FlexPanel AlignItems="Center" Spacing="8">
  <TextBox controlskit:FlexPanel.Grow="1" />
  <Button Content="Search" />
</controlskit:FlexPanel>
```

## StaggeredPanel

Masonry layout: columns of equal width, each new child goes into the currently shortest column.
Children are arranged at their own desired size, so give each one a `Width` (normally `DesiredColumnWidth`) and a height;
a child without a width collapses to zero, and a wider one spans several columns.

| Property | Type, default | Meaning |
|---|---|---|
| `DesiredColumnWidth` | `double`, `250` | target column width; the number of columns follows from the available width |
| `ColumnSpacing` | `double`, `0` | horizontal gap |
| `RowSpacing` | `double`, `0` | vertical gap |
| `Padding` | `Thickness`, `0` | space inside the panel |

With `HorizontalAlignment="Stretch"` columns may grow wider than `DesiredColumnWidth` to fill the space.
Inside a parent that sizes to its content (a `StackPanel` with `HorizontalAlignment="Left"`, a horizontal
`StackPanel`, an `Auto` grid column), set `HorizontalAlignment="Left"` on the panel: when stretched there,
it reserves less height than its tallest column and overlaps the element below.

```xml
<ScrollViewer>
  <controlskit:StaggeredPanel DesiredColumnWidth="200" ColumnSpacing="12" RowSpacing="12" Padding="12">
    <Border Width="200" Height="140" CornerRadius="8" Background="#EF5350" />
    <Border Width="200" Height="220" CornerRadius="8" Background="#34D399" />
    <Border Width="200" Height="180" CornerRadius="8" Background="#6366F1" />
  </controlskit:StaggeredPanel>
</ScrollViewer>
```

To show data, use it as the `ItemsPanel` of an `ItemsControl`:

```xml
<ItemsControl ItemsSource="{Binding Photos}">
  <ItemsControl.ItemsPanel>
    <ItemsPanelTemplate>
      <controlskit:StaggeredPanel DesiredColumnWidth="200" ColumnSpacing="12" RowSpacing="12" />
    </ItemsPanelTemplate>
  </ItemsControl.ItemsPanel>
</ItemsControl>
```

## AdaptiveColumnsPanel

Puts its visible children side by side in equal columns (one column per child) when the panel is
wider than `NoColumnsBelowWidth`, and stacks them vertically otherwise. Children keep their own
alignment inside their slot.

| Property | Type, default | Meaning |
|---|---|---|
| `NoColumnsBelowWidth` | `double`, `500` | at or below this width, children stack |

```xml
<controlskit:AdaptiveColumnsPanel NoColumnsBelowWidth="600">
  <Border Margin="8" Padding="16" Background="#F3F4F6"><TextBlock Text="Plan A" /></Border>
  <Border Margin="8" Padding="16" Background="#F3F4F6"><TextBlock Text="Plan B" /></Border>
  <Border Margin="8" Padding="16" Background="#F3F4F6"><TextBlock Text="Plan C" /></Border>
</controlskit:AdaptiveColumnsPanel>
```
