using Ara3D.Geometry;

namespace Ara3D.IfcMeshingComparison.Harness.GeometryOracles;

/// <summary>Point-in-mesh probes via winding-number / ray cast for convex and near-convex solids.</summary>
public static class ContainmentOracle
{
    /// <summary>
    /// Ray-cast along +X from <paramref name="point"/>; odd crossings ⇒ inside.
    /// Best for closed solids without self-intersection.
    /// </summary>
    public static bool ContainsPoint(TriangleMesh3D mesh, Vector3 point, float eps = 1e-5f)
    {
        var crossings = 0;
        for (var i = 0; i < mesh.FaceIndices.Count; i++)
        {
            var f = mesh.FaceIndices[i];
            var a = mesh.Points[f.A].Vector3;
            var b = mesh.Points[f.B].Vector3;
            var c = mesh.Points[f.C].Vector3;
            if (RayHitsTriangle(point, Vector3.UnitX, a, b, c, eps))
                crossings++;
        }
        return (crossings % 2) == 1;
    }

    static bool RayHitsTriangle(
        Vector3 origin, Vector3 dir,
        Vector3 v0, Vector3 v1, Vector3 v2,
        float eps)
    {
        var e1 = v1 - v0;
        var e2 = v2 - v0;
        var p = Vector3.Cross(dir, e2);
        var det = Vector3.Dot(e1, p);
        if (MathF.Abs(det) < eps)
            return false;
        var invDet = 1f / det;
        var tvec = origin - v0;
        var u = Vector3.Dot(tvec, p) * invDet;
        if (u < 0 || u > 1)
            return false;
        var q = Vector3.Cross(tvec, e1);
        var v = Vector3.Dot(dir, q) * invDet;
        if (v < 0 || u + v > 1)
            return false;
        var t = Vector3.Dot(e2, q) * invDet;
        return t > eps;
    }
}
