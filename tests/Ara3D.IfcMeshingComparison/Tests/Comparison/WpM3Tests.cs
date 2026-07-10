using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.Comparison;

[TestFixture]
public sealed class WpM3Tests
{
    const float Displacement = 1.5f;

    [Test]
    public void Tier2_IdenticalMeshes_HaveNearZeroSurfaceDistance()
    {
        var rod = Box(3f, 0.6f, 0.6f);
        var metrics = ShapeDiagnostics.CompareMeshSurfaces(rod, rod);
        Assert.That(metrics, Is.Not.Null);

        Assert.That(metrics!.ChamferSymmetric, Is.LessThan(1e-4));
        Assert.That(metrics.HausdorffSymmetric, Is.LessThan(1e-4));
        Assert.That(metrics.ConvexHullIoU, Is.GreaterThan(0.98));
    }

    [Test]
    public void Tier2_DisplacedRod_SurfaceDistanceMatchesDisplacement()
    {
        var rod = Box(3f, 0.6f, 0.6f);
        var shifted = rod.Translate(new Vector3(0f, Displacement, 0f));

        var metrics = ShapeDiagnostics.CompareMeshSurfaces(rod, shifted);
        Assert.That(metrics, Is.Not.Null);

        Assert.That(metrics!.HausdorffSymmetric, Is.EqualTo(Displacement).Within(0.05));
        Assert.That(metrics.ChamferSymmetric, Is.InRange(Displacement * 0.5, Displacement));
    }

    [Test]
    public void Tier2_EntityComparison_OrdersByWorstHausdorff()
    {
        var rod = Box(3f, 0.6f, 0.6f);
        var baseModel = SingleInstanceModel(rod, entityId: 100, Matrix4x4.Identity);
        var shifted = SingleInstanceModel(rod, 100, Matrix4x4.CreateTranslation(new Vector3(0f, Displacement, 0f)));

        var identical = ShapeDiagnostics.CompareEntitiesTier2(baseModel, baseModel);
        var displaced = ShapeDiagnostics.CompareEntitiesTier2(baseModel, shifted);

        Assert.That(identical, Has.Count.EqualTo(1));
        Assert.That(displaced, Has.Count.EqualTo(1));
        Assert.That(displaced[0].HausdorffSymmetric, Is.GreaterThan(identical[0].HausdorffSymmetric));
    }

    [Test]
    [Explicit("Tier 2 diagnostic dump: per-entity Chamfer/Hausdorff for duplex")]
    public void Tier2DiagnosticsDuplex()
    {
        var ifcPath = TestFiles.Duplex;
        TestFiles.RequireExists(ifcPath);
        using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var oracle = ModelComparer.LoadOracle(ifcPath);

        var diagnostics = ShapeDiagnostics.CompareEntitiesTier2(candidate, oracle);
        TestContext.WriteLine(
            $"Tier 2 diagnostics ({diagnostics.Count} shared entities), worst 20 by symmetric Hausdorff:");
        foreach (var d in diagnostics.Take(20))
        {
            var entity = stepFile.EntityResolver.GetEntityOrDefault(d.EntityId);
            TestContext.WriteLine(
                $"  #{d.EntityId} {entity?.GetEntityName() ?? "?"}: " +
                $"chamfer={d.ChamferSymmetric:F4} hausdorff={d.HausdorffSymmetric:F4} " +
                $"hullIoU={(d.ConvexHullIoU.HasValue ? d.ConvexHullIoU.Value.ToString("F3") : "n/a")}");
        }

        if (diagnostics.Count > 0)
        {
            TestContext.WriteLine(
                $"Mean symmetric Chamfer {diagnostics.Average(d => d.ChamferSymmetric):F4}, " +
                $"mean symmetric Hausdorff {diagnostics.Average(d => d.HausdorffSymmetric):F4}");
        }
    }

    [Test]
    [Explicit("Tier 2 diagnostic dump: per-entity Chamfer/Hausdorff for DigitalHub")]
    [Category("Slow")]
    public void Tier2DiagnosticsDigitalHub()
    {
        var ifcPath = new FilePath(@"c:\Users\cdigg\git\studio\data\FM_ARC_DigitalHub.ifc");
        TestFiles.RequireExists(ifcPath);
        using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var oracle = ModelComparer.LoadOracle(ifcPath);

        var diagnostics = ShapeDiagnostics.CompareEntitiesTier2(candidate, oracle);
        TestContext.WriteLine(
            $"Tier 2 diagnostics ({diagnostics.Count} shared entities), worst 20 by symmetric Hausdorff:");
        foreach (var d in diagnostics.Take(20))
        {
            var entity = stepFile.EntityResolver.GetEntityOrDefault(d.EntityId);
            TestContext.WriteLine(
                $"  #{d.EntityId} {entity?.GetEntityName() ?? "?"}: " +
                $"chamfer={d.ChamferSymmetric:F4} hausdorff={d.HausdorffSymmetric:F4} " +
                $"hullIoU={(d.ConvexHullIoU.HasValue ? d.ConvexHullIoU.Value.ToString("F3") : "n/a")}");
        }

        if (diagnostics.Count > 0)
        {
            TestContext.WriteLine(
                $"Mean symmetric Chamfer {diagnostics.Average(d => d.ChamferSymmetric):F4}, " +
                $"mean symmetric Hausdorff {diagnostics.Average(d => d.HausdorffSymmetric):F4}");
        }
    }

    static Model3D SingleInstanceModel(TriangleMesh3D mesh, int entityId, Matrix4x4 matrix)
    {
        var builder = new Model3DBuilder();
        builder.Meshes.Add(mesh);
        builder.AddInstance(0, matrix, Material.Default, entityId);
        return builder.Build();
    }

    static TriangleMesh3D Box(float sx, float sy, float sz)
    {
        var x = sx * 0.5f;
        var y = sy * 0.5f;
        var z = sz * 0.5f;
        var points = new List<Point3D>
        {
            new(-x, -y, -z), new(x, -y, -z), new(x, y, -z), new(-x, y, -z),
            new(-x, -y, z), new(x, -y, z), new(x, y, z), new(-x, y, z),
        };
        var faces = new List<Integer3>
        {
            new(0, 3, 2), new(0, 2, 1),
            new(4, 5, 6), new(4, 6, 7),
            new(0, 1, 5), new(0, 5, 4),
            new(2, 3, 7), new(2, 7, 6),
            new(0, 4, 7), new(0, 7, 3),
            new(1, 2, 6), new(1, 6, 5),
        };
        return new TriangleMesh3D(points, faces);
    }
}
