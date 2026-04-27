namespace Ara3D.Studio.Samples;

public sealed class Vector3Statistics
{
    public ScalarStatistics X { get; }
    public ScalarStatistics Y { get; }
    public ScalarStatistics Z { get; }

    public double SumX => X.Sum;
    public double SumY => Y.Sum;
    public double SumZ => Z.Sum;

    public double AverageX => X.Average;
    public double AverageY => Y.Average;
    public double AverageZ => Z.Average;

    public Vector3 Min => ((float)X.Min, (float)Y.Min, (float)Z.Min);
    public Vector3 Max => ((float)X.Max, (float)Y.Max, (float)Z.Max);
    public Vector3 Average => ((float)X.Average, (float)Y.Average, (float)Z.Average);
    public Vector3 Median => ((float)X.Median, (float)Y.Median, (float)Z.Median);
    public Vector3 Variance => ((float)X.Variance, (float)Y.Variance, (float)Z.Variance);

    public Bounds3D Bounds => (Min, Max);

    public int Count => X.Count;
    public bool MultiPassStatistics => X.MultiPassStats;
    public bool OrderedStatistics => X.OrderedStats;

    public Vector3Statistics(IReadOnlyList<Vector3> values, bool multiPassStats = true, bool orderedStats = true)
    {
        X = new ScalarStatistics(values.Select(v => (double)v.X), multiPassStats, orderedStats);
        Y = new ScalarStatistics(values.Select(v => (double)v.Y), multiPassStats, orderedStats);
        Z = new ScalarStatistics(values.Select(v => (double)v.Z), multiPassStats, orderedStats);
    }

    public Vector3 Minus3StdDev =>
        ((float)X.Minus3StdDev, (float)Y.Minus3StdDev, (float)Z.Minus3StdDev);

    public Vector3 Plus3StdDev =>
        ((float)X.Plus3StdDev, (float)Y.Plus3StdDev, (float)Z.Plus3StdDev);

    public Vector3 FirstQuartile =>
        ((float)X.FirstQuartile, (float)Y.FirstQuartile, (float)Z.FirstQuartile);

    public Vector3 ThirdQuartile =>
        ((float)X.ThirdQuartile, (float)Y.ThirdQuartile, (float)Z.ThirdQuartile);

    public Vector3 First5Percent =>
        ((float)X.First5Percent, (float)Y.First5Percent, (float)Z.First5Percent);

    public Vector3 Last5Percent =>
        ((float)X.Last5Percent, (float)Y.Last5Percent, (float)Z.Last5Percent);

    public Vector3 Skewness =>
        ((float)X.Skewness, (float)Y.Skewness, (float)Z.Skewness);

    public Vector3 Kurtosis =>
        ((float)X.Kurtosis, (float)Y.Kurtosis, (float)Z.Kurtosis);
}