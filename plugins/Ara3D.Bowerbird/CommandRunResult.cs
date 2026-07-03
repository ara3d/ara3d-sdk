using Ara3D.Utils;
using Ara3D.Utils.Roslyn;

namespace Ara3D.Bowerbird;

/// <summary>
/// Result of compiling, loading, and optionally executing a command.
/// </summary>
public class CommandRunResult
{
    public CommandDescriptor Descriptor { get; init; }
    public bool CompileSuccess { get; init; }
    public bool LoadSuccess { get; init; }
    public bool ExecuteSuccess { get; init; }
    public CompilerOutput Compilation { get; init; }
    public INamedCommand Command { get; init; }
    public Exception Exception { get; init; }
    public IReadOnlyList<string> Diagnostics { get; init; } = Array.Empty<string>();
    public FilePath CompileLogPath { get; init; }
    public bool FromCache { get; init; }
    public FilePath OutputDll { get; init; }

    public bool Success => CompileSuccess && LoadSuccess && ExecuteSuccess;
}
