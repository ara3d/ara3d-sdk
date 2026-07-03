using Ara3D.Logging;
using Ara3D.Utils;
using Ara3D.Utils.Roslyn;

namespace Ara3D.Bowerbird;

/// <summary>
/// Orchestrates compile, log, load, and optional execute for one command folder.
/// </summary>
public class CommandRunner
{
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
        CompilerOutput output = null;
        INamedCommand command = null;
        Exception exception = null;

        try
        {
            output = Compiler.Compile(descriptor, Options, token: token);
            CompilationLogWriter.Write(descriptor, output);

            if (output == null || !output.Success)
            {
                return new CommandRunResult
                {
                    Descriptor = descriptor,
                    CompileSuccess = false,
                    Compilation = output,
                    Diagnostics = output?.AllDiagnostics.ToList() ?? [],
                    CompileLogPath = descriptor.CompileLogPath,
                };
            }

            command = CommandLoader.Load(output.OutputFilePath, descriptor.Manifest.TypeName);

            if (!execute)
            {
                return new CommandRunResult
                {
                    Descriptor = descriptor,
                    CompileSuccess = true,
                    LoadSuccess = true,
                    ExecuteSuccess = true,
                    Compilation = output,
                    Command = command,
                    Diagnostics = output.AllDiagnostics.ToList(),
                    CompileLogPath = descriptor.CompileLogPath,
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
                Compilation = output,
                Command = command,
                Diagnostics = output.AllDiagnostics.ToList(),
                CompileLogPath = descriptor.CompileLogPath,
            };
        }
        catch (Exception ex)
        {
            exception = ex;
            Logger?.LogError(ex);
            CompilationLogWriter.Write(descriptor, output, ex);

            return new CommandRunResult
            {
                Descriptor = descriptor,
                CompileSuccess = output?.Success ?? false,
                LoadSuccess = command != null,
                ExecuteSuccess = false,
                Compilation = output,
                Command = command,
                Exception = ex,
                Diagnostics = output?.AllDiagnostics.ToList() ?? [],
                CompileLogPath = descriptor.CompileLogPath,
            };
        }
    }
}
