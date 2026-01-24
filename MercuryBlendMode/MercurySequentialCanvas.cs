using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace MercuryBlendMode;

public class MercurySequentialCanvas : ItemsControl
{
    static MercurySequentialCanvas()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MercurySequentialCanvas), new FrameworkPropertyMetadata(typeof(MercurySequentialCanvas)));
    }

    public MercurySequentialCanvas()
    {
        // Fallback Template if no Style is found
        var template = new ControlTemplate(typeof(MercurySequentialCanvas));
        var factory = new FrameworkElementFactory(typeof(ItemsPresenter));
        template.VisualTree = factory;
        this.Template = template;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new ContentPresenter();
    }
}

public class MercurySequentialPanel : Canvas
{
    private FrameworkElement _captureSource;

    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = InternalChildren.Cast<UIElement>().ToList();
        var sorted = children.OrderBy(Panel.GetZIndex).ToList();

        // One-time setup of capture source
        if (_captureSource == null)
        {
            _captureSource = VisualTreeHelper.GetParent(this) as FrameworkElement;
        }

        for (int i = 0; i < sorted.Count; i++)
        {
            var child = sorted[i];
            double x = GetLeft(child);
            double y = GetTop(child);
            if (double.IsNaN(x)) x = 0;
            if (double.IsNaN(y)) y = 0;

            child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
            
            var mode = MercuryCanvas.GetBlendMode(child);
            ApplySequentialEffect(child, mode);
        }

        return finalSize;
    }

    private void ApplySequentialEffect(UIElement child, BlendMode mode)
    {
        if (child is not FrameworkElement fe) return;

        if (mode == BlendMode.SRC_OVER)
        {
            if (fe.Effect is BlendModeEffectBase) fe.Effect = null;
            return;
        }

        // Use the specific effect class based on the mode
        BlendModeEffectBase effect = fe.Effect as BlendModeEffectBase;
        
        bool modeChanged = effect == null || GetModeFromEffect(effect) != mode;

        if (modeChanged)
        {
            var oldEffect = effect;
            effect = CreateEffectForMode(mode);
            
            // Preserve background brush if it exists
            if (oldEffect != null && effect != null)
            {
                effect.Bkg = oldEffect.Bkg;
            }
            
            fe.Effect = effect;
        }

        if (effect == null) return;

        // CRITICAL: We capture the PARENT of the Sequential Canvas to avoid recursion.
        // In the Demo, this is the Border or the Window.
        var parent = VisualTreeHelper.GetParent(this) as FrameworkElement;
        if (parent == null) return;

        // Use a persistent brush to avoid allocations
        if (effect.Bkg is not VisualBrush brush)
        {
            brush = new VisualBrush(parent)
            {
                ViewboxUnits = BrushMappingMode.Absolute,
                ViewportUnits = BrushMappingMode.Absolute,
                Stretch = Stretch.None,
                AutoLayoutContent = false
            };
            effect.Bkg = brush;
        }
        else
        {
            brush.Visual = parent;
        }

        try
        {
            // Calculate the exact offset relative to the parent container
            var offset = fe.TranslatePoint(new Point(0, 0), parent);
            if (fe.ActualWidth > 0 && fe.ActualHeight > 0)
            {
                brush.Viewbox = new Rect(offset.X, offset.Y, fe.ActualWidth, fe.ActualHeight);
                brush.Viewport = new Rect(0, 0, fe.ActualWidth, fe.ActualHeight);
            }
        }
        catch { }
    }

    private BlendModeEffectBase CreateEffectForMode(BlendMode mode)
    {
        return mode switch
        {
            BlendMode.ADD => new AddEffect(),
            BlendMode.MULTIPLY => new MultiplyEffect(),
            BlendMode.SCREEN => new ScreenEffect(),
            BlendMode.OVERLAY => new OverlayEffect(),
            BlendMode.DARKEN => new DarkenEffect(),
            BlendMode.LIGHTEN => new LightenEffect(),
            BlendMode.COLOR_DODGE => new ColorDodgeEffect(),
            BlendMode.COLOR_BURN => new ColorBurnEffect(),
            BlendMode.HARD_LIGHT => new HardLightEffect(),
            BlendMode.SOFT_LIGHT => new SoftLightEffect(),
            BlendMode.DIFFERENCE => new DifferenceEffect(),
            BlendMode.EXCLUSION => new ExclusionEffect(),
            _ => null
        };
    }

    private BlendMode GetModeFromEffect(BlendModeEffectBase effect)
    {
        if (effect is AddEffect) return BlendMode.ADD;
        if (effect is MultiplyEffect) return BlendMode.MULTIPLY;
        if (effect is ScreenEffect) return BlendMode.SCREEN;
        if (effect is OverlayEffect) return BlendMode.OVERLAY;
        if (effect is DarkenEffect) return BlendMode.DARKEN;
        if (effect is LightenEffect) return BlendMode.LIGHTEN;
        if (effect is ColorDodgeEffect) return BlendMode.COLOR_DODGE;
        if (effect is ColorBurnEffect) return BlendMode.COLOR_BURN;
        if (effect is HardLightEffect) return BlendMode.HARD_LIGHT;
        if (effect is SoftLightEffect) return BlendMode.SOFT_LIGHT;
        if (effect is DifferenceEffect) return BlendMode.DIFFERENCE;
        if (effect is ExclusionEffect) return BlendMode.EXCLUSION;
        return BlendMode.SRC_OVER;
    }

    protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
    {
        base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        // Do NOT use LayoutUpdated here, it causes the infinite loop crash.
    }
}
