namespace Ara3D.Studio.Samples.Demos;

/// <summary>
/// A minimal functional BREP (boundary representation) sketch, as it might look if added
/// to Ara3D.Geometry (and eventually Plato). A solid is vertices + edges + faces:
/// vertices are points, edges are parametric curves between vertices, and faces are
/// parametric surfaces whose boundaries are loops of oriented edge-uses.
/// Everything is immutable; deformation composes functions rather than resampling points,
/// so a deformed solid remains an exact (resolution-independent) BREP.
/// Trimming (loops restricting the surface domain) is intentionally out of scope.
/// </summary>
public sealed record BrepSolid(
    IReadOnlyList<Point3D> Vertices,
    IReadOnlyList<BrepEdge> Edges,
    IReadOnlyList<BrepFace> Faces);

public sealed record BrepEdge(int V0, int V1, Curve3D Curve);

public enum EdgeOrientation
{
    Forward,
    Reversed,
}

public readonly record struct BrepEdgeUse(int Edge, EdgeOrientation Orientation)
{
    public bool Forward => Orientation == EdgeOrientation.Forward;
}

public sealed record BrepLoop(IReadOnlyList<BrepEdgeUse> Uses);

/// <summary>
/// Orientation convention: the surface normal is du x dv and must point out of the solid;
/// loops traverse the boundary counter-clockwise when viewed from outside.
/// IsBilinear marks faces whose surface is (at most) bilinear, so a single quad
/// tessellates them exactly.
/// </summary>
public sealed record BrepFace(
    ParametricSurface Surface,
    IReadOnlyList<BrepLoop> Loops,
    bool IsBilinear = false);

public static class BrepExtensions
{
    public static BrepEdge Deform(this BrepEdge edge, Func<Point3D, Point3D> f)
        => edge with { Curve = edge.Curve.Deform(f) };

    public static BrepFace Deform(this BrepFace face, Func<Point3D, Point3D> f)
        => face with { Surface = face.Surface.Deform(f) };

    public static BrepSolid Deform(this BrepSolid solid, Func<Point3D, Point3D> f)
        => new(solid.Vertices.Map(f),
            solid.Edges.Map(e => e.Deform(f)),
            solid.Faces.Map(face => face.Deform(f)));

    public static BrepSolid Translate(this BrepSolid solid, Vector3 v)
        => solid.Deform(p => (p.X + v.X, p.Y + v.Y, p.Z + v.Z));

    public static BrepSolid Concat(this BrepSolid a, BrepSolid b)
    {
        var vertexOffset = a.Vertices.Count;
        var edgeOffset = a.Edges.Count;
        return new(
            a.Vertices.Concat(b.Vertices).ToList(),
            a.Edges.Concat(b.Edges.Select(e =>
                e with { V0 = e.V0 + vertexOffset, V1 = e.V1 + vertexOffset })).ToList(),
            a.Faces.Concat(b.Faces.Select(f =>
                f with { Loops = f.Loops.Map(l =>
                    new BrepLoop(l.Uses.Map(u => u with { Edge = u.Edge + edgeOffset }))) })).ToList());
    }

    public static int EulerCharacteristic(this BrepSolid solid)
        => solid.Vertices.Count - solid.Edges.Count + solid.Faces.Count;

    public static IEnumerable<BrepEdgeUse> AllEdgeUses(this BrepSolid solid)
        => solid.Faces.SelectMany(f => f.Loops).SelectMany(l => l.Uses);

    /// <summary>
    /// A solid is watertight when every edge is used exactly twice, once in each direction.
    /// </summary>
    public static bool IsWatertight(this BrepSolid solid)
    {
        var forward = new int[solid.Edges.Count];
        var backward = new int[solid.Edges.Count];
        foreach (var use in solid.AllEdgeUses())
        {
            if (use.Forward) forward[use.Edge]++;
            else backward[use.Edge]++;
        }
        for (var i = 0; i < solid.Edges.Count; i++)
            if (forward[i] != 1 || backward[i] != 1)
                return false;
        return true;
    }

    public static int EdgeUseCount(this BrepFace face, int edge)
        => face.Loops.SelectMany(l => l.Uses).Count(u => u.Edge == edge);

    public static IReadOnlyList<int> FacesOfEdge(this BrepSolid solid, int edge)
        => Enumerable.Range(0, solid.Faces.Count)
            .Where(f => solid.Faces[f].EdgeUseCount(edge) > 0)
            .ToList();

    public static TriangleMesh3D Tessellate(this BrepFace face, int resolution)
        => face.Surface.Triangulate(face.IsBilinear ? 2 : resolution + 1);

    public static IReadOnlyList<TriangleMesh3D> TessellateFaces(this BrepSolid solid, int resolution)
        => solid.Faces.Map(f => f.Tessellate(resolution));

    /// <summary>Returns numSegments + 1 points along the edge curve, endpoints included.</summary>
    public static IReadOnlyList<Point3D> SampleEdge(this BrepSolid solid, int edge, int numSegments)
        => solid.Edges[edge].Curve.Sample(numSegments + 1);

