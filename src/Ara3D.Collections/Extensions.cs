using System.Collections.Generic;

namespace Ara3D.Collections
{
    public static class Extensions
    {
        public static ISequence<T> ToSequence<T>(this IEnumerable<T> self)
            => self.ToIArray();

        public static IEnumerable<T> Enumerate<T>(this ISequence<T> self)
            => self.Iterator.Enumerate();

        public static IEnumerable<T> Enumerate<T>(this IIterator<T> self)
        {
            for (;self.HasValue; self = self.Next)
                yield return self.Value;
        }
    }
}