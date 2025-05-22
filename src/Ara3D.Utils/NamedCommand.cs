using System;
using System.Windows.Input;

namespace Ara3D.Utils
{
    /// <summary>
    /// A command represents a single executable action.
    /// It can be used as a way of naming and grouping a set of mutations
    /// to a repository in a way that can easily be reproduced and monitored.
    /// It can also be easily exposed in a UI (e.g., bound to a menu item)
    /// </summary>
    public interface INamedCommand : ICommand
    {
        string Name { get; }
        void NotifyCanExecuteChanged();
    }

    public interface INamedCommand<in T> : INamedCommand
    {
        bool CanExecute(T parameter);
        void Execute(T parameter);
    }

    public class NamedCommand : INamedCommand
    {
        public NamedCommand(Delegate execute, Delegate canExecute = null)
            : this(execute.Method?.Name ?? "", execute, canExecute)
        { }

        public NamedCommand(string name, Delegate execute, Delegate canExecute = null)
        {
            Name = name;
            ExecuteDelegate = execute;
            CanExecuteDelegate = canExecute;
        }

        public bool CanExecute(object parameter = null)
        {
            if (CanExecuteDelegate == null) 
                return true;
            
            if (CanExecuteDelegate.Method.GetParameters().Length > 0)
                return (bool)CanExecuteDelegate.DynamicInvoke(parameter);

            return (bool)CanExecuteDelegate.DynamicInvoke();
        }

        public void Execute(object parameter = null)
        {
            var nParams = ExecuteDelegate.Method.GetParameters().Length;
            if (nParams == 0)
                ExecuteDelegate.DynamicInvoke();
            else if (nParams == 1)
                ExecuteDelegate.DynamicInvoke(parameter);
            else
                throw new Exception($"Invalid number of arguments received 1, expected {nParams}");
        }

        public void NotifyCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public Delegate CanExecuteDelegate { get; }
        public Delegate ExecuteDelegate { get; }
        public event EventHandler CanExecuteChanged;
        public string Name { get; }
    }
}