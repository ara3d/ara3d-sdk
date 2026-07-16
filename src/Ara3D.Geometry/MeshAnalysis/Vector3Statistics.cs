using Ara3D.Utils;

namespace Ara3D.Geometry;

public sealed class Vector3Statistics
{
    public ScalarStatistics X { get; }
    public ScalarStatistics Y { get; }
    public ScalarStatistics Z { get; }
    public ScalarStatistics Lengths { get; }

    public int Count { get; }

    public Vector3 Sum => ((float)X.Sum, (float)Y.Sum, (float)Z.Sum);
    public Vector3 Min => ((float)X.Min, (float)Y.Min, (float)Z.Min);
    public Vector3 Max => ((float)X.Max, (float)Y.Max, (float)Z.Max);
    public Vector3 Average => ((float)X.Average, (float)Y.Average, (float)Z.Average);

    public Vector3 Range => Max - Min;
    public Vector3 Center => (Min + Max) * 0.5f;
    public Bounds3D Bounds => (Min, Max);

    public Vector3 Variance => ((float)X.Variance, (float)Y.Variance, (float)Z.Variance);
    public Vector3 PopulationVariance => ((float)X.PopulationVariance, (float)Y.PopulationVariance, (float)Z.PopulationVariance);
    public Vector3 SampleVariance => ((float)X.SampleVariance, (float)Y.SampleVariance, (float)Z.SampleVariance);

    public Vector3 StdDev => ((float)X.StdDev, (float)Y.StdDev, (float)Z.StdDev);
    public Vector3 PopulationStdDev => ((float)X.PopulationStdDev, (float)Y.PopulationStdDev, (float)Z.PopulationStdDev);
    public Vector3 SampleStdDev => ((float)X.SampleStdDev, (float)Y.SampleStdDev, (float)Z.SampleStdDev);

    public Vector3 RootMeanSquare => ((float)X.RootMeanSquare, (float)Y.RootMeanSquare, (float)Z.RootMeanSquare);
    public Vector3 MeanAbsoluteDeviation => ((float)X.MeanAbsoluteDeviation, (float)Y.MeanAbsoluteDeviation, (float)Z.MeanAbsoluteDeviation);

    public Vector3 Minus3StdDev => ((float)X.Minus3StdDev, (float)Y.Minus3StdDev, (float)Z.Minus3StdDev);
    public Vector3 Plus3StdDev => ((float)X.Plus3StdDev, (float)Y.Plus3StdDev, (float)Z.Plus3StdDev);

    public Vector3 Skewness => ((float)X.Skewness, (float)Y.Skewness, (float)Z.Skewness);
    public Vector3 Kurtosis => ((float)X.Kurtosis, (float)Y.Kurtosis, (float)Z.Kurtosis);

    public bool HasInvalidValues => X.HasInvalidValues || Y.HasInvalidValues || Z.HasInvalidValues;

    public bool IsEmpty => Count == 0;
    public bool IsConstant => X.IsConstant && Y.IsConstant && Z.IsConstant;

    public Vector3Statistics(IReadOnlyList<Vector3> values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        Count = values.Count;
        X = new ScalarStatistics(values.Map(v => (double)v.X));
        Y = new ScalarStatistics(values.Map(v => (double)v.Y));
        Z = new ScalarStatistics(values.Map(v => (double)v.Z));

        Lengths = new ScalarStatistics(values.Map(v => (double)v.Length()));
    }
}