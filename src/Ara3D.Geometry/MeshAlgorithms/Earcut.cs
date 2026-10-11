// Ported from mapbox/earcut 3.0.1 (src/earcut.js), under its licence:
//
// ISC License
//
// Copyright (c) 2024, Mapbox
//
// Permission to use, copy, modify, and/or distribute this software for any purpose
// with or without fee is hereby granted, provided that the above copyright notice
// and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
// REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
// INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS
// OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
// TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF
// THIS SOFTWARE.

namespace Ara3D.Geometry;

/// <summary>Polygon triangulation with holes by ear slicing, ported line by line from
/// mapbox/earcut 3.0.1 (the triangulator That Open's viewer uses for Fragments shell profiles),
/// with doubles for coordinates. The result is triangles as vertex indices into the input, with
/// no points added; either ring orientation is accepted, and each triangle of a simple polygon
/// winds counter-clockwise (positive signed area). Degenerate input gives fewer triangles, never
/// an exception. Its tests decide nothing against an absolute tolerance, so a profile measured
/// in millimetres or in kilometres triangulates the same way.</summary>
public static class Earcut
{
    /// <summary>Triangulates <paramref name="outer"/> less its <paramref name="holes"/>. A vertex
    /// index counts through the outer ring first, then each hole in order.</summary>
    public static List<Integer3> Triangulate(IReadOnlyList<Vector2> outer, IReadOnlyList<IReadOnlyList<Vector2>> holes)
    {
        var data = new List<double>(2 * (outer.Count + holes.Sum(h => h.Count)));
        var holeStarts = new List<int>(holes.Count);
        for (var r = 0; r <= holes.Count; r++)
        {
            if (r > 0)
                holeStarts.Add(data.Count / 2);
            foreach (var p in r == 0 ? outer : holes[r - 1])
            {
                data.Add(p.X.Value);
                data.Add(p.Y.Value);
            }
        }

        var indices = Triangulate(data, holeStarts);
        var triangles = new List<Integer3>(indices.Count / 3);
        for (var t = 0; t + 2 < indices.Count; t += 3)
            triangles.Add(new Integer3(indices[t], indices[t + 1], indices[t + 2]));
        return triangles;
    }

    private sealed class Node(int i, double x, double y)
    {
        public readonly int I = i;
        public readonly double X = x;
        public readonly double Y = y;
        public Node Prev = null!;
        public Node Next = null!;
        public int Z;
        public Node? PrevZ;
        public Node? NextZ;
        public bool Steiner;
    }

    /// <param name="data">x0, y0, x1, y1, ...</param>
    /// <param name="holeIndices">The vertex index (not coordinate index) where each hole starts.</param>
    public static List<int> Triangulate(IReadOnlyList<double> data, IReadOnlyList<int> holeIndices)
    {
        const int dim = 2;
        var triangles = new List<int>();
        var hasHoles = holeIndices.Count > 0;
        var outerLen = hasHoles ? holeIndices[0] * dim : data.Count;
        var outerNode = LinkedList(data, 0, outerLen, true);

        if (outerNode == null || outerNode.Next == outerNode.Prev)
            return triangles;

        double minX = 0, minY = 0, invSize = 0;

        if (hasHoles)
            outerNode = EliminateHoles(data, holeIndices, outerNode);

        // if the shape is not too simple, we'll use z-order curve hash later; calculate polygon bbox
        if (data.Count > 80 * dim)
        {
            minX = double.PositiveInfinity;
            minY = double.PositiveInfinity;
            var maxX = double.NegativeInfinity;
            var maxY = double.NegativeInfinity;

            for (var i = dim; i < outerLen; i += dim)
            {
                var x = data[i];
                var y = data[i + 1];
                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
            }

            // minX, minY and invSize are later used to transform coords into integers for z-order calculation
            invSize = Math.Max(maxX - minX, maxY - minY);
            invSize = invSize != 0 ? 32767 / invSize : 0;
        }

        EarcutLinked(outerNode, triangles, minX, minY, invSize, 0);
        return triangles;
    }

