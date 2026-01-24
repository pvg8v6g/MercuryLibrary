# MercuryBlendMode

A WPF library that provides Photoshop-like blend modes for images using GPU-accelerated Pixel Shaders.

## Features

- **GPU Accelerated**: Uses HLSL Pixel Shaders (ps_3_0) for high-performance blending.
- **Multiple Blend Modes**: Supports common modes like Multiply, Screen, Overlay, Soft Light, etc.
- **Easy Integration**: Simple `BlendedImage` control that inherits from `Image`.
- **Animation Support**: Built-in support for sprite sheet animations.
- **Viewport Support**: JavaFX-like `Viewport` property for cropping images.

## Installation

1. Add the `MercuryBlendMode` project/library to your solution.
2. Ensure your project references `MercuryBlendMode.csproj`.

## Usage

### 1. Add Namespace
Add the following namespace to your XAML file:

```xml
xmlns:mbm="clr-namespace:MercuryBlendMode;assembly=MercuryBlendMode"
```

### 2. Use MercuryCanvas
The `MercuryCanvas` container is designed for high-performance blending. It provides attached properties that can be applied to any child element.

#### Using in an ItemsControl (Dynamic Images)
When using an `ItemsControl` with dynamic images, you can apply the blend mode via `ItemContainerStyle`. Setting `AutoBackground="True"` allows the library to automatically capture the canvas background behind the item.

```xml
<mbm:MercuryCanvas Background="White">
    <ItemsControl ItemsSource="{Binding CompositeCollection}">
        <ItemsControl.ItemsPanel>
            <ItemsPanelTemplate>
                <mbm:MercuryCanvas Background="White"
                                  Width="{Binding GameData.GameWidth}"
                                  Height="{Binding GameData.GameHeight}" />
            </ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
        <ItemsControl.ItemContainerStyle>
            <Style TargetType="ContentPresenter">
                <!-- Bind the blend mode to your data model -->
                <Setter Property="mbm:MercuryCanvas.BlendMode" Value="{Binding SelectedMode}" />
                <!-- Automatically capture the background from the canvas -->
                <Setter Property="mbm:MercuryCanvas.AutoBackground" Value="True" />
                
                <Setter Property="Canvas.Left" Value="{Binding X}" />
                <Setter Property="Canvas.Top" Value="{Binding Y}" />
                <Setter Property="Panel.ZIndex" Value="{Binding Z}" />
            </Style>
        </ItemsControl.ItemContainerStyle>
        <ItemsControl.Resources>
            <DataTemplate DataType="{x:Type wpf:PictureBox}">
                <Image Source="{Binding Viewport}" />
            </DataTemplate>
        </ItemsControl.Resources>
    </ItemsControl>
</mbm:MercuryCanvas>
```

### Properties
#### MercuryCanvas Attached Properties
- **BlendMode**: An enum specifying the blending algorithm (e.g., `ADD`, `MULTIPLY`, `SCREEN`).
- **BackgroundBrush**: A `Brush` representing the background to blend against.
- **AutoBackground**: A `bool`. If `True`, the canvas automatically creates a `VisualBrush` of itself and manages the `Viewbox`/`Viewport` to match the element's position. This is the easiest way to blend with everything behind the element.

#### Available Blend Modes
- `SRC_OVER` (Default)
- `SRC_ATOP`
- `ADD`
- `MULTIPLY`
- `SCREEN`
- `OVERLAY`
- `DARKEN`
- `LIGHTEN`
- `COLOR_DODGE`
- `COLOR_BURN`
- `HARD_LIGHT`
- `SOFT_LIGHT`
- `DIFFERENCE`
- `EXCLUSION`

## Requirements

- .NET 10.0 or higher
- Windows (WPF)
- DirectX 9.0c compatible GPU (for Pixel Shader 3.0 support)
