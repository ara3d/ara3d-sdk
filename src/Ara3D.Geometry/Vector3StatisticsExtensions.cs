namespace Ara3D.Geometry;

public static class Vector3StatisticsExtensions
{
    public static Vector3Statistics GetStatistics(this IEnumerable<Vector3> values)
        => new(values.ToList());

    public static Vector3Statistics GetStatistics(this IEnumerable<Point3D> values)
        => values.Select(p => p.Vector3).ToList().GetStatistics();

    public static Vector3Statistics GetStatistics(this IReadOnlyList<Vector3> values)
        => new(values);

    public static Vector3Statistics GetStatistics(this IReadOnlyList<Point3D> values)
        => values.Map(p => p.Vector3).GetStatistics();

    public static Vector3WeightedStatistics GetStatistics(this IReadOnlyList<Vector3> values, IReadOnlyList<double> weights)
        => new(values, weights);

    public static Vector3WeightedStatistics GetStatistics(this IReadOnlyList<Point3D> values, IReadOnlyList<double> weights)
        => values.Map(p => p.Vector3).GetStatistics(weights);
}