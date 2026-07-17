namespace Ara3D.Geometry;

/// <summary>
/// Blue-noise point sampling: points that stay a minimum distance apart yet pack tightly,
/// giving even coverage with no visible structure (unlike the clumps and gaps of uniform random).
/// <see cref="Sample"/> generates a fresh field with Bridson's Poisson-disk algorithm;
/// <see cref="SelectSubset"/> thins an existing point set to the same spacing (blue noise as a filter);
/// <see cref="UniformRandom"/> is the white-noise counterpart kept here as the visual foil.
/// </summary>
public static class PoissonDiskSampling
{
    public const int DefaultAttempts = 30;

    // Deterministic SplitMix64 stream, so a seed reproduces the same field.
    private struct Rng
    {
        private ulong _state;
        public Rng(int seed) => _state = unchecked((ulong)seed * 2654435769UL + 0x9E3779B97F4A7C15UL);

        public double Next()
        {
            unchecked
            {
                _state += 0x9E3779B97F4A7C15UL;
                var z = _state;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                z ^= z >> 31;
                return (z >> 11) * (1.0 / (1UL << 53));
            }
        }

        public double Range(double lo, double hi)
            => lo + Next() * (hi - lo);
    }

    /// <summary>
    /// Bridson Poisson-disk sampling of the rectangle [0,width] × [0,height]. Every returned point is
    /// at least <paramref name="radius"/> from every other. A background grid of cell size
    /// radius/√2 holds at most one point per cell, so each distance test scans only nearby cells (O(n)).
    /// <paramref name="k"/> is the number of candidate darts thrown around each active point before it retires.
    /// </summary>
    public static IReadOnlyList<Vector2> Sample(double width, double height, double radius, int seed = 0, int k = DefaultAttempts)
    {
        if (radius <= 0) throw new ArgumentOutOfRangeException(nameof(radius));

        var cell = radius / Math.Sqrt(2.0);
        var gw = (int)Math.Floor(width / cell) + 1;
        var gh = (int)Math.Floor(height / cell) + 1;
        var grid = new int[gw * gh];
        for (var i = 0; i < grid.Length; i++)
            grid[i] = -1;

        var sx = new List<double>();
        var sy = new List<double>();
        var active = new List<int>();
        var rng = new Rng(seed);
        var r2 = radius * radius;

        void Emit(double x, double y)
        {
            var gx = Math.Min((int)(x / cell), gw - 1);
            var gy = Math.Min((int)(y / cell), gh - 1);
            grid[gy * gw + gx] = sx.Count;
            active.Add(sx.Count);
            sx.Add(x);
            sy.Add(y);
        }

        bool FarFromNeighbors(double x, double y)
        {
            var gx = (int)(x / cell);
            var gy = (int)(y / cell);
            for (var yy = Math.Max(gy - 2, 0); yy <= Math.Min(gy + 2, gh - 1); yy++)
            for (var xx = Math.Max(gx - 2, 0); xx <= Math.Min(gx + 2, gw - 1); xx++)
            {
                var s = grid[yy * gw + xx];
                if (s < 0) continue;
                var dx = sx[s] - x;
                var dy = sy[s] - y;
                if (dx * dx + dy * dy < r2) return false;
            }
            return true;
        }

        Emit(width / 2, height / 2);
        while (active.Count > 0)
        {
            var pick = (int)(rng.Next() * active.Count);
            var ax = sx[active[pick]];
            var ay = sy[active[pick]];
            var placed = false;
            for (var attempt = 0; attempt < k; attempt++)
            {
                // Uniform in the annulus [radius, 2·radius] around the active point.
                var angle = rng.Next() * Math.PI * 2;
                var dist = radius * Math.Sqrt(rng.Range(1.0, 4.0));
                var px = ax + Math.Cos(angle) * dist;
                var py = ay + Math.Sin(angle) * dist;
                if (px >= 0 && px < width && py >= 0 && py < height && FarFromNeighbors(px, py))
                {
                    Emit(px, py);
                    placed = true;
                    break;
                }
            }
            if (!placed)
                active.RemoveAt(pick);
        }

        var result = new Vector2[sx.Count];
        for (var i = 0; i < result.Length; i++)
            result[i] = new Vector2((float)sx[i], (float)sy[i]);
        return result;
    }

    /// <summary>Uniform-random ("white noise") points in [0,width] × [0,height] — the clumpy foil for blue noise.</summary>
    public static IReadOnlyList<Vector2> UniformRandom(double width, double height, int count, int seed = 0)
    {
        var rng = new Rng(seed);
        var result = new Vector2[count];
        for (var i = 0; i < count; i++)
            result[i] = new Vector2((float)(rng.Next() * width), (float)(rng.Next() * height));
        return result;
    }

    /// <summary>
    /// Greedy blue-noise thinning: visit <paramref name="points"/> in a seed-shuffled order and keep a
    /// point only when it is at least <paramref name="radius"/> from every already-kept point. Because it
    /// draws from real 3D positions it conforms to any surface exactly, with no ray casting or parameterization.
    /// </summary>
    public static IReadOnlyList<Vector3> SelectSubset(IReadOnlyList<Vector3> points, double radius, int seed = 0)
    {
        var n = points.Count;
        var xs = new double[n];
        var ys = new double[n];
        var zs = new double[n];
        var order = new int[n];
        for (var i = 0; i < n; i++)
        {
            xs[i] = points[i].X;
            ys[i] = points[i].Y;
            zs[i] = points[i].Z;
            order[i] = i;
        }

        var rng = new Rng(seed);
        for (var i = n - 1; i > 0; i--)
        {
            var j = (int)(rng.Next() * (i + 1));
            (order[i], order[j]) = (order[j], order[i]);
        }

        var r2 = radius * radius;
        var keptX = new List<double>();
        var keptY = new List<double>();
        var keptZ = new List<double>();
        for (var oi = 0; oi < n; oi++)
        {
            var i = order[oi];
            double x = xs[i], y = ys[i], z = zs[i];
            var ok = true;
            for (var j = 0; j < keptX.Count; j++)
            {
                var dx = keptX[j] - x;
                var dy = keptY[j] - y;
                var dz = keptZ[j] - z;
                if (dx * dx + dy * dy + dz * dz < r2) { ok = false; break; }
            }
            if (!ok) continue;
            keptX.Add(x);
            keptY.Add(y);
            keptZ.Add(z);
        }

        var result = new Vector3[keptX.Count];
        for (var i = 0; i < result.Length; i++)
            result[i] = new Vector3((float)keptX[i], (float)keptY[i], (float)keptZ[i]);
        return result;
    }
}
