using Color = Ara3D.Geometry.Color;
using Material = Ara3D.Models.Material;

namespace Ara3D.Studio.Samples.Demos;

public enum VoronoiPointPattern
{
    Random,
    Grid,
    Circle,
}

/// <summary>
/// Interactive Voronoi diagram explainer — the sibling of <see cref="DelaunayTriangulationDemo"/>.
///
/// A Voronoi cell is "everywhere closer to *this* site than to any other." Each cell is built by
/// clip-per-site: start from the bounding rectangle and repeatedly clip it by the perpendicular
/// bisector half-plane against every other site (Sutherland–Hodgman). The result is always a bounded,
/// convex, correct cell — ideal for teaching, O(n^2) but fine at demo point counts.
///
/// The teaching hook is duality: Voronoi is the dual of the Delaunay triangulation. Turn on
/// ShowDelaunayDual to overlay the Delaunay edges (blue) and the triangle circumcenters (the Voronoi
/// *vertices*) on top of the cell mosaic — the single clearest way to see the relationship.
///
/// Drag Reveal from 0 to 1 to add sites one at a time (the Voronoi analogue of the Delaunay Insertion
/// slider): the last site is gold and its freshly carved cell is highlighted orange.
/// </summary>
[Category(Cat.ExperimentalDemos)]
[Description("Interactive Voronoi diagram explainer with incremental site reveal and an optional Delaunay-dual overlay.")]
public class VoronoiDiagramDemo : IGenerator
{
    [Range(2, 300)] public int PointCount = 32;
    [Range(0.1f, 10f)] public float Radius = 2f;
    public VoronoiPointPattern Pattern = VoronoiPointPattern.Random;
    [Range(0f, 1f)] public float Reveal = 1f;
    public bool ShowCells = true;
    public bool ShowEdges = true;
    public bool ShowSites = true;
    public bool ShowDelaunayDual;
    [Range(0.05f, 1f)] public float BoundsPadding = 0.25f;
    [Range(0.002f, 0.1f)] public float EdgeRadius = 0.015f;
    [Range(0.01f, 0.3f)] public float PointSize = 0.05f;

    // ---- Point patterns (mirrors the Delaunay demo, in 2D) -------------------------------------

    IReadOnlyList<Vector2> RandomSites(int seed)
    {
        var random = new Random(seed);
        var sites = new List<Vector2>(PointCount);
        for (var i = 0; i < PointCount; ++i)
        {
            var angle = random.NextSingle() * MathF.Tau;
            var r = Radius * MathF.Sqrt(random.NextSingle());
            sites.Add(new Vector2(r * MathF.Cos(angle), r * MathF.Sin(angle)));
        }

        return sites;
    }

    IReadOnlyList<Vector2> GridSites()
    {
        var side = Math.Max(2, (int)MathF.Ceiling(MathF.Sqrt(PointCount)));
        var spacing = Radius * 2f / (side - 1);
        var sites = new List<Vector2>(side * side);
        for (var y = 0; y < side; ++y)
        for (var x = 0; x < side; ++x)
            sites.Add(new Vector2(-Radius + x * spacing, -Radius + y * spacing));

        return sites;
    }

    IReadOnlyList<Vector2> CircleSites()
    {
        var sites = new List<Vector2>(PointCount);
        for (var i = 0; i < PointCount; ++i)
        {
            var angle = i / (float)PointCount * MathF.Tau;
            sites.Add(new Vector2(Radius * MathF.Cos(angle), Radius * MathF.Sin(angle)));
        }

        return sites;
    }

    IReadOnlyList<Vector2> GetSites()
        => Pattern switch
        {
            VoronoiPointPattern.Grid => GridSites(),
            VoronoiPointPattern.Circle => CircleSites(),
            _ => RandomSites(42),
        };

    // ---- Clip-per-site Voronoi cell ------------------------------------------------------------

