# ControlsKit controls

## AnimatedNavigationBar

Namespace: `xmlns:controlskit="clr-namespace:OpenSilver.ControlsKit;assembly=OpenSilver.ControlsKit.Controls"`.

A `ListBox` laid out horizontally whose selection indicator slides to the selected item. Everything a
`ListBox` has works: `ListBoxItem` children or `ItemsSource`, `SelectedIndex`, `SelectedItem`,
`SelectionChanged`.

| Property | Type, default | Meaning |
|---|---|---|
| `IndicatorBackground` | `Brush`, `Transparent` | fill of the sliding indicator; set it, or the selection is invisible |
| `SelectedForeground` | `Brush`, `White` | text color of the selected item |
| `IndicatorCornerRadius` | `CornerRadius`, `0` | |
| `ItemPadding` | `Thickness`, `20,10,20,10` | |
| `AnimationDuration` | `int`, `300` | **milliseconds as a number** (`AnimationDuration="200"`), not a `TimeSpan` |
| `IndicatorTemplate` | `DataTemplate` | replaces the indicator's look |

```xml
<controlskit:AnimatedNavigationBar x:Name="Tabs" SelectedIndex="0"
    Background="#F3F4F6" Foreground="#4B5563"
    IndicatorBackground="#3B82F6" SelectedForeground="White" IndicatorCornerRadius="8"
    SelectionChanged="Tabs_SelectionChanged">
  <ListBoxItem Content="Overview" />
  <ListBoxItem Content="Orders" />
  <ListBoxItem Content="Settings" />
</controlskit:AnimatedNavigationBar>
```

To switch pages, bind or set the content from `SelectedIndex` in `SelectionChanged`; the bar does not
host pages itself.

## ExtendedButton

Namespace: `xmlns:controlskitcontrols="clr-namespace:OpenSilver.ControlsKit.Controls;assembly=OpenSilver.ControlsKit.Controls"`.

A `Button` (so `Click` and `Command` work) that draws its own icon and text.

- **Text**: `ButtonText` (string). When it is empty, the button shows `Content.ToString()`, so
  complex `Content` is not rendered; use `ButtonText`.
- **Icon**: `IconGeometry` (path data such as `M5,20H19V18H5M19,9H15V3H9V9H5L12,16L19,9Z`),
  `IconSize` (`16`), `IconBrush` (black), `IconPosition` (`Left`, `Top`, `Right`, `Bottom`),
  `IconTextSpacing` (`8`), `DisplayMode` (`IconAndText`, `IconOnly`, `TextOnly`).
- **States**: `HoverBackground`, `HoverForeground`, `HoverBorderBrush`, `HoverIconBrush`,
  `PressedBackground`, `PressedForeground`, `PressedBorderBrush`, `PressedIconBrush`.
  When unset, the hover and pressed backgrounds are derived from `Background` (lighter on hover,
  darker when pressed); set them to override. `PressedScale` (`1.0`, e.g. `0.97`), `PressedOffset` (`Point`),
  `AnimationDuration` (`TimeSpan`, `0:0:0.15`).
- **Layout**: `ContentPadding` (`12,6,12,6`), `IconMargin`, `TextMargin`, `IconAlignment` and
  `TextAlignment` (`Left`, `Top`, `Right`, `Bottom`, `Center`, `Stretch`); `HasShadow` (`False`).

```xml
<controlskitcontrols:ExtendedButton ButtonText="Download" Click="Download_Click"
    IconGeometry="M5,20H19V18H5M19,9H15V3H9V9H5L12,16L19,9Z" IconBrush="White"
    Background="#0891B2" HoverBackground="#0E7490" PressedScale="0.97"
    Foreground="White" HasShadow="True" />
```

## ExtendedSwitch

Namespace: `controlskitcontrols`, as for `ExtendedButton`.

A `ToggleButton`: use `IsChecked` (bindable, `Mode=TwoWay`), `Checked`, `Unchecked`, `Click`. It has no
label; put a `TextBlock` next to it.

- **Size**: `SwitchSize` = `Small` (36x20), `Medium` (default, 50x25), `Large` (64x32), or `Custom`, which
  keeps the `TrackWidth`, `TrackHeight` and `ThumbSize` you set. Setting those without
  `SwitchSize="Custom"` is overridden by the size preset.
- **Colors**: `TrackBrush` (light gray), `CheckedTrackBrush` (DodgerBlue), `ThumbBrush` (white),
  `HoverTrackBrush`, `HoverCheckedTrackBrush`, `HoverThumbBrush`, `TrackBorderBrush`,
  `TrackBorderThickness`, `ThumbBorderBrush`, `ThumbBorderThickness`.
- **Shape and motion**: `CornerRadius` and `ThumbCornerRadius` (fully rounded when unset), `ThumbMargin`
  (`2`), `HasShadow`, `AnimationStyle` (`Linear`, `EaseInOut` default, `Bounce`, `None`),
  `AnimationDuration` (`TimeSpan`, `0:0:0.2`).

```xml
<StackPanel Orientation="Horizontal">
  <controlskitcontrols:ExtendedSwitch IsChecked="{Binding IsDarkMode, Mode=TwoWay}"
      SwitchSize="Small" CheckedTrackBrush="#4F46E5" />
  <TextBlock Text="Dark mode" Margin="8,0,0,0" VerticalAlignment="Center" />
</StackPanel>
```
