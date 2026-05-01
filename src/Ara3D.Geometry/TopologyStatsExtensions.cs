namespace Ara3D.Geometry;

using System.Diagnostics;

public readonly record struct NormalOrientationStats(
    double UpFacingAreaRatio,
    double DownFacingAreaRatio,
    double HorizontalFacingAreaRatio,
    double VerticalFacingAreaRatio,
    double SlopedFacingAreaRatio);

public readonly record struct TessellationStats(
    double AverageEdgeLength,
    double EdgeLengthCoefficientOfVariation,
    double TriangleAspectRatioAverage,
    double TriangleAspectRatioMax,
    double DegenerateTriangleRatio);

public readonly record struct BoundaryStats(
    double BoundaryLength,
    double BoundaryLengthToSurfaceAreaRatio,
    double BoundaryLengthToBoundsDiagonalRatio);

public readonly record struct TopologyFeatureStats(
    int ConnectedFaceComponentCount,
    int ConnectedVertexComponentCount,
    double SurfaceArea,
    double EstimatedVolume,
    NormalOrientationStats NormalOrientation,
    BoundaryStats Boundary,
    TessellationStats Tessellation);


public static class TopologyStatsExtensions
{
    private const double Epsilon = 1e-15;
    private const double TinyLength = 1e-30;
    private const double TinyArea = 1e-30;

    public static TopologyFeatureStats GetFeatureStats(
        this Topology self,
        Bounds3D bounds,
        double sharpAngleRadians = Math.PI / 6.0)
    {
        ArgumentNullException.ThrowIfNull(self);
        Debug.Assert(sharpAngleRadians >= 0);
        Debug.Assert(sharpAngleRadians <= Math.PI);

        var surfaceArea = self.GetSurfaceArea();
        var boundsDiagonal = bounds.DiagonalLength();

        Debug.Assert(surfaceArea >= 0);
        Debug.Assert(boundsDiagonal >= 0);

        return new TopologyFeatureStats(
            self.GetConnectedFaceComponents().Count,
            self.GetConnectedVertexComponents().Count,
            surfaceArea,
            self.GetEstimatedVolume(),
            self.GetNormalOrientationStats(),
            self.GetBoundaryStats(surfaceArea, boundsDiagonal),
            self.GetTessellationStats());
    }

    private static double SafeRatio(double numerator, double denominator)
    {
        Debug.Assert(!double.IsNaN(numerator));
        Debug.Assert(!double.IsNaN(denominator));

        return Math.Abs(denominator) <= Epsilon
            ? 0
            : numerator / denominator;
    }

    public static BoundaryStats GetBoundaryStats(
        this Topology self,
        double surfaceArea,
        double boundsDiagonal)
    {
        ArgumentNullException.ThrowIfNull(self);
        Debug.Assert(surfaceArea >= 0);
        Debug.Assert(boundsDiagonal >= 0);

        var length = self.GetBoundaryLength();

        return new BoundaryStats(
            length,
            SafeRatio(length, surfaceArea),
            SafeRatio(length, boundsDiagonal));
    }

    public static double GetBoundaryLength(this Topology self)
    {
        ArgumentNullException.ThrowIfNull(self);

        double sum = 0;

        foreach (var e in self.BoundaryEdges)
        {
            var length = self.GetEdgeLength(e);
            Debug.Assert(length >= 0);
            sum += length;
        }

        return sum;
    }

