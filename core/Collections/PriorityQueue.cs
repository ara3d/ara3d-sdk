using System;

namespace Ara3D.Collections
{
    public class PriorityQueue<T> : IPriorityQueue<T>
    {
        public void Enqueue(int priority, T element)
        {
            throw new NotImplementedException();
        }

        public T PeekHighestPriority()
        {
            throw new NotImplementedException();
        }

        public T DequeueHighestPriority()
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty => throw new NotImplementedException();
    }
}