using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// An ordered chain of 3D points — a single connected polyline, open or closed (tracker
/// studio-168). Distinct from the procedural <see cref="Curve3D"/>: this is explicit sampled
/// geometry (an imported path, a routed pipe centreline, a traced edge). Its default rendering
/// draws it as line segments. For an arbitrary many-branch network use a LineMesh3D directly,
/// which already flows and renders.
/// </summary>
public sealed class Polyline3D : IDeformable3D<Polyline3D>
{
    public IReadOnlyList<Point3D> Points { get; }
    public bool Closed { get; }
    public int Count => Points.Count;

    public Polyline3D(IReadOnlyList<Point3D> points, bool closed = false)
    {
        Points = points ?? [];
        Closed = closed;
    }

    public Polyline3D Map(Func<Point3D, Point3D> f)
        => new(Points.Select(f).ToList(), Closed);

    public Polyline3D Deform(Func<Point3D, Point3D> f)
        => Map(f);

    public Polyline3D Transform(Transform3D t)
        => Deform(t.TransformPoint);
}
