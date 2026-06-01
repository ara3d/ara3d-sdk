using System.Diagnostics;

namespace Ara3D.Geometry;

// NOTE: these were written with the help of ChatGPT in the context of creating a PCA class.
public static class GeometryNumerics
{
    public const double DefaultEpsilon = 1e-12;
    public const float DefaultFloatEpsilon = 1e-6f;

    public static bool IsFinite(this Vector3 v)
        => float.IsFinite(v.X) && float.IsFinite(v.Y) && float.IsFinite(v.Z);

    public static bool IsUnit(this Vector3 v, float tolerance = 1e-4f)
        => Math.Abs(v.LengthSquared() - 1f) <= tolerance;

    public static Vector3 NormalizeSafe(this Vector3 v, Vector3 fallback, float epsilon = DefaultFloatEpsilon)
    {
        Debug.Assert(fallback.IsFinite());
        Debug.Assert(fallback.LengthSquared() > epsilon * epsilon);

        if (!v.IsFinite())
            return fallback.Normalize;

        var lenSq = v.LengthSquared();
        return lenSq > epsilon * epsilon
            ? v / MathF.Sqrt(lenSq)
            : fallback.Normalize;
    }

    public static Vector3 AnyPerpendicular(Vector3 axis)
    {
        Debug.Assert(axis.IsFinite());
        Debug.Assert(axis.LengthSquared() > 0);

        axis = axis.Normalize();

        var other = Math.Abs(axis.X) < 0.9f
            ? Vector3.UnitX
            : Vector3.UnitY;

        return axis.NormalizedCross(other);
    }

    public static double DistanceToLine(Vector3 p, Vector3 linePoint, Vector3 lineDir)
    {
        Debug.Assert(p.IsFinite());
        Debug.Assert(linePoint.IsFinite());
        Debug.Assert(lineDir.IsUnit());

        var d = p - linePoint;
        var axial = Vector3.Dot(d, lineDir);
        var radial = d - axial * lineDir;
        return radial.Length();
    }

    public static Vector3 ProjectOntoLine(Vector3 p, Vector3 linePoint, Vector3 lineDir)
    {
        Debug.Assert(p.IsFinite());
        Debug.Assert(linePoint.IsFinite());
        Debug.Assert(lineDir.IsUnit());

        var t = Vector3.Dot(p - linePoint, lineDir);
        return linePoint + t * lineDir;
    }

    public static double SignedDistanceAlongLine(Vector3 p, Vector3 linePoint, Vector3 lineDir)
    {
        Debug.Assert(p.IsFinite());
        Debug.Assert(linePoint.IsFinite());
        Debug.Assert(lineDir.IsUnit());

        return Vector3.Dot(p - linePoint, lineDir);
    }

    public static Vector3 Reject(Vector3 v, Vector3 unitAxis)
    {
        Debug.Assert(unitAxis.IsUnit());
        return v - Vector3.Dot(v, unitAxis) * unitAxis;
    }
}