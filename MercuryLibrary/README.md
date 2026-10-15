# MercuryLibrary

MercuryLibrary is a specialized WinUI 3 component library designed to simplify the creation of high-performance, interactive graphical applications.
It provides a set of tools and controls built on top of Win2D, bridging the gap between standard WinUI controls and the needs of game-like or
real-time rendering environments.

The library is particularly useful for developers who need to manage complex sprite-based rendering, coordinate-based positioning, and low-latency
update loops within a modern Windows application using MVVM patterns.

### Version

Current version: `1.0.15`

### Features

- **GameCanvasView**: A high-performance canvas control for WinUI 3 powered by Win2D, designed for real-time rendering and game-like applications.
- **UpdateCallback**: A bindable property to hook into the game update loop from your ViewModel, allowing for clean MVVM-compliant logic.
- **PictureBox**: A sprite-like component for `GameCanvasView` supporting positioning (X, Y, Z), viewport clipping, blend modes, and image effects (scale, rotation, hue, saturation, contrast, brightness, opacity).
- **High Performance**: Leverages Win2D's hardware acceleration for smooth graphics.
- **WinUI 3 Ready**: Built on the latest Windows App SDK.
- **New String Is Null or Empty Extension**
- **New Dictionary GetOrDefault Extension**: Safely retrieve values from a dictionary with a default fallback. (v1.0.8)
 - **New BlendMode enum with attribute mapping (v1.0.9)**: Friendlier `BlendMode` values (e.g., `Multiply`, `Screen`, `Overlay`) map to Win2D's `BlendEffectMode` via a custom `BlendModeAttribute`.
 - **New EnumExtensions.GetAttribute helper (v1.0.9)**: Utility to read attributes from enum members, used to resolve the underlying `BlendEffectMode` at runtime.
 - **PictureBox BlendMode API update (v1.0.9)**: `PictureBox.BlendMode` now uses the library’s `BlendMode` enum for clearer, stable public API.
 - **Rendering pipeline glue for blends (v1.0.9)**: `GameCanvasView` uses the new enum+attribute approach to apply Win2D blend effects during drawing.
 - **New PictureBox image-effect properties (v1.0.10)**: `PictureBox` now exposes `ScaleX`/`ScaleY` (default `1.0`), `Rotate` (degrees), `Hue`, `Saturation`, `Contrast`, and `Brightness`, using decimals for percentage-style values (`1.0` = 100%) and neutral color values at `0`.
 - **Effect rendering chain in GameCanvasView (v1.0.10)**: `GameCanvasView` applies a Win2D effect chain (crop → hue → saturation → contrast → exposure → scale/rotate → opacity) per sprite on both the blended and non-blended draw paths.
 - **NuGet now ships the compiled XAML resources (v1.0.11)**: the package includes `MercuryLibrary.pri` alongside the assembly so apps consuming the library via NuGet can load `GameCanvasView` without a `XamlParseException`.
 - **Thread-safe draw loop (v1.0.12)**: `GameCanvasView` now snapshots the `Sprites` collection before rendering each frame, so mutating the bound collection from the UI thread (e.g. clearing/re-adding sprites) no longer throws `InvalidOperationException: Collection was modified`.
 - **Optional FPS counter (v1.0.12)**: `GameCanvasView` now exposes a bindable `ShowFps` property (default `false`) that toggles the on-canvas FPS overlay.
 - **Safe sprite loading before device creation (v1.0.13)**: `GameCanvasView` no longer throws `ArgumentException: The control does not currently have a CanvasDevice associated with it.` when sprites are added to the bound collection before the control has created its Win2D device (e.g. when a popup's canvas is populated as it opens).
 - **Bindable `ClearColor` (v1.0.14)**: `GameCanvasView` now exposes a bindable `ClearColor` property (default `Black`) so the canvas background color can be chosen by the consumer.
 - **Anchor overlay (v1.0.14)**: `GameCanvasView` now exposes bindable `AnchorRect` and `AnchorColor` properties to draw a reference rectangle (e.g. a red anchor box) behind all sprites; it is hidden by default (`AnchorColor` defaults to transparent).
 - **Blend modes composite against the background (v1.0.15)**: `GameCanvasView` now seeds its render accumulator with the `ClearColor` and anchor overlay, so blend-mode sprites blend against the canvas background and the sprites beneath them instead of against transparency (which previously made blended sprites disappear when no opaque sprite was behind them).
 - **Selection highlight (v1.0.15)**: `GameCanvasView` now exposes bindable `SelectionRect`, `SelectionColor` (default transparent/off) and `SelectionThickness` (default `2`) properties to draw a border rectangle on top of all sprites (e.g. to indicate a selected cell in an editor).

### Changes

- 1.0.15
  - `GameCanvasView` seeds its draw accumulator with the `ClearColor` + anchor so blend-mode sprites composite against the background scene (and each other) rather than transparency. Fixes blend modes appearing to "do nothing" when there is no opaque sprite behind the blended sprite.
  - Added bindable `SelectionRect`/`SelectionColor`/`SelectionThickness` properties to `GameCanvasView` that draw a selection border on top of the rendered sprites (off by default).
- 1.0.14
  - Added a bindable `ClearColor` property (default `Black`) to `GameCanvasView` for choosing the canvas background color.
  - Added bindable `AnchorRect`/`AnchorColor` properties to `GameCanvasView` that render a reference rectangle behind the sprites (transparent/off by default).
- 1.0.13
  - `GameCanvasView` now guards sprite-bitmap loading in its collection-changed handler with `ReadyToDraw` (and a try/catch), deferring the load to `CreateResources` when the Win2D device isn't available yet. This fixes the `ArgumentException`/"no CanvasDevice associated" crash when the bound `Sprites` collection is populated before the canvas has initialized.
- 1.0.12
  - `GameCanvasView` now iterates a snapshot of the `Sprites` collection in its draw loop, preventing "Collection was modified; enumeration operation may not execute" errors (and the associated flicker) when the bound collection is updated from the UI thread.
  - Added a bindable `ShowFps` property (default `false`) to `GameCanvasView` that controls whether the FPS counter is drawn on the canvas.
- 1.0.11
  - Fixed packaging so `MercuryLibrary.pri` (the compiled XAML/XBF resources) is included in the NuGet package next to `MercuryLibrary.dll`.
  - Resolves `Microsoft.UI.Xaml.Markup.XamlParseException` thrown from `GameCanvasView.InitializeComponent()` when the library was consumed as a NuGet package instead of a project reference.
- 1.0.10
  - Added `ImageComponents.PictureBox` image-effect properties: `ScaleX`, `ScaleY`, `Rotate`, `Hue`, `Saturation`, `Contrast`, and `Brightness`.
  - Percentage-style values use decimals (`1.0` = 100%); neutral color values default to `0`.
  - Updated `CanvasComponents.GameCanvasView` to render each sprite through a Win2D effect chain (crop → hue → saturation → contrast → exposure → scale/rotate → opacity) on both the blended and non-blended draw paths.
- 1.0.9
  - Introduced `Enumerations.BlendMode` with `[BlendModeAttribute]` mapping to Win2D `BlendEffectMode`.
  - Added `EnumExtensions.GetAttribute<T>` to fetch attributes from enum values.
  - Updated `ImageComponents.PictureBox.BlendMode` type from `BlendEffectMode?` to `BlendMode?`.
  - Updated `CanvasComponents.GameCanvasView` to resolve and apply blend effects using the new system.

### Installation

Add the `MercuryLibrary` project to your solution and reference it, or install the NuGet package:

```bash
dotnet add package MercuryLibrary --version 1.0.15
```

### Usage

#### XAML

Include the namespace in your XAML file:

```xml
xmlns:mercury="using:MercuryLibrary.CanvasComponents"
```

Then add the `GameCanvasView`. You can bind the `Sprites` collection and the `UpdateCallback`:

```xml

<mercury:GameCanvasView x:Name="GameCanvas"
                        UpdateRate="16.66666667"
                        Sprites="{x:Bind ViewModel.MySprites, Mode=OneWay}"
                        UpdateCallback="{x:Bind ViewModel.UpdateHandler, Mode=OneWay}"/>
```

#### C# (MVVM Update Loop)

In your ViewModel, define a property that returns the update handler method, and the handler method itself:

```csharp
using Microsoft.Graphics.Canvas.UI.Xaml;

public Action<ICanvasAnimatedControl, CanvasAnimatedUpdateEventArgs> UpdateHandler => OnUpdate;

public void OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
{
    // Update your logic here
    foreach (var sprite in MySprites)
    {
        sprite.X += 1;
    }
}
```

#### C# (Direct Collection Access)

Alternatively, you can populate the `Sprites` collection directly in C#:

```csharp
using MercuryLibrary.ImageComponents;
using Microsoft.Graphics.Canvas.Effects;
using Windows.Foundation;
using System.Collections.ObjectModel;

// ...

GameCanvas.Sprites = new ObservableCollection<PictureBox>
{
    new PictureBox { ImagePath = "Graphics/under.png", X = 100, Y = 100, Z = 0 },
    new PictureBox { ImagePath = "Graphics/moving.png", X = 200, Y = 200, Z = 1, Viewport = new Rect(48, 0, 48, 48) },
    new PictureBox { ImagePath = "Graphics/over.png", X = 300, Y = 300, Z = 2, BlendMode = BlendEffectMode.LinearDodge }
};
```

### Dependencies

- Microsoft.WindowsAppSDK (>= 1.6.241114003)
- Microsoft.Graphics.Win2D (>= 1.3.0)
- Microsoft.Windows.SDK.BuildTools (>= 10.0.26100.1742)

### License

This library is free to use for any purpose. Citing the author is not necessary, but appreciated.

Copyright © 2026
