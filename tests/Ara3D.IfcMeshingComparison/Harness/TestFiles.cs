using Ara3D.IfcLoader;
using Ara3D.IO.StepParser;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Harness;

/// <summary>Catalog of IFC test file folders with named accessors.</summary>
public static class TestFiles
{
    static readonly DirectoryPath ProjectRoot = new DirectoryPath(AppContext.BaseDirectory)
        .GetParent()
        .GetParent()
        .GetParent();

    public static readonly DirectoryPath LocalIfcDir = ProjectRoot.RelativeFolder("data", "ifc");
    public static readonly DirectoryPath WebIfcBfastDir = ProjectRoot.RelativeFolder("data", "bfast", "webifc");
    public static readonly DirectoryPath ReportsDir = ProjectRoot.RelativeFolder("data", "reports");

    /// <summary>IFC corpus at the monorepo root (<c>studio/data/</c>).</summary>
    public static readonly DirectoryPath StudioDataDir = ProjectRoot
        .GetParent()
        .GetParent()
        .GetParent()
        .RelativeFolder("data");

    public static readonly DirectoryPath WebIfcPublic = new(@"C:\Users\cdigg\git\ifc-sharp\engine_web-ifc\tests\ifcfiles\public");
    public static readonly DirectoryPath SpeckleIfcs = new(@"C:\Users\cdigg\git\3d-format-shootout\data\git-repo-copies\speckle\ifcs");

    public static FilePath Example => ResolveIfc(WebIfcPublic, "example.ifc");
    public static FilePath IfcOpenHouse => ResolveIfc(WebIfcPublic, "IfcOpenHouse_IFC4.ifc");
    public static FilePath SampleEntities => ResolveIfc(WebIfcPublic, "Sample_entities.ifc");
    public static FilePath Issue044CompositeProfile => ResolveIfc(WebIfcPublic, "ISSUE_044_test_IFCCOMPOSITEPROFILEDEF.ifc");
    public static FilePath Issue171SurfaceCurveSwept => ResolveIfc(WebIfcPublic, "ISSUE_171_IfcSurfaceCurveSweptAreaSolid.ifc");
    public static FilePath Ac20FzkHaus => ResolveIfc(WebIfcPublic, "AC20-FZK-Haus.ifc");
    public static FilePath SteelPlates => ResolveIfc(SpeckleIfcs, "steelplates.ifc");
    public static FilePath Railing => ResolveIfc(SpeckleIfcs, "railing.ifc");
    public static FilePath AiscSculptureBrep => ResolveIfc(SpeckleIfcs, "171210AISC_Sculpture_brep.ifc");
    public static FilePath Small => ResolveIfc(SpeckleIfcs, "small.ifc");
    public static FilePath DentalClinic => ResolveIfc(WebIfcPublic, "dental_clinic.ifc");
    public static FilePath Duplex => ResolveIfc(WebIfcPublic, "duplex.ifc");
    public static FilePath OfficeA => ResolveIfc(WebIfcPublic, "Office_A_20110811.ifc");

    public static IEnumerable<FilePath> QuickComparisonFiles()
        => new[] { IfcOpenHouse, Example, SteelPlates };

    public static IEnumerable<FilePath> StudioDataFiles()
    {
        if (!StudioDataDir.Exists())
            yield break;

        foreach (var file in Directory.EnumerateFiles(StudioDataDir, "*.ifc", SearchOption.TopDirectoryOnly))
            yield return new FilePath(file);
    }

    static FilePath ResolveIfc(DirectoryPath externalFolder, string fileName)
    {
        var local = LocalIfcDir.RelativeFile(fileName);
        return local.Exists() ? local : externalFolder.RelativeFile(fileName);
    }

    public static IEnumerable<FilePath> AllKnownFiles()
    {
        if (LocalIfcDir.Exists())
        {
            var localFiles = Directory
                .EnumerateFiles(LocalIfcDir, "*.ifc", SearchOption.TopDirectoryOnly)
                .Select(f => new FilePath(f))
                .OrderBy(f => f.GetFileName(), StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (localFiles.Count > 0)
            {
                foreach (var file in localFiles)
                    yield return file;
                yield break;
            }
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var folder in new[] { WebIfcPublic, SpeckleIfcs })
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
}
