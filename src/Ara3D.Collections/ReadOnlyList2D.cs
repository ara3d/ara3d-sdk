using System;
using System.Collections;
using System.Collections.Generic;

namespace Ara3D.Collections
{
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
}