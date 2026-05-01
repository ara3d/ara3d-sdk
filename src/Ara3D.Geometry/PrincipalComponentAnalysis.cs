
namespace Ara3D.Geometry;

public class PrincipalComponentAnalysis
{
    public const double Epsilon = 1e-12;

    // Symmetric 3x3 matrix
    // C00, C01, C02
    //      C11, C12
    //           C22
    public readonly double Cov00;
    public readonly double Cov01;
    public readonly double Cov02;
    public readonly double Cov11;
    public readonly double Cov12;
    public readonly double Cov22;
    
    public readonly double EigenX;
    public readonly double EigenY;
    public readonly double EigenZ;

    public readonly double LargestEigenValue;
    public readonly double MiddleEigenValue;
    public readonly double SmallestEigenValue;

    public readonly Vector3 PrincipalAxis;
    public readonly Vector3 SecondaryAxis;
    public readonly Vector3 TertiaryAxis;

    public readonly double Linearity;
    public readonly double Planarity;
    public readonly double Scattering;

    public int Count { get; }

    public readonly double MeanX;
    public readonly double MeanY;
    public readonly double MeanZ;

    public Vector3 Mean => new Vector3((float)MeanX, (float)MeanY, (float)MeanZ);

    public double TotalVariance => LargestEigenValue + MiddleEigenValue + SmallestEigenValue;
    public bool IsPointLike => TotalVariance <= Epsilon;
    public Axes3D Axes => new(PrincipalAxis, SecondaryAxis, TertiaryAxis);
    public Frame3D Frame => new(Mean, Axes.ToOrthonormalBasis());

    public PrincipalComponentAnalysis(
        IReadOnlyList<Vector3> pts,
        IReadOnlyList<double>? weights = null)
    {
        if (pts == null)
            throw new ArgumentNullException(nameof(pts));

        Count = pts.Count;
        if (weights != null && weights.Count != pts.Count)
            throw new ArgumentException("Weights must match point count.");

        var sumW = 0.0;
    
        // --- Compute weighted mean --------------------------------------------
        for (int i = 0; i < pts.Count; i++)
        {
            var w = weights != null ? weights[i] : 1.0;
            var p = pts[i];

            sumW += w;
            MeanX += w * p.X;
            MeanY += w * p.Y;
            MeanZ += w * p.Z;
        }

        if (sumW <= Epsilon)
            throw new InvalidOperationException("Total weight is zero.");

        MeanX /= sumW;
        MeanY /= sumW;
        MeanZ /= sumW;

        // --- Compute weighted covariance ---------------------------------------
        for (int i = 0; i < pts.Count; i++)
        {
            var w = weights != null ? weights[i] : 1.0;
            var p = pts[i];

            var dx = p.X - MeanX;
            var dy = p.Y - MeanY;
            var dz = p.Z - MeanZ;

            Cov00 += w * dx * dx;
            Cov01 += w * dx * dy;
            Cov02 += w * dx * dz;
            Cov11 += w * dy * dy;
            Cov12 += w * dy * dz;
            Cov22 += w * dz * dz;
        }

        var invW = 1.0 / sumW;

        Cov00 *= invW;
        Cov01 *= invW;
        Cov02 *= invW;
        Cov11 *= invW;
        Cov12 *= invW;
        Cov22 *= invW;

        // --- Eigen decomposition (unchanged) -----------------------------------
        (EigenX, EigenY, EigenZ) = EigenValues(Cov00, Cov01, Cov02, Cov11, Cov12, Cov22);

        var sorted = SortDescending(EigenX, EigenY, EigenZ);

        LargestEigenValue = sorted.x;
        MiddleEigenValue = sorted.y;
        SmallestEigenValue = sorted.z;

        PrincipalAxis = EigenVector(LargestEigenValue);
        SecondaryAxis = MakeOrthogonal(EigenVector(MiddleEigenValue), PrincipalAxis);
        TertiaryAxis = PrincipalAxis.NormalizedCross(SecondaryAxis);

        if (LargestEigenValue > Epsilon)
        {
            Linearity = (LargestEigenValue - MiddleEigenValue) / LargestEigenValue;
            Planarity = (MiddleEigenValue - SmallestEigenValue) / LargestEigenValue;
            Scattering = SmallestEigenValue / LargestEigenValue;
        }
        else
        {
            Linearity = 0;
            Planarity = 0;
            Scattering = 0;
        }
    }

    public bool IsMostlyLinear(double threshold = 0.8)
        => Linearity >= threshold;

    public bool IsMostlyPlanar(double threshold = 0.8)
        => Planarity >= threshold;

    public bool IsMostlyVolumetric(double threshold = 0.2)
        => Scattering >= threshold;

    public double DistanceToMean(Vector3 p)
        => p.Distance(Mean);

    public double SignedDistanceAlongPrincipalAxis(Vector3 p)
        => (p - Mean).Dot(PrincipalAxis);

    public double SignedDistanceAlongSecondaryAxis(Vector3 p)
        => (p - Mean).Dot(SecondaryAxis);

