using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

/// <summary>WP-W11: DigitalHub mesh-dedup / mesh-bbox pairing.</summary>
[TestFixture]
public sealed class WpW11Tests
{
    static FilePath DigitalHubIfc => new(@"c:\Users\cdigg\git\studio\data\FM_ARC_DigitalHub.ifc");

    [Test]
    [Explicit("WP-W11 diagnosis: DigitalHub mesh count vs bbox frontier")]
    [Category("Slow")]
    public void DigitalHub_MeshDedupDiagnosis()
    {
        TestFiles.RequireExists(DigitalHubIfc);
        using var file = new IfcFile(DigitalHubIfc, includeGeometry: false);
        var (model, _) = ModelAssembler.BuildModel(file);
        var oracle = ModelComparer.LoadOracle(DigitalHubIfc);

        var uniqueOracle = GroupByTopology(oracle.Meshes.ToList()).Count;
        TestContext.WriteLine(
            $"candidate meshes={model.Meshes.Count}, oracle meshes={oracle.Meshes.Count}, " +
            $"unique oracle topologies={uniqueOracle}");

        var result = ModelComparer.Compare(model, oracle, DigitalHubIfc.GetFileName());
        TestContext.WriteLine(ModelComparer.FormatResult(result));
        TestContext.WriteLine(
            $"mesh-bbox {result.MeshBoundingBox.Score:F3} " +
            $"({result.MeshBoundingBox.MatchedCount}/{result.MeshBoundingBox.ComparedCount}), " +
            $"mesh-shape {result.MeshShapeScore:F3}");
    }

    [Test]
    [Category("Slow")]
    public void DigitalHub_MeshBbox_AtLeast055()
    {
        TestFiles.RequireExists(DigitalHubIfc);

        var bfastPath = WebIfcBfastOracle.OraclePath(DigitalHubIfc);
        if (!bfastPath.Exists() || WebIfcBfastOracle.NeedsRegeneration(DigitalHubIfc, bfastPath))
            WebIfcBfastOracle.Generate(DigitalHubIfc, TestContext.WriteLine);

        var result = ModelComparer.CompareFile(DigitalHubIfc);
        TestContext.WriteLine(ModelComparer.FormatResult(result));

        Assert.That(result.MeshBoundingBox.Score, Is.GreaterThanOrEqualTo(0.55),
            "mesh-bbox should reach ≥0.55 after canonical local bounds pairing");
        Assert.That(result.EntityShape.Score, Is.GreaterThanOrEqualTo(0.90),
            "no entity-level regression on DigitalHub");
    }

    static List<List<int>> GroupByTopology(IReadOnlyList<TriangleMesh3D> meshes)
    {
        var groups = new List<List<int>>();
        for (var i = 0; i < meshes.Count; i++)
        {
            var found = -1;
            for (var g = 0; g < groups.Count; g++)
            {
                if (TopologyEqual(meshes[i], meshes[groups[g][0]]))
                {
                    found = g;
                    break;
                }
            }
            if (found < 0)
                groups.Add(new List<int> { i });
            else
                groups[found].Add(i);
        }
        return groups;
    }

    static bool TopologyEqual(TriangleMesh3D a, TriangleMesh3D b)
    {
        if (a.Points.Count != b.Points.Count || a.FaceIndices.Count != b.FaceIndices.Count)
            return false;
        for (var i = 0; i < a.Points.Count; i++)
            if (!a.Points[i].Equals(b.Points[i]))
                return false;
        for (var i = 0; i < a.FaceIndices.Count; i++)
        {
            var fa = a.FaceIndices[i];
            var fb = b.FaceIndices[i];
            if (fa.A != fb.A || fa.B != fb.B || fa.C != fb.C)
                return false;
        }
        return true;
    }
}
