using System.Runtime.CompilerServices;

namespace Ara3D.Memory;

/// <summary>
/// Quick and dirty hash functions used for Binary Data.
/// For better hash functions, use System.IO.Hashing or System.Security.Cryptography.
/// </summary>
public static unsafe class BinaryHash
{
    /// <summary>
    /// Fast, non-cryptographic 32-bit hash of potentially long buffers of binary data.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash32(byte* ptr, long length)
    {
        const uint offset = 2166136261;
        const uint prime = 16777619;

        var hash = offset;
        unchecked
        {
            var i = 0;
            var len = length;

            for (; i <= len - 4; i += 4)
            {
                hash ^= Unsafe.ReadUnaligned<uint>(ptr + i);
                hash *= prime;
            }

            // tail: 1-byte chunks
            for (; i < len; ++i)
            {
                hash ^= ptr[i];
                hash *= prime;
            }
        }
        return hash;
    }
}

