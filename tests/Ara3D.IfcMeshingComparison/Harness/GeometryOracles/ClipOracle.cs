using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;

namespace Ara3D.IfcMeshingComparison.Harness.GeometryOracles;

/// <summary>Keep-side, plane-vertex, and post-clip winding checks for boolean cuts.</summary>
public static class ClipOracle
{
    public static bool NearPlaneVerticesLieOnPlane(
        TriangleMesh3D mesh,
        Vector3 planePoint,
        Vector3 planeNormal,
        float planeBand = 1e-2f,
        float eps = 1e-3f)
    {
        var n = planeNormal.Normalize;
        for (var i = 0; i < mesh.Points.Count; i++)
        {
            var d = Vector3.Dot(mesh.Points[i].Vector3 - planePoint, n);
            if (MathF.Abs(d) <= planeBand && MathF.Abs(d) > eps)
                return false;
        }
        return true;
    }

    public static bool PostClipFacesOutward(TriangleMesh3D clipped, float minFraction = 0.85f)
        => WindingOracle.OutwardNormalFraction(clipped) >= minFraction;

    /// <summary>
    /// Grid-samples (x,z) over the solid AABB and classifies whether a vertex exists near each kept/discarded region.
    /// For a Z-plane mid cut with polygonal gate: outside the prism, high-Z must remain; inside, high-Z must be gone.
    /// </summary>
    public static (bool KeptHighOutside, bool ClippedInside) SamplePolygonalZClipRegions(
        TriangleMesh3D mesh,
        float planeZ,
        float prismMinX,
        float highZThreshold)
    {
        var keptHighOutside = false;
        var clippedInside = false;
        for (var i = 0; i < mesh.Points.Count; i++)
        {
            var p = mesh.Points[i].Vector3;
            if (p.Z > highZThreshold && p.X < prismMinX)
                keptHighOutside = true;
            if (MathF.Abs(p.Z - planeZ) < 1e-3f && p.X > prismMinX - 0.1f)
                clippedInside = true;
        }
        return (keptHighOutside, clippedInside);
    }

    public static bool ClippedFacesMatchReferenceNormal(
        TriangleMesh3D clipped,
        Vector3 referenceNormal,
        float minFraction = 0.99f)
    {
        if (clipped.FaceIndices.Count == 0)
            return false;
        var refN = referenceNormal.Normalize;
        var ok = 0;
        for (var i = 0; i < clipped.FaceIndices.Count; i++)
        {
            var f = clipped.FaceIndices[i];
            var a = clipped.Points[f.A].Vector3;
            var b = clipped.Points[f.B].Vector3;
            var c = clipped.Points[f.C].Vector3;
            var n = Vector3.Cross(b - a, c - a);
            if (n.LengthSquared() < 1e-20f || Vector3.Dot(n.Normalize, refN) > 0)
                ok++;
        }
        return ok / (float)clipped.FaceIndices.Count >= minFraction;
    }

    public static bool MaxBoundAtMost(TriangleMesh3D mesh, float maxZ, float tol = 1e-4f)
        => MeshHelpers.GetBounds(mesh).Max.Z.Value <= maxZ + tol;

    public static bool MinBoundAtLeast(TriangleMesh3D mesh, float minZ, float tol = 1e-4f)
        => MeshHelpers.GetBounds(mesh).Min.Z.Value >= minZ - tol;
}
