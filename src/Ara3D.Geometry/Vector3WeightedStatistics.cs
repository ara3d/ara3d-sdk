using Ara3D.Utils;

namespace Ara3D.Geometry;

public sealed class Vector3WeightedStatistics
{
    public ScalarWeightedStatistics X { get; }
    public ScalarWeightedStatistics Y { get; }
    public ScalarWeightedStatistics Z { get; }
    public ScalarWeightedStatistics Lengths { get; }

    public int Count { get; }
    public double TotalWeight => X.TotalWeight;

    public Vector3 Average => ((float)X.Average, (float)Y.Average, (float)Z.Average);
    public Vector3 Min => ((float)X.Min, (float)Y.Min, (float)Z.Min);
    public Vector3 Max => ((float)X.Max, (float)Y.Max, (float)Z.Max);
    public Vector3 Range => Max - Min;
    public Vector3 Center => (Min + Max) * 0.5f;
    public Bounds3D Bounds => (Min, Max);

    public Vector3 Variance => ((float)X.Variance, (float)Y.Variance, (float)Z.Variance);
    public Vector3 StdDev => ((float)X.StdDev, (float)Y.StdDev, (float)Z.StdDev);
    public Vector3 RootMeanSquare => ((float)X.RootMeanSquare, (float)Y.RootMeanSquare, (float)Z.RootMeanSquare);
    public Vector3 MeanAbsoluteDeviation => ((float)X.MeanAbsoluteDeviation, (float)Y.MeanAbsoluteDeviation, (float)Z.MeanAbsoluteDeviation);
    
    public Vector3 Skewness => ((float)X.Skewness, (float)Y.Skewness, (float)Z.Skewness);
    public Vector3 Kurtosis => ((float)X.Kurtosis, (float)Y.Kurtosis, (float)Z.Kurtosis);

    public bool HasInvalidValues => X.HasInvalidValues || Y.HasInvalidValues || Z.HasInvalidValues;

    public bool IsEmpty => Count == 0 || TotalWeight == 0.0;
    public bool IsConstant => X.IsConstant && Y.IsConstant && Z.IsConstant;

    public Vector3WeightedStatistics(IReadOnlyList<Vector3> values, IReadOnlyList<double> weights)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        if (weights == null)
            throw new ArgumentNullException(nameof(weights));

        if (values.Count != weights.Count)
            throw new ArgumentException("Values and weights must have the same count.", nameof(weights));

        Count = values.Count;

        X = new ScalarWeightedStatistics(values.Map(v => (double)v.X), weights);
        Y = new ScalarWeightedStatistics(values.Map(v => (double)v.Y), weights);
        Z = new ScalarWeightedStatistics(values.Map(v => (double)v.Z), weights);

        Lengths = new ScalarWeightedStatistics(values.Map(v => (double)v.Length()), weights);
    }
}