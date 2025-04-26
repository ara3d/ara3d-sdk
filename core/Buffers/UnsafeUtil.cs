using System;
using System.IO;

namespace Ara3D.Buffers
{
    public static unsafe class UnsafeUtil
    {
        /// <summary>
        /// Helper for reading arbitrary unmanaged types from a Stream. 
        /// </summary>
        public static void ReadBytesBuffered(this Stream stream, byte* dest, long count, int bufferSize = 4096)
        {
            var buffer = new byte[bufferSize];
            fixed (byte* pBuffer = buffer)
            {
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, (int)System.Math.Min(buffer.Length, count))) > 0)
                {
                    if (dest != null)
                        Buffer.MemoryCopy(pBuffer, dest, count, bytesRead);
                    count -= bytesRead;
                    dest += bytesRead;
                }
            }
        }

        /// <summary>
        /// Helper for writing arbitrary large numbers of bytes 
        /// </summary>
        public static void Write(this Stream stream, IntPtr ptr, long count, int bufferSize = 4096)
            => stream.Write((byte*)ptr.ToPointer(), count, bufferSize);

        /// <summary>
        /// Helper for writing arbitrary large numbers of bytes 
        /// </summary>
        public static void Write(this Stream stream, Span<byte> span, int bufferSize = 4096)
        {
            fixed (byte* ptr = span)
                stream.Write(ptr, span.Length, bufferSize);
        }

        /// <summary>
        /// Helper for writing arbitrary large numbers of bytes 
        /// </summary>
        public static void Write(this Stream stream, byte* src, long count, int bufferSize = 4096)
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
        /// Helper for reading arbitrary unmanaged types from a Stream. 
        /// </summary>
        public static void Read<T>(this Stream stream, T* dest) where T : unmanaged
            => stream.ReadBytesBuffered((byte*)dest, sizeof(T));

        /// <summary>
        /// Helper for reading arrays of arbitrary unmanaged types from a Stream, that might be over 2GB of size.
        /// Arrays are limited to 2gb in size unless gcAllowVeryLargeObjects is set to true
        /// in the runtime environment.
        /// https://docs.microsoft.com/en-us/dotnet/api/system.array?redirectedfrom=MSDN&view=netframework-4.7.2#remarks
        /// That said, in C#, you can still never load more int.MaxValue (approx 2billion)
        /// numbers of items.
        /// </summary>
        public static T[] ReadArray<T>(this Stream stream, int count) where T : unmanaged
        {
            var r = new T[count];
            fixed (T* pDest = r)
            {
                var pBytes = (byte*)pDest;
                stream.ReadBytesBuffered(pBytes, (long)count * sizeof(T));
            }

            return r;
        }

        /// <summary>
        /// A wrapper for stream.Seek(numBytes, SeekOrigin.Current) to avoid allocating memory for unrecognized buffers.
        /// </summary>
        public static void SkipBytes(this Stream stream, long numBytes)
            => stream.Seek(numBytes, SeekOrigin.Current);

        /// <summary>
        /// Helper for reading arrays of arbitrary unmanaged types from a Stream, that might be over 2GB of size.
        /// That said, in C#, you can never load more int.MaxValue numbers of items. 
        /// </summary>
        public static T[] ReadArrayFromNumberOfBytes<T>(this Stream stream, long numBytes) where T : unmanaged
        {
            var count = numBytes / sizeof(T);
            if (numBytes % sizeof(T) != 0)
                throw new Exception(
                    $"The number of bytes {numBytes} is not divisible by the size of the type {sizeof(T)}");
            if (count >= int.MaxValue)
                throw new Exception(
                    $"{count} exceeds the maximum number of items that can be read into an array {int.MaxValue}");
            return stream.ReadArray<T>((int)count);
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
    }
}