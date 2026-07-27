namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Convert)]
[Description("Sweeps a circular profile along a parametric curve to produce a tube surface.")]
public class CurveToTube : IModifier
{
    [Range(0.01f, 5f)] public float Radius = 0.1f;
    [Range(3, 64)] public int Sides = 16;
    [Range(2, 256)] public int Samples = 64;

    public QuadGrid3D Eval(Curve3D curve)
        => TubeConverters.TubeFromFrames(curve.GetTransforms(Samples), Radius, Sides, closedPath: false);
}

[Category(Cat.Convert)]
[Description("Sweeps a circular profile along a polyline to produce a tube surface.")]
public class PolylineToTube : IModifier
{
    [Range(0.01f, 5f)] public float Radius = 0.1f;
    [Range(3, 64)] public int Sides = 16;

    public QuadGrid3D Eval(Polyline3D polyline)
        => TubeConverters.TubeFromFrames(polyline.GetPathTransforms(), Radius, Sides, polyline.Closed);
}

[Category(Cat.Convert)]
[Description("Replaces each input line segment with a cylindrical tube (instanced).")]
public class LinesToTubes : IModifier
{
    [Range(0.01f, 5f)] public float Radius = 0.1f;
    [Range(3, 64)] public int Sides = 16;

    public IModel3D Eval(LineMesh3D lines)
        => new Model3DBuilder().AddCylinders(lines.Lines, Radius, Sides).Build();
}

[Category(Cat.Convert)]
[Description("Samples a parametric curve into an explicit polyline.")]
public class CurveToPolyline : IModifier
{
    [Range(2, 1024)] public int Samples = 64;
    public bool Closed;

    public Polyline3D Eval(Curve3D curve)
        => new(curve.Sample(Samples), Closed);
}

[Category(Cat.Convert)]
[Description("Converts a polyline into a line mesh of connected segments.")]
public class PolylineToLines : IModifier
{
    public LineMesh3D Eval(Polyline3D polyline)
        => polyline.Points.ToLineMesh(polyline.Closed);
}

[Category(Cat.Convert)]
[Description("Samples a parametric curve into a line mesh.")]
public class CurveToLines : IModifier
{
    [Range(2, 1024)] public int Samples = 64;
    public bool Closed;

    public LineMesh3D Eval(Curve3D curve)
        => curve.Sample(Samples).ToLineMesh(Closed);
}

/// <summary>Shared helpers for path → tube converters.</summary>
public static class TubeConverters
{
    public static readonly Angle QuarterTurn = 0.25f.Turns();

    public static IReadOnlyList<Point3D> CircularProfile(float radius, int sides)
        => Curves.Circle.Scale(radius).RotateX(QuarterTurn).Sample(Math.Max(3, sides));

    public static QuadGrid3D TubeFromFrames(
        IReadOnlyList<Transform3D> frames,
        float radius,
        int sides,
        bool closedPath)
    {
        if (frames == null || frames.Count < 2)
            return new QuadGrid3D(FunctionalReadOnlyList2D<Point3D>.Default, false, false);
        return CircularProfile(radius, sides).Sweep(frames, connectU: true, connectV: closedPath);
    }

    public static IReadOnlyList<Transform3D> GetPathTransforms(this Polyline3D polyline)
    {
        var pts = polyline.Points;
        var n = pts.Count;
        if (n < 2)
            return [];

        var frames = new Transform3D[n];
        for (var i = 0; i < n; i++)
        {
            if (polyline.Closed)
            {
                frames[i] = pts[i].LookAt(pts[(i + 1) % n]);
                continue;
            }

            if (i < n - 1)
            {
                frames[i] = pts[i].LookAt(pts[i + 1]);
                continue;
            }

            // Last open vertex: keep the previous segment's direction, origin at the tip.
            frames[i] = pts[i - 1].LookAt(pts[i]).WithOrigin(pts[i]);
        }

        return frames;
    }
}
