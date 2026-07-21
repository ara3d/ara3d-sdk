using System;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class RebarOrientationQueries
{
    private const double Tolerance = 1e-9;

    /// <summary>
    /// True when <paramref name="line"/> is perpendicular to the plane of <paramref name="face"/>'s
    /// first triangle, after applying the optional instance transforms. Used to test whether a rebar
    /// segment runs vertical relative to its host face.
    /// </summary>
    public static bool IsVertical(this Face face, Line line, Transform? faceTransform = null, Transform? lineTransform = null)
    {
        var mesh = face.Triangulate();
        if (mesh.Vertices.Count < 3)
            return false;

        var p0 = mesh.Vertices[0];
        var p1 = mesh.Vertices[1];
        var p2 = mesh.Vertices[2];
        var lineStart = line.GetEndPoint(0);
        var lineEnd = line.GetEndPoint(1);

        if (faceTransform != null)
        {
            p0 = faceTransform.OfPoint(p0);
            p1 = faceTransform.OfPoint(p1);
            p2 = faceTransform.OfPoint(p2);
        }

        if (lineTransform != null)
        {
            lineStart = lineTransform.OfPoint(lineStart);
            lineEnd = lineTransform.OfPoint(lineEnd);
        }

        var edge1 = p0 - p1;
        var edge2 = p0 - p2;
        var lineVec = lineStart - lineEnd;
        return Math.Abs(edge1.DotProduct(lineVec)) <= Tolerance
            && Math.Abs(edge2.DotProduct(lineVec)) <= Tolerance;
    }

    /// <summary>True when <paramref name="line1"/> and <paramref name="line2"/> are mutually perpendicular.</summary>
    public static bool IsVertical(this Line line1, Line line2)
    {
        var v1 = line1.GetEndPoint(0) - line1.GetEndPoint(1);
        var v2 = line2.GetEndPoint(0) - line2.GetEndPoint(1);
        return Math.Abs(v1.DotProduct(v2)) < Tolerance;
    }
}
