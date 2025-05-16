using System;

namespace Ara3D.Collections
{
    public class EmptySequence<T> : IArray<T>, IIterator<T>, ISet<T>
    {
        public IIterator<T> Iterator => this;
        public T this[int n] => throw new IndexOutOfRangeException();
        public int Count => 0;
        public bool Contains(T x) => false;
        public T Value => throw new IndexOutOfRangeException();
        public bool HasValue => false;
        public IIterator<T> Next => throw new Exception();
        public static readonly EmptySequence<T> Default = new EmptySequence<T>();
    }
}