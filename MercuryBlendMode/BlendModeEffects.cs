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
