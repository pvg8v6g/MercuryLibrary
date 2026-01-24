using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace MercuryBlendMode;

public abstract class BlendModeEffectBase : ShaderEffect
{
    public static readonly DependencyProperty BkgProperty = RegisterPixelShaderSamplerProperty("Bkg", typeof(BlendModeEffectBase), 1);

    protected BlendModeEffectBase()
    {
        UpdateShaderValue(BkgProperty);
    }

    public Brush Bkg
    {
        get => (Brush)GetValue(BkgProperty);
        set => SetValue(BkgProperty, value);
    }
}

public class AddEffect : BlendModeEffectBase
{
    private static readonly PixelShader _pixelShader = new() { UriSource = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/Add.ps", System.UriKind.Absolute) };
    public AddEffect() { this.PixelShader = _pixelShader; }
}

public class MultiplyEffect : BlendModeEffectBase
{
    private static readonly PixelShader _pixelShader = new() { UriSource = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/Multiply.ps", System.UriKind.Absolute) };
    public MultiplyEffect() { this.PixelShader = _pixelShader; }
}

public class ScreenEffect : BlendModeEffectBase
{
    private static readonly PixelShader _pixelShader = new() { UriSource = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/Screen.ps", System.UriKind.Absolute) };
    public ScreenEffect() { this.PixelShader = _pixelShader; }
}

public class OverlayEffect : BlendModeEffectBase
{
    private static readonly PixelShader _pixelShader = new() { UriSource = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/Overlay.ps", System.UriKind.Absolute) };
    public OverlayEffect() { this.PixelShader = _pixelShader; }
}

public class DarkenEffect : BlendModeEffectBase
{
    private static readonly PixelShader _pixelShader = new() { UriSource = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/Darken.ps", System.UriKind.Absolute) };
    public DarkenEffect() { this.PixelShader = _pixelShader; }
}

public class LightenEffect : BlendModeEffectBase
{
    private static readonly PixelShader _pixelShader = new() { UriSource = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/Lighten.ps", System.UriKind.Absolute) };
    public LightenEffect() { this.PixelShader = _pixelShader; }
}

public class ColorDodgeEffect : BlendModeEffectBase
{
    private static readonly PixelShader _pixelShader = new() { UriSource = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/ColorDodge.ps", System.UriKind.Absolute) };
    public ColorDodgeEffect() { this.PixelShader = _pixelShader; }
}

public class ColorBurnEffect : BlendModeEffectBase
{
    private static readonly PixelShader _pixelShader = new() { UriSource = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/ColorBurn.ps", System.UriKind.Absolute) };
    public ColorBurnEffect() { this.PixelShader = _pixelShader; }
}

public class HardLightEffect : BlendModeEffectBase
{
    private static readonly PixelShader _pixelShader = new() { UriSource = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/HardLight.ps", System.UriKind.Absolute) };
    public HardLightEffect() { this.PixelShader = _pixelShader; }
}

public class SoftLightEffect : BlendModeEffectBase
{
    private static readonly PixelShader _pixelShader = new() { UriSource = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/SoftLight.ps", System.UriKind.Absolute) };
    public SoftLightEffect() { this.PixelShader = _pixelShader; }
}

public class DifferenceEffect : BlendModeEffectBase
{
    private static readonly PixelShader _pixelShader = new() { UriSource = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/Difference.ps", System.UriKind.Absolute) };
    public DifferenceEffect() { this.PixelShader = _pixelShader; }
}

public class ExclusionEffect : BlendModeEffectBase
{
    private static readonly PixelShader _pixelShader = new() { UriSource = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/Exclusion.ps", System.UriKind.Absolute) };
    public ExclusionEffect() { this.PixelShader = _pixelShader; }
}