    public double SignedDistanceAlongTertiaryAxis(Vector3 p)
        => (p - Mean).Dot(TertiaryAxis);

    public Vector3 ProjectOntoPrincipalLine(Vector3 p)
        => Mean + PrincipalAxis * (float)SignedDistanceAlongPrincipalAxis(p);

    public double DistanceToPrincipalLine(Vector3 p)
        => p.Distance(ProjectOntoPrincipalLine(p));

    public double SignedDistanceToBestFitPlane(Vector3 p)
        => (p - Mean).Dot(TertiaryAxis);

    public double DistanceToBestFitPlane(Vector3 p)
        => Math.Abs(SignedDistanceToBestFitPlane(p));

    public Vector3 ProjectOntoBestFitPlane(Vector3 p)
        => p - TertiaryAxis * (float)SignedDistanceToBestFitPlane(p);

    public static (double x, double y, double z) SortDescending(double x, double y, double z)
    {
        if (x < y) (x, y) = (y, x);
        if (y < z) (y, z) = (z, y);
        if (x < y) (x, y) = (y, x);
        return (x, y, z);
    }

    public static (double x, double y, double z) EigenValues(
        double a00, double a01, double a02,
        double a11, double a12,
        double a22)
    {
        var trace = a00 + a11 + a22;

        var trA2 =
            a00 * a00 +
            a11 * a11 +
            a22 * a22 +
            2.0 * (a01 * a01 + a02 * a02 + a12 * a12);

        var c1 = 0.5 * (trace * trace - trA2);

        var c0 =
            a00 * (a11 * a22 - a12 * a12) -
            a01 * (a01 * a22 - a12 * a02) +
            a02 * (a01 * a12 - a11 * a02);

        var p = c1 - trace * trace / 3.0;
        var q = 2.0 * trace * trace * trace / 27.0 - trace * c1 / 3.0 + c0;

        if (Math.Abs(p) < Epsilon)
        {
            var e = trace / 3.0;
            return (e, e, e);
        }

        var discriminant = q * q / 4.0 + p * p * p / 27.0;

        double Cbrt(double v)
            => v >= 0.0
                ? Math.Pow(v, 1.0 / 3.0)
                : -Math.Pow(-v, 1.0 / 3.0);

        if (discriminant > Epsilon)
        {
            var s = Math.Sqrt(discriminant);
            var u = Cbrt(-q / 2.0 + s);
            var v = Cbrt(-q / 2.0 - s);
            var x = u + v;

            var e0 = x + trace / 3.0;
            var e1 = -x / 2.0 + trace / 3.0;

            return (e0, e1, e1);
        }
        else
        {
            var denom = Math.Sqrt(-p * p * p / 27.0);
            var arg = -q / 2.0 / denom;
            arg = Math.Clamp(arg, -1.0, 1.0);

            var phi = Math.Acos(arg);
            var t = 2.0 * Math.Sqrt(-p / 3.0);

            var e0 = t * Math.Cos(phi / 3.0) + trace / 3.0;
            var e1 = t * Math.Cos((phi + 2.0 * Math.PI) / 3.0) + trace / 3.0;
            var e2 = t * Math.Cos((phi + 4.0 * Math.PI) / 3.0) + trace / 3.0;

            return (e0, e1, e2);
        }
    }

    public Vector3 EigenVector(double eigenValue)
    {
        var val = (float)eigenValue;

        var r0 = new Vector3((float)Cov00 - val, (float)Cov01, (float)Cov02);
        var r1 = new Vector3((float)Cov01, (float)Cov11 - val, (float)Cov12);
        var r2 = new Vector3((float)Cov02, (float)Cov12, (float)Cov22 - val);

        var v = Vector3.Cross(r0, r1);

        if (v.LengthSquared() < 1e-10f)
            v = Vector3.Cross(r0, r2);

        if (v.LengthSquared() < 1e-10f)
            v = Vector3.Cross(r1, r2);

        if (v.LengthSquared() < 1e-10f)
            v = FallbackAxis(eigenValue);

        return v.Normalize;
    }

    private static Vector3 MakeOrthogonal(Vector3 v, Vector3 axis)
    {
        v -= axis * Vector3.Dot(v, axis);

        if (v.LengthSquared() < 1e-10f)
            v = AnyPerpendicular(axis);

        return v.Normalize;
    }

    private Vector3 FallbackAxis(double eigenValue)
    {
        if (Math.Abs(eigenValue - LargestEigenValue) <= Epsilon)
            return Vector3.UnitX;

        if (Math.Abs(eigenValue - MiddleEigenValue) <= Epsilon)
            return Vector3.UnitY;

        return Vector3.UnitZ;
    }

    private static Vector3 AnyPerpendicular(Vector3 axis)
    {
        var other = Math.Abs(axis.X) < 0.9f
            ? Vector3.UnitX
            : Vector3.UnitY;
        return axis.NormalizedCross(other);
    }
}