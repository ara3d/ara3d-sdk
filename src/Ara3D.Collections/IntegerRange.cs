using System.Collections;
using System.Collections.Generic;

namespace Ara3D.Collections
{
    public readonly struct IntegerRange : IReadOnlyList<int>
    {
        public int From { get; }
        public int this[int n] => From + n;
        public int Count { get; }
        public int Value => From;
        public bool HasValue => Count > 0;
        public IntegerRange(int from, int count) => (From, Count) = (from, count);
        public IEnumerator<int> GetEnumerator() => new ReadOnlyListEnumerator<int>(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}   