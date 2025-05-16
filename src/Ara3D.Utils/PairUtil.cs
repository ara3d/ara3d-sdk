using System;

namespace Ara3D.Utils
{
    public static class PairUtil
    {
        public static (T1, T2) PairWith<T1, T2>(this T1 first, T2 second)
            => (first, second);

        public static (T2, T1) Swap<T1, T2>(this (T1, T2) pair)
            => (pair.Item2, pair.Item1);

        public static (T, T) Order<T>(this (T, T) pair) where T : IComparable<T> =>
            pair.Item1.CompareTo(pair.Item2) <= 0 ? pair : pair.Swap();

        public static (T2, T2) Select<T1, T2>(this (T1, T1) pair, Func<T1, T2> f)
            => (f(pair.Item1), f(pair.Item2));

        public static string ToDelimitedString<T1, T2>(this (T1, T2) pair, string delim = ":")
            => $"{pair.Item1}{delim}{pair.Item2}";

        public static T[] ToArray<T>(this (T, T) self)
            => new[] { self.Item1, self.Item2 };
    }
}