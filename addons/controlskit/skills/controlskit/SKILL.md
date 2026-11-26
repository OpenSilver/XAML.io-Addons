---
name: controlskit
description: >-
  Reference for the OpenSilver.ControlsKit controls: FlexPanel, StaggeredPanel,
  AdaptiveColumnsPanel, AnimatedNavigationBar, ExtendedButton and ExtendedSwitch.
  Use it when writing or changing XAML that uses one of them. Not needed for pages
  that only use built-in controls.
---

# OpenSilver.ControlsKit

Six controls from the `OpenSilver.ControlsKit` NuGet package, all in the assembly
`OpenSilver.ControlsKit.Controls`.

## Before using a control

1. The app project must reference the `OpenSilver.ControlsKit` package.
2. Declare the namespace you need on the root element of the XAML file. There are two,
   and using the wrong one is the most common error ("type not found"):

   | Prefix (convention) | Namespace | Types |
   |---|---|---|
   | `controlskit` | `clr-namespace:OpenSilver.ControlsKit;assembly=OpenSilver.ControlsKit.Controls` | `FlexPanel`, `StaggeredPanel`, `AdaptiveColumnsPanel`, `AnimatedNavigationBar` and the enums `JustifyContent`, `AlignContent`, `AlignItems` |
   | `controlskitcontrols` | `clr-namespace:OpenSilver.ControlsKit.Controls;assembly=OpenSilver.ControlsKit.Controls` | `ExtendedButton`, `ExtendedSwitch` and their enums |

## Which control

| Need | Use | Instead of |
|---|---|---|
| A row or column with CSS flexbox spacing (`space-between`, centering, wrap, grow) | `FlexPanel` | nested `StackPanel`s or a `Grid` with spacer columns |
| Tiles of different heights packed into columns (masonry) | `StaggeredPanel` | a `WrapPanel`, which leaves gaps |
| Side-by-side blocks that stack on narrow screens | `AdaptiveColumnsPanel` | code-behind that switches layouts on `SizeChanged` |
| A segmented tab bar with a sliding selection highlight | `AnimatedNavigationBar` | a `ListBox` restyled by hand |
| A button with an icon, hover and pressed colors, and a shadow | `ExtendedButton` | a `Button` with a custom `ControlTemplate` |
| An on/off toggle switch | `ExtendedSwitch` | a restyled `CheckBox` |

A plain `StackPanel`, `Grid` or `Button` is still the right choice when none of these
features is needed.

Details and examples:

- [reference/panels.md](reference/panels.md): `FlexPanel`, `StaggeredPanel`, `AdaptiveColumnsPanel`.
- [reference/controls.md](reference/controls.md): `AnimatedNavigationBar`, `ExtendedButton`, `ExtendedSwitch`.
