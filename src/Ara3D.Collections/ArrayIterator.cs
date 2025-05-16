namespace Ara3D.Collections
{
    public class ArrayIterator<T> : IIterator<T>
    {
        public IArray<T> Array { get; }
        public int Index { get; }

        public ArrayIterator(IArray<T> array, int index = 0) =>
            (Array, Index) = (array, index);

        public T Value => Array[Index];

        public bool HasValue => Index < Array.Count;

        public IIterator<T> Next => new ArrayIterator<T>(Array, Index + 1);
    }
}