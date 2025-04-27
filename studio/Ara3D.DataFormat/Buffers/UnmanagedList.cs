using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ara3D.Data
{
    /// <summary>
    /// This is a simple "grow-only" list for unmanaged types that uses aligned memory.
    /// </summary>
    public unsafe class UnmanagedList<T> 
        : IDisposable, IBuffer<T>, IEnumerable<T> 
        where T : unmanaged
    {
        public T* Buffer;
        public uint Capacity;
        public readonly uint Alignment;
        public long Count { get; private set; }
        public T* Pointer => Buffer;
        public ulong SizeInBytes => (uint)Count * (uint)sizeof(T);

        // Constructor to allocate initial capacity in unmanaged memory
        public UnmanagedList(uint capacity = 1024, uint count = 0, uint alignment = 256)
        {
            Count = count;
            Capacity = capacity;
            Alignment = alignment == 0 ? (uint)sizeof(T) : alignment;
            Buffer = (T*)NativeMemory.AlignedAlloc(Capacity * (nuint)sizeof(T), Alignment);

#if DEBUG
            // Make sure ptr is not null
            if (Buffer == null)
                throw new OutOfMemoryException("AlignedAlloc returned null.");

            // Check the pointer alignment
            nuint addr = (nuint)Buffer;
            if (addr % Alignment != 0)
                throw new Exception($"Pointer {addr:X} is not aligned to {alignment} bytes.");
#endif
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (Count == Capacity)
                Grow();
            var tmp1 = (nuint)(Buffer + 1);
            var tmp0 = (nuint)Buffer;
            Debug.Assert((tmp1 - tmp0) == (nuint)sizeof(T));
            Buffer[Count++] = item;
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
            Capacity *= 2;
            Buffer = (T*)NativeMemory.AlignedRealloc(Buffer, Capacity * (nuint)sizeof(T), Alignment);

#if DEBUG
            // Make sure ptr is not null
            if (Buffer == null)
                throw new OutOfMemoryException("AlignedAlloc returned null.");

            // Check the pointer alignment
            nuint addr = (nuint)Buffer;
            if (addr % Alignment != 0)
                throw new Exception($"Pointer {addr:X} is not aligned to {Alignment} bytes.");
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
            => Count = 0;

        // Free the unmanaged memory. After calling Dispose, this instance should not be used.
        public void Dispose()
        {
            if (Buffer != null)
            {
                NativeMemory.AlignedFree(Buffer);
                Buffer = null;
            }

            Capacity = 0;
            Count = 0;
        }
        
        public Span<T> AsSpan()
            => new(Buffer, (int)Count);

        public ref T this[long i] 
            => ref Buffer[i];

        public IEnumerator<T> GetEnumerator()
        {
            for (var i=0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}