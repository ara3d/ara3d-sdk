namespace Ara3D.Collections
{
    public class Stack<T> : IStack<T>
    {
        private readonly ISequence<T> _seq;

        public Stack(ISequence<T> seq = null)
            => _seq = seq ?? EmptySequence<T>.Default;

        public IStack<T> Push(T x)
            => new Stack<T>(_seq.Prepend(x));

        public IStack<T> Pop()
            => new Stack<T>(_seq.GetRest());

        public T Peek()
            => _seq.First();

        public bool IsEmpty
            => _seq.IsEmpty();
    }
}
