using Ara3D.Geometry;

namespace Ara3D.SDK.GeometryTests;

[TestFixture]
public class DerivedDataTests
{
    static TriangleMesh3D UnitCube => PlatonicSolids.TriangulatedCube;

    static TriangleMesh3D SingleTriangle
        => new(
            new[] { new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(0, 1, 0) },
            new[] { new Integer3(0, 1, 2) });

    [Test]
    public static void ComputesOncePerOwnerAndName()
    {
        var owner = new object();
        var count = 0;
        int Compute() { count++; return 42; }

        Assert.That(DerivedDataCache.Default.GetOrCompute(owner, "n", Compute), Is.EqualTo(42));
        Assert.That(DerivedDataCache.Default.GetOrCompute(owner, "n", Compute), Is.EqualTo(42));
        Assert.That(count, Is.EqualTo(1));

        DerivedDataCache.Default.GetOrCompute(new object(), "n", Compute);
        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public static void DistinguishesNamesAndDependencies()
    {
        var owner = new object();
        var depA = new object();
        var depB = new object();
        var count = 0;
        int Compute() { count++; return count; }

        var a = DerivedDataCache.Default.GetOrCompute(owner, new DerivedKey("k", depA), Compute);
        var b = DerivedDataCache.Default.GetOrCompute(owner, new DerivedKey("k", depB), Compute);
        var other = DerivedDataCache.Default.GetOrCompute(owner, new DerivedKey("other", depA), Compute);
        var aAgain = DerivedDataCache.Default.GetOrCompute(owner, new DerivedKey("k", depA), Compute);

        Assert.That(a, Is.EqualTo(1));
        Assert.That(b, Is.EqualTo(2));
        Assert.That(other, Is.EqualTo(3));
        Assert.That(aAgain, Is.EqualTo(1));
    }

    [Test]
    public static void SeparateCacheInstancesAreIsolated()
    {
        var owner = new object();
        var count = 0;
        int Compute() { count++; return count; }

        var a = new DerivedDataCache();
        var b = new DerivedDataCache();
        Assert.That(a.GetOrCompute(owner, "n", Compute), Is.EqualTo(1));
        Assert.That(b.GetOrCompute(owner, "n", Compute), Is.EqualTo(2));
        Assert.That(a.GetOrCompute(owner, "n", Compute), Is.EqualTo(1));
    }

    [Test]
    public static void MeshAccessorsReturnSameInstance()
    {
        var mesh = UnitCube;
        Assert.That(ReferenceEquals(mesh.DerivedVertexNormals(), mesh.DerivedVertexNormals()), Is.True);
        Assert.That(ReferenceEquals(mesh.DerivedFaceNormals(), mesh.DerivedFaceNormals()), Is.True);
        Assert.That(ReferenceEquals(mesh.DerivedAabbTree(), mesh.DerivedAabbTree()), Is.True);
        Assert.That(ReferenceEquals(mesh.DerivedTopology(), mesh.DerivedTopology()), Is.True);
        Assert.That(ReferenceEquals(mesh.DerivedStatistics(), mesh.DerivedStatistics()), Is.True);

        var copy = mesh;
        Assert.That(ReferenceEquals(copy.DerivedVertexNormals(), mesh.DerivedVertexNormals()), Is.True);
    }

    [Test]
    public static void NewFaceIndicesGetFreshFaceDependentData()
    {
        var mesh = UnitCube;
        var flipped = mesh.FlipFaces();
        Assert.That(ReferenceEquals(mesh.Points, flipped.Points), Is.True);
        Assert.That(ReferenceEquals(mesh.DerivedVertexNormals(), flipped.DerivedVertexNormals()), Is.False);
        Assert.That(ReferenceEquals(mesh.DerivedAabbTree(), flipped.DerivedAabbTree()), Is.False);
    }

    [Test]
    public static void DerivedBoundsMatchesBounds()
    {
        var mesh = UnitCube;
        Assert.That(mesh.DerivedBounds(), Is.EqualTo(mesh.Bounds));
    }

    [Test]
    public static void AabbTreeCoversMesh()
    {
        var mesh = UnitCube;
        var tree = mesh.DerivedAabbTree();
        Assert.That(tree.Count, Is.EqualTo(mesh.FaceIndices.Count));
        Assert.That(tree.TotalBounds.Contains(mesh.Bounds.Min).Value, Is.True);
        Assert.That(tree.TotalBounds.Contains(mesh.Bounds.Max).Value, Is.True);
    }

    [Test]
    public static void VertexNormalsOfSingleTriangle()
    {
        var normals = SingleTriangle.DerivedVertexNormals();
        Assert.That(normals.Length, Is.EqualTo(3));
        for (var i = 0; i < normals.Length; i++)
            Assert.That((float)normals[i].Z, Is.EqualTo(1f).Within(1e-5f));
    }
}
