using System.Collections.Generic;

namespace Ara3D.Collections
{
    public interface IReadOnlyList3D<T>
        : IReadOnlyList<T>
    {
        int Columns { get; }
        int Rows { get; }
        int Layers { get; }
        T this[int column, int row, int layer] { get; }
    }
}