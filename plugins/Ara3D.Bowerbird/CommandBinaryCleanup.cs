using Ara3D.Utils;

namespace Ara3D.Bowerbird;

/// <summary>
/// Prunes old compiled command DLLs in a bin folder.
/// </summary>
public static class CommandBinaryCleanup
{
    public const int DefaultKeepCount = 10;

    public static void PruneOldDlls(DirectoryPath outputFolder, int keep = DefaultKeepCount)
    {
        if (!outputFolder.Exists())
            return;

        foreach (var fp in outputFolder.GetFiles("*.dll")
            .OrderByDescending(f => f.GetLastWriteTime())
            .Skip(keep))
        {
            try
            {
                fp.Delete();
            }
            catch
            {
                // Locked DLLs from prior runs may remain until the host exits.
            }
        }
    }
}
