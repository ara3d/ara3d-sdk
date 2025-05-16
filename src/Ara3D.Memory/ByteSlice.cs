using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Ara3D.Memory
{
    /// <summary>
    /// A ByteSlice is an unsafe pointer to a section of allocated and fixed memory. 
    /// This is similar to a Span, except you can store it on the heap, and it can be greater than 2GB.
    /// This makes it very convenient for efficient low-level operations.
    /// However, you need to be very careful when using it to assure that the memory is fixed
    /// during the lifespan of the ByteSlice and if you use it as an IReadOnlyList, it must not be larger than 2GB.
    /// </summary>
    [SkipLocalsInit]
    public readonly unsafe struct ByteSlice : IEquatable<ByteSlice>, IComparable<ByteSlice>, IReadOnlyList<byte>
    {
        public readonly byte* Ptr;
        public readonly long Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSlice(byte* begin, byte* end)
            : this(begin, (int)(end - begin))
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once ConvertToPrimaryConstructor
        public ByteSlice(byte* ptr, long length)
        {
            Ptr = ptr;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte First()
            => *Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Last()
            => Ptr[Length - 1];

        public byte* End
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr + Length;
        }

        public byte* Begin
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsBefore(ByteSlice other)
            => End <= other.Begin;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan()
            => new(Ptr, CheckedLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsReadOnlySpan()
            => new(Ptr, CheckedLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => ToAsciiString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToAsciiString()
            => Encoding.ASCII.GetString(Ptr, CheckedLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToUtf8String()
            => Encoding.UTF8.GetString(Ptr, CheckedLength);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte At(int index)
            => Ptr[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSlice Slice(long from, long count)
            => new(Ptr + from, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSlice Skip(long count)
            => Slice(count, Length - count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSlice Take(long count)
            => Slice(0, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSlice TakeLast(long count)
            => Slice(Length - count, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSlice SkipLast(long count)
            => Slice(0, Length - count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSlice Trim(long before, long after)
            => Slice(before, Length - before - after);

        public IEnumerator<byte> GetEnumerator()
        {
            for (var i = 0; i < Length; i++)
                yield return this[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
            => obj is ByteSlice span && Equals(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
            => unchecked((int)BinaryHash.Hash32(Ptr, Length));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ByteSlice other)
            => AsReadOnlySpan().SequenceEqual(other.AsReadOnlySpan());

        public int CompareTo(ByteSlice other)
            => AsReadOnlySpan().SequenceCompareTo(other.AsReadOnlySpan());

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Count == 0;
        }

        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr == null;
        }

        /// <summary>
        /// Returns the length as an integer, but will throw if the length is greater than 2GB.
        /// </summary>
        public int CheckedLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => checked((int)Length);
        }

        public byte this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr[index];
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CheckedLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToArray()
            => AsSpan().ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Span<byte>(ByteSlice self)
            => self.AsSpan();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ByteSlice(Span<byte> self)
            => FromSpan(self);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator byte*(ByteSlice self)
            => self.Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator void*(ByteSlice self)
            => self.Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ByteSlice a, ByteSlice b)
            => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ByteSlice a, ByteSlice b)
            => !a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ReadOnlySpan<byte>(ByteSlice self)
            => self.AsSpan();

        /// <summary>
        /// This is a slow operation, you should be using "IReadOnlyList&lt;byte&gt;" instead.
        /// </summary>
        /// <param name="self"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator byte[](ByteSlice self)
            => self.ToArray();

        /// <summary>
        /// Note this is unsafe and should only be used when you are sure the memory is fixed.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ByteSlice FromSpan(Span<byte> span)
        {
            ref var r0 = ref MemoryMarshal.GetReference(span);
            return new ByteSlice((byte*)Unsafe.AsPointer(ref r0), span.Length);
        }

        public static readonly ByteSlice Empty = new(null, 0);

        /// <summary>Safely view the slice as a <see cref="Span{T}"/>; throws if mis-aligned or mis-sized.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan<T>() where T : unmanaged
            => new(GetPointer<T>(), checked((int)(Length / Marshal.SizeOf<T>())));

        /// <summary>Read-only counterpart to <see cref="AsSpan{T}"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> AsReadOnlySpan<T>() where T : unmanaged
            => AsSpan<T>();

        /// <summary>
        /// Safely cast the underlying pointer to an unmanaged type.
        /// Throws an <see cref="InvalidOperationException"/> if the length is not divisible by the size of T or is misaligned.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* GetPointer<T>() where T : unmanaged
        {
            if (Length % Unsafe.SizeOf<T>() != 0)
                throw new InvalidOperationException($"Length of bytes is not divisible by {sizeof(T)}-byte");
            var r = (T*)Ptr;
            if (r != Ptr)
                throw new InvalidOperationException($"Pointer is not aligned to {sizeof(T)}-byte");
            return r;
        }
    }
}