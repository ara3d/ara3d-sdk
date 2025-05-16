using System;

namespace Ara3D.Collections
{
    public class Queue<T> : IQueue<T>
    {
        private readonly ISequence<T> _front;
        private readonly ISequence<T> _back;

        public Queue(ISequence<T> front = null, ISequence<T> back = null)
        {
            _front = front ?? EmptySequence<T>.Default;
            _back = back ?? EmptySequence<T>.Default;
            if (_back.IsEmpty() && !_front.IsEmpty())
                throw new Exception($"Front must be empty if the back is empty");
        }

        public IQueue<T> Enqueue(T x)
            => _back.IsEmpty() 
                ? new Queue<T>(_front, _back.Prepend(x)) 
                : new Queue<T>(_front.Prepend(x), _back);

        public T Peek()
            => _back.First();

        public IQueue<T> Dequeue()
            => _back.GetRest().IsEmpty()
                ? new Queue<T>(null, _front.Reverse())
                : new Queue<T>(_front, _back.GetRest());

        public bool IsEmpty
            => _back.IsEmpty();
    }
}