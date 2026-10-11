using Ara3D.Geometry;

[TestFixture]
public class ShellMesherTests
{
    static List<Point3D> Points(params (float X, float Y, float Z)[] points)
        => points.Select(p => new Point3D(p.X, p.Y, p.Z)).ToList();

    static Vector3 Normal(TriangleMesh3D mesh, Integer3 f)
    {
        var p = mesh.Points;
        return (p[f.B] - p[f.A]).Cross(p[f.C] - p[f.A]);
    }

    static double Area(TriangleMesh3D mesh)
        => mesh.FaceIndices.Sum(f => 0.5 * Normal(mesh, f).Length().Value);

    /// <summary>Signed volume by the divergence theorem: positive when every face winds
    /// counter-clockwise seen from outside.</summary>
    static double Volume(TriangleMesh3D mesh)
        => mesh.FaceIndices.Sum(f => mesh.Points[f.A].Vector3.Dot(Normal(mesh, f)).Value / 6.0);

    /// <summary>A 4 by 4 square in the x-z plane, wound counter-clockwise seen from +y, with a 2 by
    /// 2 square hole in its middle.</summary>
    static readonly List<Point3D> SquareWithHolePoints = Points(
        (0, 0, 0), (0, 0, 4), (4, 0, 4), (4, 0, 0), (1, 0, 1), (3, 0, 1), (3, 0, 3), (1, 0, 3));

    [Test]
    public void FaceWithHole_CoversTheSquareLessTheHole_FacingItsLoopsNormal()
    {
        var (mesh, skipped) = ShellMesher.Triangulate(new ShellLoops(SquareWithHolePoints, [[0, 1, 2, 3]], [(0, [4, 5, 6, 7])]));
        Assert.Multiple(() =>
        {
            Assert.That(skipped, Is.Zero);
            Assert.That(mesh.FaceIndices, Has.Count.EqualTo(8), "8 points bridged: n - 2 + 2");
            Assert.That(Area(mesh), Is.EqualTo(12).Within(1e-5));
            Assert.That(mesh.FaceIndices.Select(f => Normal(mesh, f).Y.Value), Is.All.GreaterThan(0));
        });
    }

    [Test]
    public void ClosedShell_EnclosesPositiveVolume()
    {
        // A unit cube, each face wound counter-clockwise seen from outside.
        var cube = new ShellLoops(
            Points((0, 0, 0), (1, 0, 0), (1, 1, 0), (0, 1, 0), (0, 0, 1), (1, 0, 1), (1, 1, 1), (0, 1, 1)),
            [[0, 3, 2, 1], [4, 5, 6, 7], [0, 1, 5, 4], [1, 2, 6, 5], [2, 3, 7, 6], [3, 0, 4, 7]],
            []);
        var (mesh, _) = ShellMesher.Triangulate(cube);
        Assert.Multiple(() =>
        {
            Assert.That(mesh.FaceIndices, Has.Count.EqualTo(12));
            Assert.That(Volume(mesh), Is.EqualTo(1).Within(1e-6));
        });
    }

    [Test]
    public void TriangleProfile_KeepsItsWinding()
    {
        var (mesh, _) = ShellMesher.Triangulate(new ShellLoops(Points((0, 0, 0), (0, 1, 0), (1, 0, 0)), [[0, 1, 2]], []));
        Assert.That(mesh.FaceIndices.Single(), Is.EqualTo(new Integer3(0, 1, 2)));
    }

    [TestCase(new[] { 0, 1, 9 }, TestName = "IndexOutsideThePoints")]
    [TestCase(new[] { 0, 1, -1 }, TestName = "NegativeIndex")]
    [TestCase(new[] { 0, 1, 3 }, TestName = "CollinearLoop")]
    [TestCase(new[] { 0, 1 }, TestName = "TwoPoints")]
    public void DegenerateProfile_IsSkippedAndCounted(int[] loop)
    {
        var points = Points((0, 0, 0), (1, 0, 0), (0, 1, 0), (2, 0, 0));
        var (mesh, skipped) = ShellMesher.Triangulate(new ShellLoops(points, [[0, 1, 2], loop], []));
        Assert.Multiple(() =>
        {
            Assert.That(mesh.FaceIndices, Has.Count.EqualTo(1));
            Assert.That(skipped, Is.EqualTo(1));
        });
    }

    [Test]
    public void NonFinitePoint_SkipsTheProfile()
    {
        var points = Points((0, 0, 0), (1, 0, 0), (float.NaN, 1, 0));
        Assert.That(ShellMesher.TriangulateFace(points, [0, 1, 2], [], []), Is.False);
    }
}
