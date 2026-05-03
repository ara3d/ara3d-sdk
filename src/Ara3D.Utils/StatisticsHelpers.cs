using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Utils;

public static class StatisticsHelpers
{
    public static ScalarStatistics Statistics<T>(this IEnumerable<T> self)
        => new(self.Select(x => Convert.ToDouble(x)).ToList());

    public static ScalarWeightedStatistics WeightedStatistics(
        this IReadOnlyList<double> values,
        IReadOnlyList<double> weights)
        => new(values, weights);

    public static double Percentile(this IReadOnlyList<double> sortedNumbers, double percent)
    {
        if (sortedNumbers.Count == 0)
            return double.NaN;

        var n = (sortedNumbers.Count - 1) * Math.Clamp(percent, 0.0, 100.0) / 100.0;

        var lowerPos = (int)Math.Floor(n);
        var upperPos = (int)Math.Ceiling(n);

        var lowerValue = sortedNumbers[lowerPos];
        var upperValue = sortedNumbers[upperPos];

        if (lowerPos == upperPos)
            return lowerValue;

        var fraction = n - lowerPos;
        return lowerValue + fraction * (upperValue - lowerValue);
    }

    public static double Median(this IReadOnlyList<double> sortedNumbers)
        => sortedNumbers.Percentile(50);

    public static string StatisticsSummaryReport<T>(this IEnumerable<T> values)
    {
        var stats = values.Statistics();
        return $"count = {stats.Count}, sum = {stats.Sum}, avg = {stats.Average}, min = {stats.Min}, max = {stats.Max}, dev = {stats.StdDev}";
    }

}