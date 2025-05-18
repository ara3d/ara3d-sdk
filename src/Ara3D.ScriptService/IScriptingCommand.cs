namespace Ara3D.ScriptService
{
    public interface IScriptingCommand 
    {
        string Name { get; }
        void Execute(object argument);
    }
}    