using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Linq;

namespace MercuryBlendMode.Demo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        System.Diagnostics.Debug.WriteLine($"[DEBUG_LOG] Render Tier: {RenderCapability.Tier >> 16}");
        if ((RenderCapability.Tier >> 16) == 0)
        {
            MessageBox.Show("Warning: Hardware acceleration is disabled (Tier 0). Shader effects will not work.");
        }
    }

    private void BlurTestButton_Click(object sender, RoutedEventArgs e)
    {
        // Find all Image controls in the Sequential Canvas
        var images = FindVisualChildren<Image>(OuterCanvas).ToList();
        
        if (images.Any(img => img.Effect is BlurEffect))
        {
            foreach (var img in images) img.Effect = null;
        }
        else
        {
            foreach (var img in images) img.Effect = new BlurEffect { Radius = 10 };
        }
    }

    public static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj != null)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }

                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
    }
}
