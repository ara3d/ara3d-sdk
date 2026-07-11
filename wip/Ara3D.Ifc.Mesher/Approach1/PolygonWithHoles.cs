using Ara3D.Geometry;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>2D profile boundary with optional holes; outer CCW, holes CW after normalization.</summary>
public sealed class PolygonWithHoles
{
    public PolygonWithHoles(IReadOnlyList<Vector2> outer, IReadOnlyList<IReadOnlyList<Vector2>>? holes = null)
    {
        Outer = NormalizeOuter(outer);
        Holes = (holes ?? []).Select(NormalizeHole).Where(h => h.Count >= 3).ToList();
    }

    public IReadOnlyList<Vector2> Outer { get; }
    public IReadOnlyList<IReadOnlyList<Vector2>> Holes { get; }

    public Bounds2D Bounds => Outer.GetBounds();
    public double SignedArea => new SimplePolygon2D(Outer).SignedArea()
        + Holes.Sum(h => new SimplePolygon2D(h).SignedArea());
    public double Area => Math.Abs(SignedArea);

    public IReadOnlyList<Triangle2D> Triangulate()
    {
        if (Holes.Count == 1 && TryTriangulateOffsetRing(Outer, Holes[0], out var ringTris))
            return ringTris;
        if (Holes.Count == 0 && TryTriangulateConvexFan(Outer, out var fanTris))
            return fanTris;
        try
        {
            return PolygonTriangulator.GetTriangles(Outer, Holes);
        }
        catch (InvalidOperationException)
        {
            // Ear-clip uses an absolute epsilon (PolygonTriangulator.Eps = 1e-6): on sub-meter profiles
            // (accessory rings, finely sampled arcs / fillets) the adjacent-edge cross products and
            // triangle areas fall below that band, so near-collinear vertices are misclassified and the
            // clip stalls. Retry in a normalized ~100-unit box so the epsilon acts relatively, then snap
            // each result vertex back to the exact profile vertex it came from (ear-clip introduces no
            // Steiner points) so Quantize keys and extrusion wall seams stay aligned.
            return TriangulateNormalized(Outer, Holes);
        }
    }

    static IReadOnlyList<Triangle2D> TriangulateNormalized(
        IReadOnlyList<Vector2> outer,
        IReadOnlyList<IReadOnlyList<Vector2>> holes)
    {
        var bounds = outer.GetBounds();
        var size = bounds.Size;
        var extent = MathF.Max(MathF.Abs((float)size.X.Value), MathF.Abs((float)size.Y.Value));
        if (!(extent > PolygonTriangulator.Eps))
            return PolygonTriangulator.GetTriangles(outer, holes);

        const float target = 100f;
        var scale = target / extent;
        var ox = (float)bounds.Min.X.Value;
        var oy = (float)bounds.Min.Y.Value;

        Vector2 Fwd(Vector2 p) => new(((float)p.X.Value - ox) * scale, ((float)p.Y.Value - oy) * scale);

        var registry = new List<(Vector2 Norm, Vector2 Orig)>();
        void Register(Vector2 p) => registry.Add((Fwd(p), p));
        foreach (var p in outer)
            Register(p);
        foreach (var hole in holes)
            foreach (var p in hole)
                Register(p);

        Vector2 Snap(Vector2 normPt)
        {
            var best = normPt;
            var bestD = float.PositiveInfinity;
            foreach (var (norm, orig) in registry)
            {
                var dx = (float)norm.X.Value - (float)normPt.X.Value;
                var dy = (float)norm.Y.Value - (float)normPt.Y.Value;
                var d = dx * dx + dy * dy;
                if (d < bestD)
                {
                    bestD = d;
                    best = orig;
                }
            }
            // Ear-clip reuses input vertices; a genuine miss (unexpected Steiner point) inverts the map.
            return bestD <= 1e-2f
                ? best
                : new Vector2((float)normPt.X.Value / scale + ox, (float)normPt.Y.Value / scale + oy);
        }

        var normOuter = outer.Select(Fwd).ToList();
        var normHoles = holes
            .Select(h => (IReadOnlyList<Vector2>)h.Select(Fwd).ToList())
            .ToList();

        // Redundant collinear vertices (polyline segments sampled with extra points) have a
        // turn-angle of ~0 at every scale, so ear-clip never treats them as convex ears and stalls.
        // Drop them from the cap ring; the extrusion walls still visit every original vertex, and the
        // dropped points lie exactly on a cap-triangle edge, so the solid stays closed.
        if (normHoles.Count == 0)
            normOuter = RemoveNearCollinear(normOuter);

        IReadOnlyList<Triangle2D> normTris;
        try
        {
            normTris = PolygonTriangulator.GetTriangles(normOuter, normHoles);
        }
        catch (InvalidOperationException) when (normHoles.Count == 0
            && RobustEarClip(normOuter) is { } recovered)
        {
            // Composite-curve rings (arc + trimmed + polyline) can leave a small reflex pocket where
            // the shared ear-clip's fixed epsilon rejects every candidate and stalls. A recovery clip
            // (clip the most-convex empty ear, breaking ties by area) always terminates on a simple
            // ring. Only reached after the shared path already failed, so no effect on passing files.
            normTris = recovered;
        }

        return normTris
            .Select(t => new Triangle2D(Snap(t.A.Vector2), Snap(t.B.Vector2), Snap(t.C.Vector2)))
            .ToList();
    }

