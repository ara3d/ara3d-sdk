namespace Ara3D.Geometry;

/// <summary>
/// Tensor-product Bézier patches. The control net is flattened row-major — index
/// <c>i * numV + j</c>, where i steps along U and j along V — so a patch of any order per
/// direction comes from one de Casteljau collapse: evaluate each U row at v, then collapse
/// the resulting points at u.
/// </summary>
public static class BezierSurfaces
{
    public static ParametricSurface BezierPatch(this IReadOnlyList<Point3D> controlNet, int numU, int numV)
        => controlNet.Count == numU * numV
            ? new ParametricSurface(uv => EvalPatch(controlNet, numU, numV, uv), false, false)
            : throw new ArgumentException(
                $"Expected {numU * numV} control points but received {controlNet.Count}", nameof(controlNet));

    /// <summary>The usual 4 × 4 patch: cubic in both directions, so the four corner points lie
    /// on the surface and the other twelve act as tangent and twist handles.</summary>
    public static ParametricSurface BicubicBezierPatch(this IReadOnlyList<Point3D> controlNet)
        => controlNet.BezierPatch(4, 4);

    private static Vector3 EvalPatch(IReadOnlyList<Point3D> net, int numU, int numV, Vector2 uv)
    {
        Span<Vector3> spine = stackalloc Vector3[numU];
        for (var i = 0; i < numU; i++)
            spine[i] = EvalRow(net, i * numV, numV, uv.Y);
        return Collapse(spine, uv.X);
    }

    private static Vector3 EvalRow(IReadOnlyList<Point3D> net, int offset, int count, Number t)
    {
        Span<Vector3> row = stackalloc Vector3[count];
        for (var i = 0; i < count; i++)
            row[i] = net[offset + i];
        return Collapse(row, t);
    }

    /// <summary>de Casteljau in place: repeated pairwise lerp collapses the control polygon to
    /// the curve point at t.</summary>
    private static Vector3 Collapse(Span<Vector3> points, Number t)
    {
        for (var k = points.Length - 1; k > 0; k--)
            for (var i = 0; i < k; i++)
                points[i] = points[i].Lerp(points[i + 1], t);
        return points[0];
    }
}
