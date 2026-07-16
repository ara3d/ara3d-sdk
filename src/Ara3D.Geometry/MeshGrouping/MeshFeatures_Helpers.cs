
using Ara3D.Geometry;

public sealed partial class MeshFeatures
{
    // =========================================================================
    // Labels
    // =========================================================================

    public string CoarseShapeLabel()
    {
        if (IsEmpty) return "Empty";
        if (IsPointLike) return "PointLike";
        if (IsRodLike) return "RodLike";
        if (IsSheetLike) return "SheetLike";
        if (IsLikelySolid && IsBlobLike) return "BlobLikeSolid";
        if (IsLikelySolid) return "Solid";
        if (IsOpenSurface) return "OpenSurface";
        if (IsPlaneLike) return "Planar";
        if (IsLineLike) return "Linear";
        return "GeneralMesh";
    }

    // =========================================================================
    // Helpers
    // =========================================================================

    public static double SafeRatio(double numerator, double denominator)
        => Math.Abs(denominator) > Eps ? numerator / denominator : double.NaN;

    public static double BoxSurfaceArea(Vector3 size)
        => 2.0 * (size.X * size.Y + size.X * size.Z + size.Y * size.Z);

    public static double MaxComponent(Vector3 v)
        => Math.Max(v.X, Math.Max(v.Y, v.Z));

    public static double MinComponent(Vector3 v)
        => Math.Min(v.X, Math.Min(v.Y, v.Z));

    public static double MinNonZeroComponent(Vector3 v)
    {
        var min = double.PositiveInfinity;

        if (Math.Abs(v.X) > Eps) min = Math.Min(min, Math.Abs(v.X));
        if (Math.Abs(v.Y) > Eps) min = Math.Min(min, Math.Abs(v.Y));
        if (Math.Abs(v.Z) > Eps) min = Math.Min(min, Math.Abs(v.Z));

        return double.IsPositiveInfinity(min) ? double.NaN : min;
    }

    public static int TryInt(Func<int> f)
    {
        try
        {
            return f();
        }
        catch
        {
            return 0;
        }
    }

    public static double Clamp01(double x)
        => double.IsNaN(x) ? 0.0 : Math.Max(0.0, Math.Min(1.0, x));

    public static double AverageFinite(params double[] xs)
        => AverageFinite((IEnumerable<double>)xs);

    public static double AverageFinite(IEnumerable<double> xs)
    {
        var sum = 0.0;
        var count = 0;

        foreach (var x in xs)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
                continue;

            sum += x;
            count++;
        }

        return count > 0 ? sum / count : double.NaN;
    }

    public static double EntropyTerm(double x)
        => x > Eps ? -x * Math.Log(x) : 0.0;

    public static double PcaRadialDistanceYZ(Point3D p)
    {
        var y = p.Vector3.Y;
        var z = p.Vector3.Z;
        return Math.Sqrt(y * y + z * z);
    }

    public double PointRatioNearX(double x, double tolerance)
    {
        if (NormalizedPoints.Count == 0 || tolerance <= Eps)
            return double.NaN;

        var count = 0;

        foreach (var p in NormalizedPoints)
        {
            if (Math.Abs(p.Vector3.X - x) <= tolerance)
                count++;
        }

        return SafeRatio(count, NormalizedPoints.Count);
    }

    public static double MinFinite(IEnumerable<double> xs)
    {
        var min = double.PositiveInfinity;

        foreach (var x in xs)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
                continue;

            min = Math.Min(min, x);
        }

        return double.IsPositiveInfinity(min) ? double.NaN : min;
    }

    public static double MaxFinite(IEnumerable<double> xs)
    {
        var max = double.NegativeInfinity;

        foreach (var x in xs)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
                continue;

            max = Math.Max(max, x);
        }

        return double.IsNegativeInfinity(max) ? double.NaN : max;
    }

    public static double StdDevFinite(IEnumerable<double> xs)
    {
        var values = xs
            .Where(x => !double.IsNaN(x) && !double.IsInfinity(x))
            .ToArray();

        if (values.Length == 0)
            return double.NaN;

        var avg = values.Average();
        var sumSq = 0.0;

        foreach (var x in values)
        {
            var d = x - avg;
            sumSq += d * d;
        }

        return Math.Sqrt(sumSq / values.Length);
    }

    // =========================================================================
    // Suggestions
    // =========================================================================
    //
    // 1. Split this class into nested groups or separate feature records:
    //    CountFeatures, AabbFeatures, ObbFeatures, SurfaceFeatures,
    //    VertexDistributionFeatures, PcaFeatures, NormalFeatures,
    //    TopologyFeatures, TessellationFeatures, and ClassificationFeatures.
    //    The current single flat class is convenient for tabular export, but it is
    //    becoming hard to maintain.
    //
    // 2. Consider generating the scalar pass-through properties automatically.
    //    Many properties are just Vector3 component expansions for table export.
    //    A descriptor-based export layer could expand Vector3 fields to X/Y/Z
    //    without requiring hand-written duplicate properties.
    //
    // 3. Move thresholds into a MeshFeatureThresholds options object.
    //    Values such as 0.75, 0.85, 20.0, and 10000.0 are domain assumptions.
    //    Making them configurable will make the feature set more useful across
    //    BIM, CAD, scan meshes, mechanical parts, and visualization assets.
    //
    // 4. Prefer explicit names over aliases in new code.
    //    For example, prefer AabbVolume over BoundsVolume, FaceNormalAverage over
    //    MeanNormal, and PcaPrincipalAxisVerticality over Verticality. The aliases
    //    are useful for compatibility but can eventually be removed.
    //
    // 5. Cache expensive topology-derived values if this class is accessed through
    //    data binding or table export. Properties such as BoundaryEdgeCount,
    //    NonManifoldEdgeCount, and BoundaryLoopCount may enumerate or allocate.
    //
    // 6. Avoid catch-all TryInt for expected geometry states.
    //    It is useful for exploratory analytics, but production code should
    //    distinguish "not computed", "invalid topology", and "unexpected bug".
    //
    // 7. Add unit-aware metadata. Features such as area, volume, length, density,
    //    and elevation should carry unit semantics, especially when exported from
    //    Revit, IFC, GLB, or mixed source models.
    //
    // 8. Add percentile-based statistics. For skewed meshes, P05/P50/P95 edge
    //    lengths, face areas, triangle aspect ratios, and vertex elevations are
    //    often more robust than min/max.
    //
    // 9. Add per-axis and oriented footprint features. AabbFootprintArea is useful,
    //    but OBB footprint, convex-hull footprint, and projected XY area may be
    //    better for BIM classification.
    //
    // 10. Add domain-specific BIM heuristics only if they are clearly separated
    //     from raw features. For example: IsWallLike, IsFloorLike, IsColumnLike,
    //     IsBeamLike, IsPipeLike, IsDuctLike, IsPanelLike, and IsFurnitureLike.
    //
    // 11. Remove or de-emphasize weak features if they do not help downstream
    //     models. Examples to validate empirically: BoxLikeScore, SurfaceLikeScore,
    //     SolidLikeScore, VerticalFacingScore, and SurfaceCompactness.
    //
    // 12. Consider adding feature provenance and reliability flags. Some features
    //     are reliable for any mesh, while others only make sense for closed,
    //     manifold, consistently oriented triangle meshes.
}
