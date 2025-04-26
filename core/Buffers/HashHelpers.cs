using System.Runtime.CompilerServices;

namespace Ara3D.Buffers
{
    public static class HashHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine(int h1, int h2)
        {
            var rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
            return ((int)rol5 + h1) ^ h2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int Hash(byte* ptr, int length)
        {
            const int seed = unchecked((int)0x811C9DC5);
            var hash = seed;

            var i = 0;
            while (i <= length - 4)
            {
                var value = ptr[i++]
                            | (ptr[i++] << 8)
                            | (ptr[i++] << 16)
                            | (ptr[i++] << 24);
                hash = Combine(hash, value);
            }

            while (i < length)
            {
                hash = Combine(hash, ptr[i++]);
            }

            return hash;
        }
    }
}