    // create a circular doubly linked list from polygon points in the specified winding order
    private static Node? LinkedList(IReadOnlyList<double> data, int start, int end, bool clockwise)
    {
        Node? last = null;

        if (clockwise == (SignedArea(data, start, end) > 0))
        {
            for (var i = start; i < end; i += 2)
                last = InsertNode(i / 2, data[i], data[i + 1], last);
        }
        else
        {
            for (var i = end - 2; i >= start; i -= 2)
                last = InsertNode(i / 2, data[i], data[i + 1], last);
        }

        if (last != null && EqualPoints(last, last.Next))
        {
            RemoveNode(last);
            last = last.Next;
        }

        return last;
    }

    // eliminate colinear or duplicate points
    private static Node? FilterPoints(Node? start, Node? end = null)
    {
        if (start == null)
            return start;
        end ??= start;

        var p = start;
        bool again;
        do
        {
            again = false;

            if (!p.Steiner && (EqualPoints(p, p.Next) || Area(p.Prev, p, p.Next) == 0))
            {
                RemoveNode(p);
                p = end = p.Prev;
                if (p == p.Next)
                    break;
                again = true;
            }
            else
            {
                p = p.Next;
            }
        } while (again || p != end);

        return end;
    }

    // main ear slicing loop which triangulates a polygon (given as a linked list)
    private static void EarcutLinked(Node? ear, List<int> triangles, double minX, double minY, double invSize, int pass)
    {
        if (ear == null)
            return;

        // interlink polygon nodes in z-order
        if (pass == 0 && invSize != 0)
            IndexCurve(ear, minX, minY, invSize);

        var stop = ear;

        // iterate through ears, slicing them one by one
        while (ear.Prev != ear.Next)
        {
            var prev = ear.Prev;
            var next = ear.Next;

            if (invSize != 0 ? IsEarHashed(ear, minX, minY, invSize) : IsEar(ear))
            {
                triangles.Add(prev.I); // cut off the triangle
                triangles.Add(ear.I);
                triangles.Add(next.I);

                RemoveNode(ear);

                // skipping the next vertex leads to less sliver triangles
                ear = next.Next;
                stop = next.Next;
                continue;
            }

            ear = next;

            // if we looped through the whole remaining polygon and can't find any more ears
            if (ear == stop)
            {
                // try filtering points and slicing again
                if (pass == 0)
                {
                    EarcutLinked(FilterPoints(ear), triangles, minX, minY, invSize, 1);
                }
                // if this didn't work, try curing all small self-intersections locally
                else if (pass == 1)
                {
                    var cured = CureLocalIntersections(FilterPoints(ear)!, triangles);
                    EarcutLinked(cured, triangles, minX, minY, invSize, 2);
                }
                // as a last resort, try splitting the remaining polygon into two
                else if (pass == 2)
                {
                    SplitEarcut(ear, triangles, minX, minY, invSize);
                }

                break;
            }
        }
    }

    // check whether a polygon node forms a valid ear with adjacent nodes
    private static bool IsEar(Node ear)
    {
        var a = ear.Prev;
        var b = ear;
        var c = ear.Next;

        if (Area(a, b, c) >= 0)
            return false; // reflex, can't be an ear

        // now make sure we don't have other points inside the potential ear
        double ax = a.X, bx = b.X, cx = c.X, ay = a.Y, by = b.Y, cy = c.Y;

        // triangle bbox
        double x0 = Math.Min(ax, Math.Min(bx, cx)), y0 = Math.Min(ay, Math.Min(by, cy));
        double x1 = Math.Max(ax, Math.Max(bx, cx)), y1 = Math.Max(ay, Math.Max(by, cy));

        var p = c.Next;
        while (p != a)
        {
            if (p.X >= x0 && p.X <= x1 && p.Y >= y0 && p.Y <= y1 &&
                PointInTriangleExceptFirst(ax, ay, bx, by, cx, cy, p.X, p.Y) &&
                Area(p.Prev, p, p.Next) >= 0)
                return false;
            p = p.Next;
        }

        return true;
    }

