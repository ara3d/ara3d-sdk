namespace Ara3D.Geometry;

public enum SdfPrimitive3D
{
    Sphere,

    XAxis,
    YAxis,
    ZAxis,
    AllAxis,

    XYPlane,
    XZPlane,
    YZPlane,

    XSlab,
    YSlab,
    ZSlab,

    Cube,
    BoxXY,
    BoxXZ,
    BoxYZ,

    TorusZ,
    TorusX,
    TorusY,

    Octahedron,

    CrossSlabs,
    CrossAxes
}

public static class SdfPrimitives
{
    public static Number AbsDist(this Number a, Number b)
        => (a - b).Abs;

    public static Number Min(Number a, Number b)
        => Math.Min(a, b);

    public static Number Max(Number a, Number b)
        => Math.Max(a, b);

    public static Number Min(Number a, Number b, Number c)
        => Min(Min(a, b), c);

    public static Number Max(Number a, Number b, Number c)
        => Max(Max(a, b), c);

    public static Sdf3D Sphere(Number radius)
        => new(p => p.Vector3.Length - radius);

    public static Sdf3D XAxis(Number radius)
        => new(p => p.Vector3.YZ.Length - radius);

    public static Sdf3D YAxis(Number radius)
        => new(p => p.Vector3.XZ.Length - radius);

    public static Sdf3D ZAxis(Number radius)
        => new(p => p.Vector3.XY.Length - radius);

    public static Sdf3D AllAxis(Number radius)
        => new(p =>
            Min(
                p.Vector3.YZ.Length - radius,
                p.Vector3.XZ.Length - radius,
                p.Vector3.XY.Length - radius));

    // Infinite planes through origin.

    public static Sdf3D XYPlane()
        => new(p => p.Vector3.Z);

    public static Sdf3D XZPlane()
        => new(p => p.Vector3.Y);

    public static Sdf3D YZPlane()
        => new(p => p.Vector3.X);

    // Infinite slabs centered on origin.
    // Useful for walls, plates, clipping volumes, etc.

    public static Sdf3D XSlab(Number halfWidth)
        => new(p => p.Vector3.X.Abs - halfWidth);

    public static Sdf3D YSlab(Number halfWidth)
        => new(p => p.Vector3.Y.Abs - halfWidth);

    public static Sdf3D ZSlab(Number halfWidth)
        => new(p => p.Vector3.Z.Abs - halfWidth);

    // Cheap box-style SDFs.
    // These are exact inside and on the surface, but not Euclidean-exact outside corners.
    // For many modeling/blending cases this is still useful.

    public static Sdf3D Cube(Number halfSize)
        => new(p =>
            Max(
                p.Vector3.X.Abs,
                p.Vector3.Y.Abs,
                p.Vector3.Z.Abs) - halfSize);

    public static Sdf3D BoxXY(Number halfX, Number halfY)
        => new(p =>
            Max(
                p.Vector3.X.Abs - halfX,
                p.Vector3.Y.Abs - halfY));

    public static Sdf3D BoxXZ(Number halfX, Number halfZ)
        => new(p =>
            Max(
                p.Vector3.X.Abs - halfX,
                p.Vector3.Z.Abs - halfZ));

    public static Sdf3D BoxYZ(Number halfY, Number halfZ)
        => new(p =>
            Max(
                p.Vector3.Y.Abs - halfY,
                p.Vector3.Z.Abs - halfZ));

    // Tori around principal axes.
    // majorRadius = distance from center to tube center.
    // minorRadius = tube radius.

    public static Sdf3D TorusZ(Number majorRadius, Number minorRadius)
        => new(p =>
        {
            var qx = p.Vector3.XY.Length - majorRadius;
            var qy = p.Vector3.Z;
            return MathF.Sqrt(qx * qx + qy * qy) - minorRadius;
        });

    public static Sdf3D TorusX(Number majorRadius, Number minorRadius)
        => new(p =>
        {
            var qx = p.Vector3.YZ.Length - majorRadius;
            var qy = p.Vector3.X;
            return MathF.Sqrt(qx * qx + qy * qy) - minorRadius;
        });

    public static Sdf3D TorusY(Number majorRadius, Number minorRadius)
        => new(p =>
        {
            var qx = p.Vector3.XZ.Length - majorRadius;
            var qy = p.Vector3.Y;
            return MathF.Sqrt(qx * qx + qy * qy) - minorRadius;
        });

    // Regular octahedron centered at origin.
    // This is a common compact SDF approximation.

    public static Sdf3D Octahedron(Number radius)
        => new(p => (p.Vector3.X.Abs + p.Vector3.Y.Abs + p.Vector3.Z.Abs - radius) * 0.5773502691896258f);

    // Unions of simple primitives.

    public static Sdf3D CrossSlabs(Number halfWidth)
        => new(p =>
            Min(
                p.Vector3.X.Abs - halfWidth,
                p.Vector3.Y.Abs - halfWidth,
                p.Vector3.Z.Abs - halfWidth));

    public static Sdf3D CrossAxes(Number radius)
        => AllAxis(radius);

    public static Sdf3D Create(
        SdfPrimitive3D primitive,
        Number a,
        Number b = default)
    {
        return primitive switch
        {
            SdfPrimitive3D.Sphere => Sphere(a),

            SdfPrimitive3D.XAxis => XAxis(a),
            SdfPrimitive3D.YAxis => YAxis(a),
            SdfPrimitive3D.ZAxis => ZAxis(a),
            SdfPrimitive3D.AllAxis => AllAxis(a),

            SdfPrimitive3D.XYPlane => XYPlane(),
            SdfPrimitive3D.XZPlane => XZPlane(),
            SdfPrimitive3D.YZPlane => YZPlane(),

            SdfPrimitive3D.XSlab => XSlab(a),
            SdfPrimitive3D.YSlab => YSlab(a),
            SdfPrimitive3D.ZSlab => ZSlab(a),

            SdfPrimitive3D.Cube => Cube(a),
            SdfPrimitive3D.BoxXY => BoxXY(a, b),
            SdfPrimitive3D.BoxXZ => BoxXZ(a, b),
            SdfPrimitive3D.BoxYZ => BoxYZ(a, b),

            SdfPrimitive3D.TorusZ => TorusZ(a, b),
            SdfPrimitive3D.TorusX => TorusX(a, b),
            SdfPrimitive3D.TorusY => TorusY(a, b),

            SdfPrimitive3D.Octahedron => Octahedron(a),

            SdfPrimitive3D.CrossSlabs => CrossSlabs(a),
            SdfPrimitive3D.CrossAxes => CrossAxes(a),

            _ => throw new ArgumentOutOfRangeException(nameof(primitive), primitive, null)
        };
    }
}