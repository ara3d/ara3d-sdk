using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ara3D.Buffers
{
    public static unsafe class ByteSpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WithSpan(this string self, Action<ByteSpan> action)
            => Encoding.UTF8.GetBytes(self).WithSpan(action);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T WithSpan<T>(this string self, Func<ByteSpan, T> func)
            => Encoding.UTF8.GetBytes(self).WithSpan(func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WithSpan(this byte[] bytes, Action<ByteSpan> action)
        {
            fixed (byte* p = &bytes[0])
                action(new ByteSpan(p, bytes.Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T WithSpan<T>(this byte[] bytes, Func<ByteSpan, T> func)
        {
            fixed (byte* p = &bytes[0])
                return func(new ByteSpan(p, bytes.Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDouble(this ByteSpan self)
            => double.Parse(self.ToSpan());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this ByteSpan self)
            => int.Parse(self.ToSpan());
    }
}