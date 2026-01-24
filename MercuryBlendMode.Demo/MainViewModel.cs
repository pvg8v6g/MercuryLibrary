using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MercuryBlendMode.Demo;

public class MainViewModel : ViewModelBase
{
    public ObservableCollection<GameObject> CompositeCollection { get; } = new();
    public Array AllBlendModes => Enum.GetValues(typeof(BlendMode));

    public ICommand AddSpriteCommand { get; }
    public ICommand RemoveSpriteCommand { get; }

    private Random _rng = new();
    private string[] _images = { "under.png", "over.png", "moving.png" };

    public MainViewModel()
    {
        AddSpriteCommand = new RelayCommand(_ => AddRandomSprite());
        RemoveSpriteCommand = new RelayCommand(p => { if (p is GameObject g) CompositeCollection.Remove(g); });

        // Initial setup: Background image
        CompositeCollection.Add(new GameObject 
        { 
            Name = "Background",
            ImagePath = "under.png", 
            X = 0, Y = 0, Z = 0, 
            BlendMode = BlendMode.SRC_OVER 
        });

        // Add a few blended sprites
        AddRandomSprite();
        AddRandomSprite();
    }

    private void AddRandomSprite()
    {
        var img = _images[_rng.Next(_images.Length)];
        CompositeCollection.Add(new GameObject
        {
            Name = $"Sprite {CompositeCollection.Count}",
            ImagePath = img,
            X = _rng.Next(50, 400),
            Y = _rng.Next(50, 300),
            Z = CompositeCollection.Count,
            BlendMode = (BlendMode)_rng.Next(Enum.GetValues(typeof(BlendMode)).Length)
        });
    }
}