    private static bool IsEarHashed(Node ear, double minX, double minY, double invSize)
    {
        var a = ear.Prev;
        var b = ear;
        var c = ear.Next;

        if (Area(a, b, c) >= 0)
            return false; // reflex, can't be an ear

        double ax = a.X, bx = b.X, cx = c.X, ay = a.Y, by = b.Y, cy = c.Y;

        // triangle bbox
        double x0 = Math.Min(ax, Math.Min(bx, cx)), y0 = Math.Min(ay, Math.Min(by, cy));
        double x1 = Math.Max(ax, Math.Max(bx, cx)), y1 = Math.Max(ay, Math.Max(by, cy));

        // z-order range for the current triangle bbox;
        var minZ = ZOrder(x0, y0, minX, minY, invSize);
        var maxZ = ZOrder(x1, y1, minX, minY, invSize);

        var p = ear.PrevZ;
        var n = ear.NextZ;

        bool Inside(Node q) =>
            q.X >= x0 && q.X <= x1 && q.Y >= y0 && q.Y <= y1 && q != a && q != c &&
            PointInTriangleExceptFirst(ax, ay, bx, by, cx, cy, q.X, q.Y) && Area(q.Prev, q, q.Next) >= 0;

        // look for points inside the triangle in both directions
        while (p != null && p.Z >= minZ && n != null && n.Z <= maxZ)
        {
            if (Inside(p)) return false;
            p = p.PrevZ;
            if (Inside(n)) return false;
            n = n.NextZ;
        }

        // look for remaining points in decreasing z-order
        while (p != null && p.Z >= minZ)
        {
            if (Inside(p)) return false;
            p = p.PrevZ;
        }

        // look for remaining points in increasing z-order
        while (n != null && n.Z <= maxZ)
        {
            if (Inside(n)) return false;
            n = n.NextZ;
        }

        return true;
    }

    // go through all polygon nodes and cure small local self-intersections
    private static Node? CureLocalIntersections(Node start, List<int> triangles)
    {
        var p = start;
        do
        {
            var a = p.Prev;
            var b = p.Next.Next;

            if (!EqualPoints(a, b) && Intersects(a, p, p.Next, b) && LocallyInside(a, b) && LocallyInside(b, a))
            {
                triangles.Add(a.I);
                triangles.Add(p.I);
                triangles.Add(b.I);

                // remove two nodes involved
                RemoveNode(p);
                RemoveNode(p.Next);

                p = start = b;
            }
            p = p.Next;
        } while (p != start);

        return FilterPoints(p);
    }

    // try splitting polygon into two and triangulate them independently
    private static void SplitEarcut(Node start, List<int> triangles, double minX, double minY, double invSize)
    {
        // look for a valid diagonal that divides the polygon into two
        var a = start;
        do
        {
            var b = a.Next.Next;
            while (b != a.Prev)
            {
                if (a.I != b.I && IsValidDiagonal(a, b))
                {
                    // split the polygon in two by the diagonal
                    var c = SplitPolygon(a, b);

                    // filter colinear points around the cuts
                    var a2 = FilterPoints(a, a.Next);
                    c = FilterPoints(c, c.Next)!;

                    // run earcut on each half
                    EarcutLinked(a2, triangles, minX, minY, invSize, 0);
                    EarcutLinked(c, triangles, minX, minY, invSize, 0);
                    return;
                }
                b = b.Next;
            }
            a = a.Next;
        } while (a != start);
    }

    // link every hole into the outer loop, producing a single-ring polygon without holes
    private static Node EliminateHoles(IReadOnlyList<double> data, IReadOnlyList<int> holeIndices, Node outerNode)
    {
        var queue = new List<Node>();

        for (int i = 0, len = holeIndices.Count; i < len; i++)
        {
            var start = holeIndices[i] * 2;
            var end = i < len - 1 ? holeIndices[i + 1] * 2 : data.Count;
            var list = LinkedList(data, start, end, false);
            if (list == null)
                continue;
            if (list == list.Next)
                list.Steiner = true;
            queue.Add(GetLeftmost(list));
        }

        queue.Sort(CompareXYSlope);

        // process holes from left to right
        foreach (var hole in queue)
            outerNode = EliminateHole(hole, outerNode);

        return outerNode;
    }

