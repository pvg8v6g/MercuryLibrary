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
            if (mode != BlendMode.SRC_OVER)
            {
                ApplySequentialEffect(child);
            }
            else
            {
                if (child is FrameworkElement fe && fe.Effect is BlendModeEffectBase) fe.Effect = null;
            }
        }

        return finalSize;
    }

    private void ApplySequentialEffect(UIElement child)
    {
        if (child is not FrameworkElement fe) return;

        var mode = MercuryCanvas.GetBlendMode(child);
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
            effect = CreateEffectForMode(mode);
            fe.Effect = effect;
        }

        if (effect == null) return;

        // Use the parent container as the source
        var parent = VisualTreeHelper.GetParent(this) as FrameworkElement;
        if (parent == null) return;

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
            var offset = fe.TranslatePoint(new Point(0, 0), parent);
            if (fe.ActualWidth > 0 && fe.ActualHeight > 0)
            {
                brush.Viewbox = new Rect(offset.X, offset.Y, fe.ActualWidth, fe.ActualHeight);
                brush.Viewport = new Rect(0, 0, fe.ActualWidth, fe.ActualHeight);
                
                // Diagnostic logging
                System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] Applied {mode} to {fe.GetType().Name}. Offset: {offset}");
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
            _ => null
        };
    }

    private BlendMode GetModeFromEffect(BlendModeEffectBase effect)
    {
        if (effect is AddEffect) return BlendMode.ADD;
        if (effect is MultiplyEffect) return BlendMode.MULTIPLY;
        if (effect is ScreenEffect) return BlendMode.SCREEN;
        return BlendMode.SRC_OVER;
    }

    protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
    {
        base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        // Do NOT use LayoutUpdated here, it causes the infinite loop crash.
    }
}
