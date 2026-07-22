using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ara3D.Memory
{
    /// <summary>
    /// This is a simple "grow-only" list for unmanaged types that uses aligned memory.
    /// It can only contain up to int.MaxValue items (approx. 2 billion), but can handle larger amounts of data. 
    /// </summary>
    public unsafe class UnmanagedList<T> : IMemoryOwner<T>        
        where T : unmanaged
    {
        public int Count { get; private set; }
        public AlignedMemory Memory { get; private set; }
        private T* _pointer;
        private long _capacity;

        // Constructor to allocate initial capacity in unmanaged memory
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnmanagedList(int capacity = 0, int count = 0)
        {
            Count = count;
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be non-negative.");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
            if (count > capacity)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be less than or equal to capacity.");
            _capacity = capacity; 
            Memory = new AlignedMemory(_capacity * ElementTypeSize);
            _pointer = Memory.Bytes.GetPointer<T>();
        }

        // This constructor is used when transferring memory to this unmanaged list (for example when casting)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnmanagedList(AlignedMemory mem, int cap, int count)
        {
            Memory = mem;
            Count = count;
            _pointer = Memory.Bytes.GetPointer<T>();
            _capacity = cap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (Count == _capacity)
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
            if (_capacity >= count)
                return;
            if (_capacity < 64)
                _capacity = 64;
            while (_capacity < count)
                _capacity *= 2;   
            Memory.Reallocate(_capacity * ElementTypeSize);
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
        public void ZeroMemory()
            => this.AsSpan().Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
            => Count = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ~UnmanagedList()
        {
            if (Memory != null)
                Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            Memory?.Dispose();
            Memory = null;
            _pointer = null;
            Count = 0;
        }
        
        /// <summary>
        /// Converts a memory owner, transferring ownership to a new memory owner block. 
        /// </summary>
        public IMemoryOwner<T1> Convert<T1>() where T1 : unmanaged
        {
            if (this is IMemoryOwner<T1> same)
                return same;
            var newCap = Memory.NumBytes / sizeof(T1);
            if (Memory.NumBytes % sizeof(T1) != 0)
                throw new Exception($"Old type {typeof(T)} cannot be cast to new type {typeof(T1)}");
            var usedBytes = Count * sizeof(T);
            var newCnt = usedBytes / sizeof(T1);
            if (usedBytes % sizeof(T1) != 0)
                throw new Exception($"Old type {typeof(T)} cannot be cast to new type {typeof(T1)}");
            var r = new UnmanagedList<T1>(Memory, (int)newCap, newCnt);
            Memory = null;
            _pointer = null;
            Count = 0;
            _capacity = 0;
            return r;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
        {
            for (var i=0; i < Count; i++)
                yield return this[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public ByteSlice Bytes
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Memory.Bytes.Take((long)Count * ElementTypeSize);
        }

        public Type Type
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => typeof(T);
        }

        public static int ElementTypeSize 
            = sizeof(T);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(IBuffer<T> other)
        {
            Clear();
            AddRange(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(IReadOnlyList<T> other)
        {
            Clear();
            AddRange(other);
        }

        public T* Ptr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _pointer;
        }
    }
}