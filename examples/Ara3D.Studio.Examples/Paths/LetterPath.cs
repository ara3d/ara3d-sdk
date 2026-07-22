using System.Reflection;

namespace Ara3D.Studio.Samples.Paths;

// This is a simple path builder inspired by the Path element of SVG
public interface IPathBuilder
{
    float X { get; }
    float Y { get; }
    IPathBuilder MoveTo(float x, float y);
    IPathBuilder LineTo(float x, float y);
    IPathBuilder BezierCubic(float x1, float y1, float x2, float y2, float x, float y);
    IPathBuilder BezierQuadratic(float x1, float y1, float x, float y);
    IPathBuilder ClosePath();
}

// Extensions 
public static class PathBuilderExtension
{
    public static IPathBuilder M(this IPathBuilder self, float x, float y) => self.MoveTo(x, y);
    public static IPathBuilder L(this IPathBuilder self, float x, float y) => self.LineTo(x, y);
    public static IPathBuilder Z(this IPathBuilder self) => self.ClosePath();
    public static IPathBuilder C(this IPathBuilder self, float x1, float y1, float x2, float y2, float x, float y) => self.BezierCubic(x1, y1, x2, y2, x, y);
    public static IPathBuilder Q(this IPathBuilder self, float x1, float y1, float x, float y) => self.BezierQuadratic(x1, y1, x, y);
    public static IPathBuilder H(this IPathBuilder self, float x) => self.LineHorizontalTo(x);
    public static IPathBuilder V(this IPathBuilder self, float y) => self.LineVerticalTo(y);
    public static IPathBuilder MoveOffset(this IPathBuilder self, float x, float y) => self.MoveTo(self.X + x, self.Y + y);
    public static IPathBuilder LineOffset(this IPathBuilder self, float x, float y) => self.LineTo(self.X + x, self.Y + y);
    public static IPathBuilder LineHorizontalTo(this IPathBuilder self, float x) => self.LineTo(x, self.Y);
    public static IPathBuilder LineVerticalTo(this IPathBuilder self, float y) => self.LineTo(0, y);
}

public class PathBuilder : IPathBuilder
{
    public PathBuilder(int samples = 16)
    {
        Samples = samples;
    }

    public float X { get; private set; }
    public float Y { get; private set; }

    public Point2D Current => (X, Y);

    public List<Point2D> Points { get; } = new();
    public List<Integer2> LineIndices { get; } = new();

    public int Samples { get; } 

    // Track whether we have a current point and where the current subpath started
    private bool _hasCurrent;
    private int _subpathStartIndex = -1;
    private int _currentIndex = -1;

    public IPathBuilder MoveTo(float x, float y)
    {
        X = x;
        Y = y;

        // Start a new subpath.
        // We record the point, but do NOT add a line segment.
        _currentIndex = AddPointIfNeeded((x, y));
        _subpathStartIndex = _currentIndex;
        _hasCurrent = true;

        return this;
    }

    public IPathBuilder LineTo(float x, float y)
    {
        EnsureHasCurrent();

        var fromIndex = _currentIndex;
        var toIndex = AddPointIfNeeded((x, y));

        AddLine(fromIndex, toIndex);

        X = x;
        Y = y;
        _currentIndex = toIndex;

        return this;
    }

    public IPathBuilder BezierCubic(float x1, float y1, float x2, float y2, float x, float y)
    {
        EnsureHasCurrent();

        var p0 = Current;
        var p1 = (Point2D)(x1, y1);
        var p2 = (Point2D)(x2, y2);
        var p3 = (Point2D)(x, y);

        // Sample the curve into line segments.
        // Start at i=1 because the curve begins at the current point already.
        for (int i = 1; i <= Samples; i++)
        {
            float t = (float)i / Samples;
            var pt = Cubic(p0, p1, p2, p3, t);
            LineTo(pt.X, pt.Y);
        }

        return this;
    }

    public IPathBuilder BezierQuadratic(float x1, float y1, float x, float y)
    {
        EnsureHasCurrent();

        var p0 = Current;
        var p1 = (Point2D)(x1, y1);
        var p2 = (Point2D)(x, y);

        for (int i = 1; i <= Samples; i++)
        {
            float t = (float)i / Samples;
            var pt = Quadratic(p0, p1, p2, t);
            LineTo(pt.X, pt.Y);
        }

        return this;
    }

