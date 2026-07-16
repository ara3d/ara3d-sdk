namespace Ara3D.Geometry;

public static class OrientedBox3DExtensions
{
    public static double GetVolume(this OrientedBox3D box)
        => box.Size.X * box.Size.Y * box.Size.Z;

    public static OrientedBox3D FitOrientedBox(this IReadOnlyList<Vector3> pts)
    {
        var pca = new PrincipalComponentAnalysis(pts);
        return FitOrientedBox(pts, pca.Frame);
    }

    public static OrientedBox3D FitOrientedBox(this IReadOnlyList<Point3D> pts)
        => pts.Map(p => p.Vector3).FitOrientedBox();

    public static OrientedBox3D FitOrientedBox(this IReadOnlyList<Vector3> pts, Frame3D frame)
    {
        if (pts == null)
            throw new ArgumentNullException(nameof(pts));
        if (pts.Count == 0)
            throw new ArgumentException("Point list is empty.", nameof(pts));

        var localBounds = BoundsInFrame(pts, frame);

        var localCenter = localBounds.Center;
        var worldCenter = frame.ToWorld(localCenter);

        return new OrientedBox3D(frame.WithOrigin(worldCenter), localBounds.Size);
    }

    public static Bounds3D BoundsInFrame(this IReadOnlyList<Vector3> pts, Frame3D frame)
    {
        var b = Bounds3D.Empty;
        foreach (var p in pts)
            b = b.Include(frame.ToLocal(p));
        return b;
    }

    public static Vector3 LongestAxis(this OrientedBox3D box)
        => box.GetAxis(box.Size.GetLongestAxisIndex());

    public static Vector3 GetAxis(this OrientedBox3D box, int n)
        => box.Frame.GetAxis(n);

    public static float MaxSide(this OrientedBox3D box)
        => box.Size.MaxComponent;

    public static float MinSide(this OrientedBox3D box)
        => box.Size.MinComponent;

    public static Number MiddleComponent(this Vector3 self)
        => self.SumComponents - self.MaxComponent - self.MinComponent;

    public static float MiddleSide(this OrientedBox3D box)
        => box.Size.MiddleComponent();

    public static Line3D CenterLine(this OrientedBox3D box)
    {
        var half = box.LongestAxis() * (box.MaxSide() * 0.5f);
        return box.Frame.Origin.ToCenteredLine(half);
    }

    public static float Radius(this OrientedBox3D box)
        => box.MiddleSide().Half();

    public static Matrix4x4 ToMatrix(this OrientedBox3D obb)
        => obb.Size.ToScaleMatrix() * obb.Frame.Matrix;

    public static OrientedBox3D ToOrientedBox(this Cylinder cylinder)
        => cylinder.Line.ToOrientedBox(cylinder.Radius);

    public static OrientedBox3D ToOrientedBox(this Line3D line, float radius)
    {
        if (!float.IsFinite(radius) || radius < 0)
            throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be finite and non-negative.");

        var a = line.A.Vector3;
        var b = line.B.Vector3;

        if (!a.IsFinite() || !b.IsFinite())
            throw new ArgumentException("Cylinder line endpoints must be finite.", nameof(line));

        var axisVector = b - a;
        var length = axisVector.Length();

        if (!float.IsFinite(length) || length <= 0)
            throw new ArgumentException("Cylinder line must have non-zero finite length.", nameof(line));

        var z = axisVector / length;
        var center = (a + b) * 0.5f;
        var basis = z.CreateBasisFromZ();
        var frame = new Frame3D(center, basis);
        var diameter = radius * 2f;
        return new OrientedBox3D(frame, new Vector3(diameter, diameter, length));
    }

}
