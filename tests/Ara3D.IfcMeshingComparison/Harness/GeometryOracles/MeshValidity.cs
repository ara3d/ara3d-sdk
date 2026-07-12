using Ara3D.Geometry;

namespace Ara3D.IfcMeshingComparison.Harness.GeometryOracles;

/// <summary>Index-range and degenerate-triangle checks for triangle meshes.</summary>
public static class MeshValidity
{
    public const float DefaultMinArea = 1e-12f;

    public static bool HasValidIndices(TriangleMesh3D mesh)
    {
        var n = mesh.Points.Count;
        for (var i = 0; i < mesh.FaceIndices.Count; i++)
        {
            var f = mesh.FaceIndices[i];
            if (f.A < 0 || f.B < 0 || f.C < 0 || f.A >= n || f.B >= n || f.C >= n)
                return false;
        }
        return true;
    }

    public static int CountDegenerateTriangles(TriangleMesh3D mesh, float minArea = DefaultMinArea)
    {
        var count = 0;
        for (var i = 0; i < mesh.FaceIndices.Count; i++)
        {
            var f = mesh.FaceIndices[i];
            var a = mesh.Points[f.A].Vector3;
            var b = mesh.Points[f.B].Vector3;
            var c = mesh.Points[f.C].Vector3;
            var area = 0.5f * Vector3.Cross(b - a, c - a).Length();
            if (area < minArea)
                count++;
        }
        return count;
    }

    public static bool IsValid(TriangleMesh3D mesh, float minArea = DefaultMinArea)
        => mesh.FaceIndices.Count > 0 &&
           HasValidIndices(mesh) &&
           CountDegenerateTriangles(mesh, minArea) == 0;
}
