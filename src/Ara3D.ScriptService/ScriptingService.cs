using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
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
        public new IReadOnlyList<IScriptedCommand> Commands { get; private set; }

        public ScriptingService(IServiceManager app, ILogger logger, ScriptingOptions options)
            : base(app)
        {
            Logger = logger ?? new Logger(LogWriter.DebugWriter, "Scripting Service");
            Options = options;
            CreateInitialFolders();
            WatchingCompiler = new DirectoryWatchingCompiler(Logger, Options.ScriptsFolder, Options.LibrariesFolder);
            WatchingCompiler.RecompileEvent += WatchingCompilerRecompileEvent;
            UpdateDataModel();
            Commands = new List<IScriptedCommand>();
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
            // TODO: get the plugins from the assembly if things ar successful
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
            var types = Compiler?.ExportedTypes ?? Array.Empty<Type>();
            var cmds = new List<IScriptedCommand>();
            foreach (var type in types)
            {
                if (!typeof(IScriptedCommand).IsAssignableFrom(type))
                    continue;

                try
                {
                    var cmd = Activator.CreateInstance(type);
                    if (cmd == null)
                    {
                        Logger.LogError($"Failed to create instance of type {type}");
                        continue;
                    }
                    var bbCmd = cmd as IScriptedCommand;
                    if (bbCmd == null)
                    {
                        Logger.LogError($"Failed to cast instance of type {type} to IScriptedCommand");
                        continue;
                    }
                    cmds.Add(bbCmd);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Exception occurred while processing type {type}");
                }
            }

            Commands = cmds;

            Repository.Value = new ScriptingDataModel()
            {
                Dll = Assembly?.Location ?? "",
                Directory = WatchingCompiler?.Directory,
                TypeNames = types.Select(t => t.FullName).OrderBy(t => t).ToArray(),
                Files = Compiler?.Input?.SourceFiles?.Select(sf => sf.FilePath).OrderBy(x => x.Value).ToArray() ?? Array.Empty<FilePath>(),
                Assemblies = Compiler?.Refs?.Select(fp => fp.Value).ToList(),
                Diagnostics = Compiler?.Compilation?.Diagnostics?.Select(d => d.ToString()).ToArray() ?? Array.Empty<string>(),
                ParseSuccess = Compiler?.Input?.HasParseErrors == false,
                EmitSuccess = Compiler?.CompilationSuccess == true,
                LoadSuccess = Assembly != null,
                Options =  Options,
                Commands = cmds.Select(c => c.Name).ToList(),
            };
        }

        public IScriptedCommand CompileSingleCommand(FilePath file)
        {
            Logger?.Log($"Requested compilation of single command {file}");

            if (Compiler == null || Compiler?.CompilationSuccess == false)
            {
                Logger?.Log("Failed: no successful previous compilation to start from.");
                return null;
            }

            var options = WatchingCompiler.GetOptionsWithNewName();
            var refs = Compiler.Refs;
            var localCompiler = new Compiler(new[] { file }, refs, options, Logger,
                CancellationToken.None);

            if (!localCompiler.CompilationSuccess)
            {
                Logger?.Log("Failed: local compilation of command.");
                return null;
            }

            var commandTypes = localCompiler.ExportedTypes.Where(t => t.ImplementsInterface(typeof(IScriptedCommand))).ToList();
            if (commandTypes.Count == 0)
            {
                Logger?.Log("Failed: could not find exported type implementing IScriptedCommand");
                return null;
            }
            
            if (commandTypes.Count > 1)
            {
                Logger?.Log("Failed: ambiguous ... found multiple exported types implementing IScriptedCommand.");
                return null;
            }

            var instance = Activator.CreateInstance(commandTypes[0]);

            if (instance == null)
            {
                Logger?.Log($"Failed: could not create instance of of {commandTypes[0]}");
                return null;
            }

            return instance as IScriptedCommand;
        }
    }
}