    public IPathBuilder ClosePath()
    {
        EnsureHasCurrent();

        // If we have a subpath start and we're not already there, connect back.
        if (_subpathStartIndex >= 0 && _currentIndex >= 0 && _currentIndex != _subpathStartIndex)
        {
            AddLine(_currentIndex, _subpathStartIndex);

            // SVG-like behavior: current point becomes the subpath start point.
            var start = Points[_subpathStartIndex];
            X = start.X;
            Y = start.Y;
            _currentIndex = _subpathStartIndex;
        }

        return this;
    }

    // -----------------------
    // Helpers
    // -----------------------

    private void EnsureHasCurrent()
    {
        if (!_hasCurrent)
            throw new InvalidOperationException("Path has no current point. Call MoveTo() before drawing.");
    }

    private int AddPointIfNeeded(Point2D p)
    {
        // If last point is identical, reuse it to avoid zero-length segments.
        if (Points.Count > 0)
        {
            var last = Points[^1];
            if (last.X == p.X && last.Y == p.Y)
                return Points.Count - 1;
        }

        Points.Add(p);
        return Points.Count - 1;
    }

    private void AddLine(int a, int b)
    {
        if (a == b) return; // ignore degenerate segments
        LineIndices.Add((a, b));
    }

    private static Point2D Quadratic(Point2D p0, Point2D p1, Point2D p2, float t)
    {
        float u = 1f - t;
        float x = (u * u) * p0.X + (2f * u * t) * p1.X + (t * t) * p2.X;
        float y = (u * u) * p0.Y + (2f * u * t) * p1.Y + (t * t) * p2.Y;
        return (x, y);
    }

    private static Point2D Cubic(Point2D p0, Point2D p1, Point2D p2, Point2D p3, float t)
    {
        float u = 1f - t;
        float uu = u * u;
        float tt = t * t;

        float uuu = uu * u;
        float ttt = tt * t;

        float x =
            (uuu) * p0.X +
            (3f * uu * t) * p1.X +
            (3f * u * tt) * p2.X +
            (ttt) * p3.X;

        float y =
            (uuu) * p0.Y +
            (3f * uu * t) * p1.Y +
            (3f * u * tt) * p2.Y +
            (ttt) * p3.Y;

        return (x, y);
    }
}

public static class PathBuilderLetters
{
    // Common “design” constants you can reuse
    const float L = 0.10f;   // left inset
    const float R = 0.90f;   // right inset
    const float B = 0.00f;   // baseline
    const float T = 1.00f;   // cap height
    const float M = 0.50f;   // mid x
    const float C = 0.50f;   // center y

    // --- A ---
    // Crossbar height in [0..1], typically ~0.55
    public static IPathBuilder LetterA(this IPathBuilder p, float crossbarY = 0.55f, float crossbarInset = 0.18f)
    {
        // Outer A outline (a triangular-ish “A” with flat bottom)
        // Start bottom-left, go to apex, down to bottom-right, back to bottom-left
        p.M(L, B)
            .L(M, T)
            .L(R, B);

        // Crossbar as a separate subpath (a simple segment “outline” as a thin rectangle-less stroke)
        // Since you’ll offset later, represent it as an open segment or a tiny closed “hairline” box.
        // Here: open segment (preferred if your downstream supports it).
        p.M(L + crossbarInset, crossbarY)
         .L(R - crossbarInset, crossbarY);

        return p;
    }

    // --- B ---
    public static IPathBuilder LetterB(this IPathBuilder p, float upperBellyY = 0.72f, float lowerBellyY = 0.28f, float bellyOut = 0.90f)
    {
        // A single outline that approximates two bowls using cubics.
        // Spine
        p.M(L, B).L(L, T);

        // Upper bowl
        p.L(0.62f, T)
         .C(bellyOut, T, bellyOut, upperBellyY, 0.62f, upperBellyY)
         .L(L, upperBellyY);

        // Lower bowl
        p.L(0.62f, upperBellyY)
         .C(bellyOut, upperBellyY, bellyOut, B, 0.62f, B)
         .L(L, B);

        p.Z();
        return p;
    }

