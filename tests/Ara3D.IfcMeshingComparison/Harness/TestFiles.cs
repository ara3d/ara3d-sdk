using Ara3D.IfcLoader;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Harness;

/// <summary>Catalog of IFC test file folders with named accessors.</summary>
public static class TestFiles
{
    static readonly DirectoryPath ProjectRoot = new DirectoryPath(AppContext.BaseDirectory)
        .GetParent()
        .GetParent()
        .GetParent();

    /// <summary>Monorepo root (<c>studio/</c>), found by walking ancestors for AGENTS.md + ara3d-sdk + data.</summary>
    public static readonly DirectoryPath StudioRoot = FindStudioRoot();

    public static readonly DirectoryPath LocalIfcDir = ProjectRoot.RelativeFolder("data", "ifc");
    public static readonly DirectoryPath WebIfcBfastDir = ProjectRoot.RelativeFolder("data", "bfast", "webifc");
    public static readonly DirectoryPath ReportsDir = ProjectRoot.RelativeFolder("data", "reports");
    public static readonly DirectoryPath TempReportsDir = StudioRoot.RelativeFolder(".temp", "ifc-mesher");

    /// <summary>IFC corpus at the monorepo root (<c>studio/data/</c>).</summary>
    public static readonly DirectoryPath StudioDataDir = StudioRoot.RelativeFolder("data");

    public static FilePath Example => ResolveIfc("example.ifc");
    public static FilePath IfcOpenHouse => ResolveIfc("IfcOpenHouse_IFC4.ifc");
    public static FilePath SampleEntities => ResolveIfc("Sample_entities.ifc");
    public static FilePath Issue044CompositeProfile => ResolveIfc("ISSUE_044_test_IFCCOMPOSITEPROFILEDEF.ifc");
    public static FilePath Issue171SurfaceCurveSwept => ResolveIfc("ISSUE_171_IfcSurfaceCurveSweptAreaSolid.ifc");
    public static FilePath Ac20FzkHaus => ResolveIfc("AC20-FZK-Haus.ifc");
    public static FilePath SteelPlates => ResolveIfc("steelplates.ifc");
    public static FilePath Railing => ResolveIfc("railing.ifc");
    public static FilePath AiscSculptureBrep => ResolveIfc("171210AISC_Sculpture_brep.ifc");
    public static FilePath Small => ResolveIfc("small.ifc");
    public static FilePath DentalClinic => ResolveIfc("dental_clinic.ifc");
    public static FilePath Duplex => ResolveIfc("duplex.ifc");
    public static FilePath OfficeA => ResolveIfc("Office_A_20110811.ifc");
    public static FilePath Schependomlaan => ResolveIfc("schependomlaan.ifc");

    public static IEnumerable<FilePath> QuickComparisonFiles()
        => new[] { IfcOpenHouse, Example, SteelPlates };

    public static IEnumerable<FilePath> StudioDataFiles()
    {
        if (!StudioDataDir.Exists())
            yield break;

        foreach (var file in Directory.EnumerateFiles(StudioDataDir, "*.ifc", SearchOption.TopDirectoryOnly))
            yield return new FilePath(file);
    }

    /// <summary>Resolves an IFC by file name, or Ignores the test if missing.</summary>
    public static FilePath ResolveOrIgnore(string fileName)
    {
        var path = ResolveIfc(fileName);
        if (!path.Exists())
            Assert.Ignore($"Missing IFC test file: {fileName}");
        return path;
    }

    public static FilePath ResolveIfc(string fileName)
    {
        foreach (var folder in SearchFolders())
        {
            var candidate = folder.RelativeFile(fileName);
            if (candidate.Exists())
                return candidate;
        }
        return LocalIfcDir.RelativeFile(fileName);
    }

    static IEnumerable<DirectoryPath> SearchFolders()
    {
        yield return LocalIfcDir;
        yield return StudioDataDir;
        var extra = Environment.GetEnvironmentVariable("ARA3D_IFC_TEST_DIRS");
        if (string.IsNullOrWhiteSpace(extra))
            yield break;
        foreach (var part in extra.Split([';', Path.PathSeparator], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (Directory.Exists(part))
                yield return new DirectoryPath(part);
        }
    }

    public static IEnumerable<FilePath> AllKnownFiles()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var folder in SearchFolders())
        {
            if (!folder.Exists())
                continue;
            foreach (var file in Directory.EnumerateFiles(folder, "*.ifc", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileName(file);
                if (seen.Add(name))
                    yield return new FilePath(file);
            }
        }
    }

    public static void RequireExists(FilePath path)
    {
        if (!path.Exists())
            Assert.Ignore($"Missing IFC test file: {path}");
    }

    public static IfcFile LoadStep(FilePath path)
    {
        RequireExists(path);
        return new IfcFile(path, includeGeometry: false);
    }

    public static IfcFile LoadWithOracleGeometry(FilePath path)
    {
        RequireExists(path);
        return new IfcFile(path, includeGeometry: true);
    }

    static DirectoryPath FindStudioRoot()
    {
        foreach (var dir in new DirectoryPath(AppContext.BaseDirectory).GetSelfAndAncestors())
        {
            var agents = Path.Combine(dir, "AGENTS.md");
            var sdk = Path.Combine(dir, "ara3d-sdk");
            var data = Path.Combine(dir, "data");
            if (File.Exists(agents) && Directory.Exists(sdk) && Directory.Exists(data))
                return dir;
        }
        return ProjectRoot.GetParent().GetParent().GetParent();
    }
}
