using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ara3D.Buffers
{
    public readonly unsafe struct ByteSpan : IEquatable<ByteSpan>, IComparable<ByteSpan>
    {
        public readonly byte* Ptr;
        public readonly int Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSpan(byte* begin, byte* end)
            : this(begin, (int)(end - begin))
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSpan(byte* ptr, int length)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte* End()
            => Ptr + Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte* Begin()
            => Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsBefore(ByteSpan other)
            => End() <= other.Begin();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> ToSpan()
            => new Span<byte>(Ptr, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => Encoding.ASCII.GetString(Ptr, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToArray()
        {
            var r = new byte[Length];
            for (var i = 0; i < Length; ++i)
                r[i] = Ptr[i];
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte At(int index)
            => Ptr[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSpan Slice(long from, int count)
            => new ByteSpan(Ptr + from, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSpan Skip(int count)
            => Slice(count, Length - count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSpan Take(int count)
            => new ByteSpan(Ptr, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSpan Drop(int count)
            => new ByteSpan(Ptr, Length - count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteSpan Trim(int before, int after)
            => new ByteSpan(Ptr + before, Length - before - after);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
            => obj is ByteSpan span && Equals(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
            => HashHelpers.Hash(Ptr, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ByteSpan other)
        {
            if (other.Length != Length) return false;
            var pA = Ptr;
            var pB = other.Ptr;
            for (var i = 0; i < Length; i++)
                if (*pA++ != *pB++)
                    return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(string other)
        {
            if (other.Length != Length) return false;
            var pA = Ptr;
            var pB = 0;
            for (var i = 0; i < Length; i++)
                if (*pA++ != other[pB++])
                    return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(ByteSpan other)
        {
            if (other.Length > Length) return -1;
            if (other.Length < Length) return 1;
            var pA = Ptr;
            var pB = other.Ptr;
            for (var i = 0; i < Length; i++)
            {
                var tmp = pA++->CompareTo(*pB++);
                if (tmp == 0)
                    continue;
                return tmp;
            }

            return 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull()
            => Ptr == null;
    }
}