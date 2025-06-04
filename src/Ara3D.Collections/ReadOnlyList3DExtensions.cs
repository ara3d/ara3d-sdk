using System;
using System.Collections.Generic;

namespace Ara3D.Collections
{
    public static class ReadOnlyList3DExtensions
    {
        public static IReadOnlyList<T> OneDimArray<T>(this IReadOnlyList3D<T> self)
            => self;

        public static ReadOnlyList3D<TR> Select<T, TR>(this IReadOnlyList3D<T> self, Func<T, TR> f)
            => new ReadOnlyList3D<TR>(self.OneDimArray().Select(f), self.NumColumns, self.NumRows, self.NumLayers);
    }
}