    public static Point3D Average(this IReadOnlyList<Point3D> points)
    {
        Number x = 0, y = 0, z = 0;
        for (var i = 0; i < points.Count; i++)
        {
            x += points[i].X;
            y += points[i].Y;
            z += points[i].Z;
        }
        return (x / points.Count, y / points.Count, z / points.Count);
    }

    public static Point3D Centroid(this BrepFace face, int resolution = 4)
        => face.Tessellate(resolution).Points.Average();

    public static Point3D Centroid(this BrepSolid solid)
        => solid.Faces.Map(f => f.Centroid()).Average();

    public static float DistanceTo(this Point3D a, Point3D b)
        => (a.Vector3 - b.Vector3).Length;

    public static int StartVertex(this BrepSolid solid, BrepEdgeUse use)
        => use.Forward ? solid.Edges[use.Edge].V0 : solid.Edges[use.Edge].V1;

    public static int EndVertex(this BrepSolid solid, BrepEdgeUse use)
        => use.Forward ? solid.Edges[use.Edge].V1 : solid.Edges[use.Edge].V0;

    /// <summary>
    /// Checks the structural invariants: edge curves must start and end at their vertices,
    /// loops must be connected cycles, the solid must be watertight, and sampled points of
    /// each boundary edge must lie on the face surface (within a tolerance relative to size).
    /// Returns an empty list when the solid is valid.
    /// </summary>
    public static IReadOnlyList<string> Validate(this BrepSolid solid, float relativeTolerance = 1e-3f)
    {
        var errors = new List<string>();

        for (var e = 0; e < solid.Edges.Count; e++)
        {
            var edge = solid.Edges[e];
            if (edge.V0 < 0 || edge.V0 >= solid.Vertices.Count ||
                edge.V1 < 0 || edge.V1 >= solid.Vertices.Count)
            {
                errors.Add($"Edge {e} references a vertex out of range");
                continue;
            }
            if (edge.Curve.Eval(0).DistanceTo(solid.Vertices[edge.V0]) > AbsoluteTolerance(solid, relativeTolerance) ||
                edge.Curve.Eval(1).DistanceTo(solid.Vertices[edge.V1]) > AbsoluteTolerance(solid, relativeTolerance))
                errors.Add($"Edge {e} curve does not start/end at its vertices");
        }

        for (var f = 0; f < solid.Faces.Count; f++)
        foreach (var loop in solid.Faces[f].Loops)
        {
            for (var i = 0; i < loop.Uses.Count; i++)
            {
                var use = loop.Uses[i];
                if (use.Edge < 0 || use.Edge >= solid.Edges.Count)
                {
                    errors.Add($"Face {f} loop references edge {use.Edge} out of range");
                    continue;
                }
                var next = loop.Uses[(i + 1) % loop.Uses.Count];
                if (solid.EndVertex(use) != solid.StartVertex(next))
                    errors.Add($"Face {f} loop breaks between edge {use.Edge} and edge {next.Edge}");
            }
            if (!BoundaryOnSurface(solid, f, loop, relativeTolerance))
                errors.Add($"Face {f} has a boundary edge that leaves its surface");
        }

        if (!solid.IsWatertight())
            errors.Add("Solid is not watertight: some edge is not used exactly once in each direction");

        return errors;
    }

    private static float AbsoluteTolerance(BrepSolid solid, float relativeTolerance)
    {
        var center = solid.Vertices.Count > 0 ? solid.Vertices.Average() : default;
        var radius = 1f;
        for (var i = 0; i < solid.Vertices.Count; i++)
            radius = Math.Max(radius, (float)center.DistanceTo(solid.Vertices[i]));
        return radius * relativeTolerance;
    }

    private static bool BoundaryOnSurface(BrepSolid solid, int face, BrepLoop loop, float relativeTolerance)
    {
        const int surfaceSamples = 24;
        const int curveSamples = 8;
        var surface = solid.Faces[face].Surface;
        var grid = surface.ToQuadGrid(surfaceSamples).Points;
        // The grid only approximates the surface, so allow a slack of ~1.5 grid cells.
        var tolerance = AbsoluteTolerance(solid, relativeTolerance)
            + 1.5f * BoundsDiagonal(grid) / surfaceSamples;
        foreach (var use in loop.Uses)
        foreach (var p in solid.Edges[use.Edge].Curve.Sample(curveSamples))
        {
            var minDistance = float.MaxValue;
            for (var i = 0; i < grid.Count; i++)
                minDistance = Math.Min(minDistance, (float)p.DistanceTo(grid[i]));
            if (minDistance > tolerance)
                return false;
        }
        return true;
    }

    private static float BoundsDiagonal(IReadOnlyList<Point3D> points)
    {
        var bounds = points.Bounds();
        return (bounds.Max.Vector3 - bounds.Min.Vector3).Length;
    }
}
