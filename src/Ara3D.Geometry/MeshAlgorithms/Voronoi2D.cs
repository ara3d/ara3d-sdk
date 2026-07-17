namespace Ara3D.Geometry;

/// <summary>
/// 2D Voronoi diagram by per-site half-plane clipping (Sutherland–Hodgman).
/// Each cell is the region of the plane closer to its site than to any other, clipped to a
/// bounding rectangle so every cell is a bounded convex CCW polygon. O(n²) in the site count —
/// intended for teaching and demo-scale point sets, not large meshes.
/// This is the geometric dual of the Delaunay triangulation (<see cref="DelaunayTriangulator"/>):
/// Voronoi cell corners are Delaunay triangle circumcenters.
/// </summary>
public static class Voronoi2D
{
    public const float Eps = 1e-7f;

    /// <summary>One Voronoi cell: the site index it belongs to, the site position, and its convex CCW boundary.</summary>
    public readonly record struct Cell(int Site, Vector2 Center, IReadOnlyList<Vector2> Polygon);

    /// <summary>Axis-aligned bounding box of the sites, expanded by <paramref name="padding"/> on every side.</summary>
    public static (Vector2 Min, Vector2 Max) BoundsOf(IReadOnlyList<Vector2> sites, float padding)
    {
        if (sites == null || sites.Count == 0)
            return (new Vector2(-padding, -padding), new Vector2(padding, padding));

        float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
        for (var i = 0; i < sites.Count; ++i)
        {
            float x = (float)sites[i].X, y = (float)sites[i].Y;
            if (x < minX) minX = x;
            if (y < minY) minY = y;
            if (x > maxX) maxX = x;
            if (y > maxY) maxY = y;
        }

        return (new Vector2(minX - padding, minY - padding), new Vector2(maxX + padding, maxY + padding));
    }

    /// <summary>Builds one bounded convex cell per site by clipping the bounding rectangle with every perpendicular bisector.</summary>
    public static IReadOnlyList<Cell> Compute(IReadOnlyList<Vector2> sites, Vector2 min, Vector2 max)
    {
        if (sites == null)
            throw new ArgumentNullException(nameof(sites));

        var cells = new List<Cell>(sites.Count);
        for (var i = 0; i < sites.Count; ++i)
            cells.Add(new Cell(i, sites[i], CellPolygon(sites, i, min, max)));

        return cells;
    }

    /// <summary>The convex CCW polygon of the single cell around <paramref name="site"/>.</summary>
    public static IReadOnlyList<Vector2> CellPolygon(IReadOnlyList<Vector2> sites, int site, Vector2 min, Vector2 max)
    {
        var poly = Rectangle(min, max);
        float px = (float)sites[site].X, py = (float)sites[site].Y;
        for (var j = 0; j < sites.Count && poly.Count >= 3; ++j)
        {
            if (j == site)
                continue;

            float qx = (float)sites[j].X, qy = (float)sites[j].Y;
            float nx = qx - px, ny = qy - py;
            if (nx * nx + ny * ny < Eps)
                continue;

            // Keep the half-plane closer to the site: points x with dot(n, x) <= dot(n, midpoint).
            var d = nx * (px + qx) * 0.5f + ny * (py + qy) * 0.5f;
            poly = ClipHalfPlane(poly, nx, ny, d);
        }

        return poly;
    }

    /// <summary>One Lloyd relaxation pass: move every unpinned site to the centroid of its Voronoi cell.</summary>
    public static IReadOnlyList<Vector2> RelaxOnce(
        IReadOnlyList<Vector2> sites, Vector2 min, Vector2 max, IReadOnlyCollection<int> pinned = null)
    {
        var cells = Compute(sites, min, max);
        var result = new List<Vector2>(sites);
        foreach (var cell in cells)
        {
            if (cell.Polygon.Count < 3 || (pinned != null && pinned.Contains(cell.Site)))
                continue;

            var c = PolygonTriangulator.Centroid(cell.Polygon);
            result[cell.Site] = new Vector2(
                Math.Clamp((float)c.X, (float)min.X, (float)max.X),
                Math.Clamp((float)c.Y, (float)min.Y, (float)max.Y));
        }

        return result;
    }

    /// <summary>
    /// Repeated Lloyd relaxation, converging toward a Centroidal Voronoi Tessellation (even, blue-noise
    /// spacing where each site sits at its own cell centroid).
    /// </summary>
    public static IReadOnlyList<Vector2> Relax(
        IReadOnlyList<Vector2> sites, Vector2 min, Vector2 max, int iterations, IReadOnlyCollection<int> pinned = null)
    {
        var result = sites;
        for (var i = 0; i < iterations; ++i)
            result = RelaxOnce(result, min, max, pinned);

        return result;
    }

    static List<Vector2> Rectangle(Vector2 min, Vector2 max)
        => new()
        {
            new Vector2(min.X, min.Y),
            new Vector2(max.X, min.Y),
            new Vector2(max.X, max.Y),
            new Vector2(min.X, max.Y),
        };

    /// <summary>Clips a convex CCW polygon to the half-plane { x : dot((nx,ny), x) &lt;= d }.</summary>
    static List<Vector2> ClipHalfPlane(IReadOnlyList<Vector2> poly, float nx, float ny, float d)
    {
        var result = new List<Vector2>(poly.Count + 1);
        for (var i = 0; i < poly.Count; ++i)
        {
            var a = poly[i];
            var b = poly[(i + 1) % poly.Count];
            float ax = (float)a.X, ay = (float)a.Y, bx = (float)b.X, by = (float)b.Y;
            var sa = nx * ax + ny * ay - d;
            var sb = nx * bx + ny * by - d;
            var aIn = sa <= 0;
            var bIn = sb <= 0;

            if (aIn)
                result.Add(a);

            if (aIn != bIn)
            {
                var t = sa / (sa - sb);
                result.Add(new Vector2(ax + t * (bx - ax), ay + t * (by - ay)));
            }
        }

        return result;
    }
}
