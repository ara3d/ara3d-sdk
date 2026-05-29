using System.Diagnostics;
using Ara3D.Utils;

namespace Ara3D.Geometry;

public sealed record RadialObjectAnalysis(
    double Score,
    Line3D Axis,
    Vector3 AxisDirection,
    double StartRadius,
    double EndRadius,
    double LateralFaceRatio,
    double NormalAlignment,
    double RadiusConsistency,
    double AxisSupport,
    int TriangleCount,
    int UsedTriangleCount)
{
    public bool IsProbablyRadial(double threshold = 0.75)
        => Score >= threshold;

    public static RadialObjectAnalysis Empty(int triangleCount = 0)
        => new(
            Score: 0,
            Axis: new Line3D(default, default),
            AxisDirection: default,
            StartRadius: 0,
            EndRadius: 0,
            LateralFaceRatio: 0,
            NormalAlignment: 0,
            RadiusConsistency: 0,
            AxisSupport: 0,
            TriangleCount: triangleCount,
            UsedTriangleCount: 0);
}

public sealed record RadialObjectAnalysisOptions
{
    public double Epsilon { get; init; } = 1e-9;

    // Side faces of cylinders, cones, and frustums have normals mostly
    // perpendicular to the radial axis. Caps have normals mostly parallel.
    public double MaxAbsNormalAxisDotForLateralFace { get; init; } = 0.35;

    // Caps should not destroy the score, but the object still needs enough
    // lateral surface evidence to support a radial interpretation.
    public double MinGoodLateralFaceRatio { get; init; } = 0.10;
    public double FullLateralFaceRatio { get; init; } = 0.35;
}

public static class RadialObjectAnalyzer
{
    public static RadialObjectAnalysis Analyze(
        IReadOnlyList<Triangle3D> triangles,
        RadialObjectAnalysisOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(triangles);

        var opt = options ?? new RadialObjectAnalysisOptions();

        Debug.Assert(opt.Epsilon > 0);

        var samples = triangles
            .Select(t => FaceSample.TryCreate(t, opt.Epsilon, out var sample) ? sample : (FaceSample?)null)
            .WhereHasValue()
            .ToArray();

        if (samples.Length == 0)
            return RadialObjectAnalysis.Empty(triangles.Count);

        var candidates = GetCandidateAxisDirections(samples, opt.Epsilon)
            .Select(d => d.TryNormalized(opt.Epsilon, out var u) ? u : (Vector3?)null)
            .WhereHasValue()
            .DistinctBy(DirectionKey.CanonicalUndirected)
            .ToArray();

        if (candidates.Length == 0)
            return RadialObjectAnalysis.Empty(triangles.Count);

        var best = candidates
            .Select(d => AnalyzeCandidate(samples, d, triangles.Count, opt))
            .OrderByDescending(a => a.Score)
            .First();

        Debug.Assert(best.Score is >= 0 and <= 1);
        Debug.Assert(best.StartRadius >= -opt.Epsilon);
        Debug.Assert(best.EndRadius >= -opt.Epsilon);

        return best;
    }

