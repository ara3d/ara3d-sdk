namespace Ara3D.Studio.Samples.Demos;

/// <summary>
/// Incrementally assembles a BrepSolid, sharing straight edges between the faces
/// that reference the same pair of vertices.
/// </summary>
public sealed class BrepBuilder
{
    private readonly List<Point3D> _vertices = new();
    private readonly List<BrepEdge> _edges = new();
    private readonly List<BrepFace> _faces = new();
    private readonly Dictionary<(int, int), int> _lineEdges = new();

    public BrepSolid ToSolid()
        => new(_vertices.ToList(), _edges.ToList(), _faces.ToList());

    public int AddVertex(Point3D p)
    {
        _vertices.Add(p);
        return _vertices.Count - 1;
    }

    public int AddEdge(int v0, int v1, Curve3D curve)
    {
        _edges.Add(new(v0, v1, curve));
        return _edges.Count - 1;
    }

    public BrepEdgeUse LineEdge(int a, int b)
    {
        var key = a < b ? (a, b) : (b, a);
        if (_lineEdges.TryGetValue(key, out var index))
            return new(index, _edges[index].V0 == a ? EdgeOrientation.Forward : EdgeOrientation.Reversed);
        var (pa, pb) = (_vertices[a], _vertices[b]);
        index = AddEdge(a, b, new Curve3D(t => pa.Lerp(pb, t)));
        _lineEdges.Add(key, index);
        return new(index, EdgeOrientation.Forward);
    }

    /// <summary>Loop of shared straight edges through a cycle of vertices.</summary>
    public BrepLoop PolygonLoop(params int[] cycle)
    {
        var uses = new BrepEdgeUse[cycle.Length];
        for (var i = 0; i < cycle.Length; i++)
            uses[i] = LineEdge(cycle[i], cycle[(i + 1) % cycle.Length]);
        return new(uses);
    }

    public int AddFace(ParametricSurface surface, params BrepLoop[] loops)
        => AddFace(surface, false, loops);

    public int AddFace(ParametricSurface surface, bool isBilinear, params BrepLoop[] loops)
    {
        _faces.Add(new(surface, loops, isBilinear));
        return _faces.Count - 1;
    }

    /// <summary>
    /// Bilinear face over four vertices in counter-clockwise order viewed
    /// from outside the solid.
    /// </summary>
    public int AddQuadFace(int a, int b, int c, int d)
    {
        var (pa, pb, pc, pd) = (_vertices[a], _vertices[b], _vertices[c], _vertices[d]);
        var surface = new ParametricSurface(
            uv => pa.Lerp(pb, uv.X).Lerp(pd.Lerp(pc, uv.X), uv.Y).Vector3, false, false);
        return AddFace(surface, true, PolygonLoop(a, b, c, d));
    }
}

public static class BrepSolids
{
    public static Point3D Lerp(this Point3D a, Point3D b, Number t)
        => (a.X.Lerp(b.X, t), a.Y.Lerp(b.Y, t), a.Z.Lerp(b.Z, t));

    public static Point3D CirclePoint(Angle a, Number radius, Number z)
        => (radius * a.Cos, radius * a.Sin, z);

    public static Point3D CirclePoint(Number t, Number radius, Number z)
        => CirclePoint(t.Turns, radius, z);

    /// <summary>Axis-aligned box: base on the XY plane, centered in X and Y. V=8, E=12, F=6.</summary>
    public static BrepSolid Box(Number width, Number depth, Number height)
    {
        var bld = new BrepBuilder();
        var (x, y) = (width / 2, depth / 2);
        // Vertex i has bit 0 = +x, bit 1 = +y, bit 2 = +z.
        var v = new int[8];
        for (var i = 0; i < 8; i++)
            v[i] = bld.AddVertex((
                (i & 1) != 0 ? x : -x,
                (i & 2) != 0 ? y : -y,
                (i & 4) != 0 ? height : 0));
        bld.AddQuadFace(v[0], v[2], v[3], v[1]);
        bld.AddQuadFace(v[4], v[5], v[7], v[6]);
        bld.AddQuadFace(v[1], v[3], v[7], v[5]);
        bld.AddQuadFace(v[0], v[4], v[6], v[2]);
        bld.AddQuadFace(v[2], v[6], v[7], v[3]);
        bld.AddQuadFace(v[0], v[1], v[5], v[4]);
        return bld.ToSolid();
    }

