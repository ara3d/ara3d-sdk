using System.Collections.Generic;

namespace Ara3D.Collections
{
    public interface IReadOnlyList2D<T>
        : IReadOnlyList<T>
    {
        int NumColumns { get; }
        int NumRows { get; }
        T this[int column, int row] { get; }
    }
}