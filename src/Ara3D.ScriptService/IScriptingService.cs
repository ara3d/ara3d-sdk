using System;
using System.Collections.Generic;
using Ara3D.Logging;
using Ara3D.Services;
using Ara3D.Utils;

namespace Ara3D.ScriptService
{
    public interface IScriptingService : IDisposable
    {
        ScriptingDataModel Value { get; }
        ScriptingOptions Options { get; }
        bool AutoRecompile { get; set; }
        ILogger Logger { get; set; }
        void Compile();
        IReadOnlyList<Script> Types { get; }
    }
}
