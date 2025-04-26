namespace Ara3D.Collections
{
    public class Unit<T> : IArray<T>, IIterator<T>
    {
        public IIterator<T> Iterator => this;
        public T this[int n] => Value;
        public int Count => 1;
        public T Value { get; }
        public bool HasValue => true;
        public IIterator<T> Next => EmptySequence<T>.Default;
        public Unit(T value) => Value = value;
    }
}