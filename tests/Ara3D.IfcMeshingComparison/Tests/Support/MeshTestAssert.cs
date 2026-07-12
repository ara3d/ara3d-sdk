using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Harness.GeometryOracles;

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

    public static void MeshValid(TriangleMesh3D mesh)
    {
        Assert.That(MeshValidity.HasValidIndices(mesh), Is.True, "face indices out of range");
        Assert.That(MeshValidity.CountDegenerateTriangles(mesh), Is.EqualTo(0), "degenerate triangles present");
    }

    public static void SolidWindingOutward(TriangleMesh3D mesh, float? expectedVolume = null, float tol = 0.01f)
    {
        MeshValid(mesh);
        var vol = MeshHelpers.SignedVolume(mesh);
        Assert.That(vol, Is.GreaterThan(0), "signed volume should be positive for outward winding");
        if (expectedVolume is { } expected)
            Assert.That(vol, Is.EqualTo(expected).Within(expected * tol + 1e-6f));
        Assert.That(WindingOracle.OutwardNormalFraction(mesh), Is.GreaterThanOrEqualTo(0.9f),
            "majority of face normals should point away from centroid");
    }

    public static void Watertight(TriangleMesh3D mesh, int maxOpenEdges = 0)
    {
        Assert.That(TopologyOracle.CountOpenEdges(mesh), Is.LessThanOrEqualTo(maxOpenEdges),
            $"open edges exceed budget of {maxOpenEdges}");
        if (maxOpenEdges == 0)
            Assert.That(TopologyOracle.IsWatertight(mesh), Is.True, "mesh should be watertight");
    }

    public static void AnalyticalVolume(TriangleMesh3D mesh, double expected, double relativeTol = 0.01)
        => Assert.That(AnalyticalOracle.MatchesVolume(mesh, expected, relativeTol), Is.True,
            $"volume {AnalyticalOracle.AbsVolume(mesh):F6} != expected {expected:F6}");

    public static void ClipKeptBounds(TriangleMesh3D mesh, float? maxZ = null, float? minZ = null, float tol = 1e-4f)
    {
        if (maxZ is { } mz)
            Assert.That(ClipOracle.MaxBoundAtMost(mesh, mz, tol), Is.True, $"max Z should be ≤ {mz}");
        if (minZ is { } nz)
            Assert.That(ClipOracle.MinBoundAtLeast(mesh, nz, tol), Is.True, $"min Z should be ≥ {nz}");
    }

    public static void PointInside(TriangleMesh3D mesh, float x, float y, float z)
        => Assert.That(ContainmentOracle.ContainsPoint(mesh, new Vector3(x, y, z)), Is.True,
            $"expected point ({x},{y},{z}) inside mesh");

    public static void PointOutside(TriangleMesh3D mesh, float x, float y, float z)
        => Assert.That(ContainmentOracle.ContainsPoint(mesh, new Vector3(x, y, z)), Is.False,
            $"expected point ({x},{y},{z}) outside mesh");
}