    private static int CompareXYSlope(Node a, Node b)
    {
        var result = a.X - b.X;
        // when the left-most point of 2 holes meet at a vertex, sort the holes counterclockwise so that when we find
        // the bridge to the outer shell is always the point that they meet at.
        if (result == 0)
        {
            result = a.Y - b.Y;
            if (result == 0)
            {
                var aSlope = (a.Next.Y - a.Y) / (a.Next.X - a.X);
                var bSlope = (b.Next.Y - b.Y) / (b.Next.X - b.X);
                result = aSlope - bSlope;
            }
        }
        return double.IsNaN(result) ? 0 : Math.Sign(result);
    }

    // find a bridge between vertices that connects hole with an outer ring and and link it
    private static Node EliminateHole(Node hole, Node outerNode)
    {
        var bridge = FindHoleBridge(hole, outerNode);
        if (bridge == null)
            return outerNode;

        var bridgeReverse = SplitPolygon(bridge, hole);

        // filter collinear points around the cuts
        FilterPoints(bridgeReverse, bridgeReverse.Next);
        return FilterPoints(bridge, bridge.Next)!;
    }

    // David Eberly's algorithm for finding a bridge between hole and outer polygon
    private static Node? FindHoleBridge(Node hole, Node outerNode)
    {
        var p = outerNode;
        var hx = hole.X;
        var hy = hole.Y;
        var qx = double.NegativeInfinity;
        Node? m = null;

        // find a segment intersected by a ray from the hole's leftmost point to the left;
        // segment's endpoint with lesser x will be potential connection point
        // unless they intersect at a vertex, then choose the vertex
        if (EqualPoints(hole, p))
            return p;
        do
        {
            if (EqualPoints(hole, p.Next))
                return p.Next;
            if (hy <= p.Y && hy >= p.Next.Y && p.Next.Y != p.Y)
            {
                var x = p.X + (hy - p.Y) * (p.Next.X - p.X) / (p.Next.Y - p.Y);
                if (x <= hx && x > qx)
                {
                    qx = x;
                    m = p.X < p.Next.X ? p : p.Next;
                    if (x == hx)
                        return m; // hole touches outer segment; pick leftmost endpoint
                }
            }
            p = p.Next;
        } while (p != outerNode);

        if (m == null)
            return null;

        // look for points inside the triangle of hole point, segment intersection and endpoint;
        // if there are no points found, we have a valid connection;
        // otherwise choose the point of the minimum angle with the ray as connection point

        var stop = m;
        var mx = m.X;
        var my = m.Y;
        var tanMin = double.PositiveInfinity;

        p = m;

        do
        {
            if (hx >= p.X && p.X >= mx && hx != p.X &&
                PointInTriangle(hy < my ? hx : qx, hy, mx, my, hy < my ? qx : hx, hy, p.X, p.Y))
            {
                var tan = Math.Abs(hy - p.Y) / (hx - p.X); // tangential

                if (LocallyInside(p, hole) &&
                    (tan < tanMin || (tan == tanMin && (p.X > m.X || (p.X == m.X && SectorContainsSector(m, p))))))
                {
                    m = p;
                    tanMin = tan;
                }
            }

            p = p.Next;
        } while (p != stop);

        return m;
    }

    // whether sector in vertex m contains sector in vertex p in the same coordinates
    private static bool SectorContainsSector(Node m, Node p)
        => Area(m.Prev, m, p.Prev) < 0 && Area(p.Next, m, m.Next) < 0;

    // interlink polygon nodes in z-order
    private static void IndexCurve(Node start, double minX, double minY, double invSize)
    {
        var p = start;
        do
        {
            if (p.Z == 0)
                p.Z = ZOrder(p.X, p.Y, minX, minY, invSize);
            p.PrevZ = p.Prev;
            p.NextZ = p.Next;
            p = p.Next;
        } while (p != start);

        p.PrevZ!.NextZ = null;
        p.PrevZ = null;

        SortLinked(p);
    }