    // Axis-aligned rectangle enclosing every site, padded so outer cells stay finite.
    static (Vector2 Min, Vector2 Max) SiteBounds(IReadOnlyList<Vector2> sites, float pad)
    {
        var minX = float.MaxValue;
        var minY = float.MaxValue;
        var maxX = float.MinValue;
        var maxY = float.MinValue;
        foreach (var s in sites)
        {
            minX = MathF.Min(minX, (float)s.X);
            minY = MathF.Min(minY, (float)s.Y);
            maxX = MathF.Max(maxX, (float)s.X);
            maxY = MathF.Max(maxY, (float)s.Y);
        }

        var min = new Vector2(minX, minY);
        var max = new Vector2(maxX, maxY);
        var span = MathF.Max(maxX - minX, maxY - minY);
        var margin = MathF.Max(span * pad, pad);
        return (new Vector2(min.X - margin, min.Y - margin), new Vector2(max.X + margin, max.Y + margin));
    }

    // Sutherland–Hodgman: clip a convex polygon to the half-plane { p : dot(p, normal) <= offset }.
    static List<Vector2> ClipHalfPlane(List<Vector2> poly, Vector2 normal, float offset)
    {
        const float eps = 1e-7f;
        var result = new List<Vector2>(poly.Count + 1);
        for (var i = 0; i < poly.Count; ++i)
        {
            var a = poly[i];
            var b = poly[(i + 1) % poly.Count];
            var da = (float)a.Dot(normal) - offset;
            var db = (float)b.Dot(normal) - offset;
            var aIn = da <= eps;
            var bIn = db <= eps;

            if (aIn)
                result.Add(a);
            if (aIn != bIn)
            {
                var t = da / (da - db);
                result.Add(a + (b - a) * t);
            }
        }

        return result;
    }

    // The cell of `site`: the bounding rectangle clipped by the perpendicular bisector against
    // every other site (keep the half-plane closer to `site`).
    static List<Vector2> Cell(Vector2 site, IReadOnlyList<Vector2> sites, (Vector2 Min, Vector2 Max) box)
    {
        var poly = new List<Vector2>
        {
            box.Min,
            new(box.Max.X, box.Min.Y),
            box.Max,
            new(box.Min.X, box.Max.Y),
        };

        foreach (var other in sites)
        {
            var normal = other - site;
            if ((float)normal.LengthSquared() < 1e-12f)
                continue;
            // p closer to site than other  <=>  dot(p, other - site) <= (|other|^2 - |site|^2) / 2
            var offset = 0.5f * ((float)other.LengthSquared() - (float)site.LengthSquared());
            poly = ClipHalfPlane(poly, normal, offset);
            if (poly.Count < 3)
                break;
        }

        return poly;
    }

    // ---- Delaunay dual overlay -----------------------------------------------------------------

