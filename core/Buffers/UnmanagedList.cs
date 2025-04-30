using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        public UnmanagedList(int capacity = 1024, int count = 0, uint alignment = 256)
        {
            Count = count;
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be non-negative.");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
            if (count > capacity)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be less than or equal to capacity.");
            Memory = new AlignedMemory(capacity * ElementTypeSize, alignment <= 0 ? (uint)sizeof(T) : alignment);
            _pointer = Memory.Bytes.GetPointer<T>();
        }

        public long Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Memory.Length() / ElementTypeSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (Count == Capacity)
                Grow();
            this[Count++] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Grow()
        {
            Memory.Reallocate(Capacity * 2 * ElementTypeSize);
            _pointer = Memory.Bytes.GetPointer<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
            => Count = 0;

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

        public ByteSlice Bytes => Memory.Bytes;

        public Type Type => typeof(T);

        public static int ElementTypeSize 
            => Marshal.SizeOf<T>();
    }
}