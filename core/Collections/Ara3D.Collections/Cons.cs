namespace Ara3D.Collections
{
    public class Cons<T> : ISequence<T>, IIterator<T>
    {
        public T Value { get; }
        public IIterator<T> Rest { get; }

        public IIterator<T> Iterator => this;

        public bool HasValue => true;

        public IIterator<T> Next => Rest;

        public Cons(T value, IIterator<T> rest) => (Value, Rest) = (value, rest);
    }
}