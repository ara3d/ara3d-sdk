using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ara3D.Utils
{
    // https://stackoverflow.com/questions/3141692/standard-deviation-of-generic-list
    // https://www.codeproject.com/Articles/27340/%2FArticles%2F27340%2FA-User-Friendly-C-Descriptive-Statistic-Class   
    public class Statistics
    {
        // First statistics are computed in a single pass 
        public readonly double Average;
        public readonly double Min = double.MaxValue;
        public readonly double Max = double.MinValue;
        public readonly double Range;
        public readonly double Sum;
        public readonly long Count;
        public readonly bool OrderedAscending;
        public readonly bool OrderedDescending;

        // Additional statistics computed when doing multiple passes 
        public readonly bool MultiPassStats;
        public readonly double Skewness;
        public readonly double Kurtosis;
        public readonly double SumOfError;
        public readonly double SumOfError2;
        public readonly double Variance;
        public readonly double StandardDeviation;
        public readonly double Minus3StdDev;
        public readonly double Plus3StdDev;

        // More expensive to compute statistics that are computed optionally only when 
        // ordered statistics is true, and MultiPass is true
        public readonly bool OrderedStats;
        public readonly double Median;
        public readonly double FirstQuartile;
        public readonly double ThirdQuartile;
        public readonly double First5Percent;
        public readonly double Last5Percent;

        /// <summary>
        /// Computes values from an IEnumerable. Only some statistics are computed if orderedStatistics 
        /// is false, or multiPass is true.
        /// </summary>
        public Statistics(IEnumerable<double> values, bool multiPassStats = true, bool orderedStats = true)
        {
            MultiPassStats = multiPassStats;
            OrderedStats = orderedStats;

            var prev = 0.0;
            OrderedAscending = true;
            OrderedDescending = true;
            var first = true;
            foreach (var value in values)
            {
                Count++;
                Sum += value;
                Min = Math.Min(Min, value);
                Max = Math.Max(Max, value);
                if (!first && value < prev)
                    OrderedAscending = false;
                if (!first && value > prev)
                    OrderedDescending = false;
                prev = value;
                first = false;
            }

            Average = Sum / Count;
            Range = Max - Min;

            if (!multiPassStats)
                return;

            var moment1 = 0.0;
            var moment2 = 0.0;
            var moment3 = 0.0;
            var moment4 = 0.0;
            foreach (var value in values)
            {
                var m = value - Average;
                var m2 = m * m;
                var m3 = m2 * m;
                var m4 = m3 * m;

                moment1 += Math.Abs(m);
                moment2 += m2;
                moment3 += m3;
                moment4 += m4;
            }

            SumOfError = moment1;
            SumOfError2 = moment2;
            Variance = SumOfError2 / (Count - 1);
            StandardDeviation = Math.Sqrt(Variance);
            Minus3StdDev = Average - 3 * StandardDeviation;
            Plus3StdDev = Average + 3 * StandardDeviation;

            // using Excel approach
            var cumulativeSkew
                = values.Select(x => Math.Pow((x - Average) / StandardDeviation, 3)).Sum();

            var n = (double)Count;
            Skewness = n / (n - 1) / (n - 2) * cumulativeSkew;

            // kurtosis: see http://en.wikipedia.org/wiki/Kurtosis 
            var m2_2 = Math.Pow(SumOfError2, 2);
            Kurtosis = ((n + 1) * n * (n - 1)) / ((n - 2) * (n - 3)) *
                (moment4 / m2_2) -
                3 * Math.Pow(n - 1, 2) / ((n - 2) * (n - 3)); // second last formula for G2

            // If not computing ordered statistics we exit earlier.
            if (!orderedStats)
                return;

            var sortedNumbers = values.OrderBy(x => x).ToList();
            Median = sortedNumbers.Percentile(50);
            FirstQuartile = sortedNumbers.Percentile(25);
            ThirdQuartile = sortedNumbers.Percentile(75);
            First5Percent = sortedNumbers.Percentile(5);
            Last5Percent = sortedNumbers.Percentile(95);
        }
    }

    /// <summary>
    /// Helper functions 
    /// </summary>
    public static class StatisticHelpers
    {
        /// <summary>
        /// Computes common statistics from a collection of values that can be converted to doubles
        /// </summary>
        public static Statistics Statistics<T>(this IEnumerable<T> self)
            // https://stackoverflow.com/questions/3343551/how-to-cast-a-value-of-generic-type-t-to-double-without-boxing
            // OPTIMIZATION: this can be made much faster by avoiding boxing.
            // https://stackoverflow.com/questions/24259261/avoiding-boxing-in-generic-blackboard
            // https://blogs.msdn.microsoft.com/bclteam/2005/03/15/avoiding-boxing-in-classes-implementing-generic-interfaces-through-reflection-dave-fetterman/
            => new Statistics(self.Select(x => Convert.ToDouble(x)));

        /// <summary>
        /// Given a sorted list returns the value at the x% position (50% is the median)
        /// This is a generalization of the median 
        /// </summary>
        public static double Percentile(this IReadOnlyList<double> sortedNumbers, float percent)
        {
            if (sortedNumbers.Count == 0)
                throw new Exception("Empty list");
            var n = (sortedNumbers.Count - 1) * percent / 100.0;

            // Clamping
            if (n < 0) n = 0;
            if (n >= sortedNumbers.Count) n = sortedNumbers.Count - 1;
            var lowerPos = (int)Math.Floor(n);
            var upperPos = (int)Math.Ceiling(n);

            var lowerValue = sortedNumbers[lowerPos];
            var upperValue = sortedNumbers[upperPos];

            if (lowerPos == upperPos)
                return lowerValue;

            var fraction = n - lowerPos;
            return lowerValue + fraction * (upperValue - lowerValue);
        }

        /// <summary>
        /// Given a sorted list returns the value at the x% position. If the list is odd, 
        /// this should return the value in the middle of the list otherwise it will
        /// return a value part way between the middle. 
        /// Throws an exception if the list does not have at least one value.
        /// </summary>
        public static double Median(this IReadOnlyList<double> sortedNumbers)
            => sortedNumbers.Percentile(50);

        /// <summary>
        /// Returns a string summary of the statistics 
        /// </summary>
        public static string StatisticsSummaryReport<T>(this IEnumerable<T> values)
        {
            var stats = values.Statistics();
            return $"count = {stats.Count}, sum = {stats.Sum}, avg = {stats.Average}, min = {stats.Min}, max = {stats.Max}, dev = {stats.StandardDeviation}";
        }

        public static Statistics[] GetComponentStatistics<T>(
            this T[] values,
            Func<T, double>[] componentAccessors)
        {
            var stats = new Statistics[componentAccessors.Length];
            Parallel.For(0, stats.Length, i =>
                stats[i] = new Statistics(values.Select(componentAccessors[i])));
            return stats;
        }
    }
}
