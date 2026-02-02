# MercuryLibrary

A WinUI 3 component library for Mercury projects.

### Version
Current version: `1.0.1`

### Features
- **GameCanvasView**: A high-performance canvas control for WinUI 3 powered by Win2D, designed for real-time rendering and game-like applications.
- **PictureBox**: A sprite-like component for `GameCanvasView` supporting positioning (X, Y, Z), viewport clipping, and blend modes.
- **WinUI 3 Ready**: Built on the latest Windows App SDK.

### Installation

Add the `MercuryLibrary` project to your solution and reference it, or install the NuGet package:

```bash
dotnet add package MercuryLibrary --version 1.0.1
```

### Usage

#### XAML
Include the namespace in your XAML file:

```xml
xmlns:mercury="using:MercuryLibrary.CanvasComponents"
```

Then add the `GameCanvasView`. You can also bind the `Sprites` property:

```xml
<mercury:GameCanvasView x:Name="GameCanvas" 
                        UpdateRate="16.66666667" 
                        Sprites="{x:Bind ViewModel.MySprites, Mode=OneWay}" />
```

#### C#
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
