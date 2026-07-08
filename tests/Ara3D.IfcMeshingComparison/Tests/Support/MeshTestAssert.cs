using Ara3D.Geometry;

namespace Ara3D.IfcMeshingComparison.Tests.Support;

internal static class MeshTestAssert
{
    public static void BoundsMin(TriangleMesh3D mesh, float x, float y, float z, float tol = 1e-4f)
    {
        var b = mesh.Points.Bounds();
        Assert.Multiple(() =>
        {
            Assert.That((float)b.Min.X, Is.EqualTo(x).Within(tol));
            Assert.That((float)b.Min.Y, Is.EqualTo(y).Within(tol));
            Assert.That((float)b.Min.Z, Is.EqualTo(z).Within(tol));
        });
    }

    public static void BoundsMax(TriangleMesh3D mesh, float x, float y, float z, float tol = 1e-4f)
    {
        var b = mesh.Points.Bounds();
        Assert.Multiple(() =>
        {
            Assert.That((float)b.Max.X, Is.EqualTo(x).Within(tol));
            Assert.That((float)b.Max.Y, Is.EqualTo(y).Within(tol));
            Assert.That((float)b.Max.Z, Is.EqualTo(z).Within(tol));
        });
    }
}
