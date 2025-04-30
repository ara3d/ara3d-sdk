using System.Runtime.CompilerServices;

namespace Ara3D.Memory
{
    public static class ByteSliceExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDouble(this ByteSlice self)
            => double.Parse(self.AsSpan());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this ByteSlice self)
            => int.Parse(self.AsSpan());
    }
}