using Ara3D.Geometry;

[TestFixture]
public class DelaunayMeshRefinerTests
{
    const float Tol = 1e-3f;

    static float MeshArea(TriangleMesh3D mesh)
    {
        var area = 0f;
        foreach (var f in mesh.FaceIndices)
        {
            var a = mesh.Points[f.A].Vector3;
            var b = mesh.Points[f.B].Vector3;
            var c = mesh.Points[f.C].Vector3;
            area += Vector3.Cross(b - a, c - a).Length() * 0.5f;
        }
        return area;
    }

    static TriangleMesh3D SingleTriangle()
        => new(
            new List<Point3D> { new(0, 0, 0), new(4, 0, 0), new(0, 4, 0) },
            new List<Integer3> { new(0, 1, 2) });

    [Test]
    public void RefinementPreservesArea()
    {
        var mesh = SingleTriangle();
        var refined = mesh.DelaunayRefine(0.5f);
        Assert.That(refined.FaceIndices.Count, Is.GreaterThan(1));
        Assert.That(MeshArea(refined), Is.EqualTo(MeshArea(mesh)).Within(Tol));
    }

    [Test]
    public void RefinementKeepsOriginalVertices()
    {
        var mesh = SingleTriangle();
        var refined = mesh.DelaunayRefine(0.5f);
        for (var i = 0; i < mesh.Points.Count; ++i)
            Assert.That((float)(refined.Points[i].Vector3 - mesh.Points[i].Vector3).Length(), Is.LessThan(Tol));
    }

    [Test]
    public void SmallFacesAreUntouched()
    {
        var mesh = SingleTriangle();
        var refined = mesh.DelaunayRefine(100f);
        Assert.That(refined.FaceIndices.Count, Is.EqualTo(1));
        Assert.That(refined.Points.Count, Is.EqualTo(3));
    }

    [Test]
    public void NewPointsLieInFacePlane()
    {
        var mesh = SingleTriangle();
        var refined = mesh.DelaunayRefine(0.5f);
        for (var i = 3; i < refined.Points.Count; ++i)
            Assert.That(MathF.Abs(refined.Points[i].Vector3.Z), Is.LessThan(Tol));
    }

    [Test]
    public void ConsistentWindingWithSourceFace()
    {
        var mesh = SingleTriangle();
        var refined = mesh.DelaunayRefine(0.5f);
        foreach (var f in refined.FaceIndices)
        {
            var a = refined.Points[f.A].Vector3;
            var b = refined.Points[f.B].Vector3;
            var c = refined.Points[f.C].Vector3;
            Assert.That((float)Vector3.Cross(b - a, c - a).Z, Is.GreaterThan(0f));
        }
    }
}
