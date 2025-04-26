using System;

namespace Ara3D.Collections
{
   
    public static class SequenceExtensions2
    {
        public static RepeatedSequence<T> Repeat<T>(this T x, int value) => new RepeatedSequence<T>(x, value);
        public static ISet<T> ToSet<T>(this Func<T, bool> predicate) => new Set<T>(predicate);
        public static Func<T, bool> Negate<T>(this Func<T, bool> func) => x => !func(x);

        public static IArray<T> ToIArray<T>(this IIterator<T> iter)
        {
            var r = new System.Collections.Generic.List<T>();
            while (iter.HasValue)
            {
                r.Add(iter.Value);
                iter = iter.Next;
            }
            return r.ToIArray();
        }

        public static ISequence<T> Where<T>(this ISequence<T> seq, Func<T, bool> predicate) => seq.Iterator.Where(predicate).ToSequence();
        public static ISequence<T> Where<T>(this ISequence<T> seq, Func<T, int, bool> predicate) => seq.Iterator.Where(predicate).ToSequence();
        
        public static ISequence<U> Select<T, U>(this ISequence<T> seq, Func<T, U> mapFunc) => seq.Iterator.Select(mapFunc).ToSequence();
        
        public static ISequence<U> Select<T, U>(this ISequence<T> seq, Func<T, int, U> mapFunc) => seq.Iterator.Select(mapFunc).ToSequence();
        public static ISequence<T> Take<T>(this ISequence<T> seq, int n) => seq.Iterator.Take(n).ToSequence();
        public static ISequence<T> TakeWhile<T>(this ISequence<T> seq, Func<T, bool> predicate) => seq.Iterator.TakeWhile(predicate).ToSequence();
        public static ISequence<T> TakeWhile<T>(this ISequence<T> seq, Func<T, int, bool> predicate) => seq.Iterator.TakeWhile(predicate).ToSequence();
        public static ISequence<T> Skip<T>(this ISequence<T> seq, int n) => seq.Iterator.Skip(n).ToSequence();
        public static ISequence<T> SkipLast<T>(this ISequence<T> seq, int n) => seq.Iterator.SkipLast(n).ToSequence();
        public static ISequence<T> SkipWhile<T>(this ISequence<T> seq, Func<T, int, bool> predicate) => seq.Iterator.SkipWhile(predicate).ToSequence();
        public static bool Any<T>(this IIterator<T> iter) => iter.HasValue;
        public static bool Any<T>(this ISequence<T> seq) => seq.Iterator.Any();
        public static bool Any<T>(this IIterator<T> iter, Func<T, bool> predicate) => iter.SkipWhile(predicate).Any();
        public static bool Any<T>(this ISequence<T> seq, Func<T, bool> predicate) => seq.Iterator.Any(predicate);
        public static bool All<T>(this ISequence<T> seq, Func<T, bool> predicate) => seq.Iterator.All(predicate);
        public static T First<T>(this ISequence<T> seq, Func<T, bool> predicate) => seq.Iterator.First(predicate);
        public static T FirstOrDefault<T>(this ISequence<T> seq) => seq.Iterator.FirstOrDefault();
        public static T FirstOrDefault<T>(this ISequence<T> seq, Func<T, bool> predicate) => seq.Iterator.FirstOrDefault(predicate);
        public static T Last<T>(this ISequence<T> seq) => seq.Iterator.Last();
        public static T Last<T>(this ISequence<T> seq, Func<T, bool> predicate) => seq.Iterator.Last(predicate);
        public static T LastOrDefault<T>(this ISequence<T> seq, Func<T, bool> predicate) => seq.Iterator.LastOrDefault(predicate);
        public static int Count<T>(this ISequence<T> seq) => seq.Iterator.Count();
        public static int Count<T>(this ISequence<T> seq, Func<T, bool> predicate) => seq.Iterator.Count(predicate);
        public static IIterator<T> Append<T>(this ISequence<T> seq, T x) => seq.Iterator.Append(x);
        public static T ElementAt<T>(this IIterator<T> iter, int n) => iter.Skip(n).First();
        public static T ElementAt<T>(this ISequence<T> seq, int n) => seq.Iterator.ElementAt(n);

        public static TAccumulate Aggregate<T, TAccumulate>(
            this IIterator<T> iter,
            TAccumulate init,
            Func<TAccumulate, T, TAccumulate> func)
        {
            while (iter.HasValue)
            {
                init = func(init, iter.Value);
                iter = iter.Next;
            }
            return init;
        }

        public static TAccumulate Aggregate<T, TAccumulate>(this ISequence<T> seq, TAccumulate init, Func<TAccumulate, T, TAccumulate> func) => seq.Iterator.Aggregate(init, func);
        public static TAccumulate Aggregate<T, TAccumulate>(this ISequence<T> seq, TAccumulate init, Func<TAccumulate, T, int, TAccumulate> func) => seq.Iterator.Aggregate(init, func);
        public static bool Contains<T>(this ISequence<T> seq, T item) => seq.Iterator.Contains(item);
        public static ISequence<T> Concat<T>(this ISequence<T> seq, ISequence<T> other) => seq.Iterator.Concat(other.Iterator).ToSequence();
        public static ISequence<U> SelectMany<T, U>(this ISequence<T> seq, Func<T, ISequence<U>> func) => seq.Iterator.SelectMany(func).ToSequence();
        public static bool IsOrderedBy<T>(this ISequence<T> seq, Func<T, T, int> ordering) => seq.Iterator.IsOrderedBy(new Comparer<T>(ordering));
        //public static IArray<T> OrderBy<T>(this ISequence<T> seq, Func<T, T, int> compare) => seq.ToList().OrderBy(compare).ToIArray();
        //public static IArray<T0> OrderBy<T0, T1>(this ISequence<T0> seq, Func<T0, T1> selector) where T1 : IComparable<T1> => seq.ToList().OrderBy(selector).ToIArray();
        public static bool IsOrderedBy<T>(this ISequence<T> seq, IComparer<T> ordering) => seq.Iterator.IsOrderedBy(ordering);
        public static int IndexOf<T>(this ISequence<T> self, Func<T, int, bool> f) => self.Iterator.IndexOf(f);
        public static int IndexOf<T>(this ISequence<T> self, Func<T, bool> f) => self.Iterator.IndexOf(f);
        public static int IndexOfLast<T>(this ISequence<T> self, Func<T, int, bool> f) => self.Iterator.IndexOfLast(f);
        public static int IndexOfLast<T>(this ISequence<T> self, Func<T, bool> f) => self.Iterator.IndexOfLast(f);
    }
}