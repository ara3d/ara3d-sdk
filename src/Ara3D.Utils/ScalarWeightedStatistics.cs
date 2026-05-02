using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ara3D.Utils;

public class ScalarWeightedStatistics
{
    public readonly int Count;
    public readonly int ValidCount;
    public readonly int NanCount;
    public readonly int InfinityCount;
    public readonly int ZeroWeightCount;
    public readonly int NegativeWeightCount;

    public bool HasInvalidValues => NanCount > 0 || InfinityCount > 0 || NegativeWeightCount > 0;

    public readonly double TotalWeight;
    public readonly double Sum;
    public readonly double SumOfSquares;
    public readonly double Average = double.NaN;
    public readonly double Min = double.PositiveInfinity;
    public readonly double Max = double.NegativeInfinity;
    public readonly double Range = double.NaN;

    public readonly double SumAbsoluteDeviation = double.NaN;
    public readonly double SumSquaredDeviation = double.NaN;
    public readonly double MeanAbsoluteDeviation = double.NaN;

    public readonly double PopulationVariance = double.NaN;
    public readonly double PopulationStdDev = double.NaN;

    public double Variance => PopulationVariance;
    public double StdDev => PopulationStdDev;

    public readonly double RootMeanSquare = double.NaN;
    public readonly double CoefficientOfVariation = double.NaN;

    public readonly double Minus3StdDev = double.NaN;
    public readonly double Plus3StdDev = double.NaN;

    public readonly double Skewness = double.NaN;
    public readonly double Kurtosis = double.NaN;

    public bool IsEmpty => ValidCount == 0 || TotalWeight == 0.0;
    public bool IsSingleValue => ValidCount == 1;
    public bool IsConstant => ValidCount > 0 && Range == 0.0;

    public ScalarWeightedStatistics(
        IReadOnlyList<double> values,
        IReadOnlyList<double> weights,
        bool singlePassStatsOnly = false)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(weights);

        if (values.Count != weights.Count)
            throw new ArgumentException("Values and weights must have the same count.");

        for (var i = 0; i < values.Count; i++)
        {
            Count++;

            var value = values[i];
            var weight = weights[i];

            if (double.IsNaN(value) || double.IsNaN(weight))
            {
                NanCount++;
                continue;
            }

            if (double.IsInfinity(value) || double.IsInfinity(weight))
            {
                InfinityCount++;
                continue;
            }

            if (weight < 0.0)
            {
                NegativeWeightCount++;
                continue;
            }

            if (weight == 0.0)
            {
                ZeroWeightCount++;
                continue;
            }

            ValidCount++;
            TotalWeight += weight;

            Sum += weight * value;
            SumOfSquares += weight * value * value;

            if (value < Min) Min = value;
            if (value > Max) Max = value;
        }

        if (IsEmpty)
            return;

        Average = Sum / TotalWeight;
        Range = Max - Min;
        RootMeanSquare = Math.Sqrt(SumOfSquares / TotalWeight);

        if (singlePassStatsOnly)
            return;

        var absDev = 0.0;
        var sqDev = 0.0;
        var thirdMoment = 0.0;
        var fourthMoment = 0.0;

        for (var i = 0; i < values.Count; i++)
        {
            var value = values[i];
            var weight = weights[i];

            if (!double.IsFinite(value) || !double.IsFinite(weight))
                continue;

            if (weight <= 0.0)
                continue;

            var d = value - Average;
            var d2 = d * d;

            absDev += weight * Math.Abs(d);
            sqDev += weight * d2;
            thirdMoment += weight * d2 * d;
            fourthMoment += weight * d2 * d2;
        }

        SumAbsoluteDeviation = absDev;
        SumSquaredDeviation = sqDev;
        MeanAbsoluteDeviation = absDev / TotalWeight;

        PopulationVariance = sqDev / TotalWeight;
        PopulationStdDev = Math.Sqrt(PopulationVariance);

        Minus3StdDev = Average - 3.0 * PopulationStdDev;
        Plus3StdDev = Average + 3.0 * PopulationStdDev;

        if (Average != 0.0)
            CoefficientOfVariation = PopulationStdDev / Math.Abs(Average);

        if (PopulationStdDev > 0.0)
        {
            var std3 = PopulationStdDev * PopulationStdDev * PopulationStdDev;
            var std4 = std3 * PopulationStdDev;

            Skewness = thirdMoment / TotalWeight / std3;
            Kurtosis = fourthMoment / TotalWeight / std4 - 3.0;
        }

        Debug.Assert(ValidCount <= Count);
        Debug.Assert(IsEmpty || Min <= Max);
        Debug.Assert(TotalWeight >= 0.0);
    }
}