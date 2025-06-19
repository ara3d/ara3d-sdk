namespace Ara3D.Geometry;

public static class PolygonUtils
{
    /* ───────────────────────── PUBLIC API ───────────────────────── */

    /// <summary>Offsets a simple, CCW-oriented polygon by <paramref name="delta"/> (miter joins).</summary>
    public static List<Vector2> OffsetPolygon(IReadOnlyList<Vector2> polygon, float delta)
    {
        var n = polygon.Count;
        if (n < 3) throw new ArgumentException("Need at least three points", nameof(polygon));

        // 1) unit outward normal for every edge
        var normals = new Vector2[n];
        for (int i = 0; i < n; ++i)
        {
            var edge = polygon[(i + 1) % n] - polygon[i];
            normals[i] = edge.Perp().Normalize;
        }

        // 2) miter every corner
        var offset = new List<Vector2>(n);
        for (int i = 0; i < n; ++i)
        {
            var n0 = normals[(i - 1 + n) % n];
            var n1 = normals[i];
            var sum = n0 + n1;

            if (sum.LengthSquared() < 1e-6f)                 // almost collinear
            {
                offset.Add(polygon[i] + n1 * delta);
            }
            else
            {
                var bisector = sum.Normalize;
                var cosHalf = bisector.Dot(n1);     // = 1 / miterScale
                offset.Add(polygon[i] + bisector * (delta / cosHalf));
            }
        }

        return offset;
    }

    /// <summary>Boolean union of two simple CCW polygons (no holes).</summary>
    public static List<Vector2> UnionPolygons(List<Vector2> subj, List<Vector2> clip)
    {
        // 1) add explicit intersection nodes to both edge loops
        var (subjLoop, clipLoop) = InsertIntersections(subj, clip);

        // 2) decide whether we *enter* or *exit* at each intersection
        ClassifyIntersections(subjLoop, clip);

        // 3) walk the stitched graph exactly once
        return WalkUnion(subjLoop, clipLoop);
    }

    /* ───────────────────────── INTERNAL TYPES ───────────────────────── */

    public sealed class Node
    {
        public Vector2 P;
        public Node Next;
        public Node Prev;

        public bool IsIntersection;
        public bool Entry;          // set during classification
        public bool Visited;

        public Node Twin;           // partner in the other polygon (if intersection)
    }

    /* ───────────────────────── UNION IMPLEMENTATION ───────────────────────── */

    private static (List<Node>, List<Node>) InsertIntersections(List<Vector2> subj, List<Vector2> clip)
    {
        var subjLoop = BuildLoop(subj);
        var clipLoop = BuildLoop(clip);

        // cartesian product of edges (slow but clear)
        foreach (var s in subjLoop.ToList())          // copy: ToList() freezes enumeration
        {
            var sN = s.Next;
            foreach (var c in clipLoop.ToList())
            {
                var cN = c.Next;

                if (TrySegmentIntersection(s.P, sN.P, c.P, cN.P,
                                            out var ip, out var tS, out var tC))
                {
                    // create nodes
                    var sI = new Node { P = ip, IsIntersection = true };
                    var cI = new Node { P = ip, IsIntersection = true };

                    // splice into respective edges (order by |p-a| – t values are monotone)
                    InsertOnEdge(s, sN, sI);
                    InsertOnEdge(c, cN, cI);

                    // link twins
                    sI.Twin = cI;
                    cI.Twin = sI;
                }
            }
        }

        return (subjLoop, clipLoop);
    }

    private static List<Node> BuildLoop(IReadOnlyList<Vector2> poly)
    {
        var nodes = poly.Select(p => new Node { P = p }).ToList();
        for (int i = 0; i < nodes.Count; ++i)
        {
            nodes[i].Next = nodes[(i + 1) % nodes.Count];
            nodes[i].Prev = nodes[(i - 1 + nodes.Count) % nodes.Count];
        }
        return nodes;
    }

