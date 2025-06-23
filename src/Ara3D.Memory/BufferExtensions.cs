using System;
using System.Runtime.InteropServices;

namespace Ara3D.Memory
{
    /// <summary>
    /// Helper functions for working with buffers 
    /// </summary>
    public static unsafe class BufferExtensions
    {
        public static long NumBytes(this IBuffer buffer)
            => buffer.Bytes.Length;

        public static byte* GetPointer(this IBuffer buffer)
            => buffer.Bytes.Begin;

        public static T* GetPointer<T>(this IBuffer<T> buffer) where T : unmanaged
            => buffer.Bytes.GetPointer<T>();

        public static Span<T> AsSpan<T>(this IBuffer<T> buffer) where T : unmanaged
            => buffer.Bytes.AsSpan<T>();

        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this IBuffer<T> buffer) where T : unmanaged
            => buffer.AsSpan();

        public static T* GetPointer<T>(this IBuffer buffer) where T: unmanaged
            => buffer.Bytes.GetPointer<T>();

        public static Buffer<byte> ToBuffer(this ByteSlice self)
            => new(self);

        public static Buffer<T> ToBuffer<T>(this ByteSlice self)
            where T : unmanaged
            => new(self);

        public static NamedBuffer<T> Rename<T>(this IBuffer<T> xs, string name) where T : unmanaged
            => new(xs, name);

        public static NamedBuffer Rename(this INamedBuffer xs, string name) 
            => new(xs, name);

        public static Buffer<T> Reinterpret<T>(this IBuffer xs) where T : unmanaged
            => new(xs.Bytes);

        public static NamedBuffer<T> Reinterpret<T>(this INamedBuffer xs) where T : unmanaged
            => ((IBuffer)xs).Reinterpret<T>().Rename(xs.Name);

        public static Buffer<T> Slice<T>(this IBuffer<T> xs, long start, long count) where T : unmanaged
            => xs.Bytes.Slice(start * Marshal.SizeOf<T>(), count * Marshal.SizeOf<T>()).ToBuffer<T>();

        public static Buffer<T> Skip<T>(this IBuffer<T> xs, long start) where T : unmanaged
            => xs.Slice(start, xs.Count - start);

        public static Buffer<T> Take<T>(this IBuffer<T> xs, long count) where T : unmanaged
            => xs.Slice(0, count);

        public static NamedBuffer ToNamedBuffer(this IBuffer buffer, string name = "") 
            => new(buffer, name);

        public static NamedBuffer<T> ToNamedBuffer<T>(this IBuffer<T> buffer, string name = "") where T : unmanaged =>
            new(buffer, name);

        public static NamedBuffer<T> ToNamedBuffer<T>(this IBuffer buffer, string name = "") where T : unmanaged =>
            new(buffer.Reinterpret<T>(), name);

        public static FixedArray<T> Fix<T>(this T[] self) where T : unmanaged
            => new(self);

        public static MemoryOwner<T> Reinterpret<T>(this IMemoryOwner self) where T : unmanaged
            => new(self);

        public static long ElementSize(this ITypedBuffer buffer)
            => Marshal.SizeOf(buffer.Type);

        public static long ElementCount(this ITypedBuffer buffer)
            => buffer.Bytes.Count / buffer.ElementSize();

        public static IntPtr ElementPointer(this ITypedBuffer buffer, long n)
            => new(buffer.GetPointer() + n * buffer.ElementSize());

        public static object GetElement(this ITypedBuffer buffer, long n)
            => Marshal.PtrToStructure(buffer.ElementPointer(n), buffer.Type);

        public static INamedMemoryOwner ToNamedMemoryOwner(this byte[] bytes, string name)
            => new NamedAlignedMemory(bytes.Fix(), name);
    }
}
