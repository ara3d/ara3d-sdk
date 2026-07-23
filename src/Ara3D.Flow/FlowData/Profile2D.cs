using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// A closed 2D outline (a profile / polygon) flowing through the graph — the source for extrude,
/// loft, revolve and sweep operations (tracker studio-168). Points are ordered around the loop;
/// the closing edge (last → first) is implicit. Its default rendering lifts the loop onto the XY
/// plane and draws it as a closed polyline.
/// </summary>
public sealed class Profile2D
{
    public IReadOnlyList<Vector2> Points { get; }
    public int Count => Points.Count;

    public Profile2D(IReadOnlyList<Vector2> points)
        => Points = points ?? [];

    public Profile2D Map(Func<Vector2, Vector2> f)
        => new(Points.Select(f).ToList());
}
