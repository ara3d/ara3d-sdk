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

    /// <summary>Host executables that must not be passed to Roslyn (namespace collisions, etc.).</summary>
    static readonly HashSet<string> ExcludedHostAssemblyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Ara3D.Bowerbird.Console",
    };

    public ILogger Logger { get; }

    public ReferenceResolver(ILogger logger = null)
        => Logger = logger;

    public IReadOnlyList<FilePath> Resolve(
        DirectoryPath commandFolder,
        DirectoryPath globalLibrariesFolder,
        IReadOnlyList<FilePath> defaultReferences = null)
    {
        var loadedAssemblies = GetLoadedAssemblies();
        var refs = new List<FilePath>(defaultReferences ?? loadedAssemblies);
        AddRefsFromFile(commandFolder, refs, loadedAssemblies);
        AddFolderLibraries(commandFolder.RelativeFolder(LibrariesFolderName), refs);
        if (globalLibrariesFolder != null)
            AddFolderLibraries(globalLibrariesFolder, refs);
        return refs.Distinct().ToList();
    }

    /// <summary>refs.txt entries and per-folder/global Libraries only — stable for compile-cache fingerprints.</summary>
    public IReadOnlyList<FilePath> ResolveFingerprintRefs(
        DirectoryPath commandFolder,
        DirectoryPath globalLibrariesFolder)
    {
        var refs = new List<FilePath>();
        AddRefsFromFile(commandFolder, refs, GetLoadedAssemblies());
        AddFolderLibraries(commandFolder.RelativeFolder(LibrariesFolderName), refs);
        if (globalLibrariesFolder != null)
            AddFolderLibraries(globalLibrariesFolder, refs);
        return refs.Distinct().ToList();
    }

    public IReadOnlyList<FilePath> ResolveHostFingerprintRefs()
        => GetLoadedAssemblies()
            .Where(fp => IsHostVersionReference(fp.GetFileNameWithoutExtension()))
            .Distinct()
            .ToList();

    static IReadOnlyList<FilePath> GetLoadedAssemblies()
        => FilterCommandReferences(RoslynUtils.LoadedAssemblyLocations()).ToList();

    void AddRefsFromFile(DirectoryPath commandFolder, List<FilePath> refs, IReadOnlyList<FilePath> loadedAssemblies)
    {
        var refsFile = commandFolder.RelativeFile(RefsFileName);
        if (!refsFile.Exists())
            return;

        var thisFolder = new FilePath(typeof(ReferenceResolver).Assembly.Location).GetDirectory();
        foreach (var line in refsFile.ReadAllLines())
        {
            if (line.IsNullOrWhiteSpace())
                continue;

            var fp = loadedAssemblies.FirstOrDefault(f => f.Value.EndsWith(line));
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

    static IEnumerable<FilePath> FilterCommandReferences(IEnumerable<FilePath> loaded)
    {
        var entryPath = GetEntryAssemblyPath();
        foreach (var fp in loaded)
        {
            if (entryPath != null && fp == entryPath)
                continue;
            if (ExcludedHostAssemblyNames.Contains(fp.GetFileNameWithoutExtension()))
                continue;
            if (IsCommandBinOutput(fp))
                continue;
            yield return fp;
        }
    }

    static bool IsCommandBinOutput(FilePath fp)
        => fp.HasExtension(".dll")
            && fp.GetDirectory().GetFolderName().Equals(CommandCompiler.BinaryFolderName, StringComparison.OrdinalIgnoreCase);

    static bool IsHostVersionReference(string assemblyName)
        => assemblyName.Equals("Ara3D.Bowerbird", StringComparison.OrdinalIgnoreCase)
           || assemblyName.Equals("Ara3D.Studio.API", StringComparison.OrdinalIgnoreCase)
           || assemblyName.Equals("ara3d", StringComparison.OrdinalIgnoreCase);

    static FilePath GetEntryAssemblyPath()
    {
        var entry = Assembly.GetEntryAssembly();
        if (entry == null || entry.IsDynamic)
            return null;
        var loc = entry.Location;
        return loc.IsNullOrWhiteSpace() ? null : new FilePath(loc);
    }
}
