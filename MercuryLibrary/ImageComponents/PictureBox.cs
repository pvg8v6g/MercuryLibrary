using MercuryLibrary.WinUI3Components;
using Microsoft.Graphics.Canvas.Effects;
using Windows.Foundation;

namespace MercuryLibrary.ImageComponents;

public class PictureBox : PropertyChangedUpdater
{
    public string ImagePath
    {
        get;
        set => SetField(ref field, value);
    } = string.Empty;

    public int X
    {
        get;
        set => SetField(ref field, value);
    }

    public int Y
    {
        get;
        set => SetField(ref field, value);
    }

    public int Z
    {
        get;
        set => SetField(ref field, value);
    }

    public BlendEffectMode? BlendMode
    {
        get;
        set => SetField(ref field, value);
    }

    public float Opacity
    {
        get;
        set => SetField(ref field, value);
    } = 1.0f;

    public Rect? Viewport
    {
        get;
        set => SetField(ref field, value);
    }
}
