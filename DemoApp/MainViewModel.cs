using System.Collections.ObjectModel;
using MercuryLibrary.Enumerations;
using MercuryLibrary.ImageComponents;
using MercuryLibrary.WinUI3Components;

namespace DemoApp;

public class MainViewModel : PropertyChangedUpdater
{
    private readonly PictureBox _sprite = new()
    {
        ImagePath = "Graphics/over.png",
        X = 250,
        Y = 150,
        Z = 1
    };

    public ObservableCollection<PictureBox> Sprites { get; } = [];

    public Action OnActivatedAction => OnActivated;

    public IReadOnlyList<BlendMode> BlendModes { get; } = Enum.GetValues<BlendMode>();

    public double X
    {
        get => _sprite.X;
        set
        {
            if (SetField(ref field, value)) _sprite.X = (int) value;
        }
    } = 250;

    public double Y
    {
        get => _sprite.Y;
        set
        {
            if (SetField(ref field, value)) _sprite.Y = (int) value;
        }
    } = 150;

    public double Opacity
    {
        get => _sprite.Opacity;
        set
        {
            if (SetField(ref field, value)) _sprite.Opacity = (float) value;
        }
    } = 1.0;

    public double ScaleX
    {
        get => _sprite.ScaleX;
        set
        {
            if (SetField(ref field, value)) _sprite.ScaleX = (float) value;
        }
    } = 1.0;

    public double ScaleY
    {
        get => _sprite.ScaleY;
        set
        {
            if (SetField(ref field, value)) _sprite.ScaleY = (float) value;
        }
    } = 1.0;

    public double Rotate
    {
        get => _sprite.Rotate;
        set
        {
            if (SetField(ref field, value)) _sprite.Rotate = (float) value;
        }
    }

    public double Hue
    {
        get => _sprite.Hue;
        set
        {
            if (SetField(ref field, value)) _sprite.Hue = (float) value;
        }
    }

    public double Contrast
    {
        get => _sprite.Contrast;
        set
        {
            if (SetField(ref field, value)) _sprite.Contrast = (float) value;
        }
    }

    public double Saturation
    {
        get => _sprite.Saturation;
        set
        {
            if (SetField(ref field, value)) _sprite.Saturation = (float) value;
        }
    }

    public double Brightness
    {
        get => _sprite.Brightness;
        set
        {
            if (SetField(ref field, value)) _sprite.Brightness = (float) value;
        }
    }

    public bool UseBlend
    {
        get;
        set
        {
            if (SetField(ref field, value)) ApplyBlend();
        }
    }

    public BlendMode SelectedBlendMode
    {
        get;
        set
        {
            if (SetField(ref field, value)) ApplyBlend();
        }
    } = BlendMode.LinearDodge;

    private void OnActivated()
    {
        if (Sprites.Count > 0) return;

        Sprites.Add(new PictureBox { ImagePath = "Graphics/under.png", X = 0, Y = 0, Z = 0 });
        Sprites.Add(_sprite);
    }

    private void ApplyBlend() => _sprite.BlendMode = UseBlend ? SelectedBlendMode : null;
}