    private static RadialObjectAnalysis AnalyzeCandidate(
        IReadOnlyList<FaceSample> samples,
        Vector3 axisDirection,
        int triangleCount,
        RadialObjectAnalysisOptions opt)
    {
        Debug.Assert(axisDirection.IsUnit(opt.Epsilon));

        var lateralSamples = samples
            .Where(s => s.Normal.AbsDot(axisDirection) <= opt.MaxAbsNormalAxisDotForLateralFace)
            .ToArray();

        var totalWeight = samples.Sum(s => s.Weight);
        var lateralWeight = lateralSamples.Sum(s => s.Weight);
        var lateralFaceRatio = lateralWeight.SafeDivide(totalWeight);

        if (lateralSamples.Length < 3)
            return FailedCandidate(axisDirection, triangleCount, samples.Count, lateralFaceRatio);

        var axisPoint = RadialAxisFitter.FitAxisPoint(lateralSamples, axisDirection, opt.Epsilon);

        var radiusSamples = lateralSamples
            .Select(s => RadiusMeasurement.TryCreate(s, axisPoint, axisDirection, opt.Epsilon, out var m) ? m : (RadiusMeasurement?)null)
            .WhereHasValue()
            .ToArray();

        if (radiusSamples.Length < 3)
            return FailedCandidate(axisDirection, triangleCount, samples.Count, lateralFaceRatio);

        var alignment = radiusSamples.WeightedAverage(
            x => x.Weight,
            x => x.NormalAlignment);

        var radiusFit = LinearRadiusFit.Fit(radiusSamples, opt.Epsilon);

        var tRange = samples.ProjectionRange(axisPoint, axisDirection);

        var startRadius = Math.Max(0, radiusFit.RadiusAt(tRange.Min));
        var endRadius = Math.Max(0, radiusFit.RadiusAt(tRange.Max));

        var axis = new Line3D(
            axisPoint + axisDirection * (float)tRange.Min,
            axisPoint + axisDirection * (float)tRange.Max);

        var axisSupport = MathUtil.SmoothStep(
            opt.MinGoodLateralFaceRatio,
            opt.FullLateralFaceRatio,
            lateralFaceRatio);

        var score = MathUtil.Clamp01(alignment * radiusFit.Consistency * axisSupport);

        Debug.Assert(score is >= 0 and <= 1);
        Debug.Assert(startRadius >= 0);
        Debug.Assert(endRadius >= 0);

        return new RadialObjectAnalysis(
            Score: score,
            Axis: axis,
            AxisDirection: axisDirection,
            StartRadius: startRadius,
            EndRadius: endRadius,
            LateralFaceRatio: lateralFaceRatio,
            NormalAlignment: alignment,
            RadiusConsistency: radiusFit.Consistency,
            AxisSupport: axisSupport,
            TriangleCount: triangleCount,
            UsedTriangleCount: samples.Count);
    }

    private static RadialObjectAnalysis FailedCandidate(
        Vector3 axisDirection,
        int triangleCount,
        int usedTriangleCount,
        double lateralFaceRatio)
        => new(
            Score: 0,
            Axis: new Line3D(default, default),
            AxisDirection: axisDirection,
            StartRadius: 0,
            EndRadius: 0,
            LateralFaceRatio: lateralFaceRatio,
            NormalAlignment: 0,
            RadiusConsistency: 0,
            AxisSupport: 0,
            TriangleCount: triangleCount,
            UsedTriangleCount: usedTriangleCount);

    private static IEnumerable<Vector3> GetCandidateAxisDirections(
        IReadOnlyList<FaceSample> samples,
        double eps)
    {
        var points = samples.Select(s => s.Centroid).ToArray();
        var normals = samples.Select(s => s.Normal).ToArray();

        // Assumed existing PCA helpers.
        var pointAxes = GetPrincipalAxes(points);
        var normalAxes = GetPrincipalAxes(normals);

        // For elongated radial objects, the major point PCA axis is often useful.
        foreach (var axis in pointAxes)
            yield return axis;

        // For side faces of radial objects, the true axis tends to be the
        // direction with minimum normal-space variance.
        var bestNormalAxis = normalAxes
            .Select(a => a.TryNormalized(eps, out var u) ? u : (Vector3?)null)
            .WhereHasValue()
            .OrderBy(a => normals.Sum(n => Math.Pow(Vector3.Dot(n, a), 2)))
            .FirstOrDefault();

        if (bestNormalAxis != default)
            yield return bestNormalAxis;

        // Fallbacks help when PCA is ambiguous, especially on symmetric shapes.
        yield return new Vector3(1, 0, 0);
        yield return new Vector3(0, 1, 0);
        yield return new Vector3(0, 0, 1);
    }

