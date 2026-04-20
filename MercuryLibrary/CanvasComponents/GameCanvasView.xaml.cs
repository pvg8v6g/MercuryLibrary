using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Windows.Foundation;
using MercuryLibrary.Attributes;
using MercuryLibrary.Extensions;
using MercuryLibrary.ImageComponents;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;

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

                    var bitmap = await CanvasBitmap.LoadAsync(InternalCanvas, fullPath);

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
            if (sprites is not null)
            {
                lock (_lock)
                {
                    // Create a render target to accumulate sprites for proper blending
                    CanvasRenderTarget? accumulator = null;

                    try
                    {
                        foreach (var sprite in sprites)
                        {
                            if (string.IsNullOrEmpty(sprite.ImagePath)) continue;
                            if (!_bitmapCache.TryGetValue(sprite.ImagePath, out var bitmap)) continue;

                            var blend = sprite.BlendMode?.GetAttribute<BlendModeAttribute>()?.BlendMode;
                            if (blend is not null)
                            {
                                // For blend modes, we need to blend with everything drawn so far
                                // Use the accumulator as the background (or transparent if first sprite)
                                using var background = accumulator ?? new CanvasRenderTarget(sender, sender.Size);
                                if (accumulator == null)
                                {
                                    using var bgDs = background.CreateDrawingSession();
                                    bgDs.Clear(Colors.Transparent);
                                }

                                // Create a foreground render target with the sprite positioned correctly
                                using var foreground = new CanvasRenderTarget(sender, sender.Size);
                                using (var fgDs = foreground.CreateDrawingSession())
                                {
                                    fgDs.Clear(Colors.Transparent);
                                    fgDs.DrawImage(bitmap, sprite.X, sprite.Y,
                                        sprite.Viewport ?? new Rect(0, 0, bitmap.Size.Width, bitmap.Size.Height),
                                        sprite.Opacity);
                                }

                                // Apply the blend effect
                                using var effect = new BlendEffect();
                                effect.Background = background;
                                effect.Foreground = foreground;
                                effect.Mode = blend.Value;

                                // Create a new accumulator with the blended result
                                var newAccumulator = new CanvasRenderTarget(sender, sender.Size);
                                using (var ds = newAccumulator.CreateDrawingSession())
                                {
                                    ds.Clear(Colors.Transparent);
                                    ds.DrawImage(effect);
                                }

                                // Dispose old accumulator and replace with new one
                                accumulator?.Dispose();
                                accumulator = newAccumulator;
                            }
                            else
                            {
                                // Draw without blend effect (standard rendering)
                                // Create new accumulator if it doesn't exist
                                if (accumulator is null)
                                {
                                    accumulator = new CanvasRenderTarget(sender, sender.Size);
                                    using var ds = accumulator.CreateDrawingSession();
                                    ds.Clear(Colors.Transparent);
                                    ds.DrawImage(bitmap, sprite.X, sprite.Y,
                                        sprite.Viewport ?? new Rect(0, 0, bitmap.Size.Width, bitmap.Size.Height),
                                        sprite.Opacity);
                                }
                                else
                                {
                                    // Add to existing accumulator
                                    var newAccumulator = new CanvasRenderTarget(sender, sender.Size);
                                    using (var ds = newAccumulator.CreateDrawingSession())
                                    {
                                        ds.Clear(Colors.Transparent);
                                        ds.DrawImage(accumulator);
                                        ds.DrawImage(bitmap, sprite.X, sprite.Y,
                                            sprite.Viewport ?? new Rect(0, 0, bitmap.Size.Width, bitmap.Size.Height),
                                            sprite.Opacity);
                                    }

                                    accumulator.Dispose();
                                    accumulator = newAccumulator;
                                }
                            }
                        }

                        // Draw the final accumulated result to the screen
                        if (accumulator != null)
                        {
                            args.DrawingSession.DrawImage(accumulator);
                        }
                    }
                    finally
                    {
                        accumulator?.Dispose();
                    }
                }
            }

            args.DrawingSession.DrawText($"FPS: {_actualFps:F1}", 10, 40, Colors.Yellow);
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
}
