namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>
/// Uniform 1-to-4 subdivision with adjustable smoothing.
/// Smoothness 0 is pure midpoint (linear) subdivision: the shape is unchanged, only sampled finer.
/// Smoothness 1 is classic Loop subdivision: new and old vertices are moved by neighbor-weighted
/// averages, so the limit surface becomes smooth (C2 almost everywhere). Values in between blend
/// the two, which makes the effect of the smoothing rules easy to see interactively.
/// </summary>
[Category(Cat.Remesh)]
[Description("Uniform 1-to-4 subdivision with adjustable smoothing, from plain midpoint to Loop subdivision.")]
public class Subdivide : IModifier
{
    [Range(1, 5)] public int Levels = 1;
    [Range(0f, 1f)] public float Smoothness = 1f;

    /// <summary>Keep corner vertices (valence-2 boundary vertices) fixed instead of smoothing them.</summary>
    public bool PinCorners;

    public int InputTriangles { get; private set; }
    public int OutputTriangles { get; private set; }

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        InputTriangles = mesh.FaceIndices.Count;
        var result = mesh;
        for (var i = 0; i < Levels; i++)
            result = result.LoopSubdivide(Smoothness, PinCorners);
        OutputTriangles = result.FaceIndices.Count;
        return result;
    }
}

/// <summary>
/// Adaptive refinement: repeatedly splits only the edges longer than a threshold (a fraction of
/// the input's average edge length) at their midpoints. Unlike uniform subdivision, triangles
/// that are already small are left alone, so detail is added only where the mesh is coarse.
/// </summary>
[Category(Cat.Remesh)]
[Description("Adaptively refines only the edges longer than a threshold, adding detail where the mesh is coarse.")]
public class RefineAdaptive : IModifier
{
    /// <summary>Edges longer than this fraction of the average edge length are split.</summary>
    [Range(0.2f, 2f)] public float MaxEdgeFraction = 0.75f;

    public int InputTriangles { get; private set; }
    public int OutputTriangles { get; private set; }

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        InputTriangles = mesh.FaceIndices.Count;
        var result = mesh.SplitLongEdges(mesh.AverageEdgeLength() * MaxEdgeFraction);
        OutputTriangles = result.FaceIndices.Count;
        return result;
    }
}

/// <summary>
/// Coarsening by edge collapse: repeatedly collapses the shortest edge to its midpoint, checking
/// the link condition so the surface stays manifold. This is the core operation inside quadric
/// (QEM) decimators; here the priority is simply edge length, which keeps the idea visible.
/// </summary>
[Category(Cat.Remesh)]
[Description("Simplifies the mesh by repeatedly collapsing the shortest edge while keeping it manifold.")]
public class CoarsenEdgeCollapse : IModifier
{
    /// <summary>Edges shorter than this multiple of the average edge length are collapsed.</summary>
    [Range(0f, 4f)] public float MinEdgeFraction = 1.5f;

    public int InputTriangles { get; private set; }
    public int OutputTriangles { get; private set; }

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        InputTriangles = mesh.FaceIndices.Count;
        var welded = mesh.WeldVertices();
        var result = welded.CollapseShortEdges(welded.AverageEdgeLength() * MinEdgeFraction);
        OutputTriangles = result.FaceIndices.Count;
        return result;
    }
}

/// <summary>
/// Coarsening by vertex clustering: space is divided into a uniform grid, all vertices in a cell
/// are merged into their average, and degenerate faces are dropped. Very fast and works on any
/// triangle soup, but unlike edge collapse it does not preserve topology (parts can fuse).
/// </summary>
[Category(Cat.Remesh)]
[Description("Simplifies the mesh by merging all vertices within each cell of a uniform grid (fast, but can fuse parts).")]
public class CoarsenClustering : IModifier
{
    /// <summary>Number of grid cells across the longest axis of the bounding box.</summary>
    [Range(2, 128)] public int CellsAcross = 32;

    public int InputTriangles { get; private set; }
    public int OutputTriangles { get; private set; }

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        InputTriangles = mesh.FaceIndices.Count;
        var result = mesh.ClusterVertices(CellsAcross);
        OutputTriangles = result.FaceIndices.Count;
        return result;
    }
}

/// <summary>
/// Colors each vertex by the quality of its worst incident triangle, where quality is the ratio
/// of the triangle to an equilateral one (1 = equilateral, 0 = degenerate sliver). Apply after a
/// refinement or coarsening step to see where each technique produces good or bad elements.
/// </summary>
[Category(Cat.Analyze)]
[Description("Colors vertices by the quality of their worst incident triangle (1 = equilateral, 0 = sliver).")]
public class ColorByTriangleQuality : IModifier
{
    public float WorstQuality { get; private set; }
    public float AverageQuality { get; private set; }

