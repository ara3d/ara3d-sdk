using System.Collections.Generic;
using Ara3D.Geometry;
using Ara3D.Utils;
using Autodesk.Revit.DB;
using RevitColor = Autodesk.Revit.DB.Color;

namespace Ara3D.Revit.Utils;

/// <summary>
/// The geometry bridge: Revit DB types to and from ara3d-sdk types (P2 exemplar).
/// Everything downstream of this file works in sdk types; Revit types appear only where
/// the operation is inherently Revit. Revit internal units (feet) are preserved verbatim —
/// no unit conversion happens here (P7); that is the units module's job.
/// </summary>
public static class RevitConverters
{
    public static Point3D ToPoint3D(this XYZ p)
        => new((float)p.X, (float)p.Y, (float)p.Z);

    public static Vector3 ToVector3(this XYZ v)
        => new((float)v.X, (float)v.Y, (float)v.Z);

    public static XYZ ToXyz(this Point3D p)
        => new(p.X, p.Y, p.Z);

    public static XYZ ToXyz(this Vector3 v)
        => new(v.X, v.Y, v.Z);

    public static Point2D ToPoint2D(this UV uv)
        => new((float)uv.U, (float)uv.V);

    public static Vector2 ToVector2(this UV uv)
        => new((float)uv.U, (float)uv.V);

    public static UV ToUv(this Point2D p)
        => new(p.X, p.Y);

    public static UV ToUv(this Vector2 v)
        => new(v.X, v.Y);

    public static Line3D ToLine3D(this XYZ a, XYZ b)
        => new(a.ToPoint3D(), b.ToPoint3D());

    /// <summary>Endpoints of a bound (linear) curve as an sdk line; ignores curvature.</summary>
    public static Line3D ToLine3D(this Curve c)
        => new(c.GetEndPoint(0).ToPoint3D(), c.GetEndPoint(1).ToPoint3D());

    public static Line ToRevitLine(this Line3D line)
        => Line.CreateBound(line.A.ToXyz(), line.B.ToXyz());

    /// <summary>A closed loop of straight segments through <paramref name="points"/> (last point connects back to the first).</summary>
    public static CurveLoop ToCurveLoop(this IReadOnlyList<Point3D> points)
    {
        var loop = new CurveLoop();
        for (var i = 0; i < points.Count; ++i)
            loop.Append(Line.CreateBound(points[i].ToXyz(), points[(i + 1) % points.Count].ToXyz()));
        return loop;
    }

    public static Bounds3D ToBounds3D(this BoundingBoxXYZ b)
        => new(b.Min.ToPoint3D(), b.Max.ToPoint3D());

    public static BoundingBoxXYZ ToBoundingBox(this Bounds3D b)
        => new() { Min = b.Min.ToXyz(), Max = b.Max.ToXyz() };

    /// <summary>Revit's 0..255 byte color to the sdk's 0..1 normalized color (opaque).</summary>
    public static Ara3D.Geometry.Color ToColor(this RevitColor c)
        => new(c.Red / 255f, c.Green / 255f, c.Blue / 255f, 1f);

    /// <summary>sdk 0..1 color to Revit's 0..255 byte color. Alpha is not representable in <see cref="RevitColor"/> and is dropped.</summary>
    public static RevitColor ToRevitColor(this Ara3D.Geometry.Color color)
        => new(color.R.Value.ToByteFromNormalized(), color.G.Value.ToByteFromNormalized(), color.B.Value.ToByteFromNormalized());

    /// <summary>
    /// Revit transform to an sdk matrix. Rows carry the basis vectors and the origin, so a
    /// point row-vector times the matrix reproduces Transform.OfPoint.
    /// </summary>
    public static Matrix4x4 ToMatrix4x4(this Transform t)
    {
        if (t.IsIdentity)
            return Matrix4x4.Identity;

        var o = t.Origin.ToVector3();
        if (t.IsTranslation)
            return Matrix4x4.CreateTranslation(o);

        var x = t.BasisX.ToVector3();
        var y = t.BasisY.ToVector3();
        var z = t.BasisZ.ToVector3();
        return new Matrix4x4(
            x.X, x.Y, x.Z, 0f,
            y.X, y.Y, y.Z, 0f,
            z.X, z.Y, z.Z, 0f,
            o.X, o.Y, o.Z, 1f);
    }

    public static Transform ToTransform(this Matrix4x4 m)
    {
        var t = Transform.Identity;
        t.BasisX = new XYZ(m.M11, m.M12, m.M13);
        t.BasisY = new XYZ(m.M21, m.M22, m.M23);
        t.BasisZ = new XYZ(m.M31, m.M32, m.M33);
        t.Origin = new XYZ(m.M41, m.M42, m.M43);
        return t;
    }
}
