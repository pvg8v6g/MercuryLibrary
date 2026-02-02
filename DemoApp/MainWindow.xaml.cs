using System.Collections.ObjectModel;
using MercuryLibrary.ImageComponents;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Xaml;

namespace DemoApp;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        GameCanvas.Sprites = new ObservableCollection<PictureBox>
        {
            new PictureBox { ImagePath = "Graphics/under.png", X = 100, Y = 100, Z = 0 },
            new PictureBox { ImagePath = "Graphics/moving.png", X = 200, Y = 200, Z = 1, BlendMode = BlendEffectMode.Exclusion },
            new PictureBox { ImagePath = "Graphics/over.png", X = 300, Y = 300, Z = 2, BlendMode = BlendEffectMode.LinearDodge }
        };
    }

    private void myButton_Click(object sender, RoutedEventArgs e)
    {
        // myButton.Content = "Clicked";
    }
}
