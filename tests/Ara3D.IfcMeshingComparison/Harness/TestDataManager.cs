using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Harness;

/// <summary>Copies external IFC corpora into the local gitignored <c>data/ifc/</c> folder.</summary>
public static class TestDataManager
{
    public static void EnsureLocalIfcCopied(Action<string>? log = null)
    {
        TestFiles.LocalIfcDir.Create();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var folder in ExtraSourceFolders())
        {
            if (!folder.Exists())
            {
                log?.Invoke($"Skipping missing source folder: {folder}");
                continue;
            }

            foreach (var src in Directory.EnumerateFiles(folder, "*.ifc", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileName(src);
                if (!seen.Add(name))
                {
                    log?.Invoke($"Duplicate filename skipped: {name} (from {folder})");
                    continue;
                }

                var dest = TestFiles.LocalIfcDir.RelativeFile(name);
                if (!dest.Exists() || File.GetLastWriteTimeUtc(src) > File.GetLastWriteTimeUtc(dest))
                {
                    File.Copy(src, dest, overwrite: true);
                    log?.Invoke($"Copied {name}");
                }
            }
        }
    }

    static IEnumerable<DirectoryPath> ExtraSourceFolders()
    {
        yield return TestFiles.StudioDataDir;
        var extra = Environment.GetEnvironmentVariable("ARA3D_IFC_TEST_DIRS");
        if (string.IsNullOrWhiteSpace(extra))
            yield break;
        foreach (var part in extra.Split([';', Path.PathSeparator], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (Directory.Exists(part))
                yield return new DirectoryPath(part);
        }
    }
}