    // --- C ---
    public static IPathBuilder LetterC(this IPathBuilder p, float inset = 0.18f)
    {
        // Rounded C using cubics, open-ish but closed outline
        var x0 = L + inset;
        var x1 = R;
        var y0 = B + 0.08f;
        var y1 = T - 0.08f;

        p.M(x1, y1)
         .C(x0, y1, x0, y1, x0, C)
         .C(x0, y0, x0, y0, x1, y0);

        return p;
    }

    // --- D ---
    public static IPathBuilder LetterD(this IPathBuilder p, float roundness = 0.92f)
    {
        p.M(L, B)
         .L(L, T)
         .L(0.55f, T)
         .C(roundness, T, roundness, B, 0.55f, B)
         .L(L, B)
         .Z();
        return p;
    }

    // --- E ---
    public static IPathBuilder LetterE(this IPathBuilder p, float midBarY = 0.52f, float midBarLen = 0.62f)
    {
        p.M(R, T).L(L, T).L(L, B).L(R, B);
        p.M(L, midBarY).L(L + midBarLen, midBarY);
        return p;
    }

    // --- F ---
    public static IPathBuilder LetterF(this IPathBuilder p, float midBarY = 0.52f, float midBarLen = 0.62f)
    {
        p.M(L, B).L(L, T).L(R, T);
        p.M(L, midBarY).L(L + midBarLen, midBarY);
        return p;
    }

    // --- G ---
    public static IPathBuilder LetterG(this IPathBuilder p, float inset = 0.18f, float barY = 0.45f)
    {
        var x0 = L + inset;
        var x1 = R;
        var y0 = B + 0.08f;
        var y1 = T - 0.08f;

        p.M(x1, y1)
         .C(x0, y1, x0, y1, x0, C)
         .C(x0, y0, x0, y0, x1, y0);

        // G “spur”
        p.M(x1, barY).L(0.62f, barY);
        return p;
    }

    // --- H ---
    public static IPathBuilder LetterH(this IPathBuilder p, float barY = 0.52f)
    {
        p.M(L, B).L(L, T);
        p.M(R, B).L(R, T);
        p.M(L, barY).L(R, barY);
        return p;
    }

    // --- I ---
    public static IPathBuilder LetterI(this IPathBuilder p)
    {
        p.M(M, B).L(M, T);
        return p;
    }

    // --- J ---
    public static IPathBuilder LetterJ(this IPathBuilder p, float hookDepth = 0.18f)
    {
        p.M(R, T).L(R, hookDepth)
         .C(R, B, 0.65f, B, 0.55f, B)
         .C(0.40f, B, 0.35f, 0.10f, 0.35f, hookDepth);
        return p;
    }

    // --- K ---
    public static IPathBuilder LetterK(this IPathBuilder p, float joinY = 0.52f)
    {
        p.M(L, B).L(L, T);
        p.M(L, joinY).L(R, T);
        p.M(L, joinY).L(R, B);
        return p;
    }

    // --- L ---
    public static IPathBuilder LetterL(this IPathBuilder p)
    {
        p.M(L, T).L(L, B).L(R, B);
        return p;
    }

    // --- M ---
    public static IPathBuilder LetterM(this IPathBuilder p, float innerPeakY = 0.65f)
    {
        p.M(L, B).L(L, T).L(M, innerPeakY).L(R, T).L(R, B);
        return p;
    }

    // --- N ---
    public static IPathBuilder LetterN(this IPathBuilder p)
    {
        p.M(L, B).L(L, T).L(R, B).L(R, T);
        return p;
    }

    // --- O ---
    public static IPathBuilder LetterO(this IPathBuilder p, float insetX = 0.14f, float insetY = 0.08f)
    {
        var x0 = L + insetX;
        var x1 = R - insetX;
        var y0 = B + insetY;
        var y1 = T - insetY;

        // Approx ellipse with 4 cubics
        var cx = (x0 + x1) * 0.5f;
        var cy = (y0 + y1) * 0.5f;
        var rx = (x1 - x0) * 0.5f;
        var ry = (y1 - y0) * 0.5f;
        var k = 0.55228475f; // circle cubic constant

        p.M(cx + rx, cy)
         .C(cx + rx, cy + k * ry, cx + k * rx, cy + ry, cx, cy + ry)
         .C(cx - k * rx, cy + ry, cx - rx, cy + k * ry, cx - rx, cy)
         .C(cx - rx, cy - k * ry, cx - k * rx, cy - ry, cx, cy - ry)
         .C(cx + k * rx, cy - ry, cx + rx, cy - k * ry, cx + rx, cy)
         .Z();

        return p;
    }

