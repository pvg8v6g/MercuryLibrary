using Microsoft.UI.Xaml;

namespace DemoApp;

public sealed partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; } = new();

    public MainWindow()
    {
        InitializeComponent();
        this.Activated += (s, e) => ViewModel.OnActivatedAction?.Invoke();
    }
}
