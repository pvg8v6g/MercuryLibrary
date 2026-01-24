using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace MercuryBlendMode;

public class BlendEffect : ShaderEffect
{
    // Register s0 is implicit (the element itself)
    public static readonly DependencyProperty BkgProperty = RegisterPixelShaderSamplerProperty("Bkg", typeof(BlendEffect), 1);
    
    // CRITICAL: Change to float to match HLSL register(c0) perfectly
    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(float), typeof(BlendEffect), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(0)));

    public BlendEffect()
    {
        try 
        {
            var uri = new System.Uri("pack://application:,,,/MercuryBlendMode;component/Shaders/Blend.ps", UriKind.Absolute);
            PixelShader = new PixelShader { UriSource = uri };
        }
        catch { }

        UpdateShaderValue(BkgProperty);
        UpdateShaderValue(ModeProperty);
    }

    public Brush Bkg
    {
        get => (Brush)GetValue(BkgProperty);
        set => SetValue(BkgProperty, value);
    }

    public float Mode
    {
        get => (float)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public BlendMode BlendMode
    {
        get => (BlendMode)(int)(float)GetValue(ModeProperty);
        set => SetValue(ModeProperty, (float)(int)value);
    }
}
