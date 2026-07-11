using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

/// <summary>Quick-file entityShape volumeRatio≈0 cluster (example + steelplates).</summary>
[TestFixture]
public sealed class WpEntityShapeVolumeTests
{
    [Test]
    [Explicit("Dump worst entityShape gaps on example.ifc")]
    [Category("Slow")]
    public void Example_EntityShape_VolumeZero_Dump()
    {
        DumpVolumeGaps(TestFiles.Example, "example.ifc");
    }

    [Test]
    [Explicit("Dump worst entityShape gaps on steelplates.ifc")]
    [Category("Slow")]
    public void SteelPlates_EntityShape_VolumeZero_Dump()
    {
        DumpVolumeGaps(TestFiles.SteelPlates, "steelplates.ifc");
    }

    [Test]
    [Category("Slow")]
    public void Example_ShsBeam12799_SignedVolumeAndBoundary()
    {
        TestFiles.RequireExists(TestFiles.Example);
        using var file = new IfcFile(TestFiles.Example, includeGeometry: false);
        var ctx = new MeshingContext(file);
        var mesh = ModelAssembler.BuildEntityMesh(ctx, ctx.GetEntity(12799));
        Assert.That(mesh, Is.Not.Null);
        var vol = Math.Abs(MeshHelpers.SignedVolume(mesh!.Value));
        var area = mesh.Value.FaceIndices.Sum(f =>
        {
            var a = mesh.Value.Points[f.A].Vector3;
            var b = mesh.Value.Points[f.B].Vector3;
            var c = mesh.Value.Points[f.C].Vector3;
            return Vector3.Cross(b - a, c - a).Length() * 0.5;
        });
        var openEdges = CountOpenEdges(mesh.Value);
        TestContext.WriteLine($"#12799 vol={vol:F6} area={area:F6} openEdges={openEdges} tris={mesh.Value.FaceIndices.Count}");

        var oracle = ModelComparer.LoadOracle(TestFiles.Example);
        var oInst = oracle.Instances.Where(i => i.EntityIndex == 12799).ToList();
        Assert.That(oInst, Is.Not.Empty);
        var oMesh = MergeEntity(oracle, 12799);
        var oVol = Math.Abs(MeshHelpers.SignedVolume(oMesh));
        var oOpen = CountOpenEdges(oMesh);
        TestContext.WriteLine($"oracle #12799 vol={oVol:F6} openEdges={oOpen} tris={oMesh.FaceIndices.Count}");

        // Hollow SHS should be nearly watertight after hole-ring alignment with offset-ring caps.
        Assert.That(openEdges, Is.LessThan(8),
            $"candidate SHS open edges should collapse after hole resample alignment (was high before fix)");
        Assert.That(vol, Is.GreaterThan(1e-4), "candidate signed volume should be non-zero for hollow SHS");
        // Volume similarity vs oracle is still loose (oracle also open); just ensure we are not empty.
        Assert.That(mesh.Value.FaceIndices.Count, Is.GreaterThan(100));
    }

    static TriangleMesh3D MergeEntity(Ara3D.Models.Model3D model, int entityId)
    {
        var meshes = model.Instances
            .Where(i => i.EntityIndex == entityId)
            .Select(i => MeshHelpers.Transform(model.Meshes[i.MeshIndex], i.Matrix4x4))
            .ToList();
        return meshes.Count == 1 ? meshes[0] : MeshHelpers.Merge(meshes);
    }

    static int CountOpenEdges(TriangleMesh3D mesh)
    {
        var counts = new Dictionary<(int, int), int>();
        void Add(int a, int b)
        {
            var key = a < b ? (a, b) : (b, a);
            counts[key] = counts.GetValueOrDefault(key) + 1;
        }
        foreach (var f in mesh.FaceIndices)
        {
            Add(f.A, f.B);
            Add(f.B, f.C);
            Add(f.C, f.A);
        }
        return counts.Count(kv => kv.Value != 2);
    }

    [Test]
    [Category("Slow")]
    public void SteelPlates_KnownBooleanBeams_HaveGeometry()
    {
        TestFiles.RequireExists(TestFiles.SteelPlates);
        using var file = new IfcFile(TestFiles.SteelPlates, includeGeometry: false);
        var ctx = new MeshingContext(file);
        foreach (var id in new[] { 1193, 633, 1385 })
        {
            var entity = ctx.GetEntityOrDefault(id);
            if (entity is null)
            {
                TestContext.WriteLine($"#{id} missing in file");
                continue;
            }
            TestContext.WriteLine($"#{id} {entity.GetEntityName()}");
            var mesh = ModelAssembler.BuildEntityMesh(ctx, entity);
            Assert.That(mesh, Is.Not.Null, $"#{id} should produce a mesh");
            Assert.That(mesh!.Value.FaceIndices.Count, Is.GreaterThan(0));
            var vol = ApproximateSignedVolume(mesh.Value);
            TestContext.WriteLine($"  tris={mesh.Value.FaceIndices.Count} approxVol={vol:F6}");
        }
    }

    static void DumpVolumeGaps(FilePath ifcPath, string label)
    {
        TestFiles.RequireExists(ifcPath);
        using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
        var result = ModelComparer.CompareFile(ifcPath);
        TestContext.WriteLine(ModelComparer.FormatResult(result));
        TestContext.WriteLine($"{label} worst entityShape (vol≈0 first):");
        foreach (var gap in result.EntityShape.WorstEntities
                     .OrderBy(g => g.VolumeRatio)
                     .ThenBy(g => g.Score)
                     .Take(25))
        {
            var entity = stepFile.EntityResolver.GetEntityOrDefault(gap.EntityId);
            var mis = gap.MisTagSuspectId >= 0 ? $" mis-tag→#{gap.MisTagSuspectId}" : "";
            TestContext.WriteLine(
                $"  #{gap.EntityId} {entity?.GetEntityName() ?? "?"}: shape={gap.Score:F3} " +
                $"vol={gap.VolumeRatio:F3} area={gap.AreaRatio:F3} bndry={gap.BoundaryRatio:F3}{mis}");
        }
    }

    static double ApproximateSignedVolume(Ara3D.Geometry.TriangleMesh3D mesh)
    {
        double vol = 0;
        foreach (var f in mesh.FaceIndices)
        {
            var a = mesh.Points[f.A].Vector3;
            var b = mesh.Points[f.B].Vector3;
            var c = mesh.Points[f.C].Vector3;
            vol += Vector3.Dot(a, Vector3.Cross(b, c));
        }
        return Math.Abs(vol) / 6.0;
    }
}
