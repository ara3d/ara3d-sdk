using System.Collections.Generic;

namespace Ara3D.Utils
{
    public interface ITree<T>
    {
        IEnumerable<T> Children { get; }
    }
}