    public static IReadOnlyList<Vector3> GetPrincipalAxes(IReadOnlyList<Vector3> vectors)
    {
        var pca = new PrincipalComponentAnalysis(vectors);
        return [pca.PrincipalAxis, pca.SecondaryAxis, pca.TertiaryAxis];
    }

}

public readonly record struct FaceSample(
    Vector3 Centroid,
    Vector3 Normal,
    double Weight)
{
    public static bool TryCreate(Triangle3D triangle, double eps, out FaceSample sample)
    {
        var a = triangle.A.Vector3;
        var b = triangle.B.Vector3;
        var c = triangle.C.Vector3;

        var areaVector = Vector3.Cross(b - a, c - a);
        var doubleArea = areaVector.Length;

        if (doubleArea <= eps || !triangle.Normal.TryNormalized(eps, out var normal))
        {
            sample = default;
            return false;
        }

        var centroid = (a + b + c) / 3;
        var area = doubleArea * 0.5;

        Debug.Assert(area > 0);
        Debug.Assert(normal.IsUnit(eps));

        sample = new FaceSample(centroid, normal, area);
        return true;
    }
}

public readonly record struct RadiusMeasurement(
    double T,
    double Radius,
    double NormalAlignment,
    double Weight)
{
    public static bool TryCreate(
        FaceSample sample,
        Vector3 axisPoint,
        Vector3 axisDirection,
        double eps,
        out RadiusMeasurement measurement)
    {
        Debug.Assert(axisDirection.IsUnit(eps));

        var toPoint = sample.Centroid - axisPoint;
        var t = Vector3.Dot(toPoint, axisDirection);
        var radial = toPoint - axisDirection * t;
        var radius = radial.Length;

        if (radius <= eps)
        {
            measurement = default;
            return false;
        }

        var radialDirection = radial / radius;
        var projectedNormal = sample.Normal.RejectFrom(axisDirection);

        if (!projectedNormal.TryNormalized(eps, out var normalDirection))
        {
            measurement = default;
            return false;
        }

        // Abs accepts either inward or outward face winding.
        // Remove Math.Abs if you specifically require outward normals.
        var alignment = MathUtil.Clamp01(Math.Abs(Vector3.Dot(radialDirection, normalDirection)));

        measurement = new RadiusMeasurement(
            T: t,
            Radius: radius,
            NormalAlignment: alignment,
            Weight: sample.Weight);

        return true;
    }
}

public readonly record struct LinearRadiusFit(
    double Intercept,
    double Slope,
    double Consistency)
{
    public double RadiusAt(double t)
        => Intercept + Slope * t;

    public static LinearRadiusFit Fit(
        IReadOnlyList<RadiusMeasurement> values,
        double eps)
    {
        var wSum = values.Sum(x => x.Weight);

        if (wSum <= eps)
            return new LinearRadiusFit(0, 0, 0);

        var tMean = values.Sum(x => x.Weight * x.T) / wSum;
        var rMean = values.Sum(x => x.Weight * x.Radius) / wSum;

        var tt = values.Sum(x => x.Weight * Math.Pow(x.T - tMean, 2));
        var tr = values.Sum(x => x.Weight * (x.T - tMean) * (x.Radius - rMean));

        var slope = Math.Abs(tt) <= eps ? 0 : tr / tt;
        var intercept = rMean - slope * tMean;

        var mse = values.Sum(x =>
        {
            var error = x.Radius - (intercept + slope * x.T);
            return x.Weight * error * error;
        }) / wSum;

        var rmse = Math.Sqrt(Math.Max(0, mse));
        var scale = Math.Max(rMean, eps);

        // 1 means the radius follows a clean linear profile along the axis.
        // This intentionally accepts cylinders, cones, and frustums.
        var consistency = 1.0 / (1.0 + rmse / scale);

        Debug.Assert(consistency is >= 0 and <= 1);

        return new LinearRadiusFit(intercept, slope, consistency);
    }
}

