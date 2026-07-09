using Ara3D.Geometry;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.Models;

namespace Ara3D.IfcMeshingComparison.Tests.Comparison;

[TestFixture]
public sealed class EntityShapeMetricTests
{

    // The Tier 0 entity-shape metric is built from rotation/translation-invariant descriptors
    // (volume, area, sorted OBB extents, PCA spectrum, sphere radius, boundary length). It must stay
    // high when the same geometry is re-oriented (where the AABB metric collapses) yet fall when the
    // geometry is genuinely rescaled.
    [Test]
    public void EntityShape_IsRotationInvariant_ButScaleSensitive()
    {
        var rod = Box(3f, 0.3f, 0.3f);
        var baseModel = SingleInstanceModel(rod, entityId: 100, Matrix4x4.Identity);

        var identical = ModelComparer.Compare(
            baseModel, SingleInstanceModel(rod, 100, Matrix4x4.Identity), "identical");
        Assert.That(identical.EntityShape.Score, Is.GreaterThan(0.98), "identical geometry scores ~1");
        Assert.That(identical.EntityShape.MatchedCount, Is.EqualTo(1));

        // Rotate the rod 90 degrees about Z: same shape, but its axis-aligned box is now completely different.
        var rot = Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2f);
        var rotated = ModelComparer.Compare(
            baseModel, SingleInstanceModel(rod, 100, rot), "rotated");
        Assert.That(rotated.EntityShape.Score, Is.GreaterThan(0.9),
            "rotation preserves intrinsic shape");
        Assert.That(rotated.EntityBoundingBox.Score, Is.LessThan(rotated.EntityShape.Score),
            "the AABB metric is penalized by re-orientation; the shape metric is not");

        // Double every dimension: intrinsically a different-sized object -> shape score must drop.
        var scaled = ModelComparer.Compare(
            baseModel, SingleInstanceModel(Box(6f, 0.6f, 0.6f), 100, Matrix4x4.Identity), "scaled");
        Assert.That(scaled.EntityShape.Score, Is.LessThan(0.6), "2x rescale is penalized");
    }

    static Model3D SingleInstanceModel(TriangleMesh3D mesh, int entityId, Matrix4x4 matrix)
    {
        var builder = new Model3DBuilder();
        builder.Meshes.Add(mesh);
        builder.AddInstance(0, matrix, Material.Default, entityId);
        return builder.Build();
    }

    // Axis-aligned box centered at the origin, consistent outward winding (so |signed volume| = sx*sy*sz).
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
            new(0, 3, 2), new(0, 2, 1), // -Z
            new(4, 5, 6), new(4, 6, 7), // +Z
            new(0, 1, 5), new(0, 5, 4), // -Y
            new(2, 3, 7), new(2, 7, 6), // +Y
            new(0, 4, 7), new(0, 7, 3), // -X
            new(1, 2, 6), new(1, 6, 5), // +X
        };
        return new TriangleMesh3D(points, faces);
    }
}
