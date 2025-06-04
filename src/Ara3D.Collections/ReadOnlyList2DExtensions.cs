using System;
using System.Collections.Generic;

namespace Ara3D.Collections
{
    public static class ReadOnlyList2DExtensions
    {
        public static ReadOnlyList2D<T> Create<T>(params IReadOnlyList<T>[] arrays)
            => arrays.ToArray2D();
        
        public static ReadOnlyList2D<T> ToArray2D<T>(this IReadOnlyList<IReadOnlyList<T>> readOnlyLists)
            => new ReadOnlyList2D<T>(readOnlyLists.SelectMany(a => a), readOnlyLists[0].Count, readOnlyLists.Count);

        public static ReadOnlyList2D<T> ToArray2D<T>(this IReadOnlyList<T> readOnlyList, int columns, int rows)
            => new ReadOnlyList2D<T>(readOnlyList, columns, rows);

        public static IReadOnlyList<T> Row<T>(this IReadOnlyList2D<T> self, int row)
            => self.SubArray(row * self.NumColumns, self.NumColumns);

        public static IReadOnlyList<IReadOnlyList<T>> Rows<T>(this IReadOnlyList2D<T> self)
            => self.NumRows.Select(self.Row);

        public static IReadOnlyList<T> Column<T>(this IReadOnlyList2D<T> self, int column)
            => self.Stride(column, self.NumColumns);

        public static IReadOnlyList<IReadOnlyList<T>> Columns<T>(this IReadOnlyList2D<T> self)
            => self.NumColumns.Select(self.Column);

        public static IReadOnlyList<T> OneDimArray<T>(this IReadOnlyList2D<T> self)
            => self;

        public static ReadOnlyList2D<TR> Select<T, TR>(this IReadOnlyList2D<T> self, Func<T, TR> f)
            => new ReadOnlyList2D<TR>(self.OneDimArray().Select(f), self.NumColumns, self.NumRows);
    }
}
