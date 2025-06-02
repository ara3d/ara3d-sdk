using System;
using System.Collections;
using System.Collections.Generic;

namespace Ara3D.Collections
{
    public class EmptyList<T> : IEnumerator<T>, IReadOnlyList<T>
    {
        public bool IsEmpty => true;
        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
        public int Count => 0;
        public T this[int index] => throw new InvalidOperationException();
        public bool MoveNext() => throw new InvalidOperationException();
        public void Reset() {}
        public T Current => throw new InvalidOperationException();
        object IEnumerator.Current => Current;
        public void Dispose() {}
        public static EmptyList<T> Default = new EmptyList<T>();
    }
}