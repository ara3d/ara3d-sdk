namespace Ara3D.Collections
{
    public class Sequence<T> : ISequence<T>
    {
        public IIterator<T> Iterator { get; }
        public Sequence(IIterator<T> iterator) => Iterator = iterator;
    }
}