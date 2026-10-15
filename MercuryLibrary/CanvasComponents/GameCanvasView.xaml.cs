using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;
using Windows.Foundation;
using MercuryLibrary.Attributes;
using MercuryLibrary.Extensions;
using MercuryLibrary.ImageComponents;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace MercuryLibrary.CanvasComponents;

public sealed partial class GameCanvasView
{
    public static readonly DependencyProperty UpdateRateProperty = DependencyProperty.Register(
        nameof(UpdateRate),
        typeof(double),
        typeof(GameCanvasView),
        new PropertyMetadata(16.66666667, OnTargetFpsChanged));

    public double UpdateRate
    {
        get => (double) GetValue(UpdateRateProperty);
        set => SetValue(UpdateRateProperty, value);
    }

    public static readonly DependencyProperty SpritesProperty = DependencyProperty.Register(
        nameof(Sprites),
        typeof(ObservableCollection<PictureBox>),
        typeof(GameCanvasView),
        new PropertyMetadata(null, OnSpritesChanged));

    public ObservableCollection<PictureBox> Sprites
    {
        get => (ObservableCollection<PictureBox>) GetValue(SpritesProperty);
        set => SetValue(SpritesProperty, value);
    }

    public static readonly DependencyProperty UpdateCallbackProperty = DependencyProperty.Register(
        nameof(UpdateCallback),
        typeof(Action<ICanvasAnimatedControl, CanvasAnimatedUpdateEventArgs>),
        typeof(GameCanvasView),
        new PropertyMetadata(null, OnUpdateCallbackChanged));

    private Action<ICanvasAnimatedControl, CanvasAnimatedUpdateEventArgs>? _cachedCallback;

