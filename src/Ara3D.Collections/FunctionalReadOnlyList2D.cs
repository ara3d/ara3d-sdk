using System;
using System.Collections;
using System.Collections.Generic;

namespace Ara3D.Collections
{
    public class FunctionalReadOnlyList2D<T> : IReadOnlyList2D<T>
    {
        public int NumColumns { get; }
        public int NumRows { get; }
        public Func<int, int, T> Func { get; }
        public T this[int column, int row] => Func(column, row);
        public IReadOnlyList<T> Data { get; }
        public T this[int index] => Data[index];
        public int Count => NumColumns * NumRows;

        public FunctionalReadOnlyList2D(int columns, int rows, Func<int, int, T> func)
        {
            Func = func;
            NumRows = rows;
            NumColumns = columns;
            Data = new ReadOnlyList<T>(Count, i => this[i % NumColumns, i / NumColumns]);
        }

        public IEnumerator<T> GetEnumerator()
            => Data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public static FunctionalReadOnlyList2D<T> Default 
            = new FunctionalReadOnlyList2D<T>(0, 0, (col, row) => default);
    }
}