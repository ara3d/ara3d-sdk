namespace Ara3D.Studio.Samples.Modifiers;

public enum VoronoiPlane
{
    XY,
    XZ,
    YZ,
}

/// <summary>
/// Centroidal-Voronoi (Lloyd) remesh of a planar / height-field mesh — the production twin of the
/// Relaxation slider in the Voronoi demo (ara3d-046). The vertices are projected to a working plane
/// and treated as Voronoi sites; repeated Lloyd relaxation slides each interior site to its cell
/// centroid, yielding an even blue-noise distribution; the relaxed points are re-triangulated
/// (Delaunay) and lifted back. Kills sliver triangles and clumping — better shading and simulation.
///
/// v1 is planar: each vertex keeps its own off-plane coordinate, so the result is exact for flat
/// input and approximate for gently curved input. PinBoundary keeps the convex outline fixed.
/// True curved-surface CVT (geodesic cells) is the deferred follow-on.
/// </summary>
[Category(nameof(Categories.Meshes))]
public class CvtRemesh : IModifier
{
    [Range(0, 40)] public int Iterations = 10;
    public VoronoiPlane Plane = VoronoiPlane.XY;
    public bool PinBoundary = true;

    public int VertexCount { get; private set; }

    static (Vector2 Uv, float W) Project(Point3D p, VoronoiPlane plane)
    {
        float x = (float)p.X, y = (float)p.Y, z = (float)p.Z;
        return plane switch
        {
            VoronoiPlane.XZ => (new Vector2(x, z), y),
            VoronoiPlane.YZ => (new Vector2(y, z), x),
            _ => (new Vector2(x, y), z),
        };
    }

    static Point3D Unproject(Vector2 uv, float w, VoronoiPlane plane)
    {
        float u = (float)uv.X, v = (float)uv.Y;
        return plane switch
        {
            VoronoiPlane.XZ => new Vector3(u, w, v),
            VoronoiPlane.YZ => new Vector3(w, u, v),
            _ => new Vector3(u, v, w),
        };
    }

    // Convex-hull vertex indices (Andrew's monotone chain), used to pin the outer boundary.
    static HashSet<int> HullIndices(IReadOnlyList<Vector2> pts)
    {
        var n = pts.Count;
        if (n < 3)
            return new HashSet<int>(Enumerable.Range(0, n));

        var order = Enumerable.Range(0, n).ToList();
        order.Sort((i, j) =>
        {
            var c = ((float)pts[i].X).CompareTo((float)pts[j].X);
            return c != 0 ? c : ((float)pts[i].Y).CompareTo((float)pts[j].Y);
        });

        var hull = new List<int>();
        for (var pass = 0; pass < 2; ++pass)
        {
            var start = hull.Count;
            foreach (var idx in pass == 0 ? order : Enumerable.Reverse(order))
            {
                while (hull.Count - start >= 2 &&
                       PolygonTriangulator.Cross(pts[hull[^2]], pts[hull[^1]], pts[idx]) <= 0)
                    hull.RemoveAt(hull.Count - 1);
                hull.Add(idx);
            }

            hull.RemoveAt(hull.Count - 1);
        }

        return new HashSet<int>(hull);
    }

    public TriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var pts = mesh.Points;
        if (pts.Count < 3)
            return mesh;

        var uv = new List<Vector2>(pts.Count);
        var w = new float[pts.Count];
        for (var i = 0; i < pts.Count; ++i)
        {
            var (p2, wi) = Project(pts[i], Plane);
            uv.Add(p2);
            w[i] = wi;
        }

        var (min, max) = Voronoi2D.BoundsOf(uv, 1e-3f);
        var pinned = PinBoundary ? HullIndices(uv) : null;
        var relaxed = Voronoi2D.Relax(uv, min, max, Iterations, pinned);

        var faces = DelaunayTriangulator.Triangulate(relaxed);

        var points = new List<Point3D>(relaxed.Count);
        for (var i = 0; i < relaxed.Count; ++i)
            points.Add(Unproject(relaxed[i], w[i], Plane));

        VertexCount = points.Count;
        ctx.Services.RefreshUI(this);
        return new TriangleMesh3D(points, faces);
    }
}
