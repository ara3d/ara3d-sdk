using System.Reflection;
using Ara3D.Logging;
using Ara3D.Utils;
using Ara3D.Utils.Roslyn;

namespace Ara3D.Bowerbird;

/// <summary>
/// Resolves metadata references for a command folder: defaults, refs.txt, global and per-folder libraries.
/// </summary>
public class ReferenceResolver
{
    public const string RefsFileName = "refs.txt";
    public const string LibrariesFolderName = "Libraries";

    public IReadOnlyList<FilePath> LoadedAssemblies { get; }
    public ILogger Logger { get; }

    public ReferenceResolver(ILogger logger = null)
        => (Logger, LoadedAssemblies) = (logger, RoslynUtils.LoadedAssemblyLocations().ToList());

    public IReadOnlyList<FilePath> Resolve(
        DirectoryPath commandFolder,
        DirectoryPath globalLibrariesFolder,
        IReadOnlyList<FilePath> defaultReferences = null)
    {
        var refs = new List<FilePath>(defaultReferences ?? LoadedAssemblies);
        AddRefsFromFile(commandFolder, refs);
        AddFolderLibraries(commandFolder.RelativeFolder(LibrariesFolderName), refs);
        if (globalLibrariesFolder != null)
            AddFolderLibraries(globalLibrariesFolder, refs);
        return refs.Distinct().ToList();
    }

    void AddRefsFromFile(DirectoryPath commandFolder, List<FilePath> refs)
    {
        var refsFile = commandFolder.RelativeFile(RefsFileName);
        if (!refsFile.Exists())
            return;

        var thisFolder = new FilePath(typeof(ReferenceResolver).Assembly.Location).GetDirectory();
        foreach (var line in refsFile.ReadAllLines())
        {
            if (line.IsNullOrWhiteSpace())
                continue;

            var fp = LoadedAssemblies.FirstOrDefault(f => f.Value.EndsWith(line));
            if (fp != null && fp.Exists())
            {
                refs.Add(fp);
                continue;
            }

            fp = new FilePath(line);
            if (fp.Exists())
            {
                refs.Add(fp);
                Assembly.LoadFile(fp.GetFullPath());
                continue;
            }

            fp = thisFolder.RelativeFile(line);
            if (fp.Exists())
            {
                refs.Add(fp);
                Assembly.LoadFile(fp.GetFullPath());
                continue;
            }

            Logger?.LogError($"Could not find referenced file: {line}");
        }
    }

    void AddFolderLibraries(DirectoryPath libsFolder, List<FilePath> refs)
    {
        if (libsFolder == null || !libsFolder.Exists())
            return;

        foreach (var file in libsFolder.GetAllFilesRecursively())
            refs.Add(file);
    }
}
