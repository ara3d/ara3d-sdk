namespace Ara3D.Studio.API;

public abstract class SimpleCommand : IScriptedCommand
{
    public abstract string Name { get; }
    public abstract void Execute();
    public void Execute(IHostApplication hostApplication) => Execute();
    public bool CanExecute(IHostApplication hostApplication) => true;
}

