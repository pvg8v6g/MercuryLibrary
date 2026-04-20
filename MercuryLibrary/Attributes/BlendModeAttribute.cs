using Microsoft.Graphics.Canvas.Effects;

namespace MercuryLibrary.Attributes;

public class BlendModeAttribute : Attribute
{
    #region Properties

    public BlendEffectMode BlendMode { get; set; }

    #endregion
}