public static class RadialAxisFitter
{
    public static Vector3 FitAxisPoint(
        IReadOnlyList<FaceSample> samples,
        Vector3 axisDirection,
        double eps)
    {
        Debug.Assert(axisDirection.IsUnit(eps));

        var basis = OrthonormalBasis2D.FromNormal(axisDirection, eps);
        var u = basis.U;
        var v = basis.V;

        // In the plane perpendicular to the candidate axis, each lateral face
        // says: "the axis point should lie somewhere along the inward/outward
        // normal line through the face centroid."
        //
        // If p is the projected face centroid and n is the projected normal:
        //
        //     cross(n, p - x) = 0
        //
        // This gives one weighted linear equation per face:
        //
        //     -ny * x + nx * y = -ny * px + nx * py

        var system = new SymmetricLeastSquares2D();

        foreach (var sample in samples)
        {
            var projectedNormal = sample.Normal.RejectFrom(axisDirection);

            if (!projectedNormal.TryNormalized(eps, out var n))
                continue;

            var px = Vector3.Dot(sample.Centroid, u);
            var py = Vector3.Dot(sample.Centroid, v);
            var nx = Vector3.Dot(n, u);
            var ny = Vector3.Dot(n, v);

            system.Add(
                a0: -ny,
                a1: nx,
                b: -ny * px + nx * py,
                weight: sample.Weight);
        }

        if (!system.TrySolve(eps, out var x, out var y))
        {
            // Degenerate or underconstrained case.
            // Use the weighted centroid projected onto the perpendicular plane.
            var centroid = samples.WeightedAverageVector(s => s.Weight, s => s.Centroid);
            return centroid.RejectFrom(axisDirection);
        }

        // This is one arbitrary point on the infinite axis.
        // Translating it along axisDirection represents the same axis.
        return u * (float)x + v * (float)y;
    }
}

public readonly record struct OrthonormalBasis2D(Vector3 U, Vector3 V)
{
    public static OrthonormalBasis2D FromNormal(Vector3 normal, double eps)
    {
        Debug.Assert(normal.IsUnit(eps));

        var fallback = Math.Abs(normal.Z) < 0.9
            ? new Vector3(0, 0, 1)
            : new Vector3(0, 1, 0);

        var u = Vector3.Cross(fallback, normal).NormalizedOrThrow(eps);
        var v = Vector3.Cross(normal, u).NormalizedOrThrow(eps);

        Debug.Assert(u.IsUnit(eps));
        Debug.Assert(v.IsUnit(eps));
        Debug.Assert(Math.Abs(Vector3.Dot(u, normal)) <= 1e-6);
        Debug.Assert(Math.Abs(Vector3.Dot(v, normal)) <= 1e-6);

        return new OrthonormalBasis2D(u, v);
    }
}

public struct SymmetricLeastSquares2D
{
    private double _ata00;
    private double _ata01;
    private double _ata11;
    private double _atb0;
    private double _atb1;

    public void Add(double a0, double a1, double b, double weight = 1)
    {
        if (weight <= 0)
            return;

        _ata00 += weight * a0 * a0;
        _ata01 += weight * a0 * a1;
        _ata11 += weight * a1 * a1;
        _atb0 += weight * a0 * b;
        _atb1 += weight * a1 * b;
    }

    public bool TrySolve(double eps, out double x, out double y)
    {
        var det = _ata00 * _ata11 - _ata01 * _ata01;

        if (Math.Abs(det) <= eps)
        {
            x = 0;
            y = 0;
            return false;
        }

        x = (_atb0 * _ata11 - _atb1 * _ata01) / det;
        y = (_ata00 * _atb1 - _ata01 * _atb0) / det;
        return true;
    }
}

public readonly record struct ProjectionRange(double Min, double Max);

