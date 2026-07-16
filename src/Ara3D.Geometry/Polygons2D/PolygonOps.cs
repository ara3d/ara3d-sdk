using Ara3D.Collections;

namespace Ara3D.Geometry;

public static class PolygonOps
{
    public const float Eps = 1e-7f;

    public static Polygon2D ToPolygon(this IReadOnlyList<Vector2> self)
        => new(self);

    public static SimplePolygon2D ToSimplePolygon(this IReadOnlyList<Vector2> self)
        => new(self);

    public static SimplePolygonWithHoles ToPolygonWithHoles(this IReadOnlyList<Vector2> self, params IReadOnlyList<Vector2>[] holes)
        => new(self.ToSimplePolygon(), holes.Select(ToSimplePolygon).ToArray());

    public static int GetNumPoints(this IPolygon2D self)
        => self.Points.Count;

    public static Vector2 GetPoint(this IPolygon2D self, int i)
        => self.Points.ElementAtModulo(i);

    // --- Geometry helpers ---
    public static double Cross(this Vector2 a, Vector2 b)
        => (double)a.X * b.Y - (double)a.Y * b.X;

    public static float Cross(Vector2 a, Vector2 b, Vector2 c)
        => (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

    public static int Orientation(Vector2 a, Vector2 b, Vector2 c, float eps = Eps)
    {
        var v = Cross(a, b, c);
        if (v > eps) return 1;
        if (v < -eps) return -1;
        return 0;
    }

    // angle between two points (direction angle of the segment a->b)
    public static Angle Angle(this Vector2 a, Vector2 b)
        => MathF.Atan2(b.Y - a.Y, b.X - a.X);

    // neighbor access (FIX: next is i+1, previous is i-1, modulo n)
    public static Vector2 GetNextPoint(this IPolygon2D self, int i)
        => self.GetPoint(i + 1);

    public static Vector2 GetPreviousPoint(this IPolygon2D self, int i)
        => self.GetPoint(i - 1);

    public static Vector2 VectorFromPrevious(this IPolygon2D self, int i)
        => self.GetPoint(i) - self.GetPreviousPoint(i);

    public static Vector2 VectorToPrevious(this IPolygon2D self, int i)
        => self.GetPreviousPoint(i) - self.GetPoint(i);

    public static Vector2 VectorToNext(this IPolygon2D self, int i)
        => self.GetNextPoint(i) - self.GetPoint(i);

    public static Vector2 VectorFromNext(this IPolygon2D self, int i)
        => self.GetPoint(i) - self.GetNextPoint(i);

    public static Line2D GetEdge(this IPolygon2D self, int i)
        => new(self.GetPoint(i), self.GetNextPoint(i));

    public static Triangle2D TriangleAtPoint(this IPolygon2D self, int i)
        => new(self.GetPreviousPoint(i), self.GetPoint(i), self.GetNextPoint(i));

    // --- Per-vertex classification / angles ---
    public static int WindingOrder(this IPolygon2D self, int i)
    {
        var a = self.GetPreviousPoint(i);
        var b = self.GetPoint(i);
        var c = self.GetNextPoint(i);
        // left turn -> +1, right turn -> -1, collinear -> 0
        return Orientation(a, b, c);
    }

    public static bool IsReflexive(this IPolygon2D self, int i)
    {
        // Reflex if interior angle > 180°
        // Make it orientation-invariant using polygon area sign.
        var areaSign = Math.Sign(self.SignedArea()); // +1 ccw, -1 cw, 0 degenerate
        var turn = self.WindingOrder(i); // +1 left, -1 right, 0 flat
        if (turn == 0) return false; // collinear -> not reflex
        // For CCW, right turns are reflex; for CW, left turns are reflex.
        return (areaSign >= 0) ? (turn < 0) : (turn > 0);
    }

    // interior angle at vertex i, in radians, in [0, 2π)
    public static Angle Angle(this IPolygon2D self, int i)
    {
        var p = self.GetPoint(i);
        var u = self.GetPreviousPoint(i) - p; // incoming
        var v = self.GetNextPoint(i) - p; // outgoing
        var dot = u.Dot(v);
        var cross = Cross(p + u, p, p + v); // equivalent to u x v
        // angle between u and v (unsigned interior angle)
        var angle = MathF.Atan2(MathF.Abs(cross), dot);
        // If reflex, interior angle > π
        return self.IsReflexive(i) ? (MathF.PI * 2f - angle) : angle;
    }

    // +1 for CCW, -1 for CW, 0 degenerate
    public static int WindingNumber(this IPolygon2D self)
    {
        var a = self.SignedArea();
        if (a > Eps) return +1;
        if (a < -Eps) return -1;
        return 0;
    }

    public static double SignedArea(this IPolygon2D poly)
    {
        var s = 0.0;
        for (int i = 0, n = poly.GetNumPoints(); i < n; i++)
        {
            var p = poly.GetPoint(i);
            var q = poly.GetNextPoint(i);
            s += Cross(p, q);
        }

        return s * 0.5;
    }

    public static bool IsConvexPoint(this IPolygon2D self, int i, float eps = Eps)
        => self.WindingOrder(i) != 0 && !self.IsReflexive(i);

    // Polygon is convex if every non-collinear vertex is not reflex.
    public static bool IsConvex(this IPolygon2D self, float eps = Eps)
        => Enumerable.Range(0, self.GetNumPoints())
            .All(i => self.WindingOrder(i) == 0 || !self.IsReflexive(i));

    public static bool IsConvex(this IConcavePolygon2D self, float eps = Eps) => false;
    public static bool IsConvex(this IConvexPolygon2D self, float eps = Eps) => true;

    // --- Segment / triangle intersection ---
    public static bool SegmentsIntersect(Line2D line1, Line2D line2, float eps = Eps)
    {
        var o1 = Orientation(line1.A, line1.B, line2.A, eps);
        var o2 = Orientation(line1.A, line1.B, line2.B, eps);
        var o3 = Orientation(line2.A, line2.B, line1.A, eps);
        var o4 = Orientation(line2.A, line2.B, line1.B, eps);

        if (o1 != o2 && o3 != o4) return true; // proper intersection

        // Collinear / touching cases:
        if (o1 == 0 && line1.OnSegment(line2.A, eps)) return true;
        if (o2 == 0 && line1.OnSegment(line2.B, eps)) return true;
        if (o3 == 0 && line2.OnSegment(line1.A, eps)) return true;
        if (o4 == 0 && line2.OnSegment(line1.B, eps)) return true;

        return false;
    }

    public static bool LinesCross(this Line2D line1, Line2D line2)
    {
        // strict crossing: exclude pure endpoint touching and collinearity overlap
        var o1 = Orientation(line1.A, line1.B, line2.A);
        var o2 = Orientation(line1.A, line1.B, line2.B);
        var o3 = Orientation(line2.A, line2.B, line1.A);
        var o4 = Orientation(line2.A, line2.B, line1.B);
        return (o1 * o2 < 0) && (o3 * o4 < 0);
    }

    public static bool LinesCrossOrTouch(this Line2D line1, Line2D line2)
        => SegmentsIntersect(line1, line2, Eps);

    // strictly inside test: use barycentric side tests with strict eps
    public static bool StrictInside(this Triangle2D tri, Vector2 p)
    {
        var c1 = Cross(tri.A, tri.B, p);
        var c2 = Cross(tri.B, tri.C, p);
        var c3 = Cross(tri.C, tri.A, p);
        var hasNeg = (c1 < -Eps) || (c2 < -Eps) || (c3 < -Eps);
        var hasPos = (c1 > Eps) || (c2 > Eps) || (c3 > Eps);
        return !(hasNeg && hasPos) && MathF.Abs(c1) > Eps && MathF.Abs(c2) > Eps && MathF.Abs(c3) > Eps;
    }


    public static bool LineCrosses(this Triangle2D tri, Line2D line)
    {
        // strict: either crosses an edge strictly or one endpoint strictly inside
        var e0 = new Line2D(tri.A, tri.B);
        var e1 = new Line2D(tri.B, tri.C);
        var e2 = new Line2D(tri.C, tri.A);

        if (line.LinesCross(e0) || line.LinesCross(e1) || line.LinesCross(e2))
            return true;

        return StrictInside(tri, line.A) || StrictInside(tri, line.B);
    }

    public static bool LineCrossesOrTouches(this Triangle2D tri, Line2D line)
    {
        var e0 = new Line2D(tri.A, tri.B);
        var e1 = new Line2D(tri.B, tri.C);
        var e2 = new Line2D(tri.C, tri.A);

        if (LinesCrossOrTouch(line, e0) || LinesCrossOrTouch(line, e1) || LinesCrossOrTouch(line, e2))
            return true;

        // inclusive containment of endpoints
        return tri.Contains(line.A) || tri.Contains(line.B);
    }

    public static Bounds2D GetBounds(this IReadOnlyList<Vector2> self)
    {
        var min = Vector2.MaxValue;
        var max = Vector2.MinValue;
        foreach (var x in self)
        {
            min = min.Min(x);
            max = max.Max(x);
        }

        return (min, max);
    }

    public static Bounds2D GetBounds(this IPolygon2D self)
        => self.Points.GetBounds();

    public static Bounds2D GetBounds(this Triangle2D self)
        => GetBounds([self.A, self.B, self.C]);

    public static Vector2 Centroid(this IPolygon2D self)
        => self.Points.Average();

    public static Vector2 Average(this IReadOnlyList<Vector2> self)
    {
        var sum = Vector2.Zero;
        foreach (var x in self)
            sum += x;
        return sum / self.Count;
    }

    public static bool Contains(this IPolygon2D self, Vector2 p)
    {
        // Boundary check first
        var n = self.GetNumPoints();
        for (var i = 0; i < n; i++)
        {
            var e = self.GetEdge(i);
            if (e.OnSegment(p)) return true;
        }

        // Ray cast to +X; count crossings where edge crosses upward in Y
        var inside = false;
        for (var i = 0; i < n; i++)
        {
            var a = self.GetPoint(i);
            var b = self.GetNextPoint(i);

            // ensure a.Y <= b.Y ordering per edge for robustness
            if (a.Y > b.Y) (a, b) = (b, a);

            // edge straddles p.Y (half-open on the top to avoid double counting)
            if (p.Y > a.Y && p.Y <= b.Y && (b.Y - a.Y) > Eps)
            {
                // compute intersection X of edge with horizontal line y = p.Y
                var t = (p.Y - a.Y) / (b.Y - a.Y);
                var xInt = a.X + t * (b.X - a.X);
                if (xInt > p.X) inside = !inside;
            }
        }

        return inside;
    }

    public static bool Contains(this Triangle2D t, Vector2 p, float eps = Eps)
    {
        var c1 = Cross(t.A, t.B, p);
        var c2 = Cross(t.B, t.C, p);
        var c3 = Cross(t.C, t.A, p);
        var hasNeg = (c1 < -eps) || (c2 < -eps) || (c3 < -eps);
        var hasPos = (c1 > eps) || (c2 > eps) || (c3 > eps);
        return !(hasNeg && hasPos);
    }

    public static bool OnSegment(this Line2D line, Vector2 p, float eps = Eps)
    {
        if (MathF.Abs(Cross(line.A, line.B, p)) > eps) return false;
        float minX = MathF.Min(line.A.X, line.B.X) - eps, maxX = MathF.Max(line.A.X, line.B.X) + eps;
        float minY = MathF.Min(line.A.Y, line.B.Y) - eps, maxY = MathF.Max(line.A.Y, line.B.Y) + eps;
        return p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY;
    }

    public static IReadOnlyList<Line2D> Edges(this IPolygon2D self)
        => self.GetNumPoints().MapRange(i => self.GetEdge(i));

    public static IReadOnlyList<float> Lengths(this IPolygon2D self)
        => self.Edges().Select(e => e.Length.Value);

    public static IReadOnlyList<Angle> Angles(this IPolygon2D self)
        => self.GetNumPoints().MapRange(i => self.Angle(i));

    public static bool IsRegular(this IPolygon2D p, float lenTol, float angTol)
    {
        var n = p.GetNumPoints();
        if (n < 3) return false;
        var lengths = p.Lengths();
        var targetLen = lengths.Average();
        if (Enumerable.Any(lengths, l => MathF.Abs(l - targetLen) > lenTol)) return false;

        // angles
        var angles = p.Angles();
        var avgAngle = angles.Average(a => a);
        return Enumerable.All(angles, a => MathF.Abs(a - avgAngle) <= angTol);
    }

    // --- Basic enumeration helpers ----------------------------------------------------

    public static IEnumerable<Vector2> Vertices(this IPolygon2D p)
        => p.Points;

    public static IEnumerable<(int i, Vector2 v)> IndexedVertices(this IPolygon2D p)
    {
        var n = p.GetNumPoints();
        for (var i = 0; i < n; i++) yield return (i, p.GetPoint(i));
    }

    public static IEnumerable<(int i, Line2D e)> IndexedEdges(this IPolygon2D p)
    {
        var n = p.GetNumPoints();
        for (var i = 0; i < n; i++) yield return (i, p.GetEdge(i));
    }

    // --- Orientation / ordering -------------------------------------------------------

    public static Polygon2D Reverse(this IPolygon2D self)
        => new Polygon2D(self.Points.Reverse());
    
    public static bool IsClockwise(this IPolygon2D p) 
        => p.WindingNumber() < 0;
    
    public static bool IsCounterClockwise(this IPolygon2D p) 
        => p.WindingNumber() > 0;

    public static Polygon2D EnsureCCW(this IPolygon2D p)
        => p.IsCounterClockwise() ? new Polygon2D(p.Points) : p.Reverse();

    public static Polygon2D EnsureCW(this IPolygon2D p)
        => p.IsClockwise() ? new Polygon2D(p.Points) : p.Reverse();

    /// Rotate starting vertex (keeps orientation).
    public static Polygon2D RotateStart(this IPolygon2D p, int startIndex)
    {
        var n = p.GetNumPoints();
        if (n == 0) return new Polygon2D(p.Points);
        startIndex = ((startIndex % n) + n) % n;
        var reordered = Enumerable.Range(0, n).Select(k => p.GetPoint(startIndex + k)).ToArray();
        return new Polygon2D(reordered);
    }

    /// Start from the lexicographically smallest vertex (x, then y).
    public static Polygon2D NormalizeStartMinXY(this IPolygon2D p)
    {
        var n = p.GetNumPoints();
        var best = 0;
        var bestV = p.GetPoint(0);
        for (var i = 1; i < n; i++)
        {
            var v = p.GetPoint(i);
            if (v.X < bestV.X || (v.X == bestV.X && v.Y < bestV.Y))
            { best = i; bestV = v; }
        }
        return p.RotateStart(best);
    }

    // --- Metrics ---------------------------------------------------------------------

    public static double Perimeter(this IPolygon2D p)
    {
        double sum = 0;
        var n = p.GetNumPoints();
        for (var i = 0; i < n; i++)
        {
            var d = p.GetNextPoint(i) - p.GetPoint(i);
            sum += d.Length();
        }
        return sum;
    }

    public static double[] EdgeLengths(this IPolygon2D p)
    {
        var n = p.GetNumPoints();
        var a = new double[n];
        for (var i = 0; i < n; i++)
            a[i] = (p.GetNextPoint(i) - p.GetPoint(i)).Length();
        return a;
    }

    public static double Area(this IPolygon2D p) => Math.Abs(p.SignedArea());

    // --- Simplicity / intersections ---------------------------------------------------

    public static bool HasSelfIntersections(this IPolygon2D p, float eps = Eps)
        => p.SelfIntersectingEdgePairs(true, eps).Any();

    /// Returns all pairs of intersecting edges (i<j). If includeTouching=false, excludes endpoint-only touches.
    public static IEnumerable<(int i, int j)> SelfIntersectingEdgePairs(this IPolygon2D p, bool includeTouching = true, float eps = Eps)
    {
        var n = p.GetNumPoints();
        for (var i = 0; i < n; i++)
        {
            var e1 = p.GetEdge(i);
            for (var j = i + 1; j < n; j++)
            {
                // skip adjacent edges and the wrap-around neighbor
                if ((j == (i + 1) % n) || (i == (j + 1) % n)) continue;

                var e2 = p.GetEdge(j);
                var hit = includeTouching ? LinesCrossOrTouch(e1, e2) : LinesCross(e1, e2);
                if (hit) yield return (i, j);
            }
        }
    }

    /// Returns the first intersection point found (if any). If multiple, the earliest (lowest i,j).
    public static bool TryFirstIntersectionPoint(this IPolygon2D p, out Vector2 point, float eps = Eps)
    {
        foreach (var (i, j) in p.SelfIntersectingEdgePairs(true, eps))
        {
            if (TrySegmentIntersection(p.GetEdge(i), p.GetEdge(j), out point, eps))
                return true;
        }
        point = default;
        return false;
    }

    /// Segment-segment intersection point (inclusive). Collinear-overlap yields 'false' but point is undefined.
    public static bool TrySegmentIntersection(Line2D a, Line2D b, out Vector2 p, float eps = Eps)
    {
        // Parametric solve: a.A + t*(a.B-a.A) = b.A + u*(b.B-b.A)
        var r = a.B - a.A;
        var s = b.B - b.A;
        var rxs = Cross(a.A, a.B, b.B) - Cross(a.A, a.B, b.A); // det via 2D cross helper
        var denom = r.X * s.Y - r.Y * s.X;

        // Parallel or collinear
        if (MathF.Abs(denom) <= eps)
        {
            p = default;
            return false;
        }

        var qp = b.A - a.A;
        var t = (qp.X * s.Y - qp.Y * s.X) / denom;
        var u = (qp.X * r.Y - qp.Y * r.X) / denom;

        if (t >= -eps && t <= 1 + eps && u >= -eps && u <= 1 + eps)
        {
            p = a.A + t * r;
            return true;
        }

        p = default;
        return false;
    }

    // --- Point containment variants ---------------------------------------------------

    /// Even-odd with boundary excluded.
    public static bool ContainsStrict(this IPolygon2D self, Vector2 p)
        => !self.ContainsOnBoundary(p) && self.Contains(p);

    /// True if point lies on any boundary segment (within eps).
    public static bool ContainsOnBoundary(this IPolygon2D self, Vector2 p, float eps = Eps)
    {
        foreach (var e in self.Edges())
            if (e.OnSegment(p, eps)) return true;
        return false;
    }

    // --- Distance to boundary / closest point ----------------------------------------

    public static double DistanceToBoundary(this IPolygon2D p, Vector2 q, out Vector2 closestPoint)
    {
        var best = double.MaxValue;
        Vector2 bestP = default;
        foreach (var e in p.Edges())
        {
            var cp = e.ClosestPointOnSegment(q);
            var d = (cp - q).Length();
            if (d < best) { best = d; bestP = cp; }
        }
        closestPoint = bestP;
        return best;
    }

    public static Vector2 ClosestPointOnSegment(this Line2D e, Vector2 q)
    {
        var ab = e.B - e.A;
        var t = Vector2.Dot(q - e.A.Vector2, ab) / Math.Max(Eps, ab.Dot(ab));
        t = Math.Clamp(t, 0f, 1f);
        return e.A + t * ab;
    }

    // --- Cleaning / normalization -----------------------------------------------------

    /// Remove collinear vertices (keeps endpoints if triangle or fewer remain).
    public static Polygon2D RemoveCollinearVertices(this IPolygon2D p, float eps = Eps)
    {
        var n = p.GetNumPoints();
        if (n <= 3) return new Polygon2D(p.Points);

        var result = new List<Vector2>(n);
        for (var i = 0; i < n; i++)
        {
            var a = p.GetPreviousPoint(i);
            var b = p.GetPoint(i);
            var c = p.GetNextPoint(i);
            if (MathF.Abs(Cross(a, b, c)) > eps)
                result.Add(b);
        }

        // guard against collapsing everything for nearly-degenerate polygons
        if (result.Count < 3) return new Polygon2D(p.Points);
        return new Polygon2D(result);
    }

    // --- Convex-only helpers ----------------------------------------------------------

    // --- Small numeric helpers --------------------------------------------------------

    public static int Mod(this int x, int m)
    {
        var r = x % m;
        return r < 0 ? r + m : r;
    }

    //==

    /// Enumerate a triangle fan (0, i, i+1) — valid if polygon is convex (or star-shaped with 0 as kernel point).
    public static IEnumerable<Triangle2D> TrianglesFan(this IPolygon2D p)
    {
        var n = p.GetNumPoints();
        if (n < 3) yield break;
        var v0 = p.GetPoint(0);
        for (var i = 1; i < n - 1; i++)
            yield return new Triangle2D(v0, p.GetPoint(i), p.GetPoint(i + 1));
    }

    /// Quick convex hull check for a candidate "ear" at i (simple, uses existing helpers).
    public static bool IsEarCandidate(this IPolygon2D p, int i)
    {
        // must be convex at i and triangle must not contain other vertices
        if (!p.IsConvexPoint(i)) return false;
        var tri = p.TriangleAtPoint(i);
        var n = p.GetNumPoints();
        for (var k = 0; k < n; k++)
        {
            if (k == i || k == (i - 1).Mod(n) || k == (i + 1).Mod(n)) continue;
            if (tri.Contains(p.GetPoint(k))) return false;
        }
        return true;
    }

    private static bool IsVisible(
        Vector2 a, Vector2 b,
        List<Vector2> outerRing,
        Polygon2D theHole,        // the hole we are currently bridging
        float eps)
    {
        var s = new Line2D(a, b);

        // 1) Segment must not cross any outer edge (except touching at the chosen outer endpoint).
        //    Build on-the-fly polygon for the current outer ring.
        var outer = new Polygon2D(outerRing);
        int nO = outer.GetNumPoints();

        for (int i = 0; i < nO; i++)
        {
            var e = outer.GetEdge(i);
            // allow touching at 'b' if e is incident to b
            bool eIncidentToB = (e.A.Equals(b) || e.B.Equals(b));
            if (eIncidentToB) continue;
            if (LinesCross(s, e) || (LinesCrossOrTouch(s, e) && !outer.ContainsOnBoundary(a) && !outer.ContainsOnBoundary(b)))
                return false;
        }

        // 2) Segment must not cross any edge of this hole (except touching at 'a')
        int nH = theHole.GetNumPoints();
        for (int i = 0; i < nH; i++)
        {
            var e = theHole.GetEdge(i);
            bool eIncidentToA = (e.A.Equals(a) || e.B.Equals(a));
            if (eIncidentToA) continue;
            if (LinesCrossOrTouch(s, e)) return false;
        }

        // 3) Midpoint must be strictly inside the outer and strictly outside the hole
        var mid = (a + b) * 0.5f;
        if (!outer.Contains(mid)) return false;
        if (theHole.Contains(mid)) return false;

        return true;
    }

    public static List<Vector2> SpliceBridge(
        List<Vector2> outer, int outerIdx,
        IReadOnlyList<Vector2> holeCW, int holeIdx)
    {
        int nO = outer.Count;
        int nH = holeCW.Count;

        var result = new List<Vector2>(nO + nH + 2);

        // outer prefix (inclusive of outerIdx)
        for (int i = 0; i <= outerIdx; i++) result.Add(outer[i]);

        var oP = outer[outerIdx];
        var hP = holeCW[holeIdx];

        // Bridge to hole
        result.Add(hP);

        // Walk hole CW from holeIdx -> holeIdx (wrap), *including* start & end
        // to create two coincident bridge edges later
        for (int k = 1; k <= nH; k++)
        {
            int j = (holeIdx + k).Mod(nH);
            result.Add(holeCW[j]);
        }

        // Bridge back to outer
        result.Add(oP);

        // outer suffix (after outerIdx)
        for (int i = outerIdx + 1; i < nO; i++) result.Add(outer[i]);

        return result;
    }

    public static List<Triangle2D> TriangulateEarClipping(this IPolygon2D polygon, float eps = Eps)
    {
        var p = polygon.RemoveCollinearVertices(eps);
        int n = p.GetNumPoints();
        var tris = new List<Triangle2D>(Math.Max(0, n - 2));
        if (n < 3) return tris;

        var ring = Enumerable.Range(0, n).ToList();
        if (p.WindingNumber() < 0) ring.Reverse();

        int safe = 4 * n;
        while (ring.Count > 3 && safe-- > 0)
        {
            bool clipped = false;
            for (int r = 0; r < ring.Count; r++)
            {
                int iPrev = ring[(r - 1).Mod(ring.Count)];
                int i = ring[r];
                int iNext = ring[(r + 1).Mod(ring.Count)];

                var a = p.GetPoint(iPrev);
                var b = p.GetPoint(i);
                var c = p.GetPoint(iNext);

                if (Orientation(a, b, c, eps) <= 0) continue;
                var ear = new Triangle2D(a, b, c);

                bool contains = false;
                for (int k = 0; k < ring.Count; k++)
                {
                    var idx = ring[k];
                    if (idx == iPrev || idx == i || idx == iNext) continue;
                    if (ear.Contains(p.GetPoint(idx), eps)) { contains = true; break; }
                }
                if (contains) continue;

                tris.Add(ear);
                ring.RemoveAt(r);
                clipped = true;
                break;
            }
            if (!clipped) break;
        }

        if (ring.Count == 3)
        {
            var A = p.GetPoint(ring[0]);
            var B = p.GetPoint(ring[1]);
            var C = p.GetPoint(ring[2]);
            if (Orientation(A, B, C, eps) < 0) (B, C) = (C, B);
            tris.Add(new Triangle2D(A, B, C));
        }
        return tris;
    }

    public static (int idx, Vector2 pt) FindVisibleOuterVertex(
        List<Vector2> outer, Polygon2D hole, Vector2 hP, float eps)
    {
        int n = outer.Count;
        int bestIdx = -1;
        float bestDX = float.PositiveInfinity;   // prefer minimal +x distance

        for (int i = 0; i < n; i++)
        {
            var v = outer[i];
            if (v.X + eps < hP.X) continue; // prefer vertices at or to the right

            if (IsVisible(hP, v, outer, hole, eps))
            {
                var dx = v.X - hP.X;
                if (dx >= -eps && dx < bestDX)
                {
                    bestDX = dx;
                    bestIdx = i;
                }
            }
        }
        return (bestIdx, bestIdx >= 0 ? outer[bestIdx] : default);
    }

    public static (int idx, Vector2 pt) FindAnyVisibleOuterVertex(
        List<Vector2> outer, Polygon2D hole, Vector2 hP, float eps)
    {
        int n = outer.Count;
        for (int i = 0; i < n; i++)
        {
            var v = outer[i];
            if (IsVisible(hP, v, outer, hole, eps))
                return (i, v);
        }
        return (-1, default);
    }

    public static bool IsSimple(this IPolygon2D p, float eps = Eps)
        => !p.HasSelfIntersections(eps);

    /*
    public static bool IsSimple(this IPolygon2D p, float eps = Eps)
    {
        int n = p.GetNumPoints();
        for (int i = 0; i < n; i++)
        {
            var e1 = p.GetEdge(i);
            for (int j = i + 1; j < n; j++)
            {
                // skip adjacent edges and the wrap-around neighbor
                if (j == i) continue;
                if ((j == (i + 1) % n) || (i == (j + 1) % n)) continue;

                var e2 = p.GetEdge(j);
                if (LinesCrossOrTouch(e1, e2))
                    return false;
            }
        }
        return true;
    }
    */

    public static TriangleMesh3D ToMesh(this IEnumerable<Triangle2D> triangles)
    {
        var points = new List<Point3D>();
        foreach (var tri in triangles)
        {
            points.Add(tri.A.To3D);
            points.Add(tri.B.To3D);
            points.Add(tri.C.To3D);
        }
        return points.ToMesh();
    }

    public static TriangleMesh3D ToMesh(this IReadOnlyList<Point3D> points)
    {
        var faceIndices = new List<Integer3>();
        for (var i=0; i < points.Count; i += 3)
            faceIndices.Add(new(i,i+1,i+2));
        return new TriangleMesh3D(points, faceIndices);
    }
}