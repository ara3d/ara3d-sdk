using System;
using System.Windows.Input;

namespace Ara3D.Utils.Wpf;

public sealed class CheckableCommand : ICommand
{
    public string? Name { get; }

    private readonly Func<bool> _getChecked;
    private readonly Action<bool> _setChecked;
    private readonly Func<bool>? _canExecute;

    public CheckableCommand(
        Func<bool> getChecked,
        Action<bool> setChecked,
        Func<bool>? canExecute = null,
        string? name = null)
    {
        _getChecked = getChecked ?? throw new ArgumentNullException(nameof(getChecked));
        _setChecked = setChecked ?? throw new ArgumentNullException(nameof(setChecked));
        _canExecute = canExecute;
        Name = name;
    }

    public bool IsChecked
    {
        get => _getChecked();
        set => _setChecked(value);
    }

    public bool CanExecute(object? parameter)
        => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;
        IsChecked = !IsChecked;
        RaiseCanExecuteChanged();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public void RaiseCanExecuteChanged()
        => CommandManager.InvalidateRequerySuggested();
}