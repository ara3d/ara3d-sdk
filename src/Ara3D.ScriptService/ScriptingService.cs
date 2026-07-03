using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ara3D.Logging;
using Ara3D.Utils;
using Ara3D.Utils.Roslyn;

namespace Ara3D.ScriptService;

// NOTE: this is a LEGACY scripting service, that is used in Bowerbird only.
// TODO: move to bowerbird 
public class ScriptingService : IScriptingService
{
    public Compilation Compilation => WatchingCompiler?.Compilation;
    public DirectoryWatchingCompiler WatchingCompiler { get; }
    public ILogger Logger { get; set; }
    public ScriptingOptions Options { get; }
    public Assembly Assembly { get; set; }
    public IReadOnlyList<Script> Types { get; private set; } = [];
    public ScriptingDataModel Value { get; private set; }

    public ScriptingService(ILogger logger, ScriptingOptions options)
    {
        Logger = logger ?? new Logger(LogWriter.DebugWriter, "Scripting Service");
        Options = options;
        CreateInitialFolders();
        
        var compilerOptions = CompilerOptions.CreateDefault();

        WatchingCompiler = new DirectoryWatchingCompiler(Logger, Options.ScriptsFolder, Options.LibrariesFolder, false, compilerOptions);
        WatchingCompiler.RecompileEvent += WatchingCompilerRecompileEvent;
        UpdateDataModel();
    }

    public void Compile()
    {
        WatchingCompiler.Compile();
    }

    public void Dispose()
    {
        WatchingCompiler.Dispose();
    }

    private void WatchingCompilerRecompileEvent(object sender, EventArgs e)
    {
        UpdateDataModel();
    }

    public void CreateInitialFolders()
    {
        Options.ScriptsFolder.Create();
        Options.LibrariesFolder.Create();
    }

    public bool AutoRecompile
    {
        get => WatchingCompiler.AutoRecompile;
        set => WatchingCompiler.AutoRecompile = value;
    }

    public void UpdateDataModel()
    {
        var typeNameToFilePath = Compilation?.Output?.TypeToSourceMap ?? new();
        Assembly = null;
        Types = [];
        if (Compilation?.Output?.Success == true)
        {
            try
            {
                var asmFile = Compilation?.Output?.OutputFilePath;
                if (asmFile != null && asmFile?.Exists() == true)
                {
                    Assembly = Assembly.LoadFile(asmFile);
                    var scriptTypes = new List<Script>();
                    foreach (var type in Assembly?.ExportedTypes ?? [])
                    {
                        var path = typeNameToFilePath.GetValueOrDefault(type.FullName ?? "");
                        scriptTypes.Add(new Script(type, path));
                    }

                    Types = scriptTypes;
                }
            }
            catch (Exception e)
            {
                Assembly = null;
                Logger?.LogError(e);
            }
        }

        Value = new ScriptingDataModel()
        {
            Dll = Assembly?.Location ?? "",
            Directory = WatchingCompiler?.Directory ?? default,
            TypeNames = Types.Select(t => t.Type.FullName).OrderBy(t => t).ToArray(),
            Files = Compilation?.Input?.InputFiles?.OrderBy(x => x.Value).ToArray() ?? [],
            Assemblies = Compilation?.Input?.Refs?.Select(fp => fp.Value).ToList(),
            Diagnostics = Compilation?.Output?.Errors.ToArray() ?? [],
            EmitSuccess = Compilation?.Output?.Success == true,
            LoadSuccess = Assembly != null,
            Options = Options,
        };
    }
}
