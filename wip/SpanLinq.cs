using System;
using System.Runtime.InteropServices;

namespace Ara3D.Buffers;

public static class SpanLinq
{
    public static unsafe Span<T1> SelectInPlace<T0, T1>(this Span<T0> self, Func<T0, T1> f) where T0 : unmanaged where T1 : unmanaged
        => sizeof(T1) > sizeof(T0)
            ? throw new ArgumentException($"T1 (size = {sizeof(T1)}) must be smaller than or equal to T0 (size = {sizeof(T0)})")
            : self.Select(f, MemoryMarshal.Cast<T0, T1>(self));

    public static Span<T1> Select<T0, T1>(this Span<T0> self, Func<T0, T1> f, Span<T1> dest) where T0 : unmanaged where T1 : unmanaged
    {
        for (var j = 0; j < self.Length; j++)
            dest[j] = f(self[j]);
        return dest;
    }

    public static Span<T1> Select<T0, T1>(this Span<T0> self, Func<T0, T1> f) where T0 : unmanaged where T1 : unmanaged
        => self.Select(f, new T1[self.Length]);

    public static unsafe Span<T1> SelectInPlace<T0, T1>(this Span<T0> self, Func<T0, int, T1> f) where T0 : unmanaged where T1 : unmanaged
        => sizeof(T1) > sizeof(T0)
            ? throw new ArgumentException($"T1 (size = {sizeof(T1)}) must be smaller than or equal to T0 (size = {sizeof(T0)})")
            : self.Select(f, self.Cast<T0, T1>());

    public static Span<T1> Select<T0, T1>(this Span<T0> self, Func<T0, int, T1> f, Span<T1> dest) where T0 : unmanaged where T1 : unmanaged
    {
        for (var j = 0; j < self.Length; j++)
            dest[j] = f(self[j], j);
        return dest;
    }

    public static Span<T1> Select<T0, T1>(this Span<T0> self, Func<T0, int, T1> f) where T0 : unmanaged where T1 : unmanaged
        => self.Select(f, new T1[self.Length]);

    public static Span<T0> Where<T0>(this Span<T0> self, Func<T0, bool> f, Span<T0> dest) where T0 : unmanaged
    {
        var cnt = 0;
        for (var i = 0; i < self.Length; i++)
            if (f(self[i]))
                dest[cnt++] = self[i];
        return dest[..cnt];
    }

    public static Span<T0> WhereInPlace<T0>(this Span<T0> self, Func<T0, int, bool> f) where T0 : unmanaged
        => Where(self, f, self);

    public static Span<T0> Where<T0>(this Span<T0> self, Func<T0, int, bool> f) where T0 : unmanaged
        => Where(self, f, new T0[self.Length]);

    public static Span<T0> Where<T0>(this Span<T0> self, Func<T0, int, bool> f, Span<T0> dest) where T0 : unmanaged
    {
        var cnt = 0;
        for (var i=0; i < self.Length; i++)
            if (f(self[i], i))
                dest[cnt++] = self[i];
        return dest[..cnt];
    }

    public static Span<T0> WhereInPlace<T0>(this Span<T0> self, Func<T0, bool> f) where T0 : unmanaged
        => Where(self, f, self);

    public static Span<T0> Where<T0>(this Span<T0> self, Func<T0, bool> f) where T0 : unmanaged
        => Where(self, f, new T0[self.Length]);

    public static Span<T> Take<T>(this Span<T> self, int count) where T : unmanaged 
        => self[..count];
        
    public static Span<T> Skip<T>(this Span<T> self, int count) where T : unmanaged 
        => self.Slice(count, self.Length - count);

    public static Span<T1> Cast<T0, T1>(this Span<T0> self) where T0 : unmanaged where T1 : unmanaged 
        => MemoryMarshal.Cast<T0, T1>(self);
}