using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MercuryBlendMode.Demo;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    public RelayCommand(Action<object?> execute) => _execute = execute;
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => _execute(parameter);
    public event EventHandler? CanExecuteChanged;
}

public class GameObject : ViewModelBase
{
    private double _x;
    public double X { get => _x; set => SetField(ref _x, value); }

    private double _y;
    public double Y { get => _y; set => SetField(ref _y, value); }

    private int _z;
    public int Z { get => _z; set => SetField(ref _z, value); }

    private BlendMode _blendMode;
    public BlendMode BlendMode { get => _blendMode; set => SetField(ref _blendMode, value); }

    public string ImagePath { get; set; } = "";
    public string Name { get; set; } = "";
}
