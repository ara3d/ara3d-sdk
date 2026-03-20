using System;
using System.Text;

namespace Ara3D.Utils;

public static class HashUtil
{
    private const uint OffsetBasis = 2166136261;
    private const uint Prime = 16777619;

    public static uint Fnv1a32bit(this byte[] data)
        => data.AsSpan().Fnv1a32bit();

    public static uint Fnv1a32bit(this Span<byte> data)
        => data.AsReadOnly().Fnv1a32bit();

    public static uint Fnv1a32bit(this ReadOnlySpan<byte> data)
    {
        var hash = OffsetBasis;
        for (var i = 0; i < data.Length; i++)
        {
            hash ^= data[i];
            hash *= Prime;
        }
        return hash;
    }

    public static uint Fnv1a32bit(this string str)
        => Encoding.UTF8.GetBytes(str).Fnv1a32bit();
}