namespace Ara3D.Collections
{
    public class IntegerRange : IArray<int>, IIterator<int>
    {
        public int From { get; }
        public int this[int n] => From + n;
        public int Count { get; }

        public IIterator<int> Iterator => this;

        public int Value => From;

        public bool HasValue => Count > 0;

        public IIterator<int> Next => new IntegerRange(From + 1, Count - 1);

        public IntegerRange(int from, int count) => (From, Count) = (from, count);
    }
}   