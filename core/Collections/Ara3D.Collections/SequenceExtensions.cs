using System;

namespace Ara3D.Collections
{
    public static class SequenceExtensions
    {
        public static Unit<T> Unit<T>(this T self)
            => new Unit<T>(self);

        public static ISequence<T> Prepend<T>(this ISequence<T> self, T value)
            => new Cons<T>(value, self.Iterator);

        public static bool IsEmpty<T>(this ISequence<T> self)
            => self?.Iterator?.HasValue == true;

        public static ISequence<T> ToSequence<T>(this IIterator<T> self)
            => new Sequence<T>(self);

        public static ISequence<T> GetRest<T>(this ISequence<T> self)
            => self.Iterator.Next.ToSequence();

        public static ISequence<T> Reverse<T>(this ISequence<T> self)
            => self.Iterator.Reverse();

        public static ISequence<T> Reverse<T>(this IIterator<T> self)
        {
            ISequence<T> r = EmptySequence<T>.Default;
            for (; self.HasValue; self = self.Next)
                r = r.Prepend(self.Value);
            return r;
        }

        public static ISequence<T2> ReverseSelect<T1, T2>(this IIterator<T1> self,
            Func<T1, T2> transform)
        {
            ISequence<T2> r = EmptySequence<T2>.Default;
            for (; self.HasValue; self = self.Next)
                r = r.Prepend(transform(self.Value));
            return r;
        }

        public static ISequence<T> ReverseFilter<T>(this IIterator<T> self,
            Func<T, bool> predicate)
        {
            ISequence<T> r = EmptySequence<T>.Default;
            for (; self.HasValue; self = self.Next)
                if (predicate(self.Value))
                    r = r.Prepend(self.Value);
            return r;
        }

        public static ISequence<T> Filter<T>(this ISequence<T> self, Func<T, bool> predicate)
            => self.Iterator.Where(predicate).ToSequence();

        public static ISequence<T2> Select<T1, T2>(this ISequence<T1> self, Func<T1, T2> transform)
            => self.Iterator.Select(transform).ToSequence();

        public static T GetLast<T>(this ISequence<T> self)
            => self.Iterator.GetLast();

        public static T GetLast<T>(this IIterator<T> self)
        {
            for (; self.HasValue; self = self.Next)
                if (!self.Next.HasValue)
                    return self.Value;
            throw new ArgumentOutOfRangeException();
        }

        public static T First<T>(this ISequence<T> self)
            => self.Iterator.Value;

        public static ISequence<T> Skip<T>(
            this ISequence<T> self, int n)
        {
            for (; n > 0; n--)
                self = self.GetRest();
            return self;
        }
    }
}