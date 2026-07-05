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

        public static ReadOnlyListAdapter2D<T> ToReadOnlyList2D<T>(this T[,] data)
            => new ReadOnlyListAdapter2D<T>(data);

        public static FunctionalReadOnlyList2D<U> Select<T, U>(this IReadOnlyList2D<T> self, Func<int, int, U> f)
            => new FunctionalReadOnlyList2D<U>(self.NumColumns, self.NumRows, f);

        public static FunctionalReadOnlyList2D<T> Select<T>(this int numColumns, int numRows, Func<int, int, T> f)
            => new FunctionalReadOnlyList2D<T>(numColumns, numRows, f);

        public static FunctionalReadOnlyList2D<U> Select<T, U>(this IReadOnlyList2D<T> self, Func<T, int, int, U> f)
            => new FunctionalReadOnlyList2D<U>(self.NumColumns, self.NumRows, (i, j) => f(self[i,j], i, j));

        public static FunctionalReadOnlyList2D<U> SampleUV<T, U>(this IReadOnlyList2D<T> self, Func<float, float, T, U> f)
            => self.Select((x, i, j) => f((float)i / self.NumColumns, (float)j / self.NumRows, x));
    }
}
