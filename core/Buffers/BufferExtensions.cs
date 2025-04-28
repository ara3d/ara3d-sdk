using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ara3D.Buffers
{
    /// <summary>
    /// Helper functions for working with buffers 
    /// </summary>
    public static class BufferExtensions
    {
        public static Buffer<T> ToBuffer<T>(this T[] xs) where T : unmanaged
            => new Buffer<T>(xs);

        public static INamedBuffer<T> Rename<T>(this INamedBuffer<T> xs, string name) where T : unmanaged
            => new NamedBuffer<T>(xs, name);

        public static INamedBuffer Rename(this INamedBuffer xs, string name) 
            => new NamedBuffer(xs, name);

        public static ReinterpretBuffer<T> Reinterpret<T>(this IBuffer xs) where T : unmanaged
            => new ReinterpretBuffer<T>(xs);

        public static INamedBuffer<T> Reinterpret<T>(this INamedBuffer xs) where T : unmanaged
            => (new ReinterpretBuffer<T>(xs)).ToNamedBuffer(xs.Name);

        public static SlicedBuffer<T> Slice<T>(this IBuffer<T> xs, int start, int count) where T : unmanaged
            => new SlicedBuffer<T>(xs, start, count);

        public static SlicedBuffer<T> Skip<T>(this IBuffer<T> xs, int start) where T : unmanaged
            => xs.Slice(start, xs.Count - start);

        public static SlicedBuffer<T> Take<T>(this IBuffer<T> xs, int count) where T : unmanaged
            => xs.Slice(0, count);

        public static NamedBuffer<T> ToNamedBuffer<T>(this T[] xs, string name = "") where T : unmanaged
            => new NamedBuffer<T>(xs, name);

        public static NamedBuffer<T> ToNamedBuffer<T>(this IEnumerable<T> xs, string name = "") where T : unmanaged
            => xs.ToArray().ToNamedBuffer(name);

        public static NamedBuffer ToNamedBuffer(this IBuffer buffer, string name = "")
            => new NamedBuffer(buffer, name);

        public static NamedBuffer<T> ToNamedBuffer<T>(this IBuffer<T> buffer, string name = "") where T: unmanaged
            => new NamedBuffer<T>(buffer, name);

        public static IEnumerable<INamedBuffer> ToNamedBuffers(this IEnumerable<IBuffer> buffers,
            IEnumerable<string> names = null)
            => names == null ? buffers.Select(b => ToNamedBuffer(b, "")) : buffers.Zip(names, ToNamedBuffer);

        public static IDictionary<string, INamedBuffer> ToDictionary(this IEnumerable<INamedBuffer> buffers)
            => buffers.ToDictionary(b => b.Name, b => b);

        public static IEnumerable<INamedBuffer> ToNamedBuffers(this IDictionary<string, IBuffer> d)
            => d.Select(kv => kv.Value.ToNamedBuffer(kv.Key));

        public static IEnumerable<INamedBuffer> ToNamedBuffers(this IDictionary<string, byte[]> d)
            => d.Select(kv => (INamedBuffer)kv.Value.ToNamedBuffer(kv.Key));

        public static long GetNumBytes(this IBuffer buffer)
            => (long)buffer.ElementCount * buffer.ElementSize;

        public static Buffer<T> ReadBufferFromNumberOfBytes<T>(this Stream stream, long numBytes) where T : unmanaged
            => stream.ReadArrayFromNumberOfBytes<T>(numBytes).ToBuffer();

        public static Buffer<T> ReadBuffer<T>(this Stream stream, int numElements) where T : unmanaged
            => stream.ReadArray<T>(numElements).ToBuffer();

        public static Buffer<byte> ReadBuffer(this Stream stream, int numBytes)
            => stream.ReadBuffer<byte>(numBytes);

        public static void Write(this Stream stream, IBuffer buffer)
            => stream.Write(buffer.Span<byte>());

        public static byte[] ToBytes(this INamedBuffer buffer)
            => buffer.Span<byte>().ToArray();
    }
}
