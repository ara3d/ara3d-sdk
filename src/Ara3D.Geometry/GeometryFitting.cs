namespace Ara3D.Geometry;

public static class GeometryFitting
{
    public static Cylinder FitCylinder(this OrientedBox3D obb, IReadOnlyList<Vector3> normals, CylinderRadiusFit fit = CylinderRadiusFit.Enclosing)
    {
        if (normals == null || normals.Count == 0)
            return obb.FitCylinder();

        var axes = new[]
        {
            obb.Frame.X,
            obb.Frame.Y,
            obb.Frame.Z
        };

        var sizes = new[]
        {
            (double)obb.Size.X,
            (double)obb.Size.Y,
            (double)obb.Size.Z
        };

        var bestAxisIndex = -1;
        var bestScore = double.NegativeInfinity;

        for (var i = 0; i < 3; i++)
        {
            var axis = axes[i];

            var sideCount = 0;
            var capCount = 0;
            var validCount = 0;

            foreach (var n in normals)
            {
                if (!n.IsFinite())
                    continue;

                if (n.LengthSquared() <= 1e-12f)
                    continue;

                validCount++;

                if (n.IsMostlyPerpendicularTo(axis))
                    sideCount++;

                if (n.IsMostlyParallelTo(axis))
                    capCount++;
            }

            if (validCount == 0)
                continue;

            var sideRatio = (double)sideCount / validCount;
            var capRatio = (double)capCount / validCount;

            var (r0, r1) = GetOtherAxes(i);

            var radialSize0 = sizes[r0];
            var radialSize1 = sizes[r1];

            var radialSimilarity = GetRadialSimilarity(radialSize0, radialSize1);

            // For a cylinder:
            // - most side normals should be perpendicular to the axis
            // - optional cap normals may be parallel to the axis
            // - the two radial OBB sizes should be similar
            var score =
                0.70 * sideRatio +
                0.10 * capRatio +
                0.20 * radialSimilarity;

            if (score > bestScore)
            {
                bestScore = score;
                bestAxisIndex = i;
            }
        }

        if (bestAxisIndex < 0)
            throw new InvalidOperationException("Could not determine a cylinder axis from the supplied normals.");

        var bestAxis = axes[bestAxisIndex].Normalize;
        var length = sizes[bestAxisIndex];

        var (d0, d1) = obb.Size.GetOtherComponents(bestAxisIndex);

        var radius = fit switch
        {
            CylinderRadiusFit.Average =>
                0.25 * (d0 + d1),

            CylinderRadiusFit.Larger =>
                0.5 * Math.Max(d0, d1),

            CylinderRadiusFit.Enclosing =>
                0.5 * Math.Sqrt(d0 * d0 + d1 * d1),

            _ => throw new ArgumentOutOfRangeException(nameof(fit), fit, null)
        };
        
        if (!double.IsFinite(length) || length <= 0)
            throw new InvalidOperationException("The oriented box has an invalid cylinder length.");

        if (!double.IsFinite(radius) || radius <= 0)
            throw new InvalidOperationException("The oriented box has an invalid cylinder radius.");

        var center = obb.Frame.Origin;
        var halfAxis = bestAxis * (float)(length * 0.5);

        var line = new Line3D(
            center - halfAxis,
            center + halfAxis);

        return new Cylinder(line, (float)radius);
    }

    private static (int A, int B) GetOtherAxes(int axisIndex)
        => axisIndex switch
        {
            0 => (1, 2),
            1 => (0, 2),
            2 => (0, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(axisIndex))
        };

    private static double GetRadialSimilarity(double a, double b)
    {
        if (!double.IsFinite(a) || !double.IsFinite(b))
            return 0.0;

        if (a <= 0 || b <= 0)
            return 0.0;

        var min = Math.Min(a, b);
        var max = Math.Max(a, b);

        return min / max;
    }

    public enum CylinderRadiusFit
    {
        Average,
        Larger,
        Enclosing
    }

    public static Cylinder FitCylinder(this OrientedBox3D obb, CylinderRadiusFit fit = CylinderRadiusFit.Enclosing)
    {
        var axisIndex = obb.Size.GetLongestAxisIndex();

        var axis = axisIndex switch
        {
            0 => obb.Frame.X,
            1 => obb.Frame.Y,
            2 => obb.Frame.Z,
            _ => throw new InvalidOperationException()
        };

        var length = axisIndex switch
        {
            0 => (double)obb.Size.X,
            1 => (double)obb.Size.Y,
            2 => (double)obb.Size.Z,
            _ => throw new InvalidOperationException()
        };

        var (d0, d1) = obb.Size.GetOtherComponents(axisIndex);

        var radius = fit switch
        {
            CylinderRadiusFit.Average =>
                0.25 * (d0 + d1),

            CylinderRadiusFit.Larger =>
                0.5 * Math.Max(d0, d1),

            CylinderRadiusFit.Enclosing =>
                0.5 * Math.Sqrt(d0 * d0 + d1 * d1),

            _ => throw new ArgumentOutOfRangeException(nameof(fit), fit, null)
        };

        if (!axis.IsFinite() || axis.LengthSquared() <= 1e-12f)
            throw new InvalidOperationException("The oriented box has an invalid primary axis.");

        if (!double.IsFinite(length) || length <= 0)
            throw new InvalidOperationException("The oriented box has an invalid cylinder length.");

        if (!double.IsFinite(radius) || radius <= 0 || radius > float.MaxValue)
            throw new InvalidOperationException("The oriented box has an invalid cylinder radius.");

        var center = obb.Frame.Origin.Vector3;

        var halfAxis = axis.Normalize * (float)(length * 0.5);

        return new(
            (center - halfAxis, center + halfAxis),
            (float)radius);
    }
}