    private static void OnUpdateCallbackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (GameCanvasView) d;
        view._cachedCallback = e.NewValue as Action<ICanvasAnimatedControl, CanvasAnimatedUpdateEventArgs>;
    }

    public Action<ICanvasAnimatedControl, CanvasAnimatedUpdateEventArgs>? UpdateCallback
    {
        get => (Action<ICanvasAnimatedControl, CanvasAnimatedUpdateEventArgs>?) GetValue(UpdateCallbackProperty);
        set => SetValue(UpdateCallbackProperty, value);
    }

    public static readonly DependencyProperty ShowFpsProperty = DependencyProperty.Register(
        nameof(ShowFps),
        typeof(bool),
        typeof(GameCanvasView),
        new PropertyMetadata(false, OnShowFpsChanged));

    private bool _cachedShowFps;

    private static void OnShowFpsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GameCanvasView) d)._cachedShowFps = (bool) e.NewValue;
    }

    public bool ShowFps
    {
        get => (bool) GetValue(ShowFpsProperty);
        set => SetValue(ShowFpsProperty, value);
    }

    public static readonly DependencyProperty ClearColorProperty = DependencyProperty.Register(
        nameof(ClearColor),
        typeof(Color),
        typeof(GameCanvasView),
        new PropertyMetadata(Colors.Black, OnClearColorChanged));

    private Color _cachedClearColor = Colors.Black;

    private static void OnClearColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (GameCanvasView) d;
        var color = (Color) e.NewValue;
        view._cachedClearColor = color;
        view.InternalCanvas.ClearColor = color;
    }

    public Color ClearColor
    {
        get => (Color) GetValue(ClearColorProperty);
        set => SetValue(ClearColorProperty, value);
    }

    public static readonly DependencyProperty AnchorRectProperty = DependencyProperty.Register(
        nameof(AnchorRect),
        typeof(Rect),
        typeof(GameCanvasView),
        new PropertyMetadata(default(Rect), OnAnchorRectChanged));

    private Rect _cachedAnchorRect;

    private static void OnAnchorRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GameCanvasView) d)._cachedAnchorRect = (Rect) e.NewValue;
    }

    public Rect AnchorRect
    {
        get => (Rect) GetValue(AnchorRectProperty);
        set => SetValue(AnchorRectProperty, value);
    }

    public static readonly DependencyProperty AnchorColorProperty = DependencyProperty.Register(
        nameof(AnchorColor),
        typeof(Color),
        typeof(GameCanvasView),
        new PropertyMetadata(Colors.Transparent, OnAnchorColorChanged));

    private Color _cachedAnchorColor = Colors.Transparent;

    private static void OnAnchorColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GameCanvasView) d)._cachedAnchorColor = (Color) e.NewValue;
    }

    public Color AnchorColor
    {
        get => (Color) GetValue(AnchorColorProperty);
        set => SetValue(AnchorColorProperty, value);
    }

    public static readonly DependencyProperty SelectionRectProperty = DependencyProperty.Register(
        nameof(SelectionRect),
        typeof(Rect),
        typeof(GameCanvasView),
        new PropertyMetadata(default(Rect), OnSelectionRectChanged));

    private Rect _cachedSelectionRect;

    private static void OnSelectionRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GameCanvasView) d)._cachedSelectionRect = (Rect) e.NewValue;
    }

    public Rect SelectionRect
    {
        get => (Rect) GetValue(SelectionRectProperty);
        set => SetValue(SelectionRectProperty, value);
    }

    public static readonly DependencyProperty SelectionColorProperty = DependencyProperty.Register(
        nameof(SelectionColor),
        typeof(Color),
        typeof(GameCanvasView),
        new PropertyMetadata(Colors.Transparent, OnSelectionColorChanged));

    private Color _cachedSelectionColor = Colors.Transparent;

    private static void OnSelectionColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GameCanvasView) d)._cachedSelectionColor = (Color) e.NewValue;
    }

    public Color SelectionColor
    {
        get => (Color) GetValue(SelectionColorProperty);
        set => SetValue(SelectionColorProperty, value);
    }

    public static readonly DependencyProperty SelectionThicknessProperty = DependencyProperty.Register(
        nameof(SelectionThickness),
        typeof(double),
        typeof(GameCanvasView),
        new PropertyMetadata(2.0, OnSelectionThicknessChanged));

    private float _cachedSelectionThickness = 2.0f;

    private static void OnSelectionThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GameCanvasView) d)._cachedSelectionThickness = (float) (double) e.NewValue;
    }

    public double SelectionThickness
    {
        get => (double) GetValue(SelectionThicknessProperty);
        set => SetValue(SelectionThicknessProperty, value);
    }

    private readonly Dictionary<string, CanvasBitmap> _bitmapCache = new();
    private readonly object _lock = new();
    private double _actualFps;
    private ObservableCollection<PictureBox>? _cachedSprites;

    public event EventHandler<CanvasAnimatedUpdateEventArgs>? GameUpdate;

    public event EventHandler<CanvasAnimatedDrawEventArgs>? GameDraw;

    public GameCanvasView()
    {
        InitializeComponent();

        InternalCanvas.CreateResources += (s, e) => { e.TrackAsyncAction(CreateResourcesAsync(s).AsAsyncAction()); };

        InternalCanvas.Update += (s, e) =>
        {
            _actualFps = 1.0 / e.Timing.ElapsedTime.TotalSeconds;
            GameUpdate?.Invoke(this, e);

            var callback = _cachedCallback;
            if (callback is null) return;
            callback.Invoke(s, e);
        };

        InternalCanvas.Draw += OnDrawInternal;

        UpdateInterval();
    }

    private static void OnTargetFpsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GameCanvasView) d).UpdateInterval();
    }

    private void UpdateInterval()
    {
        InternalCanvas.TargetElapsedTime = TimeSpan.FromMilliseconds(UpdateRate);
    }

    private static void OnSpritesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (GameCanvasView) d;

        if (e.OldValue is ObservableCollection<PictureBox> oldCollection)
        {
            oldCollection.CollectionChanged -= view.OnSpritesCollectionChanged;
        }

        if (e.NewValue is ObservableCollection<PictureBox> newCollection)
        {
            // Cache the collection reference for thread-safe access from draw thread
            view._cachedSprites = newCollection;

            newCollection.CollectionChanged += view.OnSpritesCollectionChanged;

            // Manually trigger load for existing items in the collection
            // This handles the case where sprites are added to the collection before binding
            if (newCollection.Count > 0)
            {
                view.OnSpritesCollectionChanged(newCollection,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                        newCollection.ToList()));
            }
        }
        else
        {
            view._cachedSprites = null;
        }
    }

    private async void OnSpritesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            // When sprites are added, load their bitmaps
            case NotifyCollectionChangedAction.Add when e.NewItems != null:
            {
                // If the control hasn't created its Win2D device yet (e.g. sprites are
                // added before the canvas is loaded/rendered, as happens when a popup's
                // canvas is populated on open), skip loading here. CreateResourcesAsync
                // will load every sprite once the device is available.
                if (!InternalCanvas.ReadyToDraw) break;

                foreach (PictureBox sprite in e.NewItems)
                {
                    if (string.IsNullOrEmpty(sprite?.ImagePath)) continue;

                    // Check if already loaded
                    lock (_lock)
                    {
                        if (_bitmapCache.ContainsKey(sprite.ImagePath)) continue;
                    }

                    var fullPath = Path.IsPathRooted(sprite.ImagePath)
                        ? sprite.ImagePath
                        : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sprite.ImagePath);

                    CanvasBitmap bitmap;
                    try
                    {
                        bitmap = await CanvasBitmap.LoadAsync(InternalCanvas, fullPath);
                    }
                    catch
                    {
                        // Device may have been lost between the ready check and the load;
                        // CreateResourcesAsync will reload the sprite when resources are recreated.
                        continue;
                    }

                    lock (_lock)
                    {
                        _bitmapCache[sprite.ImagePath] = bitmap;
                    }
                }

                break;
            }
            // When sprites are removed, dispose their bitmaps
            case NotifyCollectionChangedAction.Remove when e.OldItems != null:
            {
                foreach (PictureBox sprite in e.OldItems)
                {
                    if (string.IsNullOrEmpty(sprite?.ImagePath)) continue;

                    lock (_lock)
                    {
                        if (!_bitmapCache.TryGetValue(sprite.ImagePath, out var bitmap)) continue;
                        bitmap.Dispose();
                        _bitmapCache.Remove(sprite.ImagePath);
                    }
                }

                break;
            }
        }
    }

    private async Task CreateResourcesAsync(ICanvasAnimatedControl sender)
    {
        // Don't clear the cache - this was causing the issue!
        // Instead, only load bitmaps that aren't already cached

        var sprites = _cachedSprites;
        if (sprites is not null)
        {
            Debug.WriteLine($"[DEBUG_LOG] Loading {sprites.Count} sprites");

            // Create a snapshot to avoid collection modified exceptions
            var spriteList = sprites.ToArray();

            foreach (var sprite in spriteList)
            {
                if (string.IsNullOrEmpty(sprite.ImagePath)) continue;

                // Skip if already loaded
                bool alreadyLoaded;
                lock (_lock)
                {
                    alreadyLoaded = _bitmapCache.ContainsKey(sprite.ImagePath);
                }

                if (alreadyLoaded) continue;

                var fullPath = Path.IsPathRooted(sprite.ImagePath)
                    ? sprite.ImagePath
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sprite.ImagePath);

                var bitmap = await CanvasBitmap.LoadAsync(sender, fullPath);

                lock (_lock)
                {
                    _bitmapCache[sprite.ImagePath] = bitmap;
                }

                Debug.WriteLine($"[DEBUG_LOG] Loaded bitmap: {sprite.ImagePath}");
            }
        }
    }

    private void OnDrawInternal(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        try
        {
            GameDraw?.Invoke(this, args);

            // Use cached sprites to avoid COM exception from accessing dependency property on background thread
            var sprites = _cachedSprites;

            lock (_lock)
            {
                // Seed the accumulator with the background "scene": the clear color and the
                // anchor overlay. This way blend-mode sprites composite against the actual
                // background (e.g. the grey canvas) as well as the sprites beneath them,
                // instead of blending against transparency and disappearing.
                var accumulator = new CanvasRenderTarget(sender, sender.Size);
                using (var seedDs = accumulator.CreateDrawingSession())
                {
                    seedDs.Clear(_cachedClearColor);
                    if (_cachedAnchorColor.A > 0 && _cachedAnchorRect is { Width: > 0, Height: > 0 })
                    {
                        seedDs.FillRectangle(_cachedAnchorRect, _cachedAnchorColor);
                    }
                }

                try
                {
                    if (sprites is not null)
                    {
                        // Snapshot the collection so concurrent UI-thread mutations
                        // (Clear/Add during a rebuild) can't throw "Collection was modified"
                        var spriteSnapshot = sprites.ToArray();

                        foreach (var sprite in spriteSnapshot)
                        {
                            if (string.IsNullOrEmpty(sprite.ImagePath)) continue;
                            if (!_bitmapCache.TryGetValue(sprite.ImagePath, out var bitmap)) continue;

                            var blend = sprite.BlendMode?.GetAttribute<BlendModeAttribute>()?.BlendMode;
                            if (blend is not null)
                            {
                                // Blend this sprite against everything accumulated so far (the scene)
                                using var foreground = new CanvasRenderTarget(sender, sender.Size);
                                using (var fgDs = foreground.CreateDrawingSession())
                                {
                                    fgDs.Clear(Colors.Transparent);
                                    DrawSprite(fgDs, sender, bitmap, sprite);
                                }

                                using var effect = new BlendEffect
                                {
                                    Background = accumulator,
                                    Foreground = foreground,
                                    Mode = blend.Value
                                };

                                var blended = new CanvasRenderTarget(sender, sender.Size);
                                using (var ds = blended.CreateDrawingSession())
                                {
                                    ds.Clear(Colors.Transparent);
                                    ds.DrawImage(effect);
                                }

                                accumulator.Dispose();
                                accumulator = blended;
                            }
                            else
                            {
                                // Standard rendering: draw straight onto the accumulated scene
                                using var ds = accumulator.CreateDrawingSession();
                                DrawSprite(ds, sender, bitmap, sprite);
                            }
                        }
                    }

                    // Draw the final accumulated result to the screen
                    args.DrawingSession.DrawImage(accumulator);
                }
                finally
                {
                    accumulator.Dispose();
                }
            }

            // Draw the selection highlight on top of everything else
            if (_cachedSelectionColor.A > 0 && _cachedSelectionRect is { Width: > 0, Height: > 0 })
            {
                args.DrawingSession.DrawRectangle(_cachedSelectionRect, _cachedSelectionColor, _cachedSelectionThickness);
            }

            if (_cachedShowFps)
            {
                args.DrawingSession.DrawText($"FPS: {_actualFps:F1}", 10, 40, Colors.Yellow);
            }
        }
        catch (Exception ex)
        {
            // Draw error message on screen
            try
            {
                args.DrawingSession.DrawText($"ERROR: {ex.Message}", 10, 10, Colors.Red);
            }
            catch
            {
                // If we can't even draw the error, just ignore
            }
        }
    }

    private static void DrawSprite(CanvasDrawingSession session, ICanvasResourceCreator sender, CanvasBitmap bitmap,
        PictureBox sprite)
    {
        var viewport = sprite.Viewport ?? new Rect(0, 0, bitmap.Size.Width, bitmap.Size.Height);
        var disposables = new List<IDisposable>();

        try
        {
            // Normalize the (possibly cropped) source to a 0,0 origin so transforms operate on a known size
            var cropped = new CanvasRenderTarget(sender, (float) viewport.Width, (float) viewport.Height, bitmap.Dpi);
            disposables.Add(cropped);
            using (var croppedSession = cropped.CreateDrawingSession())
            {
                croppedSession.Clear(Colors.Transparent);
                croppedSession.DrawImage(bitmap, new Rect(0, 0, viewport.Width, viewport.Height), viewport);
            }

            ICanvasImage image = cropped;

            // Color adjustments (neutral values are 0, matching the AnimationEffect defaults)
            if (sprite.Hue != 0.0f)
            {
                var hue = new HueRotationEffect
                {
                    Source = image,
                    Angle = sprite.Hue * MathF.PI
                };
                disposables.Add(hue);
                image = hue;
            }

            if (sprite.Saturation != 0.0f)
            {
                var saturation = new SaturationEffect
                {
                    Source = image,
                    Saturation = Math.Clamp(1.0f + sprite.Saturation, 0.0f, 1.0f)
                };
                disposables.Add(saturation);
                image = saturation;
            }

            if (sprite.Contrast != 0.0f)
            {
                var contrast = new ContrastEffect
                {
                    Source = image,
                    Contrast = Math.Clamp(sprite.Contrast, -1.0f, 1.0f)
                };
                disposables.Add(contrast);
                image = contrast;
            }

            if (sprite.Brightness != 0.0f)
            {
                var brightness = new ExposureEffect
                {
                    Source = image,
                    Exposure = sprite.Brightness
                };
                disposables.Add(brightness);
                image = brightness;
            }

            // Scale and rotation about the sprite center
            if (sprite.ScaleX != 1.0f || sprite.ScaleY != 1.0f || sprite.Rotate != 0.0f)
            {
                var center = new Vector2((float) (viewport.Width / 2.0), (float) (viewport.Height / 2.0));
                var matrix = Matrix3x2.CreateScale(sprite.ScaleX, sprite.ScaleY, center) *
                             Matrix3x2.CreateRotation(sprite.Rotate * MathF.PI / 180.0f, center);

                var transform = new Transform2DEffect
                {
                    Source = image,
                    TransformMatrix = matrix
                };
                disposables.Add(transform);
                image = transform;
            }

            // Opacity (1.0 = 100%)
            if (sprite.Opacity != 1.0f)
            {
                var opacity = new OpacityEffect
                {
                    Source = image,
                    Opacity = Math.Clamp(sprite.Opacity, 0.0f, 1.0f)
                };
                disposables.Add(opacity);
                image = opacity;
            }

            session.DrawImage(image, new Vector2(sprite.X, sprite.Y));
        }
        finally
        {
            for (var i = disposables.Count - 1; i >= 0; i--)
            {
                disposables[i].Dispose();
            }
        }
    }
}