    public static TessellationStats GetTessellationStats(
        this Topology self,
        double degenerateAreaEpsilon = 1e-12)
    {
        ArgumentNullException.ThrowIfNull(self);
        Debug.Assert(degenerateAreaEpsilon >= 0);

        int edgeCount = 0;
        double edgeLengthSum = 0;
        double edgeLengthSquaredSum = 0;

        foreach (var e in self.GetUndirectedEdgeIds())
        {
            var length = self.GetEdgeLength(e);

            Debug.Assert(length >= 0);

            edgeCount++;
            edgeLengthSum += length;
            edgeLengthSquaredSum += length * length;
        }

        var avgEdgeLength = SafeRatio(edgeLengthSum, edgeCount);

        var variance = edgeCount == 0
            ? 0
            : edgeLengthSquaredSum / edgeCount - avgEdgeLength * avgEdgeLength;

        // Protect against tiny negative roundoff.
        var edgeStdDev = Math.Sqrt(Math.Max(0, variance));

        double aspectSum = 0;
        double aspectMax = 0;
        int validAspectCount = 0;
        int degenerateCount = 0;

        foreach (var f in self.GetFaceIds())
        {
            var h0 = self.GetHalfEdgeId(f, 0);
            var h1 = self.GetHalfEdgeId(f, 1);
            var h2 = self.GetHalfEdgeId(f, 2);

            var a = self.GetEdgeLength(h0);
            var b = self.GetEdgeLength(h1);
            var c = self.GetEdgeLength(h2);

            Debug.Assert(a >= 0);
            Debug.Assert(b >= 0);
            Debug.Assert(c >= 0);

            var min = Math.Min(a, Math.Min(b, c));
            var max = Math.Max(a, Math.Max(b, c));
            var area = self.GetFaceArea(f);

            Debug.Assert(area >= 0);

            if (area <= degenerateAreaEpsilon || min <= TinyLength)
            {
                degenerateCount++;
                continue;
            }

            var aspect = max / min;

            Debug.Assert(aspect >= 1);

            aspectSum += aspect;
            aspectMax = Math.Max(aspectMax, aspect);
            validAspectCount++;
        }

        return new TessellationStats(
            avgEdgeLength,
            SafeRatio(edgeStdDev, avgEdgeLength),
            SafeRatio(aspectSum, validAspectCount),
            aspectMax,
            SafeRatio(degenerateCount, self.FaceCount));
    }

    public static NormalOrientationStats GetNormalOrientationStats(
        this Topology self,
        double horizontalDotThreshold = 0.9,
        double verticalAbsDotThreshold = 0.1)
    {
        ArgumentNullException.ThrowIfNull(self);

        Debug.Assert(horizontalDotThreshold is >= 0 and <= 1);
        Debug.Assert(verticalAbsDotThreshold is >= 0 and <= 1);
        Debug.Assert(verticalAbsDotThreshold <= horizontalDotThreshold);

        var up = Vector3.UnitZ;

        double totalArea = 0;
        double upArea = 0;
        double downArea = 0;
        double horizontalArea = 0;
        double verticalArea = 0;
        double slopedArea = 0;

        foreach (var f in self.GetFaceIds())
        {
            var area = self.GetFaceArea(f);
            Debug.Assert(area >= 0);

            if (area <= TinyArea)
                continue;

            var n = self.GetFaceNormal(f);
            var lenSq = n.LengthSquared();

            Debug.Assert(lenSq > 0);

            // Defensive normalization in case GetFaceNormal() is not guaranteed unit length.
            if (Math.Abs(lenSq - 1f) > 1e-4f)
                n = n.Normalize;

            var d = Vector3.Dot(n, up);
            var ad = Math.Abs(d);

            Debug.Assert(ad <= 1.0001f);

            totalArea += area;

            if (d >= horizontalDotThreshold)
                upArea += area;
            else if (d <= -horizontalDotThreshold)
                downArea += area;

            if (ad >= horizontalDotThreshold)
                horizontalArea += area;
            else if (ad <= verticalAbsDotThreshold)
                verticalArea += area;
            else
                slopedArea += area;
        }

        return new NormalOrientationStats(
            SafeRatio(upArea, totalArea),
            SafeRatio(downArea, totalArea),
            SafeRatio(horizontalArea, totalArea),
            SafeRatio(verticalArea, totalArea),
            SafeRatio(slopedArea, totalArea));
    }
}