using System;
using Ara3D.Bowerbird.Interfaces;

namespace Ara3D.Bowerbird.Core
{
    public class ScriptingHost : IScriptingHost
    {
        public Action<IScriptingCommand> Action;

        public ScriptingHost(Action<IScriptingCommand> action)
            => Action = action;

        public void ExecuteCommand(IScriptingCommand cmd)
            => Action.Invoke(cmd);

        public static IScriptingHost Create(Action<IScriptingCommand> action)
            => new ScriptingHost(action);

        public static IScriptingHost CreateDefault()
            => new ScriptingHost(cmd => cmd.Execute(null));
    }
}