public static class GeometryEnumerableExtensions
{
    public static double WeightedAverage<T>(
        this IReadOnlyList<T> values,
        Func<T, double> weight,
        Func<T, double> value)
    {
        var w = values.Sum(weight);

        return w <= 0
            ? 0
            : values.Sum(x => weight(x) * value(x)) / w;
    }

    public static Vector3 WeightedAverageVector<T>(
        this IReadOnlyList<T> values,
        Func<T, double> weight,
        Func<T, Vector3> value)
    {
        var w = (float)values.Sum(weight);

        return w <= 0
            ? default
            : values.Aggregate(default(Vector3), (acc, x) => acc + value(x) * (float)weight(x)) / w;
    }

    public static ProjectionRange ProjectionRange<T>(
        this IReadOnlyList<T> values,
        Vector3 origin,
        Vector3 direction,
        Func<T, Vector3>? point = null)
    {
        Debug.Assert(values.Count > 0);

        point ??= x => x switch
        {
            FaceSample s => s.Centroid,
            Vector3 v => v,
            _ => throw new InvalidOperationException("No point selector was supplied.")
        };

        var first = Vector3.Dot(point(values[0]) - origin, direction);
        var min = first;
        var max = first;

        foreach (var value in values.Skip(1))
        {
            var t = Vector3.Dot(point(value) - origin, direction);
            min = Math.Min(min, t);
            max = Math.Max(max, t);
        }

        return new ProjectionRange(min, max);
    }

    public static ProjectionRange ProjectionRange(
        this IReadOnlyList<FaceSample> samples,
        Vector3 origin,
        Vector3 direction)
        => samples.ProjectionRange(origin, direction, s => s.Centroid);
}

public static class LocalVector3Extensions
{
    public static bool TryNormalized(this Vector3 v, double eps, out Vector3 result)
    {
        var length = v.Length;

        if (length <= eps)
        {
            result = default;
            return false;
        }

        result = v / length;
        return true;
    }

    public static Vector3 NormalizedOrThrow(this Vector3 v, double eps)
        => v.TryNormalized(eps, out var result)
            ? result
            : throw new ArgumentException("Cannot normalize a near-zero vector.", nameof(v));

    public static bool IsUnit(this Vector3 v, double eps)
        => Math.Abs(v.Length - 1.0) <= Math.Sqrt(eps);

    public static double AbsDot(this Vector3 a, Vector3 b)
        => Math.Abs(Vector3.Dot(a, b));

    public static Vector3 ProjectOnto(this Vector3 v, Vector3 unitDirection)
    {
        Debug.Assert(unitDirection.IsUnit(1e-9));
        return unitDirection * Vector3.Dot(v, unitDirection);
    }

    public static Vector3 RejectFrom(this Vector3 v, Vector3 unitDirection)
    {
        Debug.Assert(unitDirection.IsUnit(1e-9));
        return v - v.ProjectOnto(unitDirection);
    }
}

public static class MathUtil
{
    public static double Clamp01(double x)
        => Math.Max(0, Math.Min(1, x));

    public static double SafeDivide(this double numerator, double denominator)
        => Math.Abs(denominator) <= 1e-12 ? 0 : numerator / denominator;

    public static double SmoothStep(double edge0, double edge1, double x)
    {
        if (edge0 >= edge1)
            return x >= edge1 ? 1 : 0;

        var t = Clamp01((x - edge0) / (edge1 - edge0));
        return t * t * (3 - 2 * t);
    }
}

public static class DirectionKey
{
    public static object CanonicalUndirected(Vector3 direction)
    {
        // Treat d and -d as the same axis direction.
        var d = direction;

        if (d.X < 0 ||
            Math.Abs(d.X) < 1e-9 && d.Y < 0 ||
            Math.Abs(d.X) < 1e-9 && Math.Abs(d.Y) < 1e-9 && d.Z < 0)
        {
            d = -d;
        }

        return (
            X: Math.Round(d.X, 6),
            Y: Math.Round(d.Y, 6),
            Z: Math.Round(d.Z, 6));
    }
}