    // --- P ---
    public static IPathBuilder LetterP(this IPathBuilder p, float bowlY = 0.62f, float outX = 0.90f)
    {
        p.M(L, B).L(L, T).L(0.60f, T)
         .C(outX, T, outX, bowlY, 0.60f, bowlY)
         .L(L, bowlY);
        return p;
    }

    // --- Q ---
    public static IPathBuilder LetterQ(this IPathBuilder p, float tailLen = 0.18f)
    {
        p.LetterO();
        p.M(0.62f, 0.28f).L(0.62f + tailLen, 0.28f - tailLen);
        return p;
    }

    // --- R ---
    public static IPathBuilder LetterR(this IPathBuilder p, float bowlY = 0.62f, float outX = 0.90f)
    {
        p.LetterP(bowlY, outX);
        p.M(L, bowlY).L(R, B);
        return p;
    }

    // --- S ---
    public static IPathBuilder LetterS(this IPathBuilder p)
    {
        // Simple “S” curve with two cubics
        p.M(R, 0.82f)
         .C(0.55f, 1.05f, L, 0.75f, 0.50f, 0.55f)
         .C(R, 0.35f, 0.55f, -0.05f, L, 0.18f);
        return p;
    }

    // --- T ---
    public static IPathBuilder LetterT(this IPathBuilder p)
    {
        p.M(L, T).L(R, T);
        p.M(M, T).L(M, B);
        return p;
    }

    // --- U ---
    public static IPathBuilder LetterU(this IPathBuilder p, float bottomY = 0.12f)
    {
        p.M(L, T).L(L, bottomY)
         .C(L, B, R, B, R, bottomY)
         .L(R, T);
        return p;
    }

    // --- V ---
    public static IPathBuilder LetterV(this IPathBuilder p)
    {
        p.M(L, T).L(M, B).L(R, T);
        return p;
    }

    // --- W ---
    public static IPathBuilder LetterW(this IPathBuilder p, float innerValleyY = 0.22f)
    {
        p.M(L, T).L(0.32f, B).L(M, innerValleyY).L(0.68f, B).L(R, T);
        return p;
    }

    // --- X ---
    public static IPathBuilder LetterX(this IPathBuilder p)
    {
        p.M(L, T).L(R, B);
        p.M(R, T).L(L, B);
        return p;
    }

    // --- Y ---
    public static IPathBuilder LetterY(this IPathBuilder p, float forkY = 0.55f)
    {
        p.M(L, T).L(M, forkY).L(R, T);
        p.M(M, forkY).L(M, B);
        return p;
    }

    // --- Z ---
    public static IPathBuilder LetterZ(this IPathBuilder p)
    {
        p.M(L, T).L(R, T).L(L, B).L(R, B);
        return p;
    }
}

[Category(Cat.Curves)]
public class LetterPath : IGenerator
{
    public static List<MethodInfo> LetterFuncs =>
        typeof(PathBuilderLetters).GetMethods().Where(m => m.Name.StartsWith("Letter")).ToList();

    public List<string> LetterNames
        => LetterFuncs.Select(mi => mi.Name.Substring("Letter".Length)).ToList();

    [Options(nameof(LetterNames))] public int Letter { get; set; }

    [Range(0, 1)] public float Parameter1 { get; }
    [Range(0, 1)] public float Parameter2 { get; }
    [Range(0, 1)] public float Parameter3 { get; }

    public LineMesh3D Eval()
    {
        var pb = new PathBuilder();
        var func = LetterFuncs[Letter];
        var args = func.GetParameters().Select(p => p.DefaultValue).ToArray();
        args[0] = pb;
        func.Invoke(null, args);
        var mesh = new LineMesh3D(pb.Points.Map(p => p.To3D), pb.LineIndices);
        return mesh;
    }
}
