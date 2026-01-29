using Microsoft.UI.Xaml;

namespace MercuryLibrary.SampleComponent;

public sealed partial class MySampleControl
{
    public MySampleControl()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Button clicked!";
    }
}
