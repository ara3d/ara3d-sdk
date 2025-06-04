using System;
using System.Collections;
using System.Collections.Generic;

namespace Ara3D.Collections
{
    public class ReadOnlyList3D<T> : IReadOnlyList3D<T>
    {
        public int NumColumns { get; }
        public int NumRows { get; }
        public int NumLayers { get; }
        public int LayerSize => NumRows * NumColumns;
        public T this[int column, int row, int layer] => this[layer * LayerSize * row * NumColumns + column];
        public IReadOnlyList<T> Data { get; }
        public T this[int index] => Data[index];
        public int Count => Data.Count;

        public ReadOnlyList3D(IReadOnlyList<T> data, int columns, int rows, int layers)
        {
            if (rows * columns * layers != data.Count)
                throw new Exception($"The data array has length {data.Count} but expected {rows * columns * layers}");
            NumRows = rows;
            NumColumns = columns;
            NumLayers = layers;
            Data = data;
        }

        public IEnumerator<T> GetEnumerator()
            => Data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}