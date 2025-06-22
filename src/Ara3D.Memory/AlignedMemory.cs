using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Ara3D.Memory
{
    /// <summary>
    /// Represents a block of unmanaged memory with 64-byte alignment.
    /// </summary>
    [SkipLocalsInit]
    public unsafe class AlignedMemory : IMemoryOwner
    {
        private bool _disposed;
        public ByteSlice Bytes { get; private set; }

        public const int Alignment = 64;

        /// <summary>
        /// Initializes a new instance of the with the specified size and alignment.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public AlignedMemory(long count)
        {
            Bytes = new ByteSlice((byte*)NativeMemory.AlignedAlloc((nuint)count, Alignment), count);
            if (Bytes.IsNull)
                throw new OutOfMemoryException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _disposed = true;
            if (!Bytes.IsEmpty)
                NativeMemory.AlignedFree(this.GetPointer());
            Bytes = ByteSlice.Empty;
        }

        ~AlignedMemory()
        {
            Dispose(false);
        }

        /// <summary>
        /// Reallocates the block of managed memory
        /// </summary>
        /// <param name="size"></param>
        public void Reallocate(long size)
        {
            if (Bytes.IsNull)
                throw new InvalidOperationException();
            Bytes = new ByteSlice((byte*)NativeMemory.AlignedRealloc(Bytes.Begin, (nuint)size, Alignment), size);
            if (Bytes.IsNull)
                throw new OutOfMemoryException();
        }

        /// <summary>
        /// Implicit cast to a ByteSlice.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static implicit operator ByteSlice(AlignedMemory memory)
            => memory.Bytes;
        /// <summary>
        /// Implicit cast to a ByteSlice.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static implicit operator ReadOnlySpan<byte>(AlignedMemory memory)
            => memory.Bytes;

        /// <summary>
        /// Implicit cast to a ByteSlice.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static implicit operator Span<byte>(AlignedMemory memory)
            => memory.Bytes;

        /// <summary>
        /// The number of bytes in the buffer.
        /// </summary>
        public long NumBytes
        {
            [MethodImpl(AggressiveInlining)] get => Bytes.Length;
        }
    }

    /// <summary>
    /// Represents a block of unmanaged memory with a 64-byte alignment.
    /// Initially allocated with a specific type.
    /// There can only be up to int.MaxValue elements in the buffer (approx. 2 Billion)
    /// </summary>
    [SkipLocalsInit]
    public unsafe class AlignedMemory<T> : AlignedMemory, IMemoryOwner<T>
        where T : unmanaged
    {
        private T* _pointer;

        [MethodImpl(AggressiveInlining)]
        public AlignedMemory(long count)
            : base(count * Marshal.SizeOf<T>())
        {
            Count = checked((int)count);
            _pointer = Bytes.GetPointer<T>();
        }

        public Type Type
        {
            [MethodImpl(AggressiveInlining)] 
            get => typeof(T);
        }

        [MethodImpl(AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; ++i)
                yield return this[i];
        }

        [MethodImpl(AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public int Count { get; }

        public ref T this[int index]
        {
            [MethodImpl(AggressiveInlining)] 
            get => ref _pointer[index];
        }

        T IReadOnlyList<T>.this[int index]
        {
            [MethodImpl(AggressiveInlining)]
            get => this[index];
        }

        protected override void Dispose(bool disposing)
        {
            _pointer = null;
            base.Dispose(disposing);
        }
    }
}