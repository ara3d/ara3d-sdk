using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;

namespace Ara3D.IfcMeshingComparison.Harness.GeometryOracles;

/// <summary>Signed-volume and outward-normal consistency checks.</summary>
public static class WindingOracle
{
    public static double SignedVolume(TriangleMesh3D mesh)
        => MeshHelpers.SignedVolume(mesh);

    public static Point3D Centroid(TriangleMesh3D mesh)
    {
        if (mesh.Points.Count == 0)
            return Point3D.Zero;
        var sum = Vector3.Zero;
        for (var i = 0; i < mesh.Points.Count; i++)
            sum += mesh.Points[i].Vector3;
        var c = sum / mesh.Points.Count;
        return new Point3D(c.X, c.Y, c.Z);
    }

    /// <summary>Fraction of faces whose normal points away from the mesh centroid (outward for convex solids).</summary>
    public static float OutwardNormalFraction(TriangleMesh3D mesh)
    {
        if (mesh.FaceIndices.Count == 0)
            return 0f;
        var center = Centroid(mesh).Vector3;
        var outward = 0;
        for (var i = 0; i < mesh.FaceIndices.Count; i++)
        {
            var f = mesh.FaceIndices[i];
            var a = mesh.Points[f.A].Vector3;
            var b = mesh.Points[f.B].Vector3;
            var c = mesh.Points[f.C].Vector3;
            var normal = Vector3.Cross(b - a, c - a);
            var faceCenter = (a + b + c) / 3f;
            if (Vector3.Dot(normal, faceCenter - center) > 0)
                outward++;
        }
        return outward / (float)mesh.FaceIndices.Count;
    }

    public static bool HasPositiveSignedVolume(TriangleMesh3D mesh, double minVolume = 1e-8)
        => SignedVolume(mesh) > minVolume;

    public static bool HasOutwardWinding(TriangleMesh3D mesh, float minFraction = 0.9f)
        => OutwardNormalFraction(mesh) >= minFraction;

    /// <summary>Applies a negative-determinant transform and checks MeshHelpers.Transform preserves outward winding.</summary>
    public static bool MirrorTransformPreservesOutwardWinding(TriangleMesh3D mesh)
    {
        var mirror = new Matrix4x4(
            -1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, 1f, 0f,
            0f, 0f, 0f, 1f);
        var transformed = MeshHelpers.Transform(mesh, mirror);
        return HasPositiveSignedVolume(transformed) && HasOutwardWinding(transformed);
    }
}
