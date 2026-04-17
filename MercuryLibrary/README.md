# MercuryLibrary

MercuryLibrary is a specialized WinUI 3 component library designed to simplify the creation of high-performance, interactive graphical applications.
It provides a set of tools and controls built on top of Win2D, bridging the gap between standard WinUI controls and the needs of game-like or
real-time rendering environments.

The library is particularly useful for developers who need to manage complex sprite-based rendering, coordinate-based positioning, and low-latency
update loops within a modern Windows application using MVVM patterns.

### Version

Current version: `1.0.8`

### Features

- **GameCanvasView**: A high-performance canvas control for WinUI 3 powered by Win2D, designed for real-time rendering and game-like applications.
- **UpdateCallback**: A bindable property to hook into the game update loop from your ViewModel, allowing for clean MVVM-compliant logic.
- **PictureBox**: A sprite-like component for `GameCanvasView` supporting positioning (X, Y, Z), viewport clipping, and blend modes.
- **High Performance**: Leverages Win2D's hardware acceleration for smooth graphics.
- **WinUI 3 Ready**: Built on the latest Windows App SDK.
- **New String Is Null or Empty Extension**
- **New Dictionary GetOrDefault Extension**: Safely retrieve values from a dictionary with a default fallback. (v1.0.8)

### Installation

Add the `MercuryLibrary` project to your solution and reference it, or install the NuGet package:

```bash
dotnet add package MercuryLibrary --version 1.0.8
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
