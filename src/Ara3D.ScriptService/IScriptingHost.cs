namespace Ara3D.ScriptService
{
    public interface IScriptingHost
    {
        void ExecuteCommand(IScriptingCommand cmd);
    }
}