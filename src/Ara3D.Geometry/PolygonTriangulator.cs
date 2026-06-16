using System.Diagnostics;

namespace Ara3D.Geometry;

public static class PolygonTriangulator
{
    public const float Eps = 1e-6f;

    // =========================
    // Public entry point
    // =========================

    public static IReadOnlyList<Triangle2D> GetTriangles(
        IReadOnlyList<Vector2> boundary,
        IReadOnlyList<IReadOnlyList<Vector2>> holes)
    {
        if (boundary == null) throw new ArgumentNullException(nameof(boundary));
        if (holes == null) throw new ArgumentNullException(nameof(holes));
        if (boundary.Count < 3) return Array.Empty<Triangle2D>();

        var outer = boundary.ToList();
        var innerHoles = holes.Select(h => h.ToList()).Where(h => h.Count >= 3).ToList();

        // Orientation normalization: outer CCW, holes CW
        EnsureCCW(outer);
        foreach (var h in innerHoles) EnsureCW(h);

        // Validate inputs early
        DebugValidateOuterAndHoles(outer, innerHoles);

        // Stitch holes robustly
        var stitched = StitchHolesIntoOuter(outer, innerHoles);

        // Validate stitched ring
        DebugValidateSimpleRing(stitched, mustBeCCW: true);

        // Ear-clip triangulate
        return EarClipTriangulate(stitched);
    }

    // =========================
    // Debug validations
    // =========================

    public static void DebugValidateOuterAndHoles(List<Vector2> outer, List<List<Vector2>> holes)
    {
        DebugValidateSimpleRing(outer, mustBeCCW: true);

        // Each hole: simple, CW, strictly inside outer
        foreach (var h in holes)
        {
            DebugValidateSimpleRing(h, mustBeCCW: false);

            // Hole inside outer
            var centroid = Centroid(h);
            Debug.Assert(PointInPolygon(outer, centroid), "Hole centroid must be inside the outer polygon.");

            // Disjoint holes: no edge-edge intersections
            foreach (var other in holes)
            {
                if (ReferenceEquals(h, other)) continue;
                Debug.Assert(!PolygonsIntersect(h, other), "Holes must be pairwise disjoint (no intersections).");
            }

            // Hole should not touch outer (shared vertex/edge creates degeneracy for simple stitch)
            Debug.Assert(!PolygonsIntersect(outer, h, allowTouch: false), "Hole must not touch or intersect outer.");
        }
    }

    public static void DebugValidateSimpleRing(IReadOnlyList<Vector2> ring, bool mustBeCCW)
    {
        Debug.Assert(ring.Count >= 3, "Ring must have at least 3 vertices.");
        Debug.Assert(!HasDuplicateConsecutiveVertices(ring), "Ring has duplicate consecutive vertices.");
        Debug.Assert(!HasSelfIntersection(ring), "Ring must be simple (no self-intersections).");

        var ccw = IsCCW(ring);
        if (mustBeCCW) Debug.Assert(ccw, "Outer ring must be CCW.");
        else Debug.Assert(!ccw, "Hole ring must be CW.");
    }

    public static bool HasSelfIntersection(IReadOnlyList<Vector2> ring)
        => PolygonEdges(ring).Any(e1 => PolygonEdges(ring).Any(e2 =>
            !ReferenceEquals(e1, e2) && NonAdjacentIntersect(e1, e2, ring)));

    public static bool HasDuplicateConsecutiveVertices(IReadOnlyList<Vector2> ring)
    {
        for (int i = 0; i < ring.Count; ++i)
            if (ring[i].DistanceSquared(ring[(i + 1) % ring.Count]) <= Eps * Eps)
                return true;
        return false;
    }

    public static bool NonAdjacentIntersect(Line2D e1, Line2D e2, IReadOnlyList<Vector2> ring)
    {
        // Skip adjacency in the ring
        int i1 = IndexOfVertex(ring, e1.A);
        int i1n = (i1 + 1) % ring.Count;
        int j1 = IndexOfVertex(ring, e2.A);
        int j1n = (j1 + 1) % ring.Count;

        bool adjacent =
            (i1 == j1) || (i1 == j1n) || (i1n == j1) || (i1n == j1n);
        if (adjacent) return false;

        return SegmentsIntersect(e1, e2, allowTouch: false);
    }

