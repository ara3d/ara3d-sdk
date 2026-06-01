namespace Ara3D.Geometry;

public readonly record struct SymmetricMatrix3x3(
    double M00,
    double M01,
    double M02,
    double M11,
    double M12,
    double M22)
{
    public static SymmetricMatrix3x3 Zero => default;

    public double Trace => M00 + M11 + M22;

    public double Determinant =>
        M00 * (M11 * M22 - M12 * M12) -
        M01 * (M01 * M22 - M12 * M02) +
        M02 * (M01 * M12 - M11 * M02);

    public Vector3 Row0(double subtractFromDiagonal = 0)
        => new((float)(M00 - subtractFromDiagonal), (float)M01, (float)M02);

    public Vector3 Row1(double subtractFromDiagonal = 0)
        => new((float)M01, (float)(M11 - subtractFromDiagonal), (float)M12);

    public Vector3 Row2(double subtractFromDiagonal = 0)
        => new((float)M02, (float)M12, (float)(M22 - subtractFromDiagonal));

    public static SymmetricMatrix3x3 WeightedCovariance(
        IReadOnlyList<Vector3> points,
        out Vector3 mean,
        IReadOnlyList<double>? weights = null,
        double epsilon = GeometryNumerics.DefaultEpsilon)
    {
        if (points == null)
            throw new ArgumentNullException(nameof(points));

        if (weights != null && weights.Count != points.Count)
            throw new ArgumentException("Weights must match point count.", nameof(weights));

        if (points.Count == 0)
            throw new ArgumentException("At least one point is required.", nameof(points));

        var sumW = 0.0;
        var sx = 0.0;
        var sy = 0.0;
        var sz = 0.0;

        for (var i = 0; i < points.Count; i++)
        {
            var p = points[i];
            if (!p.IsFinite())
                throw new ArgumentException($"Point {i} is not finite.", nameof(points));

            var w = weights?[i] ?? 1.0;
            if (!double.IsFinite(w) || w < 0)
                throw new ArgumentException($"Weight {i} must be finite and non-negative.", nameof(weights));

            sumW += w;
            sx += w * p.X;
            sy += w * p.Y;
            sz += w * p.Z;
        }

        if (sumW <= epsilon)
            throw new ArgumentException("Total weight must be positive.", nameof(weights));

        var mx = sx / sumW;
        var my = sy / sumW;
        var mz = sz / sumW;

        mean = new Vector3((float)mx, (float)my, (float)mz);

        var c00 = 0.0;
        var c01 = 0.0;
        var c02 = 0.0;
        var c11 = 0.0;
        var c12 = 0.0;
        var c22 = 0.0;

        for (var i = 0; i < points.Count; i++)
        {
            var p = points[i];
            var w = weights?[i] ?? 1.0;

            var dx = p.X - mx;
            var dy = p.Y - my;
            var dz = p.Z - mz;

            c00 += w * dx * dx;
            c01 += w * dx * dy;
            c02 += w * dx * dz;
            c11 += w * dy * dy;
            c12 += w * dy * dz;
            c22 += w * dz * dz;
        }

        var invW = 1.0 / sumW;

        return new SymmetricMatrix3x3(
            c00 * invW,
            c01 * invW,
            c02 * invW,
            c11 * invW,
            c12 * invW,
            c22 * invW);
    }
}