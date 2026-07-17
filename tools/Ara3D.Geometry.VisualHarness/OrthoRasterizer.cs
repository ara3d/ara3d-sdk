namespace Ara3D.Geometry.VisualHarness;

public enum ViewKind { Top, Front, Side }

/// <summary>
/// Tiny deterministic orthographic software rasterizer over SDK math. Consumes a
/// <see cref="ColoredTriangleMesh3D"/> so it honors per-vertex/per-instance color (ara3d-036 requirement:
/// many demos put their whole signal in hue, e.g. red defect markers among blue). Z-buffered, flat-lit.
/// </summary>
public static class OrthoRasterizer
{
    private const double Margin = 16.0;

    // Fixed light direction (world), normalized.
    private static readonly (double X, double Y, double Z) Light = Normalize(0.4, 0.5, 0.85);

    public static byte[] Render(ColoredTriangleMesh3D cm, ViewKind view, int width, int height)
    {
        var pts = cm.Mesh.Points;
        var faces = cm.Mesh.FaceIndices;
        var vc = cm.VertexColors;
        var n = pts.Count;

        var wx = new double[n];
        var wy = new double[n];
        var wz = new double[n];
        var su = new double[n];
        var sv = new double[n];
        var depth = new double[n];
        for (var i = 0; i < n; i++)
        {
            double x = pts[i].X, y = pts[i].Y, z = pts[i].Z;
            wx[i] = x; wy[i] = y; wz[i] = z;
            switch (view)
            {
                case ViewKind.Top:   su[i] = x; sv[i] = y; depth[i] = z; break;   // camera +Z looking down
                case ViewKind.Front: su[i] = x; sv[i] = z; depth[i] = -y; break;  // camera -Y
                default:             su[i] = y; sv[i] = z; depth[i] = x; break;   // camera +X (Side)
            }
        }

        var (uMin, uMax) = Range(su);
        var (vMin, vMax) = Range(sv);
        var uRange = Math.Max(uMax - uMin, 1e-9);
        var vRange = Math.Max(vMax - vMin, 1e-9);
        var scale = Math.Min((width - 2 * Margin) / uRange, (height - 2 * Margin) / vRange);
        var offX = (width - uRange * scale) / 2;
        var offY = (height - vRange * scale) / 2;

        var px = new double[n];
        var py = new double[n];
        for (var i = 0; i < n; i++)
        {
            px[i] = offX + (su[i] - uMin) * scale;
            py[i] = height - 1 - (offY + (sv[i] - vMin) * scale); // flip: +v is up
        }

        var fb = new byte[width * height * 3];
        for (var i = 0; i < fb.Length; i++) fb[i] = 255; // white background
        var zbuf = new double[width * height];
        for (var i = 0; i < zbuf.Length; i++) zbuf[i] = double.NegativeInfinity;

        for (var f = 0; f < faces.Count; f++)
        {
            int a = (int)faces[f].A, b = (int)faces[f].B, c = (int)faces[f].C;

            var intensity = FaceLight(wx, wy, wz, a, b, c);
            var (r0, g0, b0) = Color(vc, a);
            var (r1, g1, b1) = Color(vc, b);
            var (r2, g2, b2) = Color(vc, c);

            var area = Edge(px[a], py[a], px[b], py[b], px[c], py[c]);
            if (Math.Abs(area) < 1e-9) continue;

            var minX = (int)Math.Floor(Math.Min(px[a], Math.Min(px[b], px[c])));
            var maxX = (int)Math.Ceiling(Math.Max(px[a], Math.Max(px[b], px[c])));
            var minY = (int)Math.Floor(Math.Min(py[a], Math.Min(py[b], py[c])));
            var maxY = (int)Math.Ceiling(Math.Max(py[a], Math.Max(py[b], py[c])));
            minX = Math.Max(minX, 0); minY = Math.Max(minY, 0);
            maxX = Math.Min(maxX, width - 1); maxY = Math.Min(maxY, height - 1);

            for (var y = minY; y <= maxY; y++)
            for (var x = minX; x <= maxX; x++)
            {
                double sx = x + 0.5, sy = y + 0.5;
                var w0 = Edge(px[b], py[b], px[c], py[c], sx, sy) / area;
                var w1 = Edge(px[c], py[c], px[a], py[a], sx, sy) / area;
                var w2 = Edge(px[a], py[a], px[b], py[b], sx, sy) / area;
                if (w0 < 0 || w1 < 0 || w2 < 0) continue;

                var d = w0 * depth[a] + w1 * depth[b] + w2 * depth[c];
                var zi = y * width + x;
                if (d <= zbuf[zi]) continue;
                zbuf[zi] = d;

                var r = (w0 * r0 + w1 * r1 + w2 * r2) * intensity;
                var g = (w0 * g0 + w1 * g1 + w2 * g2) * intensity;
                var bl = (w0 * b0 + w1 * b1 + w2 * b2) * intensity;
                var o = zi * 3;
                fb[o] = ToByte(r);
                fb[o + 1] = ToByte(g);
                fb[o + 2] = ToByte(bl);
            }
        }

        return fb;
    }

    private static double FaceLight(double[] wx, double[] wy, double[] wz, int a, int b, int c)
    {
        double e1x = wx[b] - wx[a], e1y = wy[b] - wy[a], e1z = wz[b] - wz[a];
        double e2x = wx[c] - wx[a], e2y = wy[c] - wy[a], e2z = wz[c] - wz[a];
        var nx = e1y * e2z - e1z * e2y;
        var ny = e1z * e2x - e1x * e2z;
        var nz = e1x * e2y - e1y * e2x;
        var len = Math.Sqrt(nx * nx + ny * ny + nz * nz);
        if (len < 1e-12) return 0.6;
        var dot = Math.Abs((nx * Light.X + ny * Light.Y + nz * Light.Z) / len); // two-sided
        return 0.35 + 0.65 * dot;
    }

    private static (double, double, double) Color(IReadOnlyList<Vector3>? vc, int i)
        => vc == null ? (0.72, 0.72, 0.72) : (vc[i].X, vc[i].Y, vc[i].Z);

    private static double Edge(double ax, double ay, double bx, double by, double cx, double cy)
        => (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);

    private static (double, double) Range(double[] a)
    {
        double lo = double.MaxValue, hi = double.MinValue;
        foreach (var v in a) { if (v < lo) lo = v; if (v > hi) hi = v; }
        return (lo, hi);
    }

    private static (double X, double Y, double Z) Normalize(double x, double y, double z)
    {
        var len = Math.Sqrt(x * x + y * y + z * z);
        return (x / len, y / len, z / len);
    }

    private static byte ToByte(double v)
        => (byte)Math.Clamp((int)Math.Round(v * 255.0), 0, 255);
}
