namespace Ara3D.Geometry;

/// <summary>What remains of the old ear-clipping triangulator: two forwarding members that
/// examples/Ara3D.Studio.Examples still calls (Demos/Stairs.cs and Modifiers/CvtRemesh.cs).
/// New code calls <see cref="Earcut"/> and <see cref="PolygonOps"/> directly; delete this class
/// once those two examples do.</summary>
public static class PolygonTriangulator
{
    /// <summary>The triangles of a simple polygon as indices into it, wound counter-clockwise.</summary>
    public static List<Integer3> Triangulate(IReadOnlyList<Vector2> polygon)
        => Earcut.Triangulate(polygon, []);

    /// <summary>(b - a) x (c - a).</summary>
    public static float Cross(Vector2 a, Vector2 b, Vector2 c)
        => PolygonOps.Cross(a, b, c);
}
