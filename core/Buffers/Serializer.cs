using System;
using System.IO;

namespace Ara3D.Memory
{
    public static unsafe class Serializer
    {
        public static AlignedMemory ReadAllBytesAligned(string path)
            => File.OpenRead(path).ReadAllBytesAligned();

        public const int DefaultBufferSize = 1024 * 1024; 

        public static AlignedMemory ReadAllBytesAligned(this Stream stream)
        {
            if (!stream.CanSeek)
                throw new NotSupportedException("Stream must support seeking");
            var r = new AlignedMemory(stream.Length);
            var dest = r.Bytes.Begin;
            var remaining = r.Bytes.Length;
            while (remaining > 0)
            {
                var count = Math.Min(remaining, DefaultBufferSize);
                var n = stream.Read(dest, (int)count);
                if (n == 0)
                    break;
                remaining -= n;
                dest += n;
            }
            if (remaining != 0)
                throw new Exception($"Failed to read all bytes from stream. {remaining} bytes remaining");
            stream.Flush();
            stream.Dispose();
            return r;
        }

        /// <summary>
        /// Helper for reading bytes to a pointer. 
        /// </summary>
        public static int Read(this Stream stream, byte* dest, int count)
            => stream.Read(new Span<byte>(dest, count));

        /// <summary>
        /// Helper for reading bytes to a buffer from the current position in a stream.
        /// </summary>
        public static IBuffer ReadBuffer(this Stream stream, int count)
        {
            var r = new AlignedMemory(count);
            stream.ReadExactly(r.Bytes);
            return r;
        }

        /// <summary>
        /// Helper for writing arbitrary large numbers of bytes 
        /// </summary>
        public static void Write(this Stream stream, IntPtr ptr, long count, int bufferSize = DefaultBufferSize)
            => stream.Write((byte*)ptr.ToPointer(), count, bufferSize);

        /// <summary>
        /// Helper for writing arbitrary large numbers of bytes 
        /// </summary>
        public static void Write(this Stream stream, Span<byte> span, int bufferSize = DefaultBufferSize)
        {
            fixed (byte* ptr = span)
                stream.Write(ptr, span.Length, bufferSize);
        }

        /// <summary>
        /// Helper for writing arbitrary large numbers of bytes 
        /// </summary>
        public static void Write(this Stream stream, byte* src, long count, int bufferSize = DefaultBufferSize)
        {
            var buffer = new byte[bufferSize];
            if (bufferSize <= 0)
                throw new Exception("Buffer size must be greater than zero");
            fixed (byte* pBuffer = buffer)
            {
                while (count > 0)
                {
                    var toWrite = (int)Math.Min(count, bufferSize);
                    Buffer.MemoryCopy(src, pBuffer, bufferSize, toWrite);
                    stream.Write(buffer, 0, toWrite);
                    count -= toWrite;
                    src += toWrite;
                }
            }
        }

        /// <summary>
        /// Helper for writing arbitrary unmanaged types 
        /// </summary>
        public static void WriteValue<T>(this Stream stream, T x) where T : unmanaged
        {
            var p = &x;
            stream.Write((byte*)p, sizeof(T));
        }

        /// <summary>
        /// Helper for writing arrays of unmanaged types 
        /// </summary>
        public static void Write<T>(this Stream stream, T[] xs) where T : unmanaged
        {
            fixed (T* p = xs)
                stream.Write((byte*)p, xs.LongLength * sizeof(T));
        }

        /// <summary>
        /// Helper for writing buffers. 
        /// </summary>
        public static void WriteBuffer(this Stream stream, IBuffer buffer)
            => stream.Write(buffer.Bytes);
    }
}