    // =========================
    // Geometry helpers
    // =========================

    public static List<Integer3> Triangulate(IReadOnlyList<Vector2> polygon)
    {
        if (polygon == null)
            throw new ArgumentNullException(nameof(polygon));

        if (polygon.Count < 3)
            throw new ArgumentException("Polygon must have at least 3 points.", nameof(polygon));

        // Build index list
        var indices = new List<int>(polygon.Count);
        for (var i = 0; i < polygon.Count; i++)
            indices.Add(i);

        // Ensure CCW winding for the ear clipping logic
        if (SignedArea(polygon) < 0)
            indices.Reverse();

        var result = new List<Integer3>();

        var guard = 0;
        while (indices.Count > 3)
        {
            var earFound = false;

            for (var i = 0; i < indices.Count; i++)
            {
                var prevIndex = indices[(i - 1 + indices.Count) % indices.Count];
                var currIndex = indices[i];
                var nextIndex = indices[(i + 1) % indices.Count];

                var a = polygon[prevIndex];
                var b = polygon[currIndex];
                var c = polygon[nextIndex];

                if (!IsConvex(a, b, c))
                    continue;

                var containsOtherPoint = false;
                for (var j = 0; j < indices.Count; j++)
                {
                    var testIndex = indices[j];
                    if (testIndex == prevIndex || testIndex == currIndex || testIndex == nextIndex)
                        continue;

                    if (PointInTriangle(polygon[testIndex], a, b, c))
                    {
                        containsOtherPoint = true;
                        break;
                    }
                }

                if (containsOtherPoint)
                    continue;

                // Found an ear
                result.Add((prevIndex, currIndex, nextIndex));
                indices.RemoveAt(i);
                earFound = true;
                break;
            }

            if (!earFound)
                throw new InvalidOperationException(
                    "Failed to triangulate polygon. It may be self-intersecting, degenerate, or contain collinear issues.");

            // Safety guard against infinite loops in malformed input
            guard++;
            if (guard > polygon.Count * polygon.Count)
                throw new InvalidOperationException("Triangulation aborted due to unexpected input.");
        }

        // Final triangle
        result.Add((indices[0], indices[1], indices[2]));
        return result;
    }

