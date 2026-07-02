using Ara3D.Logging;
using Ara3D.Utils;
using Ara3D.Utils.Roslyn;

namespace Ara3D.Bowerbird;

/// <summary>
/// Compiles one command folder to folder/bin/{folderName}.dll.
/// </summary>
public class CommandCompiler
{
    public const string BinaryFolderName = "bin";

    public ReferenceResolver ReferenceResolver { get; }
    public ILogger Logger { get; }

    public CommandCompiler(ILogger logger = null, ReferenceResolver referenceResolver = null)
        => (Logger, ReferenceResolver) = (logger, referenceResolver ?? new ReferenceResolver(logger));

    public CompilerOutput Compile(
        CommandDescriptor descriptor,
        BowerbirdOptions options,
        CommandCompileCacheKey? cacheKey = null,
        CancellationToken token = default)
    {
        descriptor.OutputFolder.Create();

        var refs = ReferenceResolver.Resolve(descriptor.Folder, options.LibrariesFolder);
        var outputFile = descriptor.OutputDll;
        var compilerOptions = CompilerOptions.CreateDefault()
            .WithNewOutputFilePath(outputFile)
            .WithNewReferences(refs);

        var input = new CompilerInput(descriptor.SourceFiles, compilerOptions, refs);
        var compilation = input.Compile(Logger, token);
        return compilation.Output;
    }
}
