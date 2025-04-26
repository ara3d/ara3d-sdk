using System.Runtime.CompilerServices;

namespace Ara3D.Buffers.Modern;

public static class ByteSpanExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDouble(this ByteSpan self)
        => double.Parse(self.ToSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt(this ByteSpan self)
        => int.Parse(self.ToSpan());
}