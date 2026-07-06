using Ara3D.Geometry;

namespace Ara3D.SDK.GeometryTests;

[TestFixture]
public class MeshClipTests
{
    static TriangleMesh3D UnitCube => PlatonicSolids.TriangulatedCube;

    [Test]
    public static void ClipBelowHorizontalPlane()
    {
        var mesh = UnitCube;
        var z = mesh.Bounds.Min.Z.Lerp(mesh.Bounds.Max.Z, 0.5f);
        var plane = new Plane(Vector3.UnitZ, -z);
        var clipped = mesh.ClipBelow(plane);

        Assert.That(clipped.Points.All(p => p.Z <= z + MeshPlaneClip.DefaultEpsilon).Value, Is.True);
        Assert.That(clipped.FaceIndices.Count, Is.GreaterThan(0));
    }

    [Test]
    public static void ClipToBoundsKeepsInteriorPoints()
    {
        var mesh = UnitCube.Scale(2).Translate(new Vector3(0, 0, -0.5f));
        var bounds = new Bounds3D(new Point3D(-0.5f, -0.5f, -0.5f), new Point3D(0.5f, 0.5f, 0.5f));
        var clipped = mesh.ClipToBounds(bounds);

        Assert.That(clipped.FaceIndices.Count, Is.GreaterThan(0));
        Assert.That(clipped.Points.All(bounds.Contains).Value, Is.True);
    }

    [Test]
    public static void ClipToBoundsWithPaddingIsLooser()
    {
        var mesh = UnitCube.Scale(2);
        var bounds = new Bounds3D(new Point3D(-0.25f, -0.25f, -0.25f), new Point3D(0.25f, 0.25f, 0.25f));
        var tight = mesh.ClipToBounds(bounds);
        var padded = mesh.ClipToBounds(bounds, padding: 0.25f);

        Assert.That(padded.FaceIndices.Count, Is.GreaterThanOrEqualTo(tight.FaceIndices.Count));
    }

    [Test]
    public static void ClipToBoundsOutsideMeshIsEmpty()
    {
        var mesh = UnitCube;
        var bounds = new Bounds3D(new Point3D(5, 5, 5), new Point3D(6, 6, 6));
        var clipped = mesh.ClipToBounds(bounds);

        Assert.That(clipped.FaceIndices.Count, Is.EqualTo(0));
    }
}
