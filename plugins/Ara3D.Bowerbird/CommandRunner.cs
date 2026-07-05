using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.Bowerbird;

/// <summary>
/// Orchestrates compile, log, load, and optional execute for one command folder.
/// </summary>
public class CommandRunner
{
    readonly Dictionary<string, CachedCommand> _instanceCache = new(StringComparer.OrdinalIgnoreCase);

    public CommandCompiler Compiler { get; }
    public BowerbirdOptions Options { get; }
    public ILogger Logger { get; }

    public CommandRunner(BowerbirdOptions options, ILogger logger = null, CommandCompiler compiler = null)
        => (Options, Logger, Compiler) = (options, logger, compiler ?? new CommandCompiler(logger));

    public CommandRunResult Compile(CommandDescriptor descriptor, CancellationToken token = default)
        => Run(descriptor, execute: false, parameter: null, executor: null, token);

    public CommandRunResult Run(
        CommandDescriptor descriptor,
        object parameter = null,
        ICommandExecutor executor = null,
        CancellationToken token = default)
        => Run(descriptor, execute: true, parameter, executor, token);

    CommandRunResult Run(
        CommandDescriptor descriptor,
        bool execute,
        object parameter,
        ICommandExecutor executor,
        CancellationToken token)
    {
        CommandCompileResult compileResult = null;
        INamedCommand command = null;
        Exception exception = null;

        try
        {
            compileResult = Compiler.Compile(descriptor, Options, token: token);
            CompilationLogWriter.Write(descriptor, compileResult);

            var compileSuccess = compileResult.FromCache || compileResult.Compilation?.Success == true;
            if (!compileSuccess)
            {
                return new CommandRunResult
                {
                    Descriptor = descriptor,
                    CompileSuccess = false,
                    Compilation = compileResult.Compilation,
                    Diagnostics = compileResult.Compilation?.AllDiagnostics.ToList() ?? [],
                    CompileLogPath = descriptor.CompileLogPath,
                    FromCache = compileResult.FromCache,
                    OutputDll = compileResult.OutputDll,
                };
            }

            if (compileSuccess)
            {
                if (_instanceCache.TryGetValue(descriptor.FolderName, out var cached)
                    && cached.Fingerprint == compileResult.CacheKey.Fingerprint
                    && cached.OutputDll == compileResult.OutputDll)
                {
                    command = cached.Command;
                }
                else
                {
                    command = CommandLoader.Load(compileResult.OutputDll, descriptor.Manifest.TypeName);
                    _instanceCache[descriptor.FolderName] = new CachedCommand
                    {
                        Fingerprint = compileResult.CacheKey.Fingerprint,
                        OutputDll = compileResult.OutputDll,
                        Command = command,
                    };
                }
            }

            if (!execute)
            {
                return new CommandRunResult
                {
                    Descriptor = descriptor,
                    CompileSuccess = true,
                    LoadSuccess = true,
                    ExecuteSuccess = true,
                    Compilation = compileResult.Compilation,
                    Command = command,
                    Diagnostics = compileResult.Compilation?.AllDiagnostics.ToList() ?? [],
                    CompileLogPath = descriptor.CompileLogPath,
                    FromCache = compileResult.FromCache,
                    OutputDll = compileResult.OutputDll,
                };
            }

            if (executor != null)
                executor.Execute(command, parameter);
            else
                command.Execute(parameter);

            return new CommandRunResult
            {
                Descriptor = descriptor,
                CompileSuccess = true,
                LoadSuccess = true,
                ExecuteSuccess = true,
                Compilation = compileResult.Compilation,
                Command = command,
                Diagnostics = compileResult.Compilation?.AllDiagnostics.ToList() ?? [],
                CompileLogPath = descriptor.CompileLogPath,
                FromCache = compileResult.FromCache,
                OutputDll = compileResult.OutputDll,
            };
        }
        catch (Exception ex)
        {
            exception = ex;
            Logger?.LogError(ex);
            if (compileResult != null)
                CompilationLogWriter.Write(descriptor, compileResult, ex);

            return new CommandRunResult
            {
                Descriptor = descriptor,
                CompileSuccess = compileResult is { FromCache: true } || compileResult?.Compilation?.Success == true,
                LoadSuccess = command != null,
                ExecuteSuccess = false,
                Compilation = compileResult?.Compilation,
                Command = command,
                Exception = ex,
                Diagnostics = compileResult?.Compilation?.AllDiagnostics.ToList() ?? [],
                CompileLogPath = descriptor.CompileLogPath,
                FromCache = compileResult?.FromCache ?? false,
                OutputDll = compileResult != null ? compileResult.OutputDll : default,
            };
        }
    }

    readonly struct CachedCommand
    {
        public string Fingerprint { get; init; }
        public FilePath OutputDll { get; init; }
        public INamedCommand Command { get; init; }
    }
}
