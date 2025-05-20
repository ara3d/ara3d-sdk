
using System.Collections.Generic;

namespace Ara3D.Collections
{
    public interface IStack<T>
    {
        IStack<T> Push(T x);
        IStack<T> Pop();
        T Peek();
        bool IsEmpty { get; }
    }
    
    public interface ITree<T>
    {
        T Value { get; }
        IReadOnlyList<ITree<T>> Subtrees { get; }
    }

    public interface IBinaryTree<T> : ITree<T>
    {
        IBinaryTree<T> Left { get; }
        IBinaryTree<T> Right { get; }
    }
}