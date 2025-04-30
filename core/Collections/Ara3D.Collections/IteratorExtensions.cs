using System;

namespace Ara3D.Collections
{
    public static class IteratorExtensions
    {
        public static IIterator<T> SkipWhileNot<T>(this IIterator<T> self, Func<T, bool> f)
        {
            while (self.HasValue && !f(self.Value))
                self = self.Next;
            return self;
        }

        public static IIterator<T> Where<T>(this IIterator<T> iter, Func<T, bool> predicate) => new WhereIterator<T>(iter, predicate);

        public static IIterator<T> Where<T>(this IIterator<T> iter, Func<T, int, bool> predicate) => new WhereIndexIterator<T>(iter, predicate, 0);

        public static IIterator<U> Select<T, U>(this IIterator<T> iter, Func<T, U> mapFunc) => new SelectIterator<T, U>(iter, mapFunc);

        public static IIterator<U> Select<T, U>(this IIterator<T> iter, Func<T, int, U> mapFunc) => new SelectIndexIterator<T, U>(iter, mapFunc);

        public static IIterator<T> Take<T>(this IIterator<T> iter, int n) => iter.TakeWhile((_, i) => i < n);

        public static IIterator<T> TakeWhile<T>(this IIterator<T> iter, Func<T, bool> predicate) => iter.TakeWhile((x, _) => predicate(x));

        public static IIterator<T> TakeWhile<T>(this IIterator<T> iter, Func<T, int, bool> predicate) => new TakeIterator<T>(iter, predicate);

        public static IIterator<T> Skip<T>(this IIterator<T> iter, int n) => iter.AdvanceWhile((_, i) => i < n);

        public static IIterator<T> SkipLast<T>(this IIterator<T> iter, int n) => iter.Take(iter.Count() - n);

        public static IIterator<T> SkipWhile<T>(this IIterator<T> iter, Func<T, bool> predicate) => iter.AdvanceWhile(x => predicate(x.Value));

        public static IIterator<T> AdvanceWhile<T>(this IIterator<T> iter, Func<IIterator<T>, int, bool> predicate)
        {
            var i = 0;
            while (predicate(iter, i++))
                iter = iter.Next;
            return iter;
        }

        public static IIterator<T> AdvanceWhile<T>(this IIterator<T> iter, Func<IIterator<T>, bool> predicate) => iter.AdvanceWhile((i, _) => predicate(i));

        public static IIterator<T> SkipWhile<T>(this IIterator<T> iter, Func<T, int, bool> predicate) => iter.AdvanceWhile((g, index) => predicate(g.Value, index));

        public static bool All<T>(this IIterator<T> iter, Func<T, bool> predicate) => !iter.SkipWhile(predicate).HasValue;

        public static T First<T>(this IIterator<T> iter) => iter.Value;

        public static T ValueOrDefault<T>(this IIterator<T> iter) => iter.HasValue ? iter.Value : default;

        public static T First<T>(this IIterator<T> iter, Func<T, bool> predicate) => iter.SkipWhile(predicate).First();

        public static T FirstOrDefault<T>(this IIterator<T> iter) => iter.ValueOrDefault();

        public static T FirstOrDefault<T>(this IIterator<T> iter, Func<T, bool> predicate) => iter.SkipWhile(predicate).FirstOrDefault();
        
        public static T Last<T>(this IIterator<T> iter) => iter.AdvanceWhile(g => g.Next.HasValue).Value;
        
        public static IIterator<T> AdvanceToLast<T>(this IIterator<T> iter, Func<T, bool> predicate) 
        {
            var prev = iter;
            for (; iter.HasValue; prev = prev.Next)
            {
                if (predicate(iter.Value))
                    prev = iter;
            }
            return prev;
        }

        public static T Last<T>(this IIterator<T> iter, Func<T, bool> predicate) => iter.AdvanceToLast(predicate).Value;
        
        public static T LastOrDefault<T>(this IIterator<T> iter, Func<T, bool> predicate) => iter.AdvanceToLast(predicate).ValueOrDefault();
        
        public static int Count<T>(this IIterator<T> iter) => iter.Aggregate(0, (i, _) => i + 1);
        
        public static int Count<T>(this IIterator<T> iter, Func<T, bool> predicate) => iter.Aggregate(0, (i, x) => predicate(x) ? i + 1 : i);
        
        public static IIterator<T> Append<T>(this IIterator<T> iter, T x) => iter.Concat(x.Unit());
        
        public static TAccumulate Aggregate<T, TAccumulate>(this IIterator<T> iter, TAccumulate init, Func<TAccumulate, T, int, TAccumulate> func)
        {
            var i = 0;
            while (iter.HasValue)
            {
                init = func(init, iter.Value, i++);
                iter = iter.Next;
            }
            return init;
        }

        public static bool Contains<T>(this IIterator<T> iter, T item) => iter.Aggregate(false, (_, x) => _ || (x?.Equals(item) ?? false));
        
        public static IIterator<T> Concat<T>(this IIterator<T> iter, IIterator<T> other) => new ConcatIterator<T>(iter, other);
        
        public static IIterator<U> SelectMany<T, U>(this IIterator<T> iter, Func<T, ISequence<U>> func) => new SelectManyIterator<T, U>(iter, func);
        
        public static bool IsOrderedBy<T>(this IIterator<T> iter, Func<T, T, int> ordering) => iter.IsOrderedBy(new Comparer<T>(ordering));
        
        public static bool IsOrderedBy<T>(this IIterator<T> iter, IComparer<T> ordering)
        {
            if (!iter.HasValue) return true;
            var prev = iter.Value;
            iter = iter.Next;
            while (iter.HasValue)
            {
                var curr = iter.Value;
                if (ordering.Compare(prev, curr) > 0)
                {
                    return false;
                }
                prev = curr;
                iter = iter.Next;
            }
            return true;
        }

        public static int IndexOf<T>(this IIterator<T> self, Func<T, int, bool> f) => self.Aggregate(-1, (acc, cur, index) => acc >= 0 ? acc : f(cur, index) ? index : -1);
        
        public static int IndexOf<T>(this IIterator<T> self, Func<T, bool> f) => self.Aggregate(-1, (acc, cur, index) => acc >= 0 ? acc : f(cur) ? index : -1);
        
        public static int IndexOfLast<T>(this IIterator<T> self, Func<T, int, bool> f) => self.Aggregate(-1, (acc, cur, index) => f(cur, index) ? index : acc);
        
        public static int IndexOfLast<T>(this IIterator<T> self, Func<T, bool> f) => self.Aggregate(-1, (acc, cur, index) => f(cur) ? index : acc);
    }
}