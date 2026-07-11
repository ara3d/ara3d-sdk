using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.IfcTypes;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

/// <summary>Per-entity diagnosis for schependomlaan max-Δ windows/doors.</summary>
[TestFixture]
public sealed class WpSchependomlaanWindowTests
{
    [Test]
    [Explicit("Diagnose #948940 velux window placement vs oracle")]
    [Category("Slow")]
    public void Schependomlaan_Window948940_PlacementDiagnosis()
    {
        var ifcPath = TestFiles.LocalIfcDir.RelativeFile("schependomlaan.ifc");
        if (!ifcPath.Exists())
            ifcPath = new FilePath(@"C:\Users\cdigg\git\3d-format-shootout\data\git-repo-copies\web-ifc\schependomlaan.ifc");
        TestFiles.RequireExists(ifcPath);

        const int entityId = 948940;
        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var oracle = ModelComparer.LoadOracle(ifcPath);

        var cand = candidate.Instances.Where(i => i.EntityIndex == entityId).ToList();
        var orac = oracle.Instances.Where(i => i.EntityIndex == entityId).ToList();
        TestContext.WriteLine($"#{entityId}: candInst={cand.Count} oracleInst={orac.Count}");
        for (var i = 0; i < Math.Max(cand.Count, orac.Count); i++)
        {
            if (i < cand.Count)
            {
                var m = cand[i].Matrix4x4;
                var mesh = candidate.Meshes[cand[i].MeshIndex];
                var bounds = MeshHelpers.GetBounds(MeshHelpers.Transform(mesh, m));
                TestContext.WriteLine(
                    $"  cand[{i}] T=({m.M41:F3},{m.M42:F3},{m.M43:F3}) tris={mesh.FaceIndices.Count} " +
                    $"center=({bounds.Center.X:F3},{bounds.Center.Y:F3},{bounds.Center.Z:F3}) " +
                    $"size=({bounds.Size.X:F3},{bounds.Size.Y:F3},{bounds.Size.Z:F3})");
            }
            if (i < orac.Count)
            {
                var m = orac[i].Matrix4x4;
                var mesh = oracle.Meshes[orac[i].MeshIndex];
                var bounds = MeshHelpers.GetBounds(MeshHelpers.Transform(mesh, m));
                TestContext.WriteLine(
                    $"  orac[{i}] T=({m.M41:F3},{m.M42:F3},{m.M43:F3}) tris={mesh.FaceIndices.Count} " +
                    $"center=({bounds.Center.X:F3},{bounds.Center.Y:F3},{bounds.Center.Z:F3}) " +
                    $"size=({bounds.Size.X:F3},{bounds.Size.Y:F3},{bounds.Size.Z:F3})");
            }
        }

        using var file = new IfcFile(ifcPath, includeGeometry: false);
        var ctx = new MeshingContext(file);
        var entity = ctx.GetEntity(entityId);
        var placement = MeshHelpers.ResolveRequired(ctx, entity, IfcProduct.Instance.ObjectPlacement);
        var productFrame = Placements.ReadLocalPlacement(ctx, placement);
        TestContext.WriteLine($"product placement origin=({productFrame.Origin.X:F3},{productFrame.Origin.Y:F3},{productFrame.Origin.Z:F3})");

        var rep = MeshHelpers.ResolveRequired(ctx, entity, IfcProduct.Instance.Representation);
        var parts = new List<CollectedPart>();
        GeometryPartCollector.CollectParts(ctx, rep, Matrix4x4.Identity, entityId, parts);
        TestContext.WriteLine($"parts={parts.Count}");
        foreach (var (part, idx) in parts.Select((p, i) => (p, i)))
        {
            var b = MeshHelpers.GetBounds(part.Mesh);
            TestContext.WriteLine(
                $"  part[{idx}] T=({part.Transform.M41:F3},{part.Transform.M42:F3},{part.Transform.M43:F3}) " +
                $"tris={part.Mesh.FaceIndices.Count} localCenter=({b.Center.X:F3},{b.Center.Y:F3},{b.Center.Z:F3}) " +
                $"localSize=({b.Size.X:F3},{b.Size.Y:F3},{b.Size.Z:F3})");
            var world = part.Transform * productFrame.Matrix;
            var wb = MeshHelpers.GetBounds(MeshHelpers.Transform(part.Mesh, world));
            TestContext.WriteLine(
                $"    world T=({world.M41:F3},{world.M42:F3},{world.M43:F3}) " +
                $"center=({wb.Center.X:F3},{wb.Center.Y:F3},{wb.Center.Z:F3})");
        }

        // Mapping source #944781 / mapped item #948927
        if (GeometryDispatcher.TryGetMappedItemTransform(ctx, ctx.GetEntity(948927), out var mapping))
            TestContext.WriteLine($"mapping T=({mapping.M41:F3},{mapping.M42:F3},{mapping.M43:F3}) fro-id={TransformComparison.CompareMatrices(mapping, Matrix4x4.Identity).Frobenius:F4}");
    }
}
