namespace Ara3D.ScriptService
{
    public interface IScriptedCommand 
    {
        string Name { get; }
        void Execute();
    }
}    