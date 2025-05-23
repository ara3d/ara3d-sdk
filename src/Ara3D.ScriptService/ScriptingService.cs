using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ara3D.Logging;
using Ara3D.Services;
using Ara3D.Utils;
using Ara3D.Utils.Roslyn;

namespace Ara3D.ScriptService
{
    public class ScriptingService : 
        SingletonModelBackedService<ScriptingDataModel>, 
        IScriptingService
    {
        public Compiler Compiler => WatchingCompiler?.Compiler;
        public DirectoryWatchingCompiler WatchingCompiler { get; }
        public ILogger Logger { get; set; }
        public ScriptingOptions Options { get; }
        public Assembly Assembly => WatchingCompiler?.Compiler?.Assembly;
        public IReadOnlyList<Type> Types { get; private set; }
        public IReadOnlyList<object> Objects { get; private set; }

        public ScriptingService(IServiceManager app, ILogger logger, ScriptingOptions options)
            : base(app)
        {
            Logger = logger ?? new Logger(LogWriter.DebugWriter, "Scripting Service");
            Options = options;
            CreateInitialFolders();
            WatchingCompiler = new DirectoryWatchingCompiler(Logger, Options.ScriptsFolder, Options.LibrariesFolder);
            WatchingCompiler.RecompileEvent += WatchingCompilerRecompileEvent;
            UpdateDataModel();
            Types = Array.Empty<Type>();
            Objects = Array.Empty<object>();
        }

        public void ExecuteCommand(IScriptedCommand command)
        {
            try
            {
                Logger.Log($"Starting command execution: {command.Name}");
                command.Execute();
                Logger.Log($"Finished command execution: {command.Name}");
            }
            catch (Exception e)
            {
                Logger.LogError($"Command execution failed: {e}");
            }
        }

        public void Compile()
        {
            WatchingCompiler.Compile();
        }

        public override void Dispose()
        {
            base.Dispose();
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

        public IReadOnlyList<object> CreateObjects(IEnumerable<Type> types)
        {
            var objects = new List<object>();
            foreach (var t in types)
            {
                if (!t.HasDefaultConstructor())
                    continue;
                try
                {
                    var obj = Activator.CreateInstance(t);
                    Logger.Log($"Created object of type {t}");
                    objects.Add(obj);
                }
                catch (Exception e)
                {
                    Logger.LogError($"Error while checking default constructor for type {t}: {e}");
                }
            }

            return objects;
        }

        public void UpdateDataModel()
        {
            Types = Compiler?.ExportedTypes.ToArray() ?? Array.Empty<Type>();
            Objects = CreateObjects(Types);

            Repository.Value = new ScriptingDataModel()
            {
                Dll = Assembly?.Location ?? "",
                Directory = WatchingCompiler?.Directory,
                TypeNames = Types.Select(t => t.FullName).OrderBy(t => t).ToArray(),
                Files = Compiler?.Input?.SourceFiles?.Select(sf => sf.FilePath).OrderBy(x => x.Value).ToArray() ?? Array.Empty<FilePath>(),
                Assemblies = Compiler?.Refs?.Select(fp => fp.Value).ToList(),
                Diagnostics = Compiler?.Compilation?.Diagnostics?.Select(d => d.ToString()).ToArray() ?? Array.Empty<string>(),
                ParseSuccess = Compiler?.Input?.HasParseErrors == false,
                EmitSuccess = Compiler?.CompilationSuccess == true,
                LoadSuccess = Assembly != null,
                Options = Options,
            };
        }
    }
}