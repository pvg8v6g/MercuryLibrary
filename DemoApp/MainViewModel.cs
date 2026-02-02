using System.Collections.ObjectModel;
using Windows.Foundation;
using MercuryLibrary.ImageComponents;
using MercuryLibrary.WinUI3Components;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace DemoApp;

public class MainViewModel : PropertyChangedUpdater
{
    public ObservableCollection<PictureBox> Sprites { get; } = [];

    public Action<ICanvasAnimatedControl, CanvasAnimatedUpdateEventArgs> UpdateHandler => OnUpdate;

    public Action OnActivatedAction => OnActivated;

    private void OnActivated()
    {
        Sprites.Add(new PictureBox { ImagePath = "Graphics/under.png", X = 100, Y = 100, Z = 0 });
        Sprites.Add(new PictureBox
            { ImagePath = "Graphics/moving.png", X = 200, Y = 200, Z = 1, Viewport = new Rect(48, 0, 48, 48), Opacity = 0.5f });
        Sprites.Add(new PictureBox
            { ImagePath = "Graphics/over.png", X = 300, Y = 300, Z = 2, BlendMode = BlendEffectMode.LinearDodge });
    }

    private void OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (Sprites.Count <= 1) return;
        var moving = Sprites[1];
        moving.X += 1;
        if (moving.X > 800) moving.X = 0;
    }
}
