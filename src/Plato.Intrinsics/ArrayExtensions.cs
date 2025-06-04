using System.Runtime.CompilerServices;
using Ara3D.Collections;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Plato;

public static class ArrayExtensions
{
    [MethodImpl(AggressiveInlining)]
    public static IEnumerable<T> Enumerate<T>(this System.Collections.Generic.IReadOnlyList<T> self) =>
        self;
    
    [MethodImpl(AggressiveInlining)]
    public static ReadOnlyList<T0> MapRange<T0>(this int count, Func<Integer, T0> f) =>
        new(count, i => f(i));

    [MethodImpl(AggressiveInlining)]
    public static ReadOnlyList<T0> MapRange<T0>(this Integer count, Func<Integer, T0> f) =>
        new(count, i => f(i));

    [MethodImpl(AggressiveInlining)]
    public static ReadOnlyList<Integer> Range(this int count) =>
        new(count, i => i);

    [MethodImpl(AggressiveInlining)]
    public static ReadOnlyList<Integer> Indices<T0>(this System.Collections.Generic.IReadOnlyList<T0> xs) =>
        xs.Count.Range();

    [MethodImpl(AggressiveInlining)]
    public static ReadOnlyList<T1> MapIndices<T0, T1>(this System.Collections.Generic.IReadOnlyList<T0> xs, Func<Integer, T1> f) =>
        xs.Count.MapRange(f);

    [MethodImpl(AggressiveInlining)]
    public static ReadOnlyList<T1> Map<T0, T1>(this System.Collections.Generic.IReadOnlyList<T0> xs, Func<T0, T1> f) =>
        xs.Select(f);

    [MethodImpl(AggressiveInlining)]
    public static ReadOnlyList<T1> Map<T0, T1>(this System.Collections.Generic.IReadOnlyList<T0> xs, Func<T0, Integer, T1> f) =>
        xs.MapIndices(i => f(xs[i], i));

    [MethodImpl(AggressiveInlining)]
    public static ReadOnlyList2D<T1> Map<T0, T1>(this Ara3D.Collections.IReadOnlyList2D<T0> xs, Func<T0, T1> f) =>
        xs.Select(f);

    [MethodImpl(AggressiveInlining)]
    public static ReadOnlyList3D<T1> Map<T0, T1>(this Ara3D.Collections.IReadOnlyList3D<T0> xs, Func<T0, T1> f) =>
        xs.Select(f);

    [MethodImpl(AggressiveInlining)]
    public static T1 Reduce<T0, T1>(this System.Collections.Generic.IReadOnlyList<T0> xs, T1 acc, Func<T1, T0, T1> f)
        => xs.Aggregate(acc, f);

    [MethodImpl(AggressiveInlining)]
    public static Boolean All<T0>(this System.Collections.Generic.IReadOnlyList<T0> xs, Func<T0, Boolean> f)
        => xs.Enumerate().All(x => f(x));

    [MethodImpl(AggressiveInlining)]
    public static Boolean Any<T0>(this System.Collections.Generic.IReadOnlyList<T0> xs, Func<T0, Boolean> f)
        => xs.Enumerate().Any(x => f(x));

    [MethodImpl(AggressiveInlining)]
    public static List<T1> FlatMap<T0, T1>(this System.Collections.Generic.IReadOnlyList<T0> xs, Func<T0, System.Collections.Generic.IReadOnlyList<T1>> f)
    {
        var r = new List<T1>();
        foreach (var x in xs)
            r.AddRange(f(x));
        return r;
    }

    [MethodImpl(AggressiveInlining)]
    public static System.Collections.Generic.IReadOnlyList<T1> WithNext<T0, T1>(this System.Collections.Generic.IReadOnlyList<T0> xs, Func<T0, T0, T1> f, bool includeFirst)
        => includeFirst
            ? xs.MapIndices(i => i<xs.Count - 1 ? f(xs[i], xs[i + 1]) : f(xs[i], xs[0]))
            : (xs.Count - 1).MapRange(i => f(xs[i], xs[i + 1]));

    /// <summary>
    /// Maps pairs of elements to a new array.
    /// </summary>
    public static ReadOnlyList<U> MapPairs<T, U>(this System.Collections.Generic.IReadOnlyList<T> xs, Func<T, T, U> f)
        => xs.SelectPairs(f);

    /// <summary>
    /// Maps every 3 elements to a new array.
    /// </summary>
    public static ReadOnlyList<U> MapTriplets<T, U>(this System.Collections.Generic.IReadOnlyList<T> xs, Func<T, T, T, U> f)
        => xs.SelectTriplets(f);

    /// <summary>
    /// Maps every 4 elements to a new array.
    /// </summary>
    public static ReadOnlyList<U> MapQuartets<T, U>(this System.Collections.Generic.IReadOnlyList<T> xs, Func<T, T, T, T, U> f)
        => xs.SelectQuartets(f);

}
