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
}