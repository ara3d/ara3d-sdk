using System.Collections.Generic;

namespace Ara3D.Collections
{
    public interface IReadOnlyList3D<T>
        : IReadOnlyList<T>
    {
        int NumColumns { get; }
        int NumRows { get; }
        int NumLayers { get; }
        T this[int column, int row, int layer] { get; }
    }
}