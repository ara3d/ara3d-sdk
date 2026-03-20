using System;

namespace Ara3D.Utils;

public static class SpanUtil
{
    public static ReadOnlySpan<T> AsReadOnly<T>(this Span<T> span)
        => span;
}