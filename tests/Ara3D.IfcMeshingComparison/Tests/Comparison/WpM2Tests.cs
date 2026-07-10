using Ara3D.Geometry;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.Models;

namespace Ara3D.IfcMeshingComparison.Tests.Comparison;

[TestFixture]
public sealed class WpM2Tests
{
    // WP-M2: entities whose candidate shape matches a different oracle entity's shape better than
    // their own (MisTagSuspectId >= 0) are excluded from the entityShape scored average.
    [Test]
    public void EntityShape_ExcludesOracleMisTagSuspects_FromScoredAverage()
    {
        var boxA = Box(2f, 1f, 0.5f);
        var boxB = Box(4f, 2f, 1f);

        // Candidate: entity 1 = boxA, entity 2 = boxB (correct geometry).
        var candidate = TwoEntityModel(
            (1, boxA, Matrix4x4.Identity),
            (2, boxB, Matrix4x4.Identity));

        // Oracle: entity 1 gets boxB's mesh, entity 2 gets boxA's mesh — a pure mis-tag permutation.
        var oracle = TwoEntityModel(
            (1, boxB, Matrix4x4.Identity),
            (2, boxA, Matrix4x4.Identity));

        var result = ModelComparer.Compare(candidate, oracle, "mis-tag-synthetic");

        Assert.That(result.EntityShape.ExcludedMisTagCount, Is.EqualTo(2),
            "both entities are mis-tag suspects (each candidate shape fits the other oracle entity)");
        Assert.That(result.EntityShape.Score, Is.GreaterThan(0.95),
            "mis-tagged pairs are excluded, leaving no scoreable gaps → score ~1");
        Assert.That(result.EntityShape.ComparedCount, Is.EqualTo(0),
            "ComparedCount counts only scoreable (non-excluded) entities");
    }

    [Test]
    public void EntityShape_RealDefectStillScoresLow_WhenNotMisTag()
    {
        var correct = Box(2f, 1f, 0.5f);
        var wrong = Box(3f, 1.5f, 0.75f);

        var candidate = SingleInstanceModel(correct, entityId: 10, Matrix4x4.Identity);
        var oracle = SingleInstanceModel(wrong, entityId: 10, Matrix4x4.Identity);

        var result = ModelComparer.Compare(candidate, oracle, "real-gap");

        Assert.That(result.EntityShape.ExcludedMisTagCount, Is.EqualTo(0));
        Assert.That(result.EntityShape.Score, Is.LessThan(0.7),
            "a genuine shape mismatch is not excluded");
    }

    static Model3D TwoEntityModel(params (int entityId, TriangleMesh3D mesh, Matrix4x4 matrix)[] parts)
    {
        var builder = new Model3DBuilder();
        foreach (var (entityId, mesh, matrix) in parts)
        {
            var meshIndex = builder.Meshes.Count;
            builder.Meshes.Add(mesh);
            builder.AddInstance(meshIndex, matrix, Material.Default, entityId);
        }
        return builder.Build();
    }

    static Model3D SingleInstanceModel(TriangleMesh3D mesh, int entityId, Matrix4x4 matrix)
        => TwoEntityModel((entityId, mesh, matrix));

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