    /// <summary>
    /// Self-contained ear-clip for a simple ring that the shared triangulator stalled on. Prefers
    /// strictly-convex empty ears; if none is found (float-noise pocket) it clips the most-convex
    /// vertex to guarantee forward progress. Returns null if the ring is genuinely degenerate.
    /// </summary>
    static IReadOnlyList<Triangle2D>? RobustEarClip(IReadOnlyList<Vector2> ring)
    {
        var n = ring.Count;
        if (n < 3)
            return null;

        var idx = Enumerable.Range(0, n).ToList();
        var area = 0.0;
        for (var i = 0; i < n; i++)
        {
            var a = ring[i];
            var b = ring[(i + 1) % n];
            area += (double)a.X.Value * b.Y.Value - (double)b.X.Value * a.Y.Value;
        }
        if (area < 0)
            idx.Reverse();

        bool StrictlyInside(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
        {
            const float eps = 1e-4f;
            var d1 = PolygonTriangulator.Cross(a, b, p);
            var d2 = PolygonTriangulator.Cross(b, c, p);
            var d3 = PolygonTriangulator.Cross(c, a, p);
            return d1 > eps && d2 > eps && d3 > eps;
        }

        var tris = new List<Triangle2D>(n - 2);
        var guard = 0;
        while (idx.Count > 3 && guard++ < n * n * 4)
        {
            var m = idx.Count;
            var clipped = false;
            var bestCross = float.NegativeInfinity;
            var bestVertex = -1;
            for (var i = 0; i < m; i++)
            {
                var a = ring[idx[(i - 1 + m) % m]];
                var b = ring[idx[i]];
                var c = ring[idx[(i + 1) % m]];
                var cross = PolygonTriangulator.Cross(a, b, c);
                if (cross > bestCross)
                {
                    bestCross = cross;
                    bestVertex = i;
                }
                if (cross <= 0)
                    continue;

                var empty = true;
                for (var j = 0; j < m; j++)
                {
                    if (j == i || j == (i - 1 + m) % m || j == (i + 1) % m)
                        continue;
                    if (StrictlyInside(a, b, c, ring[idx[j]]))
                    {
                        empty = false;
                        break;
                    }
                }
                if (!empty)
                    continue;

                tris.Add(new Triangle2D(a, b, c));
                idx.RemoveAt(i);
                clipped = true;
                break;
            }

            if (clipped)
                continue;
            if (bestVertex < 0 || bestCross <= 0)
                return null;

            var pa = ring[idx[(bestVertex - 1 + m) % m]];
            var pb = ring[idx[bestVertex]];
            var pc = ring[idx[(bestVertex + 1) % m]];
            tris.Add(new Triangle2D(pa, pb, pc));
            idx.RemoveAt(bestVertex);
        }

        if (idx.Count != 3)
            return null;
        tris.Add(new Triangle2D(ring[idx[0]], ring[idx[1]], ring[idx[2]]));
        return tris;
    }

    /// <summary>Removes vertices whose turn-angle is negligible (scale-invariant sine test).</summary>
    static List<Vector2> RemoveNearCollinear(IReadOnlyList<Vector2> ring)
    {
        const float sinTol = 1e-3f;
        var pts = ring.ToList();
        var changed = true;
        while (changed && pts.Count > 3)
        {
            changed = false;
            for (var i = 0; i < pts.Count && pts.Count > 3; i++)
            {
                var n = pts.Count;
                var a = pts[(i - 1 + n) % n];
                var b = pts[i];
                var c = pts[(i + 1) % n];
                var e1 = MathF.Sqrt((float)a.DistanceSquared(b));
                var e2 = MathF.Sqrt((float)b.DistanceSquared(c));
                if (e1 <= PolygonTriangulator.Eps || e2 <= PolygonTriangulator.Eps)
                {
                    pts.RemoveAt(i);
                    changed = true;
                    i--;
                    continue;
                }
                var sin = MathF.Abs(PolygonTriangulator.Cross(a, b, c)) / (e1 * e2);
                if (sin < sinTol)
                {
                    pts.RemoveAt(i);
                    changed = true;
                    i--;
                }
            }
        }
        return pts;
    }

    /// <summary>Fan triangulation for convex rings; ear-clip uses absolute eps and fails on small circles.</summary>
    static bool TryTriangulateConvexFan(IReadOnlyList<Vector2> ring, out IReadOnlyList<Triangle2D> triangles)
    {
        triangles = [];
        var n = ring.Count;
        if (n < 3)
            return false;
        if (PolygonTriangulator.HasSelfIntersection(ring))
            return false;

        var bounds = ring.GetBounds();
        var size = bounds.Size;
        var scale = Math.Max(MathF.Abs((float)size.X.Value), MathF.Abs((float)size.Y.Value));
        var crossEps = Math.Max(PolygonTriangulator.Eps * PolygonTriangulator.Eps, scale * scale * 1e-10f);

        var sign = 0;
        for (var i = 0; i < n; i++)
        {
            var cross = PolygonTriangulator.Cross(ring[(i - 1 + n) % n], ring[i], ring[(i + 1) % n]);
            if (MathF.Abs(cross) <= crossEps)
                return false;
            var vertexSign = cross > 0 ? 1 : -1;
            sign = sign == 0 ? vertexSign : sign;
            if (sign != vertexSign)
                return false;
        }

        var tris = new List<Triangle2D>(n - 2);
        for (var i = 1; i < n - 1; i++)
            tris.Add(new Triangle2D(ring[0], ring[i], ring[i + 1]));
        triangles = tris;
        return true;
    }

    static bool TryTriangulateOffsetRing(
        IReadOnlyList<Vector2> outer,
        IReadOnlyList<Vector2> inner,
        out IReadOnlyList<Triangle2D> triangles)
    {
        triangles = [];
        if (outer.Count < 3 || inner.Count < 3)
            return false;
        if (PolygonTriangulator.HasSelfIntersection(outer) || PolygonTriangulator.HasSelfIntersection(inner))
            return false;

        var outerList = outer.ToList();
        foreach (var p in inner)
        {
            if (!PolygonTriangulator.PointInPolygon(outerList, p))
                return false;
        }

        var innerRing = inner.Count == outer.Count ? inner : ResampleClosedRing(inner, outer.Count);
        return TryTriangulateCongruentRing(outer, innerRing, out triangles);
    }

    /// <summary>Resamples a closed ring to <paramref name="targetCount"/> vertices (arc-length).</summary>
    public static List<Vector2> ResampleClosedRing(IReadOnlyList<Vector2> ring, int targetCount)
    {
        if (ring.Count == targetCount)
            return ring.ToList();
        if (ring.Count < 3 || targetCount < 3)
            return ring.ToList();

        var segLen = new float[ring.Count];
        var total = 0f;
        for (var i = 0; i < ring.Count; i++)
        {
            var d = ring[i].Distance(ring[(i + 1) % ring.Count]);
            segLen[i] = d;
            total += d;
        }
        if (total <= PolygonTriangulator.Eps)
            return ring.ToList();

        var result = new List<Vector2>(targetCount);
        var step = total / targetCount;
        var seg = 0;
        var segStart = 0f;
        for (var k = 0; k < targetCount; k++)
        {
            var target = k * step;
            while (seg < ring.Count - 1 && segStart + segLen[seg] < target - PolygonTriangulator.Eps)
            {
                segStart += segLen[seg];
                seg++;
            }

            var a = ring[seg];
            var b = ring[(seg + 1) % ring.Count];
            var len = segLen[seg];
            var t = len <= PolygonTriangulator.Eps ? 0f : (target - segStart) / len;
            result.Add(new Vector2(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t));
        }
        return result;
    }

    internal static bool TryTriangulateCongruentRing(
        IReadOnlyList<Vector2> outer,
        IReadOnlyList<Vector2> inner,
        out IReadOnlyList<Triangle2D> triangles)
    {
        triangles = [];
        if (outer.Count != inner.Count || outer.Count < 3)
            return false;

        var tris = new List<Triangle2D>(outer.Count * 2);
        for (var i = 0; i < outer.Count; i++)
        {
            var j = (i + 1) % outer.Count;
            tris.Add(new Triangle2D(outer[i], outer[j], inner[j]));
            tris.Add(new Triangle2D(outer[i], inner[j], inner[i]));
        }
        triangles = tris;
        return true;
    }

    public IReadOnlyList<Point3D> To3DPoints(Frame3D frame)
        => Outer.Select(p => (Point3D)frame.ToWorld(new Vector3(p.X, p.Y, 0))).ToList();

    static List<Vector2> NormalizeOuter(IReadOnlyList<Vector2> points)
    {
        var list = RemoveDuplicateClosure(points.ToList());
        if (list.Count >= 3 && PolygonTriangulator.IsCCW(list) == false)
            list.Reverse();
        return list;
    }

    static List<Vector2> NormalizeHole(IReadOnlyList<Vector2> points)
    {
        var list = RemoveDuplicateClosure(points.ToList());
        if (list.Count >= 3 && PolygonTriangulator.IsCCW(list))
            list.Reverse();
        return list;
    }

    /// <summary>Removes consecutive duplicates and explicit geometric closure for profile rings.</summary>
    public static List<Vector2> CleanRing(IReadOnlyList<Vector2> points, float joinToleranceSquared = 0)
    {
        if (points.Count == 0)
            return [];

        var epsSq = joinToleranceSquared > 0
            ? joinToleranceSquared
            : PolygonTriangulator.Eps * PolygonTriangulator.Eps;
        var ring = new List<Vector2> { points[0] };
        for (var i = 1; i < points.Count; i++)
        {
            if (points[i].DistanceSquared(ring[^1]) > epsSq)
                ring.Add(points[i]);
        }

        while (ring.Count >= 2 && ring[0].DistanceSquared(ring[^1]) <= epsSq)
            ring.RemoveAt(ring.Count - 1);

        return ring;
    }

    static List<Vector2> RemoveDuplicateClosure(List<Vector2> points)
        => CleanRing(points);
}
