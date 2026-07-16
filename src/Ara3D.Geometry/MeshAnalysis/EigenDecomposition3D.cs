using System.Diagnostics;

namespace Ara3D.Geometry;

public readonly record struct EigenDecomposition3D(
    double LargestValue,
    double MiddleValue,
    double SmallestValue,
    Vector3 LargestVector,
    Vector3 MiddleVector,
    Vector3 SmallestVector)
{
    public double TotalVariance => LargestValue + MiddleValue + SmallestValue;

    public bool IsDegenerate(double epsilon = GeometryUtil.DefaultEpsilon)
        => TotalVariance <= epsilon;

    public static EigenDecomposition3D Decompose(
        SymmetricMatrix3x3 m,
        int maxSweeps = 16,
        double epsilon = GeometryUtil.DefaultEpsilon)
    {
        // Dense symmetric matrix for Jacobi iteration.
        var a = new double[3, 3]
        {
            { m.M00, m.M01, m.M02 },
            { m.M01, m.M11, m.M12 },
            { m.M02, m.M12, m.M22 }
        };

        var v = new double[3, 3]
        {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 }
        };

        static void Rotate(double[,] a, double[,] v, int p, int q)
        {
            var app = a[p, p];
            var aqq = a[q, q];
            var apq = a[p, q];

            if (Math.Abs(apq) <= GeometryUtil.DefaultEpsilon)
                return;

            var tau = (aqq - app) / (2.0 * apq);
            var t = Math.Sign(tau) / (Math.Abs(tau) + Math.Sqrt(1.0 + tau * tau));

            if (tau == 0.0)
                t = 1.0;

            var c = 1.0 / Math.Sqrt(1.0 + t * t);
            var s = t * c;

            a[p, p] = app - t * apq;
            a[q, q] = aqq + t * apq;
            a[p, q] = 0.0;
            a[q, p] = 0.0;

            for (var r = 0; r < 3; r++)
            {
                if (r == p || r == q)
                    continue;

                var arp = a[r, p];
                var arq = a[r, q];

                a[r, p] = c * arp - s * arq;
                a[p, r] = a[r, p];

                a[r, q] = s * arp + c * arq;
                a[q, r] = a[r, q];
            }

            for (var r = 0; r < 3; r++)
            {
                var vrp = v[r, p];
                var vrq = v[r, q];

                v[r, p] = c * vrp - s * vrq;
                v[r, q] = s * vrp + c * vrq;
            }
        }

        for (var sweep = 0; sweep < maxSweeps; sweep++)
        {
            var off =
                Math.Abs(a[0, 1]) +
                Math.Abs(a[0, 2]) +
                Math.Abs(a[1, 2]);

            if (off <= epsilon)
                break;

            Rotate(a, v, 0, 1);
            Rotate(a, v, 0, 2);
            Rotate(a, v, 1, 2);
        }

        var items = new[]
        {
            (value: a[0, 0], vector: Column(v, 0)),
            (value: a[1, 1], vector: Column(v, 1)),
            (value: a[2, 2], vector: Column(v, 2))
        };

        Array.Sort(items, (x, y) => y.value.CompareTo(x.value));

        var largest = items[0];
        var middle = items[1];
        var smallest = items[2];

        var e0 = largest.vector.NormalizeSafe(Vector3.UnitX);
        var e1 = MakeOrthogonal(middle.vector, e0);
        var e2 = e0.NormalizedCross(e1);
        
        // Preserve the approximate handedness of the third vector.
        if (Vector3.Dot(e2, smallest.vector) < 0)
            e2 = -e2;

        Debug.Assert(largest.value >= middle.value - epsilon);
        Debug.Assert(middle.value >= smallest.value - epsilon);
        Debug.Assert(e0.IsUnit());
        Debug.Assert(e1.IsUnit());
        Debug.Assert(e2.IsUnit());
        Debug.Assert(Math.Abs(Vector3.Dot(e0, e1)) < 1e-4f);
        Debug.Assert(Math.Abs(Vector3.Dot(e0, e2)) < 1e-4f);
        Debug.Assert(Math.Abs(Vector3.Dot(e1, e2)) < 1e-4f);

        return new EigenDecomposition3D(
            largest.value,
            middle.value,
            smallest.value,
            e0,
            e1,
            e2);
    }

    private static Vector3 Column(double[,] m, int col)
        => new((float)m[0, col], (float)m[1, col], (float)m[2, col]);

    private static Vector3 MakeOrthogonal(Vector3 v, Vector3 axis)
    {
        Debug.Assert(axis.IsUnit());

        v -= Vector3.Dot(v, axis) * axis;

        if (v.LengthSquared() < 1e-10f)
            v = axis.AnyPerpendicular();

        return v.Normalize;
    }
}