    // Simon Tatham's linked list merge sort algorithm
    // http://www.chiark.greenend.org.uk/~sgtatham/algorithms/listsort.html
    private static Node? SortLinked(Node? list)
    {
        int numMerges;
        var inSize = 1;

        do
        {
            var p = list;
            list = null;
            Node? tail = null;
            numMerges = 0;

            while (p != null)
            {
                numMerges++;
                var q = p;
                var pSize = 0;
                for (var i = 0; i < inSize; i++)
                {
                    pSize++;
                    q = q.NextZ;
                    if (q == null)
                        break;
                }
                var qSize = inSize;

                while (pSize > 0 || (qSize > 0 && q != null))
                {
                    Node e;
                    if (pSize != 0 && (qSize == 0 || q == null || p!.Z <= q.Z))
                    {
                        e = p!;
                        p = p!.NextZ;
                        pSize--;
                    }
                    else
                    {
                        e = q!;
                        q = q!.NextZ;
                        qSize--;
                    }

                    if (tail != null)
                        tail.NextZ = e;
                    else
                        list = e;

                    e.PrevZ = tail;
                    tail = e;
                }

                p = q;
            }

            tail!.NextZ = null;
            inSize *= 2;
        } while (numMerges > 1);

        return list;
    }

    // z-order of a point given coords and inverse of the longer side of data bbox
    private static int ZOrder(double xd, double yd, double minX, double minY, double invSize)
    {
        // coords are transformed into non-negative 15-bit integer range
        var x = (int)((xd - minX) * invSize);
        var y = (int)((yd - minY) * invSize);

        x = (x | (x << 8)) & 0x00FF00FF;
        x = (x | (x << 4)) & 0x0F0F0F0F;
        x = (x | (x << 2)) & 0x33333333;
        x = (x | (x << 1)) & 0x55555555;

        y = (y | (y << 8)) & 0x00FF00FF;
        y = (y | (y << 4)) & 0x0F0F0F0F;
        y = (y | (y << 2)) & 0x33333333;
        y = (y | (y << 1)) & 0x55555555;

        return x | (y << 1);
    }

    // find the leftmost node of a polygon ring
    private static Node GetLeftmost(Node start)
    {
        var p = start;
        var leftmost = start;
        do
        {
            if (p.X < leftmost.X || (p.X == leftmost.X && p.Y < leftmost.Y))
                leftmost = p;
            p = p.Next;
        } while (p != start);

        return leftmost;
    }

    // check if a point lies within a convex triangle
    private static bool PointInTriangle(double ax, double ay, double bx, double by, double cx, double cy, double px, double py)
        => (cx - px) * (ay - py) >= (ax - px) * (cy - py) &&
           (ax - px) * (by - py) >= (bx - px) * (ay - py) &&
           (bx - px) * (cy - py) >= (cx - px) * (by - py);

    // check if a point lies within a convex triangle but false if its equal to the first point of the triangle
    private static bool PointInTriangleExceptFirst(double ax, double ay, double bx, double by, double cx, double cy, double px, double py)
        => !(ax == px && ay == py) && PointInTriangle(ax, ay, bx, by, cx, cy, px, py);

    // check if a diagonal between two polygon nodes is valid (lies in polygon interior)
    private static bool IsValidDiagonal(Node a, Node b)
        => a.Next.I != b.I && a.Prev.I != b.I && !IntersectsPolygon(a, b) && // doesn't intersect other edges
           ((LocallyInside(a, b) && LocallyInside(b, a) && MiddleInside(a, b) && // locally visible
             (Area(a.Prev, a, b.Prev) != 0 || Area(a, b.Prev, b) != 0)) || // does not create opposite-facing sectors
            (EqualPoints(a, b) && Area(a.Prev, a, a.Next) > 0 && Area(b.Prev, b, b.Next) > 0)); // special zero-length case

