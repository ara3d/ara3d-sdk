using System;
using System.Collections;
using System.Collections.Generic;

namespace Ara3D.Collections
{
    public interface IReadOnlyList2D<T>
        : IReadOnlyList<T>
    {
        int Columns { get; }
        int Rows { get; }
        T this[int column, int row] { get; }
    }

    public class ReadOnlyList2D<T> : IReadOnlyList2D<T>
    {
        public int Columns { get; }
        public int Rows { get; }
        public T this[int column, int row] => this[row * Columns + column];
        public IReadOnlyList<T> Data { get; }
        public T this[int index] => Data[index];
        public int Count => Data.Count;
        
        public ReadOnlyList2D(IReadOnlyList<T> data, int columns, int rows)
        {
            if (rows * columns != data.Count)
                throw new Exception($"The data array has length {data.Count} but expected {rows * columns}");
            Rows = rows;
            Columns = columns;
            Data = data;
        }

        public IEnumerator<T> GetEnumerator()
            => Data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    public static class Array2D
    {
        public static IReadOnlyList2D<T> Create<T>(params IReadOnlyList<T>[] arrays)
            => arrays.ToIArray().ToArray2D();
        
        public static IReadOnlyList2D<T> ToArray2D<T>(this IReadOnlyList<IReadOnlyList<T>> readOnlyLists)
            => new ReadOnlyList2D<T>(readOnlyLists.SelectMany(a => a), readOnlyLists[0].Count, readOnlyLists.Count);

        public static IReadOnlyList2D<T> ToArray2D<T>(this IReadOnlyList<T> readOnlyList, int columns, int rows)
            => new ReadOnlyList2D<T>(readOnlyList, columns, rows);

        public static IReadOnlyList<T> GetRow<T>(this IReadOnlyList2D<T> self, int row)
            => self.SubArray(row * self.Columns, self.Columns);

        public static IReadOnlyList<T> GetColumn<T>(this IReadOnlyList2D<T> self, int column)
            => self.Stride(column, self.Columns);

        public static IReadOnlyList<T> OneDimArray<T>(this IReadOnlyList2D<T> self)
            => self;

        public static IReadOnlyList2D<TR> Select<T, TR>(this IReadOnlyList2D<T> self, Func<T, TR> f)
            => new ReadOnlyList2D<TR>(self.OneDimArray().Select(f), self.Columns, self.Rows);
    }
}
