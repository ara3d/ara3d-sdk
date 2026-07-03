using Ara3D.Logging;
using Ara3D.Utils;
using Ara3D.Utils.Roslyn;

namespace Ara3D.Bowerbird;

/// <summary>
/// Compiles one command folder to folder/bin/{folderName}-{timestamp}.dll.
/// </summary>
public class CommandCompiler
{
    public const string BinaryFolderName = "bin";

    public ReferenceResolver ReferenceResolver { get; }
    public ILogger Logger { get; }

    public CommandCompiler(ILogger logger = null, ReferenceResolver referenceResolver = null)
        => (Logger, ReferenceResolver) = (logger, referenceResolver ?? new ReferenceResolver(logger));

    public CommandCompileResult Compile(
        CommandDescriptor descriptor,
        BowerbirdOptions options,
        CancellationToken token = default)
    {
        descriptor.OutputFolder.Create();

        var refs = ReferenceResolver.Resolve(descriptor.Folder, options.LibrariesFolder);
        var fingerprintRefs = ReferenceResolver.ResolveFingerprintRefs(descriptor.Folder, options.LibrariesFolder);
        var cacheKey = CommandCompileCacheKey.Compute(descriptor, fingerprintRefs);

        if (options.EnableCompileCache)
        {
            var cachedDll = CommandCompileCache.TryGet(descriptor, cacheKey);
            if (cachedDll != null)
            {
                return new CommandCompileResult
                {
                    CacheKey = cacheKey,
                    OutputDll = cachedDll.Value,
                    FromCache = true,
                };
            }
        }

        var outputFile = descriptor.NewOutputDll();
        var compilerOptions = CompilerOptions.CreateDefault()
            .WithNewOutputFilePath(outputFile)
            .WithNewReferences(refs);

        var input = new CompilerInput(descriptor.SourceFiles, compilerOptions, refs);
        var compilation = input.Compile(Logger, token);
        var output = compilation.Output;
        if (output?.Success == true)
        {
            CommandCompileCache.Write(descriptor, cacheKey, output.OutputFilePath);
            CommandBinaryCleanup.PruneOldDlls(descriptor.OutputFolder);
        }

        return new CommandCompileResult
        {
            CacheKey = cacheKey,
            OutputDll = output?.Success == true ? output.OutputFilePath.GetFullPath() : default,
            FromCache = false,
            Compilation = output,
        };
    }
}
