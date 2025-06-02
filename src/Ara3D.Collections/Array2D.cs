using System;
using System.Collections.Generic;

namespace Ara3D.Collections
{
    public static class Array2D
    {
        public static ReadOnlyList2D<T> Create<T>(params IReadOnlyList<T>[] arrays)
            => arrays.ToArray2D();
        
        public static ReadOnlyList2D<T> ToArray2D<T>(this IReadOnlyList<IReadOnlyList<T>> readOnlyLists)
            => new ReadOnlyList2D<T>(readOnlyLists.SelectMany(a => a), readOnlyLists[0].Count, readOnlyLists.Count);

        public static ReadOnlyList2D<T> ToArray2D<T>(this IReadOnlyList<T> readOnlyList, int columns, int rows)
            => new ReadOnlyList2D<T>(readOnlyList, columns, rows);

        public static IReadOnlyList<T> GetRow<T>(this IReadOnlyList2D<T> self, int row)
            => self.SubArray(row * self.Columns, self.Columns);

        public static IReadOnlyList<T> GetColumn<T>(this IReadOnlyList2D<T> self, int column)
            => self.Stride(column, self.Columns);

        public static IReadOnlyList<T> OneDimArray<T>(this IReadOnlyList2D<T> self)
            => self;

        public static ReadOnlyList2D<TR> Select<T, TR>(this IReadOnlyList2D<T> self, Func<T, TR> f)
            => new ReadOnlyList2D<TR>(self.OneDimArray().Select(f), self.Columns, self.Rows);
    }
}