    public ColoredTriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var quality = mesh.VertexQualities();
        var stats = quality.Statistics();
        WorstQuality = (float)stats.Min;
        AverageQuality = (float)stats.Average;
        var colors = quality.Select(q => QualityColor((float)q));
        return new ColoredTriangleMesh3D(mesh, colors);
    }

    // Red (bad) through yellow to green (good).
    static Vector3 QualityColor(float q)
        => q < 0.5f
            ? new Vector3(1, q * 2, 0)
            : new Vector3(2 - q * 2, 1, 0);
}

public static class RefineAndCoarsenHelpers
{
    public static double AverageEdgeLength(this TriangleMesh3D mesh)
    {
        var topo = mesh.DerivedTopology();
        var attrs = mesh.DerivedAttributes();
        var sum = 0.0;
        var count = 0;
        foreach (var edgeId in topo.GetUndirectedEdgeIds())
        {
            sum += attrs.GetEdgeLength(edgeId);
            count++;
        }
        return count > 0 ? sum / count : 0;
    }

    /// <summary>
    /// One level of 1-to-4 subdivision. Each edge gains a midpoint vertex and each triangle
    /// becomes four. When smoothness is above zero, new edge vertices use the Loop 3/8-1/8
    /// stencil and original vertices are relaxed toward their neighbor average with the
    /// valence-dependent Loop weight; boundaries use the cubic-spline boundary rules.
    /// When pinCorners is true, valence-2 boundary vertices (sharp corners) stay fixed.
    /// </summary>
    public static TriangleMesh3D LoopSubdivide(this TriangleMesh3D mesh, float smoothness, bool pinCorners = false)
    {
        var topo = mesh.DerivedTopology();
        var points = mesh.Points;
        var newPoints = new List<Point3D>(points.Count + topo.EdgeCount);

        for (var i = 0; i < points.Count; i++)
            newPoints.Add(SmoothEvenVertex(topo, points, (VertexId)i, smoothness, pinCorners));

        var edgeMidpoints = new Dictionary<(int, int), int>(topo.EdgeCount);
        foreach (var edgeId in topo.GetUndirectedEdgeIds())
        {
            var a = (int)topo.GetVertexA(edgeId);
            var b = (int)topo.GetVertexB(edgeId);
            edgeMidpoints[EdgeKey(a, b)] = newPoints.Count;
            newPoints.Add(SmoothOddVertex(topo, points, edgeId, smoothness));
        }

        var faces = new List<Integer3>(mesh.FaceIndices.Count * 4);
        foreach (var f in mesh.FaceIndices)
        {
            int a = f.A, b = f.B, c = f.C;
            var ab = edgeMidpoints[EdgeKey(a, b)];
            var bc = edgeMidpoints[EdgeKey(b, c)];
            var ca = edgeMidpoints[EdgeKey(c, a)];
            faces.Add(new Integer3(a, ab, ca));
            faces.Add(new Integer3(ab, b, bc));
            faces.Add(new Integer3(ca, bc, c));
            faces.Add(new Integer3(ab, bc, ca));
        }

        return new TriangleMesh3D(newPoints, faces);
    }

    static (int, int) EdgeKey(int a, int b)
        => a < b ? (a, b) : (b, a);

    static Point3D SmoothEvenVertex(Topology topo, IReadOnlyList<Point3D> points, VertexId id, float smoothness, bool pinCorners)
    {
        var original = points[(int)id].Vector3;
        if (smoothness <= 0)
            return original;
        if (pinCorners && topo.IsBoundary(id) && IsCorner(topo, points, id, original))
            return original;
        var smoothed = topo.IsBoundary(id)
            ? BoundaryEvenPosition(topo, points, id, original)
            : InteriorEvenPosition(topo, points, id, original);
        return original.Lerp(smoothed, smoothness);
    }

    static Vector3 InteriorEvenPosition(Topology topo, IReadOnlyList<Point3D> points, VertexId id, Vector3 original)
    {
        var sum = Vector3.Zero;
        var n = 0;
        foreach (var neighbor in topo.GetNeighborVertexIds(id))
        {
            sum += points[(int)neighbor].Vector3;
            n++;
        }
        if (n < 3)
            return original;
        var beta = n == 3 ? 3f / 16 : 3f / (8 * n);
        return original * (1 - n * beta) + sum * beta;
    }

    // A boundary vertex is a corner when its two boundary edges meet at a sharp angle
    // (more than ~30 degrees away from a straight line).
    static bool IsCorner(Topology topo, IReadOnlyList<Point3D> points, VertexId id, Vector3 position)
    {
        var dirs = new List<Vector3>(2);
        foreach (var edgeId in topo.GetIncidentUndirectedEdgeIds(id))
        {
            if (!topo.IsBoundary(edgeId))
                continue;
            var other = topo.GetVertexA(edgeId) == id ? topo.GetVertexB(edgeId) : topo.GetVertexA(edgeId);
            dirs.Add((points[(int)other].Vector3 - position).Normalize);
        }
        if (dirs.Count != 2)
            return true;
        return Vector3.Dot(dirs[0], dirs[1]) > -0.866f;
    }