    // signed area of a triangle
    private static double Area(Node p, Node q, Node r)
        => (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

    // check if two points are equal
    private static bool EqualPoints(Node p1, Node p2)
        => p1.X == p2.X && p1.Y == p2.Y;

    // check if two segments intersect
    private static bool Intersects(Node p1, Node q1, Node p2, Node q2)
    {
        var o1 = Math.Sign(Area(p1, q1, p2));
        var o2 = Math.Sign(Area(p1, q1, q2));
        var o3 = Math.Sign(Area(p2, q2, p1));
        var o4 = Math.Sign(Area(p2, q2, q1));

        if (o1 != o2 && o3 != o4) return true; // general case

        if (o1 == 0 && OnSegment(p1, p2, q1)) return true; // p1, q1 and p2 are collinear and p2 lies on p1q1
        if (o2 == 0 && OnSegment(p1, q2, q1)) return true; // p1, q1 and q2 are collinear and q2 lies on p1q1
        if (o3 == 0 && OnSegment(p2, p1, q2)) return true; // p2, q2 and p1 are collinear and p1 lies on p2q2
        if (o4 == 0 && OnSegment(p2, q1, q2)) return true; // p2, q2 and q1 are collinear and q1 lies on p2q2

        return false;
    }

    // for collinear points p, q, r, check if point q lies on segment pr
    private static bool OnSegment(Node p, Node q, Node r)
        => q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) && q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y);

    // check if a polygon diagonal intersects any polygon segments
    private static bool IntersectsPolygon(Node a, Node b)
    {
        var p = a;
        do
        {
            if (p.I != a.I && p.Next.I != a.I && p.I != b.I && p.Next.I != b.I && Intersects(p, p.Next, a, b))
                return true;
            p = p.Next;
        } while (p != a);

        return false;
    }

    // check if a polygon diagonal is locally inside the polygon
    private static bool LocallyInside(Node a, Node b)
        => Area(a.Prev, a, a.Next) < 0
            ? Area(a, b, a.Next) >= 0 && Area(a, a.Prev, b) >= 0
            : Area(a, b, a.Prev) < 0 || Area(a, a.Next, b) < 0;

    // check if the middle point of a polygon diagonal is inside the polygon
    private static bool MiddleInside(Node a, Node b)
    {
        var p = a;
        var inside = false;
        var px = (a.X + b.X) / 2;
        var py = (a.Y + b.Y) / 2;
        do
        {
            if (((p.Y > py) != (p.Next.Y > py)) && p.Next.Y != p.Y &&
                (px < (p.Next.X - p.X) * (py - p.Y) / (p.Next.Y - p.Y) + p.X))
                inside = !inside;
            p = p.Next;
        } while (p != a);

        return inside;
    }

    // link two polygon vertices with a bridge; if the vertices belong to the same ring, it splits polygon into two;
    // if one belongs to the outer ring and another to a hole, it merges it into a single ring
    private static Node SplitPolygon(Node a, Node b)
    {
        var a2 = new Node(a.I, a.X, a.Y);
        var b2 = new Node(b.I, b.X, b.Y);
        var an = a.Next;
        var bp = b.Prev;

        a.Next = b;
        b.Prev = a;

        a2.Next = an;
        an.Prev = a2;

        b2.Next = a2;
        a2.Prev = b2;

        bp.Next = b2;
        b2.Prev = bp;

        return b2;
    }

    // create a node and optionally link it with previous one (in a circular doubly linked list)
    private static Node InsertNode(int i, double x, double y, Node? last)
    {
        var p = new Node(i, x, y);

        if (last == null)
        {
            p.Prev = p;
            p.Next = p;
        }
        else
        {
            p.Next = last.Next;
            p.Prev = last;
            last.Next.Prev = p;
            last.Next = p;
        }
        return p;
    }

    private static void RemoveNode(Node p)
    {
        p.Next.Prev = p.Prev;
        p.Prev.Next = p.Next;

        if (p.PrevZ != null) p.PrevZ.NextZ = p.NextZ;
        if (p.NextZ != null) p.NextZ.PrevZ = p.PrevZ;
    }

    private static double SignedArea(IReadOnlyList<double> data, int start, int end)
    {
        double sum = 0;
        for (int i = start, j = end - 2; i < end; i += 2)
        {
            sum += (data[j] - data[i]) * (data[i + 1] + data[j + 1]);
            j = i;
        }
        return sum;
    }
}
