using Ara3D.Utils;

namespace Ara3D.Bowerbird;

/// <summary>
/// One command folder: manifest, sources, and predictable output paths.
/// </summary>
public class CommandDescriptor
{
    public DirectoryPath Folder { get; }
    public string FolderName { get; }
    public CommandManifest Manifest { get; }
    public IReadOnlyList<FilePath> SourceFiles { get; }
    public FilePath ManifestPath { get; }

    public string DisplayName => Manifest.DisplayName;
    public DirectoryPath OutputFolder => Folder.RelativeFolder(CommandCompiler.BinaryFolderName);
    public FilePath OutputDll => OutputFolder.RelativeFile($"{FolderName}.dll");
    public FilePath CompileLogPath => OutputFolder.RelativeFile(CompilationLogWriter.LogFileName);

    public FilePath NewOutputDll() => OutputDll.ToUniqueTimeStampedFileName();

    public FilePath GetLatestCompiledDll()
        => OutputFolder.GetMostRecentFile($"{FolderName}*.dll");

    public CommandDescriptor(
        DirectoryPath folder,
        CommandManifest manifest,
        IReadOnlyList<FilePath> sourceFiles,
        FilePath manifestPath)
    {
        Folder = folder;
        FolderName = folder.GetFolderName();
        Manifest = manifest;
        SourceFiles = sourceFiles;
        ManifestPath = manifestPath;
    }
}