    private static void InsertOnEdge(Node a, Node b, Node insert)
    {
        // edges are walked left-to-right once; distance from 'a' is monotone
        var curr = a;
        while (curr.Next != b &&
               (curr.Next.P - a.P).LengthSquared() < (insert.P - a.P).LengthSquared())
        {
            curr = curr.Next;
        }

        insert.Next = curr.Next;
        insert.Prev = curr;

        curr.Next.Prev = insert;
        curr.Next = insert;
    }

    private static void ClassifyIntersections(IEnumerable<Node> subjLoop, IReadOnlyList<Vector2> clipPoly)
    {
        foreach (var n in subjLoop.Where(x => x.IsIntersection))
        {
            var tinyStep = (n.Next.P - n.P).Normalize * 1e-3f;
            var inside = PointInPolygon(clipPoly, n.P + tinyStep);

            // for union: entering when we were *outside* and go *inside*
            n.Entry = !inside;
            n.Twin.Entry = inside;            // opposite for the other polygon
        }
    }

    private static List<Vector2> WalkUnion(List<Node> subj, List<Node> clip)
    {
        // handle the easy no-intersection cases up-front
        if (!subj.Any(n => n.IsIntersection))
            return Math.Abs(PolygonArea(subj)) >= Math.Abs(PolygonArea(clip))
                   ? subj.Select(n => n.P).ToList()
                   : clip.Select(n => n.P).ToList();

        var result = new List<Vector2>();
        var current = subj.First(n => n.IsIntersection && !n.Visited);

        bool inSubj = true;

        do
        {
            current.Visited = true;
            result.Add(current.P);

            // at an intersection: decide whether to swap loops
            if (current.IsIntersection && current.Entry == inSubj)
            {
                current = current.Twin;          // jump across
                inSubj = !inSubj;
            }

            current = current.Next;              // advance in the current loop
        }
        while (!current.Visited);

        return result;
    }

    /* ───────────────────────── SMALL GEOMETRIC HELPERS ───────────────────────── */

    public static bool PointInPolygon(IReadOnlyList<Vector2> poly, Vector2 p)
    {
        bool inside = false;
        for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
        {
            var pi = poly[i];
            var pj = poly[j];

            if (((pi.Y > p.Y) != (pj.Y > p.Y)) &&
                 p.X < (pj.X - pi.X) * (p.Y - pi.Y) / (pj.Y - pi.Y) + pi.X)
                inside = !inside;
        }
        return inside;
    }

    public static bool TrySegmentIntersection(
        Vector2 p, Vector2 p2,
        Vector2 q, Vector2 q2,
        out Vector2 ip, out float tP, out float tQ)
    {
        ip = default; tP = tQ = 0;

        var r = p2 - p;
        var s = q2 - q;
        var rxs = r.Cross(s);
        if (MathF.Abs(rxs) < 1e-8f) return false;           // parallel or collinear

        var qp = q - p;
        tP = qp.Cross(s) / rxs;
        tQ = qp.Cross(r) / rxs;

        if (tP is >= 0 and <= 1 && tQ is >= 0 and <= 1)
        {
            ip = p + tP * r;
            return true;
        }
        return false;
    }

    public static float PolygonArea(IEnumerable<Node> loop)
        => PolygonArea(loop.Select(n => n.P).ToList());

    public static float PolygonArea(IReadOnlyList<Vector2> poly)
    {
        float a = 0;
        for (int i = 0; i < poly.Count; ++i)
        {
            var p = poly[i];
            var q = poly[(i + 1) % poly.Count];
            a += p.X * q.Y - q.X * p.Y;
        }
        return 0.5f * a;
    }

    /* ───────────────────────── EXTENSION METHODS ───────────────────────── */

    private static Vector2 Perp(this Vector2 v) => new(-v.Y, v.X);
    private static float Cross(this Vector2 a, Vector2 b)
        => a.X * b.Y - a.Y * b.X;
}
