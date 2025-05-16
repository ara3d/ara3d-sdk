using System;

namespace Ara3D.Collections
{
    /// <summary>
    /// Finish implementing this priority queue class. 
    /// by providing implementations for the unimplemented functions.
    /// Do not add any new fields or properties, but you may add new functions.
    /// Do not use functions from the systems library. 
    /// </summary>
    public class PriorityQueueWithTree<T>
    {
        private BinaryTree<(int, T)> _tree;

        public T Extract()
        {
            if (IsEmpty())
                throw new InvalidOperationException("Empty priority queue");
            throw new NotImplementedException();
        }

        public void Add(int priority, T item)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            return _tree == null;
        }
    }
}