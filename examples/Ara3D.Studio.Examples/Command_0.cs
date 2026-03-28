using System.Windows;
using MessageBox = System.Windows.MessageBox;

public class Command_0 : IScriptedCommand
{
    public string Name 
        => "Command_0";

    public void Execute(IHostApplication app)
    {
        MessageBox.Show(Name);
    }

    public bool CanExecute(IHostApplication app)
    {
        return true;
    }
}