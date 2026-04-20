using MercuryLibrary.Attributes;
using Microsoft.Graphics.Canvas.Effects;

namespace MercuryLibrary.Enumerations;

public enum BlendMode
{
    [BlendMode(BlendMode = BlendEffectMode.Multiply)]
    Multiply,

    [BlendMode(BlendMode = BlendEffectMode.Screen)]
    Screen,

    [BlendMode(BlendMode = BlendEffectMode.Darken)]
    Darken,

    [BlendMode(BlendMode = BlendEffectMode.Lighten)]
    Lighten,

    [BlendMode(BlendMode = BlendEffectMode.Dissolve)]
    Dissolve,

    [BlendMode(BlendMode = BlendEffectMode.ColorBurn)]
    ColorBurn,

    [BlendMode(BlendMode = BlendEffectMode.LinearBurn)]
    LinearBurn,

    [BlendMode(BlendMode = BlendEffectMode.DarkerColor)]
    DarkerColor,

    [BlendMode(BlendMode = BlendEffectMode.LighterColor)]
    LighterColor,

    [BlendMode(BlendMode = BlendEffectMode.ColorDodge)]
    ColorDodge,

    [BlendMode(BlendMode = BlendEffectMode.LinearDodge)]
    LinearDodge,

    [BlendMode(BlendMode = BlendEffectMode.Overlay)]
    Overlay,

    [BlendMode(BlendMode = BlendEffectMode.SoftLight)]
    SoftLight,

    [BlendMode(BlendMode = BlendEffectMode.HardLight)]
    HardLight,

    [BlendMode(BlendMode = BlendEffectMode.VividLight)]
    VividLight,

    [BlendMode(BlendMode = BlendEffectMode.LinearLight)]
    LinearLight,

    [BlendMode(BlendMode = BlendEffectMode.PinLight)]
    PinLight,

    [BlendMode(BlendMode = BlendEffectMode.HardMix)]
    HardMix,

    [BlendMode(BlendMode = BlendEffectMode.Difference)]
    Difference,

    [BlendMode(BlendMode = BlendEffectMode.Exclusion)]
    Exclusion,

    [BlendMode(BlendMode = BlendEffectMode.Hue)]
    Hue,

    [BlendMode(BlendMode = BlendEffectMode.Saturation)]
    Saturation,

    [BlendMode(BlendMode = BlendEffectMode.Color)]
    Color,

    [BlendMode(BlendMode = BlendEffectMode.Luminosity)]
    Luminosity,

    [BlendMode(BlendMode = BlendEffectMode.Subtract)]
    Subtract,

    [BlendMode(BlendMode = BlendEffectMode.Division)]
    Division,
}
