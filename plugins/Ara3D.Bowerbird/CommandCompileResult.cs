using Ara3D.Utils;
using Ara3D.Utils.Roslyn;

namespace Ara3D.Bowerbird;

/// <summary>
/// Outcome of compiling or resolving a cached command DLL.
/// </summary>
public class CommandCompileResult
{
    public CompilerOutput Compilation { get; init; }
    public FilePath OutputDll { get; init; }
    public bool FromCache { get; init; }
    public CommandCompileCacheKey CacheKey { get; init; }
}