    /// <summary>
    /// Cylinder with a seam edge: the side face's loop traverses the seam twice
    /// (once in each direction), and each circle is a closed edge. V=2, E=3, F=3.
    /// </summary>
    public static BrepSolid Cylinder(Number radius, Number height)
    {
        var bld = new BrepBuilder();
        var v0 = bld.AddVertex((radius, 0, 0));
        var v1 = bld.AddVertex((radius, 0, height));
        var bottom = bld.AddEdge(v0, v0, new Curve3D(t => CirclePoint(t, radius, 0)));
        var top = bld.AddEdge(v1, v1, new Curve3D(t => CirclePoint(t, radius, height)));
        var seam = bld.AddEdge(v0, v1, new Curve3D(t => (radius, 0, height * t)));
        const EdgeOrientation fwd = EdgeOrientation.Forward;
        const EdgeOrientation rev = EdgeOrientation.Reversed;
        bld.AddFace(
            new ParametricSurface(uv => CirclePoint(uv.X, radius, height * uv.Y).Vector3, true, false),
            new BrepLoop([new(bottom, fwd), new(seam, fwd), new(top, rev), new(seam, rev)]));
        bld.AddFace(
            new ParametricSurface(uv => CirclePoint(uv.X, radius * (1 - uv.Y), height).Vector3, true, false),
            new BrepLoop([new(top, fwd)]));
        bld.AddFace(
            new ParametricSurface(uv => CirclePoint(uv.X, radius * uv.Y, 0).Vector3, true, false),
            new BrepLoop([new(bottom, rev)]));
        return bld.ToSolid();
    }

    /// <summary>Extruded regular polygon. V=2n, E=3n, F=n+2.</summary>
    public static BrepSolid Prism(int sides, Number radius, Number height)
    {
        var bld = new BrepBuilder();
        var bottom = new int[sides];
        var topVerts = new int[sides];
        for (var i = 0; i < sides; i++)
        {
            var p = CirclePoint(i / (Number)sides, radius, 0);
            bottom[i] = bld.AddVertex(p);
            topVerts[i] = bld.AddVertex((p.X, p.Y, height));
        }

        for (var i = 0; i < sides; i++)
        {
            var j = (i + 1) % sides;
            bld.AddQuadFace(bottom[i], bottom[j], topVerts[j], topVerts[i]);
        }

        // Cap surfaces sweep from the polygon rim to the centroid; the rim curve is a
        // piecewise-linear interpolation over the perimeter, so it matches the shared edges.
        Point3D RimPoint(Number u, Number z)
        {
            var scaled = u * sides;
            var k = Math.Min((int)Math.Floor((float)scaled), sides - 1);
            var p0 = CirclePoint(k / (Number)sides, radius, z);
            var p1 = CirclePoint((k + 1) / (Number)sides, radius, z);
            return p0.Lerp(p1, scaled - k);
        }

        var bottomReversed = new int[sides];
        for (var i = 0; i < sides; i++)
            bottomReversed[i] = bottom[sides - 1 - i];

        var topCenter = new Point3D(0, 0, height);
        var bottomCenter = new Point3D(0, 0, 0);
        bld.AddFace(
            new ParametricSurface(uv => RimPoint(uv.X, height).Lerp(topCenter, uv.Y).Vector3, true, false),
            bld.PolygonLoop(topVerts));
        bld.AddFace(
            new ParametricSurface(uv => bottomCenter.Lerp(RimPoint(uv.X, 0), uv.Y).Vector3, true, false),
            bld.PolygonLoop(bottomReversed));
        return bld.ToSolid();
    }
}
