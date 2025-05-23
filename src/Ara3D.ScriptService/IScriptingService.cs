using System;
using System.Collections.Generic;
using Ara3D.Logging;
using Ara3D.Services;

namespace Ara3D.ScriptService
{
    public interface IScriptingService 
        : ISingletonModelBackedService<ScriptingDataModel>, IDisposable
    {
        ScriptingOptions Options { get; }
        bool AutoRecompile { get; set; }
        ILogger Logger { get; set; }
        void Compile();
        IReadOnlyList<Type> Types { get; }
    }
}
