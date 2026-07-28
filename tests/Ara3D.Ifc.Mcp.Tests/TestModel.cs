using Ara3D.Ifc.Mcp;

namespace Ara3D.Ifc.Mcp.Tests;

/// <summary>Locates the checked-in sample models. Tests that need one are skipped rather than
/// failed when the data folder is absent, so the suite still runs on a bare checkout.</summary>
public static class TestModel
{
    public const string FzkHaus = "AC20-FZK-Haus.ifc";

    public static string RequirePath(string fileName)
    {
        var path = FindPath(fileName);
        if (path == null)
            Assert.Ignore($"Sample model {fileName} not found under a 'data' folder.");
        return path!;
    }

    public static IfcSession RequireSession(IfcSessionCache cache, string fileName)
        => cache.Get(RequirePath(fileName));

    private static string? FindPath(string fileName)
    {
        var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "data", fileName);
            if (File.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }

        return null;
    }
}
