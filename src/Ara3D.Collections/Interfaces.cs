
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

    public interface IQueue<T>
    {
        IQueue<T> Enqueue(T x);
        IQueue<T> Dequeue();
        T Peek();
        bool IsEmpty { get; }
    }

    public interface IDeque<T>
    {
        IDeque<T> PushFront(T x);
        T PeekFront();
        IDeque<T> PopFront();
        IDeque<T> PushBack(T x);
        T PeekBack();
        IDeque<T> PopBack();
        bool IsEmpty { get; }
    }

    public interface IPriorityQueue<T>
    {
        void Enqueue(int priority, T element);
        T PeekHighestPriority();
        T DequeueHighestPriority();
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
    
    public interface ISet<T>
    {
        bool Contains(T x);
    }

    public interface IMultiSet<T>
        : ISet<T>
    {
        int CountOf(T x);
    }

    /// <summary>
    /// Used for comparing two objects
    /// </summary>
    public interface IComparer<T>
    {
        int Compare(T x, T y);
    }

    /// <summary>
    /// Collections with a specific ordering store their ordering function. 
    /// </summary>
    public interface IOrdered<T>
    {
        IComparer<T> Ordering { get; }
    }

    /// <summary>
    /// This is used by types that support the notion of being searchable in 
    /// at least O(Log N) time. For example a sorted sequence or a binary tree. 
    /// </summary>
    public interface ISearchable<TValue, TKey>
    {
        TKey FindKey(TValue item);
    }

    /// <summary>
    /// An ordered array enables much faster finding of items.
    /// </summary>
    public interface IOrderedArray<T> : IArray<T>, IOrdered<T>, ISearchable<T, int>
    {
    }

    /// <summary>
    /// An sequence with a specific fixed ordering. 
    /// </summary>
    public interface IOrderedSequence<T> : IReadOnlyList<T>, IOrdered<T>
    {
    }

    /// <summary>
    /// A monotonically increasing sequence of integers 
    /// </summary>
    public interface IRange : IOrderedArray<int>, ISet<int>
    {
        int From { get; }
    }
}