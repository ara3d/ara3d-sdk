namespace Ara3D.Collections
{
    public class Stack<T> : IStack<T>
    {
        public T Value;
        public IStack<T> Next;

        public Stack(T value, IStack<T> next)
            => (Value, Next) = (value, next);

        public IStack<T> Push(T x)
            => new Stack<T>(x, this);

        public IStack<T> Pop()
            => Next;

        public T Peek()
            => Value;

        public bool IsEmpty 
            => false;
    }
}