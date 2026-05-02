using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ara3D.Utils;

public class ScalarStatistics
{
    public readonly int Count;
    public readonly int ValidCount;
    public readonly int NanCount;
    public readonly int InfinityCount;

    public bool HasInvalidValues => NanCount > 0 || InfinityCount > 0;

    public readonly double Sum;
    public readonly double SumOfSquares;
    public readonly double Average = double.NaN;
    public readonly double Min = double.PositiveInfinity;
    public readonly double Max = double.NegativeInfinity;
    public readonly double Range = double.NaN;

    public readonly bool OrderedAscending = true;
    public readonly bool OrderedDescending = true;

    public readonly double SumAbsoluteDeviation = double.NaN;
    public readonly double SumSquaredDeviation = double.NaN;
    public readonly double MeanAbsoluteDeviation = double.NaN;

    public readonly double PopulationVariance = double.NaN;
    public readonly double SampleVariance = double.NaN;
    public double Variance => SampleVariance;

    public readonly double PopulationStdDev = double.NaN;
    public readonly double SampleStdDev = double.NaN;
    public double StdDev => SampleStdDev;

    public readonly double RootMeanSquare = double.NaN;
    public readonly double CoefficientOfVariation = double.NaN;

    public readonly double Minus3StdDev = double.NaN;
    public readonly double Plus3StdDev = double.NaN;

    public readonly double Skewness = double.NaN;
    public readonly double Kurtosis = double.NaN;

    public bool IsEmpty => ValidCount == 0;
    public bool IsSingleValue => ValidCount == 1;
    public bool IsConstant => ValidCount > 0 && Range == 0.0;

    public double SumOfError => SumAbsoluteDeviation;
    public double SumOfError2 => SumSquaredDeviation;

    public ScalarStatistics(IReadOnlyList<double> values, bool singlePassStatsOnly = false)
    {
        ArgumentNullException.ThrowIfNull(values);

        var hasPrevious = false;
        var previous = 0.0;

        foreach (var value in values)
        {
            Count++;

            if (double.IsNaN(value))
            {
                NanCount++;
                continue;
            }

            if (double.IsInfinity(value))
            {
                InfinityCount++;
                continue;
            }

            ValidCount++;

            Sum += value;
            SumOfSquares += value * value;

            if (value < Min) Min = value;
            if (value > Max) Max = value;

            if (hasPrevious)
            {
                if (value < previous) OrderedAscending = false;
                if (value > previous) OrderedDescending = false;
            }

            previous = value;
            hasPrevious = true;
        }

        if (ValidCount == 0)
            return;

        Average = Sum / ValidCount;
        Range = Max - Min;
        RootMeanSquare = Math.Sqrt(SumOfSquares / ValidCount);

        if (singlePassStatsOnly)
            return;

        var absDev = 0.0;
        var sqDev = 0.0;
        var thirdMoment = 0.0;
        var fourthMoment = 0.0;

        foreach (var value in values)
        {
            if (!double.IsFinite(value))
                continue;

            var d = value - Average;
            var d2 = d * d;

            absDev += Math.Abs(d);
            sqDev += d2;
            thirdMoment += d2 * d;
            fourthMoment += d2 * d2;
        }

        SumAbsoluteDeviation = absDev;
        SumSquaredDeviation = sqDev;
        MeanAbsoluteDeviation = absDev / ValidCount;

        PopulationVariance = sqDev / ValidCount;
        PopulationStdDev = Math.Sqrt(PopulationVariance);

        if (ValidCount >= 2)
        {
            SampleVariance = sqDev / (ValidCount - 1);
            SampleStdDev = Math.Sqrt(SampleVariance);

            Minus3StdDev = Average - 3.0 * SampleStdDev;
            Plus3StdDev = Average + 3.0 * SampleStdDev;
        }

        if (Average != 0.0 && !double.IsNaN(SampleStdDev))
            CoefficientOfVariation = SampleStdDev / Math.Abs(Average);

        if (ValidCount >= 3 && SampleStdDev > 0.0)
        {
            var n = (double)ValidCount;
            var s3 = SampleStdDev * SampleStdDev * SampleStdDev;
            Skewness = n / ((n - 1.0) * (n - 2.0)) * thirdMoment / s3;
        }

        if (ValidCount >= 4 && sqDev > 0.0)
        {
            var n = (double)ValidCount;
            var sqDev2 = sqDev * sqDev;

            Kurtosis =
                ((n + 1.0) * n * (n - 1.0)) /
                ((n - 2.0) * (n - 3.0)) *
                (fourthMoment / sqDev2)
                - 3.0 * Math.Pow(n - 1.0, 2.0) /
                ((n - 2.0) * (n - 3.0));
        }

        Debug.Assert(ValidCount <= Count);
        Debug.Assert(IsEmpty || Min <= Max);
    }

    public double Normalize(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return double.NaN;
        if (Range == 0.0)
            return 0.0;
        return (value - Min) / Range;
    }
}