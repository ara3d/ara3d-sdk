namespace Ara3D.Bowerbird.Interfaces
{
    public interface IScriptingCommand 
    {
        string Name { get; }
        void Execute(object argument);
    }
}    