    private static float SignedArea(IReadOnlyList<Vector2> polygon)
    {
        var area = 0f;
        for (var i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % polygon.Count];
            area += a.X * b.Y - b.X * a.Y;
        }
        return area * 0.5f;
    }

    private static bool IsConvex(Vector2 a, Vector2 b, Vector2 c)
    {
        return Cross(b - a, c - b) > 0f;
    }

    private static float Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    public static bool IsCCW(IReadOnlyList<Vector2> poly) => SignedArea(poly) > 0;

    public static void EnsureCCW(List<Vector2> poly)
    {
        if (!IsCCW(poly)) poly.Reverse();
    }

    public static void EnsureCW(List<Vector2> poly)
    {
        if (IsCCW(poly)) poly.Reverse();
    }

    public static float Cross(Vector2 a, Vector2 b, Vector2 c) // (b-a) x (c-a)
        => (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

    public static int Orient(Vector2 a, Vector2 b, Vector2 c)
    {
        float v = Cross(a, b, c);
        if (v > Eps) return 1;
        if (v < -Eps) return -1;
        return 0;
    }

    public static bool OnSegment(Vector2 a, Vector2 b, Vector2 p)
    {
        if (MathF.Abs(Cross(a, b, p)) > Eps) return false;
        float minX = MathF.Min(a.X, b.X) - Eps, maxX = MathF.Max(a.X, b.X) + Eps;
        float minY = MathF.Min(a.Y, b.Y) - Eps, maxY = MathF.Max(a.Y, b.Y) + Eps;
        return p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY;
    }

    public static bool SegmentsIntersect(Line2D e1, Line2D e2, bool allowTouch = true)
        => SegmentsIntersect(e1.A, e1.B, e2.A, e2.B, allowTouch);

    public static bool SegmentsIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d, bool allowTouch = true)
    {
        int o1 = Orient(a, b, c);
        int o2 = Orient(a, b, d);
        int o3 = Orient(c, d, a);
        int o4 = Orient(c, d, b);

        if (o1 != o2 && o3 != o4) return true;

        if (allowTouch)
        {
            if (o1 == 0 && OnSegment(a, b, c)) return true;
            if (o2 == 0 && OnSegment(a, b, d)) return true;
            if (o3 == 0 && OnSegment(c, d, a)) return true;
            if (o4 == 0 && OnSegment(c, d, b)) return true;
        }
        return false;
    }

    public static bool PolygonsIntersect(IReadOnlyList<Vector2> a, IReadOnlyList<Vector2> b, bool allowTouch = true)
    {
        foreach (var ea in PolygonEdges(a))
        foreach (var eb in PolygonEdges(b))
            if (SegmentsIntersect(ea, eb, allowTouch))
                return true;
        return false;
    }

    public static bool PointInPolygon(IReadOnlyList<Vector2> poly, Vector2 p)
    {
        // Ray casting to +X
        bool inside = false;
        int n = poly.Count;
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            var a = poly[j];
            var b = poly[i];
            bool cond = ((a.Y > p.Y) != (b.Y > p.Y)) &&
                        (p.X < (b.X - a.X) * (p.Y - a.Y) / ((b.Y - a.Y) == 0 ? 1e-30f : (b.Y - a.Y)) + a.X);
            if (cond) inside = !inside;
        }
        return inside;
    }

    public static IEnumerable<Line2D> PolygonEdges(IReadOnlyList<Vector2> poly)
    {
        for (int i = 0, n = poly.Count; i < n; ++i)
            yield return new Line2D(poly[i], poly[(i + 1) % n]);
    }

    public static int IndexOfVertex(IReadOnlyList<Vector2> poly, Vector2 v)
    {
        for (int i = 0; i < poly.Count; ++i)
            if (poly[i].DistanceSquared(v) <= Eps * Eps) return i;
        return -1;
    }

    public static Vector2 Centroid(IReadOnlyList<Vector2> poly)
    {
        // Simple centroid (area-weighted is unnecessary for assertions)
        float x = 0, y = 0;
        foreach (var p in poly) { x += p.X; y += p.Y; }
        return new Vector2(x / poly.Count, y / poly.Count);
    }

    // =========================
    // Stitching (hole bridging)
    // =========================

    public static List<Vector2> StitchHolesIntoOuter(List<Vector2> outer, List<List<Vector2>> holes)
    {
        var stitched = new List<Vector2>(outer);

        foreach (var hole in holes)
        {
            // Choose rightmost hole vertex H (max X, tie break max Y)
            int hr = RightmostVertexIndex(hole);
            var H = hole[hr];

            // Intersect +X ray from H with stitched outer to get first crossing edge
            var (edgeIndex, xHit, hitPoint) = FirstRayHitToRight(H, stitched);
            Debug.Assert(edgeIndex >= 0, "Could not find a rightward ray hit on outer; ensure hole is inside outer.");

            // Candidate bridge endpoints: the two endpoints of the hit edge, plus their neighbors to escape reflex traps
            var candidates = CandidateBridgeVertices(stitched, edgeIndex);

            // Try candidates ordered by distance; pick the first valid bridge
            int bestIdx = -1;
            float bestD2 = float.PositiveInfinity;

            foreach (int idx in candidates)
            {
                var O = stitched[idx];
                if (!IsValidBridge(H, O, stitched, holes, outer)) continue;

                float d2 = H.DistanceSquared(O);
                if (d2 < bestD2)
                {
                    bestD2 = d2;
                    bestIdx = idx;
                }
            }

            // Fallback: broaden search to all outer vertices if none of the focused candidates work
            if (bestIdx < 0)
            {
                for (int i = 0; i < stitched.Count; ++i)
                {
                    var O = stitched[i];
                    if (!IsValidBridge(H, O, stitched, holes, outer)) continue;
                    float d2 = H.DistanceSquared(O);
                    if (d2 < bestD2) { bestD2 = d2; bestIdx = i; }
                }
            }

            if (bestIdx < 0)
                throw new InvalidOperationException("Failed to find a valid bridge from hole to outer. Check geometry.");

            // Splice hole into stitched ring
            stitched = SpliceHole(stitched, hole, hr, bestIdx);

            // Cleanup & re-validate
            stitched = DedupImmediate(stitched);
            stitched = RemoveCollinear(stitched);
            EnsureCCW(stitched);
            DebugValidateSimpleRing(stitched, mustBeCCW: true);
        }

        return stitched;
    }

    public static int RightmostVertexIndex(IReadOnlyList<Vector2> poly)
    {
        int idx = 0;
        for (int i = 1; i < poly.Count; ++i)
        {
            if (poly[i].X > poly[idx].X + Eps ||
                (Math.Abs(poly[i].X - poly[idx].X) <= Eps && poly[i].Y > poly[idx].Y))
                idx = i;
        }
        return idx;
    }

    public static (int edgeIndex, float xHit, Vector2 hitPoint) FirstRayHitToRight(Vector2 H, IReadOnlyList<Vector2> outer)
    {
        int bestEdge = -1;
        float bestX = float.PositiveInfinity;
        Vector2 bestPt = default;

        var rayY = H.Y;

        var edges = PolygonEdges(outer).ToList();
        for (int ei = 0; ei < edges.Count; ++ei)
        {
            var e = edges[ei];
            var a = e.A; var b = e.B;

            // Skip horizontal edges at ray level to avoid instability; handle by y-range test with half-open interval
            bool crossesY = ((a.Y > rayY) != (b.Y > rayY));
            if (!crossesY) continue;

            // Compute intersection X with the horizontal line y = rayY
            float t = (rayY - a.Y) / ((b.Y - a.Y) == 0 ? float.Epsilon : (b.Y - a.Y));
            float x = a.X + t * (b.X - a.X);

            if (x > H.X + Eps && x < bestX)
            {
                bestX = x;
                bestEdge = ei;
                bestPt = new Vector2(x, rayY);
            }
        }

        return (bestEdge, bestX, bestPt);
    }

    public static List<int> CandidateBridgeVertices(IReadOnlyList<Vector2> ring, int edgeIndex)
    {
        // Endpoints of the edge & their neighbors (wrap-safe)
        int n = ring.Count;
        int i0 = edgeIndex;
        int i1 = (edgeIndex + 1) % n;

        // Neighbor indices
        int i0Prev = (i0 - 1 + n) % n;
        int i1Next = (i1 + 1) % n;

        // Provide a small prioritized set
        var list = new List<int> { i0, i1, i0Prev, i1Next };
        // Deduplicate while preserving order
        var seen = new HashSet<int>();
        return list.Where(i => seen.Add(i)).ToList();
    }

    public static bool IsValidBridge(Vector2 H, Vector2 O,
        IReadOnlyList<Vector2> stitched, IReadOnlyList<IReadOnlyList<Vector2>> holes, IReadOnlyList<Vector2> outer)
    {
        if (H.DistanceSquared(O) <= Eps * Eps) return false;

        // The bridge segment must not cross any edge of stitched ring
        if (SegmentCrossesAnyEdge(H, O, stitched)) return false;

        // And not cross any hole edge
        foreach (var h in holes)
            if (SegmentCrossesAnyEdge(H, O, h)) return false;

        // Midpoint inside outer and outside all holes
        var mid = (H + O) * 0.5f; // TODO(adapt to Ara3D): if operator+ isn't available use new Vector2((H.X+O.X)*0.5f, (H.Y+O.Y)*0.5f)
        if (!PointInPolygon(outer, mid)) return false;
        foreach (var h in holes)
            if (PointInPolygon(h, mid)) return false;

        return true;
    }

    public static bool SegmentCrossesAnyEdge(Vector2 a, Vector2 b, IReadOnlyList<Vector2> ring)
    {
        int n = ring.Count;
        for (int i = 0; i < n; ++i)
        {
            var c = ring[i];
            var d = ring[(i + 1) % n];

            // Skip edges that share endpoints with (a,b)
            if ((a.DistanceSquared(c) <= Eps * Eps && b.DistanceSquared(d) <= Eps * Eps) ||
                (a.DistanceSquared(d) <= Eps * Eps && b.DistanceSquared(c) <= Eps * Eps))
                continue;
            if (a.DistanceSquared(c) <= Eps * Eps ||
                a.DistanceSquared(d) <= Eps * Eps ||
                b.DistanceSquared(c) <= Eps * Eps ||
                b.DistanceSquared(d) <= Eps * Eps)
                continue;

            if (SegmentsIntersect(a, b, c, d, allowTouch: false)) return true;
        }
        return false;
    }

    public static List<Vector2> SpliceHole(List<Vector2> stitched, List<Vector2> hole, int holeStartIdx, int outerIdx)
    {
        // We walk: stitched[0..outerIdx], H, then hole loop starting at holeStartIdx (CW),
        // back to H, then stitched[outerIdx..end].
        var H = hole[holeStartIdx];

        var result = new List<Vector2>(stitched.Count + hole.Count + 2);

        // stitched up to outerIdx (inclusive)
        for (int i = 0; i <= outerIdx; ++i) result.Add(stitched[i]);

        // bridge to H
        result.Add(H);

        // hole cycle starting at holeStartIdx, going CW
        for (int k = 1; k <= hole.Count; ++k)
            result.Add(hole[(holeStartIdx + k) % hole.Count]);

        // back to H (closes the detour)
        result.Add(H);

        // continue stitched after outerIdx
        for (int i = outerIdx; i < stitched.Count; ++i)
            result.Add(stitched[(i + 1) % stitched.Count]);

        return result;
    }

    public static List<Vector2> DedupImmediate(List<Vector2> pts)
    {
        var r = new List<Vector2>(pts.Count);
        Vector2? last = null;
        foreach (var p in pts)
        {
            if (last.HasValue && last.Value.DistanceSquared(p) <= Eps * Eps) continue;
            r.Add(p);
            last = p;
        }
        if (r.Count >= 2 && r[0].DistanceSquared(r[^1]) <= Eps * Eps)
            r.RemoveAt(r.Count - 1);
        return r;
    }

    public static List<Vector2> RemoveCollinear(List<Vector2> pts)
    {
        if (pts.Count <= 3) return new List<Vector2>(pts);
        var r = new List<Vector2>(pts.Count);
        for (int i = 0; i < pts.Count; ++i)
        {
            var a = pts[(i - 1 + pts.Count) % pts.Count];
            var b = pts[i];
            var c = pts[(i + 1) % pts.Count];
            if (MathF.Abs(Cross(a, b, c)) > Eps) r.Add(b); // keep non-collinear
            else
            {
                // If collinear but b equals a or c, drop duplicate anyway
                if (b.DistanceSquared(a) > Eps * Eps && b.DistanceSquared(c) > Eps * Eps)
                    r.Add(b);
            }
        }
        return r;
    }

    // =========================
    // Ear clipping
    // =========================

    public sealed class Node
    {
        public Vector2 P;
        public int Prev, Next;
        public bool Removed;
    }

    public static IReadOnlyList<Triangle2D> EarClipTriangulate(List<Vector2> poly)
    {
        int n = poly.Count;
        if (n < 3) return Array.Empty<Triangle2D>();
        if (!IsCCW(poly)) poly.Reverse();

        // linked list representation
        var nodes = new Node[n];
        for (int i = 0; i < n; ++i)
        {
            nodes[i] = new Node { P = poly[i], Prev = (i - 1 + n) % n, Next = (i + 1) % n, Removed = false };
        }

        var tris = new List<Triangle2D>(Math.Max(1, n - 2));
        int remaining = n;
        int cur = 0;

        // Local helpers
        bool IsConvex(int i)
        {
            var a = nodes[nodes[i].Prev].P;
            var b = nodes[i].P;
            var c = nodes[nodes[i].Next].P;
            return Cross(a, b, c) > Eps; // CCW polygon: positive cross = convex
        }

        bool AnyPointInTriangle(int i)
        {
            var a = nodes[nodes[i].Prev].P;
            var b = nodes[i].P;
            var c = nodes[nodes[i].Next].P;

            for (int k = 0; k < n; ++k)
            {
                if (nodes[k].Removed) continue;
                if (k == i || k == nodes[i].Prev || k == nodes[i].Next) continue;
                var p = nodes[k].P;
                if (PointInTriangle(a, b, c, p)) return true;
            }
            return false;
        }

        bool DiagonalIsClear(int i)
        {
            var a = nodes[nodes[i].Prev].P;
            var c = nodes[nodes[i].Next].P;

            for (int e = 0; e < n; ++e)
            {
                if (nodes[e].Removed) continue;
                int en = nodes[e].Next;
                if (nodes[en].Removed) continue;

                var p = nodes[e].P;
                var q = nodes[en].P;

                // Skip edges that share endpoints with a or c or are the ear edges
                if ((p.DistanceSquared(a) <= Eps * Eps && q.DistanceSquared(nodes[i].P) <= Eps * Eps) ||
                    (p.DistanceSquared(nodes[i].P) <= Eps * Eps && q.DistanceSquared(c) <= Eps * Eps))
                    continue;
                if ((p.DistanceSquared(c) <= Eps * Eps && q.DistanceSquared(nodes[i].P) <= Eps * Eps) ||
                    (p.DistanceSquared(nodes[i].P) <= Eps * Eps && q.DistanceSquared(a) <= Eps * Eps))
                    continue;

                if (p.DistanceSquared(a) <= Eps * Eps || p.DistanceSquared(c) <= Eps * Eps ||
                    q.DistanceSquared(a) <= Eps * Eps || q.DistanceSquared(c) <= Eps * Eps)
                    continue;

                if (SegmentsIntersect(a, c, p, q, allowTouch: false)) return false;
            }
            return true;
        }

        bool IsEar(int i)
        {
            if (nodes[i].Removed) return false;
            if (!IsConvex(i)) return false;
            if (!DiagonalIsClear(i)) return false;
            if (AnyPointInTriangle(i)) return false;
            return true;
        }

        int guard = 0;
        while (remaining > 2 && guard++ < 200000)
        {
            bool clipped = false;

            for (int t = 0; t < n; ++t)
            {
                int i = (cur + t) % n;
                if (nodes[i].Removed) continue;

                if (IsEar(i))
                {
                    var a = nodes[nodes[i].Prev].P;
                    var b = nodes[i].P;
                    var c = nodes[nodes[i].Next].P;

                    tris.Add((a,b,c));

                    // remove i
                    nodes[nodes[i].Prev].Next = nodes[i].Next;
                    nodes[nodes[i].Next].Prev = nodes[i].Prev;
                    nodes[i].Removed = true;
                    remaining--;
                    cur = nodes[i].Next;
                    clipped = true;
                    break;
                }
            }

            if (!clipped)
            {
                // Provide more info for debugging
                var dbg = $"Ear clipping failed: n={n}, remaining={remaining}. " +
                          "Likely non-simple ring or degeneracy after stitching.";
                throw new InvalidOperationException(dbg);
            }
        }

        Debug.Assert(tris.Count == n - 2, "Ear clipping produced unexpected triangle count.");
        return tris;
    }

    public static bool PointInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
    {
        // Barycentric sign checks (edges inclusive)
        float c1 = Cross(a, b, p);
        float c2 = Cross(b, c, p);
        float c3 = Cross(c, a, p);
        bool hasNeg = (c1 < -Eps) || (c2 < -Eps) || (c3 < -Eps);
        bool hasPos = (c1 > Eps) || (c2 > Eps) || (c3 > Eps);
        return !(hasNeg && hasPos);
    }
}