using System;

namespace Ara3D.Collections
{
    [Mutable]
    public class ArrayBuilder<T> 
    {
        private const int InitialCapacity = 16;
        private T[] _array = new T[InitialCapacity];
        private int Capacity => _array.Length;
        public int Count { get; private set; }

        public static int ComputeNewSize(int oldCapacity, int desiredCount)
        {
            while (oldCapacity < desiredCount)
            {
                oldCapacity *= 2;
            }

            return oldCapacity;
        }

        private void Resize(int size)
        {
            var old = _array;
            _array = new T[size];
            Array.Copy(old, _array, old.Length);
        }

        public T this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        public void Add(T x)
        {
            Count += 1;
            if (Count > Capacity)
                Resize(ComputeNewSize(Capacity, Count));
            this[Count - 1] = x;
        }

        public void AddRange(IArray<T> xs)
        {
            var oldCount = Count;
            Count += xs.Count;
            if (Count > Capacity)
                Resize(ComputeNewSize(Capacity, Count));
            for (var i=0; i < xs.Count; ++i)
                this[oldCount+1] = xs[i];
        }

        public void Insert(int index, T item)
        {
            var oldCount = Count;
            Add(item);
            Array.Copy(_array, index, _array, index + 1, oldCount - index);
            this[index] = item;
        }

        public void Remove(int index)
        {
            Array.Copy(_array, index + 1, _array, index, Count - index + 1);
            Count -= 1;
        }

        public void RemoveLast()
        {
            Count -= 1;
        }

        public IArray<T> ToIArray()
        {
            var r = _array.ToIArray().Take(Count);
            _array = null;
            return r;
        }
    }
}