using System;
using System.Windows.Input;

namespace Ara3D.Utils.Wpf;

public class Command : ICommand
{
    public string Name;
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public Command(Action execute, Func<bool> canExecute = null, string name = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
        Name = name;
    }

    public bool CanExecute(object parameter)
        => _canExecute == null || _canExecute();

    public void Execute(object parameter)
        => _execute();

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}