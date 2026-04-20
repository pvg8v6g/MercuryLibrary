using Windows.Foundation;
using MercuryLibrary.Enumerations;
using MercuryLibrary.WinUI3Components;

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

    public BlendMode? BlendMode
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
