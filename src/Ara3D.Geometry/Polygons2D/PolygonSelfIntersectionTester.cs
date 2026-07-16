namespace Ara3D.Geometry;

public static class PolygonSelfIntersectionTester
{
    public static (int i, int j)? HasSelfIntersectionsSweep(this IPolygon2D poly, bool includeTouching = true, float eps = PolygonOps.Eps)
    {
        // Build segments with indices (skip zero-length)
        int n = poly.GetNumPoints();
        (int i, int j) pair;
        var segs = new List<(int idx, Line2D e)>(n);
        for (int i = 0; i < n; i++)
        {
            var e = poly.GetEdge(i);
            if ((e.B - e.A).Length() > eps) segs.Add((i, e));
        }

        // Event structure: (x, y, isLeft, segIndex)
        var events = new List<(float x, float y, bool left, int idx, Line2D e)>(2 * segs.Count);
        foreach (var (idx, e) in segs)
        {
            // Left = smaller x; tie by y
            var left = e.A.X < e.B.X || (e.A.X == e.B.X && e.A.Y <= e.B.Y);
            var L = left ? e.A : e.B;
            var R = left ? e.B : e.A;
            events.Add((L.X, L.Y, true, idx, e));
            events.Add((R.X, R.Y, false, idx, e));
        }

        events.Sort((a, b) =>
        {
            int cx = a.x.CompareTo(b.x);
            if (cx != 0) return cx;
            int cy = a.y.CompareTo(b.y);
            if (cy != 0) return cy;
            // Process left before right at same coordinate
            return a.left == b.left ? 0 : (a.left ? -1 : 1);
        });

        // Sweep line state
        var state = new SweepState();

        bool Adjacent(int i, int j)
        {
            // Adjacent edges (including wrap-around) do not count as self-intersections
            if (i == j) return true;
            return (j == (i + 1).Mod(n)) || (i == (j + 1).Mod(n));
        }

        void CheckNeighbors(Line2D e, int idx)
        {
            // Check predecessor and successor in the sweep structure
            if (state.TryNeighbors(e, out var prev, out var next))
            {
                if (prev.HasValue)
                {
                    var (e2, idx2) = prev.Value;
                    if (!Adjacent(idx, idx2))
                    {
                        bool hit = includeTouching ? PolygonOps.LinesCrossOrTouch(e, e2) : PolygonOps.LinesCross(e, e2);
                        if (hit) { pair = (Math.Min(idx, idx2), Math.Max(idx, idx2)); return; }
                    }
                }
                if (next.HasValue)
                {
                    var (e3, idx3) = next.Value;
                    if (!Adjacent(idx, idx3))
                    {
                        bool hit = includeTouching ? PolygonOps.LinesCrossOrTouch(e, e3) : PolygonOps.LinesCross(e, e3);
                        if (hit) { pair = (Math.Min(idx, idx3), Math.Max(idx, idx3)); return; }
                    }
                }
            }
            // signal not found via sentinel
            pair = (-1, -1);
        }

        foreach (var ev in events)
        {
            state.SweepX = ev.x;

            if (ev.left)
            {
                state.Insert(ev.e, ev.idx);
                CheckNeighbors(ev.e, ev.idx);
                if (pair.i >= 0) return pair;
            }
            else
            {
                // On removal, neighbors of the removed edge may now intersect
                if (state.TryGetNeighborsAround(ev.e, out var leftN, out var rightN))
                {
                    state.Remove(ev.e);
                    if (leftN.HasValue && rightN.HasValue)
                    {
                        var (le, li) = leftN.Value;
                        var (re, ri) = rightN.Value;
                        if (!Adjacent(li, ri))
                        {
                            bool hit = includeTouching ? PolygonOps.LinesCrossOrTouch(le, re) : PolygonOps.LinesCross(le, re);
                            if (hit)
                            {
                                return (Math.Min(li, ri), Math.Max(li, ri)); 
                            }
                        }
                    }
                }
                else
                {
                    state.Remove(ev.e);
                }
            }
        }

        return null;
    }

    // Minimal sweep structure with y-order at current SweepX.
    private sealed class SweepState
    {
        // The comparer uses this to evaluate segment y at sweepX.
        public float SweepX;

        private readonly SortedSet<(Line2D e, int idx)> _set;
        private readonly SegmentComparer _cmp;

        public SweepState()
        {
            _cmp = new SegmentComparer(() => SweepX);
            _set = new SortedSet<(Line2D e, int idx)>(_cmp);
        }

        public void Insert(Line2D e, int idx) => _set.Add((e, idx));
        public void Remove(Line2D e) => _set.RemoveWhere(t => ReferenceEquals(t.e, e) || (t.e.A.Equals(e.A) && t.e.B.Equals(e.B)));

        public bool TryNeighbors(Line2D e, out (Line2D e, int idx)? prev, out (Line2D e, int idx)? next)
        {
            prev = next = null;
            if (!_set.TryGetValue((e, -1), out var cur)) return false;
            var p = _set.GetViewBetween(default, cur).Count > 0 ? _set.GetViewBetween(default, cur).Max : ((Line2D, int)?)null;
            var n = _set.GetViewBetween(cur, default).Count > 0 ? _set.GetViewBetween(cur, default).Min : ((Line2D, int)?)null;
            if (p.HasValue && !(p.Value.Item1.A.Equals(cur.e.A) && p.Value.Item1.B.Equals(cur.e.B))) prev = p.Value;
            if (n.HasValue && !(n.Value.Item1.A.Equals(cur.e.A) && n.Value.Item1.B.Equals(cur.e.B))) next = n.Value;
            return true;
        }

        public bool TryGetNeighborsAround(Line2D e, out (Line2D e, int idx)? left, out (Line2D e, int idx)? right)
        {
            left = right = null;
            if (!_set.TryGetValue((e, -1), out var cur)) return false;

            // predecessor
            var head = _set.GetViewBetween(default, cur);
            if (head.Count > 0) left = head.Max;

            // successor
            var tail = _set.GetViewBetween(cur, default);
            if (tail.Count > 0) right = tail.Min;

            return true;
        }

        private sealed class SegmentComparer : IComparer<(Line2D e, int idx)>
        {
            private readonly Func<float> _getX;
            public SegmentComparer(Func<float> getX) => _getX = getX;

            public int Compare((Line2D e, int idx) a, (Line2D e, int idx) b)
            {
                if (ReferenceEquals(a.e, b.e)) return 0;
                var x = _getX();

                var ya = YAt(a.e, x);
                var yb = YAt(b.e, x);
                int cy = ya.CompareTo(yb);
                if (cy != 0) return cy;

                // Tie-breakers to keep ordering strict and stable
                int ax = a.e.A.X.CompareTo(b.e.A.X);
                if (ax != 0) return ax;
                int ay = a.e.A.Y.CompareTo(b.e.A.Y);
                if (ay != 0) return ay;
                int bx = a.e.B.X.CompareTo(b.e.B.X);
                if (bx != 0) return bx;
                int by = a.e.B.Y.CompareTo(b.e.B.Y);
                if (by != 0) return by;

                return a.idx.CompareTo(b.idx);
            }

            private static float YAt(Line2D e, float x)
            {
                // Vertical segment: return lower y at sweep x
                var dx = e.B.X - e.A.X;
                if (MathF.Abs(dx) < PolygonOps.Eps) return MathF.Min(e.A.Y, e.B.Y);
                var t = (x - e.A.X) / dx;
                t = Math.Clamp(t, 0f, 1f);
                return e.A.Y + t * (e.B.Y - e.A.Y);
            }
        }
    }

}