    static Vector2 Circumcenter(Vector2 a, Vector2 b, Vector2 c)
    {
        float ax = (float)a.X, ay = (float)a.Y;
        float bx = (float)b.X, by = (float)b.Y;
        float cx = (float)c.X, cy = (float)c.Y;
        var d = 2f * (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
        if (MathF.Abs(d) < 1e-12f)
            return new Vector2((ax + bx + cx) / 3f, (ay + by + cy) / 3f);

        var a2 = ax * ax + ay * ay;
        var b2 = bx * bx + by * by;
        var c2 = cx * cx + cy * cy;
        var ux = (a2 * (by - cy) + b2 * (cy - ay) + c2 * (ay - by)) / d;
        var uy = (a2 * (cx - bx) + b2 * (ax - cx) + c2 * (bx - ax)) / d;
        return new Vector2(ux, uy);
    }

    // ---- Geometry helpers ----------------------------------------------------------------------

    static TriangleMesh3D DoubleSidedFan(IReadOnlyList<Vector2> poly, float z)
    {
        var points = poly.Select(p => (Point3D)new Vector3(p.X, p.Y, z)).ToList();
        var faces = new List<Integer3>((poly.Count - 2) * 2);
        for (var i = 1; i < poly.Count - 1; ++i)
        {
            faces.Add(new Integer3(0, i, i + 1));
            faces.Add(new Integer3(0, i + 1, i));
        }

        return new TriangleMesh3D(points, faces);
    }

    public IModel3D Eval()
    {
        var allSites = GetSites();
        var revealed = Math.Clamp((int)MathF.Round(Reveal * allSites.Count), 1, allSites.Count);
        var sites = allSites.Take(revealed).ToList();
        var lastIndex = revealed - 1;
        var partial = revealed < allSites.Count;
        var box = SiteBounds(allSites, BoundsPadding);

        var cells = sites.Select(s => Cell(s, sites, box)).ToList();

        var mb = new Model3DBuilder();

        if (ShowCells)
        {
            for (var i = 0; i < cells.Count; ++i)
            {
                var cell = cells[i];
                if (cell.Count < 3)
                    continue;
                var isNew = partial && i == lastIndex;
                var color = isNew ? new Color(1f, 0.55f, 0.15f, 1f) : DelaunayTriangulationDemo.Pastel(i / (float)Math.Max(cells.Count, 1));
                mb.AddInstance(DoubleSidedFan(cell, 0f), new Material(color, 0f, 0.7f));
            }
        }

        if (ShowEdges)
        {
            var tube = mb.AddMeshWithoutInstance(DelaunayTriangulationDemo.UnitTube(EdgeRadius));
            var material = new Material(new Color(0.25f, 0.25f, 0.3f, 1f), 0.1f, 0.5f);
            var lift = new Vector3(0, 0, EdgeRadius);
            foreach (var cell in cells)
            {
                if (cell.Count < 2)
                    continue;
                for (var i = 0; i < cell.Count; ++i)
                {
                    var a = cell[i];
                    var b = cell[(i + 1) % cell.Count];
                    var line = new Line3D(new Vector3(a.X, a.Y, 0) + lift, new Vector3(b.X, b.Y, 0) + lift);
                    if (line.Length > 1e-6f)
                        mb.AddInstance(tube, line.AlignZAxisTransform(), material);
                }
            }
        }

        if (ShowDelaunayDual && sites.Count >= 3)
        {
            var faces = DelaunayTriangulator.Triangulate(sites);
            var tube = mb.AddMeshWithoutInstance(DelaunayTriangulationDemo.UnitTube(EdgeRadius * 0.75f));
            var edgeMaterial = new Material(new Color(0.1f, 0.45f, 0.9f, 1f), 0.2f, 0.4f);
            var vertexMaterial = new Material(new Color(1f, 0.75f, 0.2f, 1f), 0.5f, 0.3f);
            var lift = new Vector3(0, 0, EdgeRadius * 3f);

            var edges = new HashSet<(int, int)>();
            foreach (var f in faces)
            {
                edges.Add(f.A < f.B ? (f.A, f.B) : (f.B, f.A));
                edges.Add(f.B < f.C ? (f.B, f.C) : (f.C, f.B));
                edges.Add(f.C < f.A ? (f.C, f.A) : (f.A, f.C));
            }

            foreach (var (a, b) in edges)
            {
                var line = new Line3D(
                    new Vector3(sites[a].X, sites[a].Y, 0) + lift,
                    new Vector3(sites[b].X, sites[b].Y, 0) + lift);
                if (line.Length > 1e-6f)
                    mb.AddInstance(tube, line.AlignZAxisTransform(), edgeMaterial);
            }

            // Voronoi vertices are the Delaunay triangle circumcenters.
            foreach (var f in faces)
            {
                var c = Circumcenter(sites[f.A], sites[f.B], sites[f.C]);
                mb.AddInstance(
                    DelaunayTriangulationDemo.Marker(new Vector3(c.X, c.Y, 0) + lift, PointSize * 0.6f),
                    vertexMaterial);
            }
        }

        if (ShowSites)
        {
            var siteMaterial = new Material(new Color(0.1f, 0.6f, 0.85f, 1f), 0.2f, 0.4f);
            var pendingMaterial = new Material(new Color(0.4f, 0.4f, 0.4f, 1f), 0f, 0.8f);
            var lastMaterial = new Material(new Color(1f, 0.8f, 0.1f, 1f), 0.6f, 0.3f);
            var lift = new Vector3(0, 0, EdgeRadius * 2f);

            for (var i = 0; i < allSites.Count; ++i)
            {
                var p = new Vector3(allSites[i].X, allSites[i].Y, 0) + lift;
                if (partial && i == lastIndex)
                    mb.AddInstance(DelaunayTriangulationDemo.Marker(p, PointSize * 1.8f), lastMaterial);
                else if (i < revealed)
                    mb.AddInstance(DelaunayTriangulationDemo.Marker(p, PointSize), siteMaterial);
                else
                    mb.AddInstance(DelaunayTriangulationDemo.Marker(p, PointSize * 0.6f), pendingMaterial);
            }
        }

        return mb.Build();
    }
}
