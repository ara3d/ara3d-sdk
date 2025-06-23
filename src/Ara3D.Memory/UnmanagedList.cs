using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ara3D.Memory
{
    /// <summary>
    /// This is a simple "grow-only" list for unmanaged types that uses aligned memory.
    /// It can only contain up to int.MaxValue items (approx. 2 billion), but can handle larger amounts of data. 
    /// </summary>
    public unsafe class UnmanagedList<T> : IDisposable, IBuffer<T>        
        where T : unmanaged
    {
        public int Count { get; private set; }
        public AlignedMemory Memory { get; private set; }
        private T* _pointer;

        // Constructor to allocate initial capacity in unmanaged memory
        public UnmanagedList(int capacity = 0, int count = 0)
        {
            Count = count;
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be non-negative.");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
            if (count > capacity)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be less than or equal to capacity.");
            Memory = new AlignedMemory(capacity * ElementTypeSize);
            _pointer = Memory.Bytes.GetPointer<T>();
        }

        public long Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Memory.NumBytes / ElementTypeSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (Count == Capacity)
                Accomodate(Count + 1);
            this[Count++] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(T[] items)
            => AddRange(items.AsSpan());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(ReadOnlySpan<T> items)
            => items.CopyTo(AllocateSpan(items.Length));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AllocateSpan(int n)
        {
            Accomodate(Count + n);
            var ptr = Bytes.GetPointer<T>() + Count;
            var span = new Span<T>(ptr, n);
            Count += n;
            return span;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IBuffer<T> items)
            => AddRange(items.AsSpan());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Accomodate(int count)
        {
            if (Capacity > count)
                return;
            var newCapacity = Capacity;
            if (newCapacity < 64)
                newCapacity = 64;
            while (newCapacity < count)
                newCapacity *= 2;   
            Memory.Reallocate(newCapacity * ElementTypeSize);
            _pointer = Memory.Bytes.GetPointer<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AccomodateMore(int n)
            => Accomodate(Count + n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCount(int n)
        {
            Accomodate(n);
            Count = n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
            => Count = 0;

        ~UnmanagedList()
        {
            if (Memory != null)
                Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            Memory.Dispose();
            Memory = null;
            _pointer = null;
            Count = 0;
        }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _pointer[index];
        }

        T IReadOnlyList<T>.this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[index];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i=0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public ByteSlice Bytes 
            => Memory.Bytes.Take((long)Count * ElementTypeSize);

        public Type Type => typeof(T);

        public static int ElementTypeSize 
            => Marshal.SizeOf<T>();
    }
}