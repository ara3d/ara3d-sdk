using System;
using System.Collections.Generic;
using Ara3D.Logging;
using Ara3D.ScriptService;
using Ara3D.Services;

namespace Ara3D.Bowerbird.Interfaces
{
    public interface IScriptingService 
        : IScriptingHost, ISingletonModelBackedService<ScriptingDataModel>, IDisposable
    {
        ScriptingOptions Options { get; }
        bool AutoRecompile { get; set; }
        ILogger Logger { get; set; }
        void Compile();
        IReadOnlyList<IScriptingCommand> Commands { get; }
    }
}