    static Vector3 BoundaryEvenPosition(Topology topo, IReadOnlyList<Point3D> points, VertexId id, Vector3 original)
    {
        var sum = Vector3.Zero;
        var n = 0;
        foreach (var edgeId in topo.GetIncidentUndirectedEdgeIds(id))
        {
            if (!topo.IsBoundary(edgeId))
                continue;
            var other = topo.GetVertexA(edgeId) == id ? topo.GetVertexB(edgeId) : topo.GetVertexA(edgeId);
            sum += points[(int)other].Vector3;
            n++;
        }
        return n == 2 ? original * 0.75f + sum * 0.125f : original;
    }

    static Point3D SmoothOddVertex(Topology topo, IReadOnlyList<Point3D> points, UndirectedEdgeId edgeId, float smoothness)
    {
        var a = points[(int)topo.GetVertexA(edgeId)].Vector3;
        var b = points[(int)topo.GetVertexB(edgeId)].Vector3;
        var midpoint = (a + b) * 0.5f;

        var halfEdges = topo.GetHalfEdgeIds(edgeId);
        if (smoothness <= 0 || halfEdges.Count != 2)
            return midpoint;

        var c = points[(int)topo.GetEndVertex(topo.GetNext(halfEdges[0]))].Vector3;
        var d = points[(int)topo.GetEndVertex(topo.GetNext(halfEdges[1]))].Vector3;
        var smoothed = (a + b) * 0.375f + (c + d) * 0.125f;
        return midpoint.Lerp(smoothed, smoothness);
    }

    /// <summary>
    /// Merges all vertices falling in the same grid cell into their average position,
    /// remaps faces, and drops the faces that became degenerate or duplicated.
    /// </summary>
    public static TriangleMesh3D ClusterVertices(this TriangleMesh3D mesh, int cellsAcross)
    {
        var points = mesh.Points;
        if (points.Count == 0)
            return mesh;

        var bounds = points.Bounds();
        var cellSize = bounds.DiagonalLength() / Math.Max(2, cellsAcross);
        if (cellSize <= 0)
            return mesh;

        var min = bounds.Min.Vector3;
        var cells = new Dictionary<(int, int, int), int>();
        var sums = new List<Vector3>();
        var counts = new List<int>();
        var remap = new int[points.Count];

        for (var i = 0; i < points.Count; i++)
        {
            var p = points[i].Vector3;
            var key = (
                (int)Math.Floor((p.X - min.X) / cellSize),
                (int)Math.Floor((p.Y - min.Y) / cellSize),
                (int)Math.Floor((p.Z - min.Z) / cellSize));
            if (!cells.TryGetValue(key, out var cluster))
            {
                cluster = sums.Count;
                cells[key] = cluster;
                sums.Add(Vector3.Zero);
                counts.Add(0);
            }
            sums[cluster] += p;
            counts[cluster]++;
            remap[i] = cluster;
        }

        var newPoints = new Point3D[sums.Count];
        for (var i = 0; i < sums.Count; i++)
            newPoints[i] = sums[i] / counts[i];

        var faces = new List<Integer3>(mesh.FaceIndices.Count);
        var seen = new HashSet<(int, int, int)>();
        foreach (var f in mesh.FaceIndices)
        {
            var a = remap[f.A];
            var b = remap[f.B];
            var c = remap[f.C];
            if (a == b || b == c || c == a)
                continue;
            if (seen.Add(FaceKey(a, b, c)))
                faces.Add(new Integer3(a, b, c));
        }

        return new TriangleMesh3D(newPoints, faces).RemoveUnreferencedPoints();
    }

    static (int, int, int) FaceKey(int a, int b, int c)
    {
        if (a > b) (a, b) = (b, a);
        if (b > c) (b, c) = (c, b);
        if (a > b) (a, b) = (b, a);
        return (a, b, c);
    }

    /// <summary>
    /// Per-vertex quality: the minimum shape quality of the triangles touching each vertex.
    /// Triangle quality is 4*sqrt(3)*area / (sum of squared edge lengths); 1 for equilateral.
    /// </summary>
    public static IReadOnlyList<double> VertexQualities(this TriangleMesh3D mesh)
    {
        var quality = new double[mesh.Points.Count];
        Array.Fill(quality, 1.0);

        foreach (var f in mesh.FaceIndices)
        {
            var q = TriangleQuality(mesh.Points[f.A].Vector3, mesh.Points[f.B].Vector3, mesh.Points[f.C].Vector3);
            quality[f.A] = Math.Min(quality[f.A], q);
            quality[f.B] = Math.Min(quality[f.B], q);
            quality[f.C] = Math.Min(quality[f.C], q);
        }

        return quality;
    }

    static double TriangleQuality(Vector3 a, Vector3 b, Vector3 c)
    {
        var ab = b - a;
        var bc = c - b;
        var ca = a - c;
        var edgeSum = ab.LengthSquared + bc.LengthSquared + ca.LengthSquared;
        if (edgeSum <= 0)
            return 0;
        var area = Vector3.Cross(ab, c - a).Length * 0.5f;
        return 4 * Math.Sqrt(3) * area / edgeSum;
    }
}
