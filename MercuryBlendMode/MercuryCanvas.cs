using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MercuryBlendMode;

public class MercuryCanvas : Canvas
{
    public static readonly DependencyProperty BlendModeProperty = DependencyProperty.RegisterAttached(
        "BlendMode", typeof(BlendMode), typeof(MercuryCanvas), new PropertyMetadata(BlendMode.SRC_OVER, OnBlendModeChanged));

    public static void SetBlendMode(DependencyObject element, BlendMode value) => element.SetValue(BlendModeProperty, value);
    public static BlendMode GetBlendMode(DependencyObject element) => (BlendMode)element.GetValue(BlendModeProperty);

    private static void OnBlendModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] OnBlendModeChanged fired for {d.GetType().Name}. New Value: {e.NewValue}");
        Console.WriteLine($"[DEBUG_LOG] OnBlendModeChanged fired for {d.GetType().Name}. New Value: {e.NewValue}");
        if (d is FrameworkElement element)
        {
            ApplyEffect(element);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] OnBlendModeChanged: Target is NOT a FrameworkElement, it is a {d?.GetType().Name}");
        }
    }

    private static void ApplyEffect(FrameworkElement element)
    {
        var mode = GetBlendMode(element);
        System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] ApplyEffect called for {element.GetType().Name}, Name={element.Name}, Mode={mode}");
        Console.WriteLine($"[DEBUG_LOG] ApplyEffect called for {element.GetType().Name}, Name={element.Name}, Mode={mode}");
        
        if (mode == BlendMode.SRC_OVER)
        {
            if (element.Effect is BlendEffect)
            {
                element.Effect = null;
                System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] Effect cleared for {element.GetType().Name}");
            }
            return;
        }

        if (element.Effect is not BlendEffect effect)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] Creating NEW BlendEffect for {element.GetType().Name}");
            effect = new BlendEffect();
            element.Effect = effect;
        }

        effect.BlendMode = mode;
        
        // Final sanity check: if the effect is NOT BlendEffect, WPF might be ignoring it
        if (element.Effect != effect)
        {
             System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] WARNING: element.Effect was NOT set correctly! Current effect: {element.Effect?.GetType().Name}");
             element.Effect = effect;
        }

        var bkg = GetBackgroundBrush(element);
        if (bkg == null && GetAutoBackground(element))
        {
            // Try to find the parent MercuryCanvas
            var parent = VisualTreeHelper.GetParent(element);
            while (parent != null && parent is not MercuryCanvas)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent is MercuryCanvas canvas)
            {
                // Each element needs its OWN VisualBrush because of the Viewbox/Viewport
                bkg = canvas.CreateElementBrush(element);
                
                // Set it as the BackgroundBrush so we don't recreate it every time
                SetBackgroundBrush(element, bkg);
                
                // Hook up the background management
                canvas.SetupBackground(element);
            }
            else if (element.IsLoaded)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] ApplyEffect: No MercuryCanvas parent found for {element.GetType().Name} even though it is loaded.");
            }
        }
        
        if (bkg != null)
        {
            effect.Bkg = bkg;
        }
    }

    private VisualBrush CreateElementBrush(FrameworkElement element)
    {
        var brush = new VisualBrush(this)
        {
            ViewboxUnits = BrushMappingMode.Absolute,
            ViewportUnits = BrushMappingMode.Absolute,
            Stretch = Stretch.None, // Alignment depends on Viewbox/Viewport
            TileMode = TileMode.None,
            AlignmentX = AlignmentX.Left,
            AlignmentY = AlignmentY.Top,
            AutoLayoutContent = false // Prevents recursion issues
        };
        // Ensure the brush stays updated
        brush.RelativeTransform = new TranslateTransform(0, 0); 
        RenderOptions.SetCachingHint(brush, CachingHint.Cache);
        return brush;
    }

    public static readonly DependencyProperty AutoBackgroundProperty = DependencyProperty.RegisterAttached(
        "AutoBackground", typeof(bool), typeof(MercuryCanvas), new PropertyMetadata(false, OnAutoBackgroundChanged));

    public static void SetAutoBackground(DependencyObject element, bool value) => element.SetValue(AutoBackgroundProperty, value);
    public static bool GetAutoBackground(DependencyObject element) => (bool)element.GetValue(AutoBackgroundProperty);

    private static void OnAutoBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] OnAutoBackgroundChanged fired for {d.GetType().Name}. New Value: {e.NewValue}");
        Console.WriteLine($"[DEBUG_LOG] OnAutoBackgroundChanged fired for {d.GetType().Name}. New Value: {e.NewValue}");
        if (d is FrameworkElement element)
        {
            ApplyEffect(element);
        }
    }

    protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
    {
        base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        if (visualAdded is FrameworkElement element)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] Child added to MercuryCanvas: {element.GetType().Name}");
            Console.WriteLine($"[DEBUG_LOG] Child added to MercuryCanvas: {element.GetType().Name}");
            if (element.IsLoaded)
            {
                SetupBackground(element);
            }
            else
            {
                element.Loaded += (s, e) => 
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] Child loaded in MercuryCanvas: {element.GetType().Name}");
                    Console.WriteLine($"[DEBUG_LOG] Child loaded in MercuryCanvas: {element.GetType().Name}");
                    SetupBackground(element);
                };
            }
        }
    }

    private void SetupBackground(FrameworkElement element)
    {
        System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] SetupBackground for {element.GetType().Name}, Name={element.Name}");
        Console.WriteLine($"[DEBUG_LOG] SetupBackground for {element.GetType().Name}, Name={element.Name}");
        ApplyEffect(element);

        if (GetAutoBackground(element) && element.Effect is BlendEffect effect)
        {
            EventHandler handler = (s, e) =>
            {
                if (!element.IsLoaded) return;
                
                // ContentPresenter might have 0 size if it's just a container
                // but we need to check the actual rendered size.
                if (element.ActualWidth == 0 || element.ActualHeight == 0) return;
                
                var rect = new Rect(0, 0, element.ActualWidth, element.ActualHeight);
                try
                {
                    // Find the canvas again to be sure (in case element moved in tree)
                    var parentCanvas = VisualTreeHelper.GetParent(element) as MercuryCanvas;
                    if (parentCanvas == null)
                    {
                        var parent = VisualTreeHelper.GetParent(element);
                        while (parent != null && parent is not MercuryCanvas)
                        {
                            parent = VisualTreeHelper.GetParent(parent);
                        }
                        parentCanvas = parent as MercuryCanvas;
                    }
                    
                    if (parentCanvas == null) 
                    {
                         // If we can't find the parent, we might be in the middle of a layout pass.
                         // Don't log too much as it might spam.
                         return;
                    }

                    // CRITICAL: We need the exact location of the element relative to the MercuryCanvas
                    Point offset;
                    try
                    {
                        // Use TranslatePoint to get the offset relative to the MercuryCanvas
                        // This is usually more reliable than PointToScreen for internal layout
                        offset = element.TranslatePoint(new Point(0, 0), parentCanvas);
                    }
                    catch
                    {
                         return;
                    }
                    
                    rect.Offset(offset.X, offset.Y);
                    
                    if (effect.Bkg is VisualBrush brush)
                    {
                        // Ensure the brush is looking at the RIGHT visual (the parent canvas)
                        if (brush.Visual != parentCanvas) brush.Visual = parentCanvas;
                        
                        // Viewbox defines WHAT part of the Visual to capture (Absolute units)
                        brush.Viewbox = rect;
                        // Viewport defines WHERE to draw that capture on the element (Absolute units)
                        brush.Viewport = new Rect(0, 0, element.ActualWidth, element.ActualHeight);

                        // Diagnostic log
                        // System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] Updated brush for {element.GetType().Name}. Viewbox: {brush.Viewbox}, Viewport: {brush.Viewport}, Offset: {offset}");
                    }
                }
                catch
                {
                    // Ignore
                }
            };

            element.SizeChanged += (s, e) => handler(s, e);
            
            // LayoutUpdated can fire too much, but it's needed if position changes without SizeChanged.
            element.LayoutUpdated += handler;
            
            // If the parent canvas itself changes size, we need to update.
            this.SizeChanged += (s, e) => handler(s, e);

            handler(null, EventArgs.Empty);
        }
    }

    public static readonly DependencyProperty BackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "BackgroundBrush", typeof(Brush), typeof(MercuryCanvas), new PropertyMetadata(null, OnBackgroundBrushChanged));

    public static void SetBackgroundBrush(DependencyObject element, Brush value) => element.SetValue(BackgroundBrushProperty, value);
    public static Brush GetBackgroundBrush(DependencyObject element) => (Brush)element.GetValue(BackgroundBrushProperty);

    private static void OnBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] OnBackgroundBrushChanged fired for {d.GetType().Name}.");
        Console.WriteLine($"[DEBUG_LOG] OnBackgroundBrushChanged fired for {d.GetType().Name}.");
        if (d is FrameworkElement element)
        {
            ApplyEffect(element);
        }
    }
}
