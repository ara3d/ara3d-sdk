using System;
using System.IO;

namespace Ara3D.Utils
{
    public static class StreamUtil
    {
        /// <summary>
        /// Advances a stream a fixed number of bytes.
        /// </summary>
        public static void Advance(this Stream stream, long count, int bufferSize = 4096)
        {
            if (stream.CanSeek)
            {
                stream.Position += count;
                return;
            }

            var buffer = new byte[bufferSize];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, (int)Math.Min(buffer.Length, count))) > 0)
            {
                count -= bytesRead;
            }
        }

        /// <summary>
        /// The official Stream.Read iis a PITA, because it could return anywhere from 0 to the number of bytes
        /// requested, even in mid-stream. This call will read everything it can until it reaches
        /// the end of the stream of "count" bytes.
        /// </summary>
        public static int SafeRead(this Stream stream, byte[] buffer, int offset, int count)
        {
            var r = stream.Read(buffer, offset, count);
            if (r != 0 && r < count)
            {
                // We didn't read everything, so let's keep trying until we get a zero
                while (true)
                {
                    var tmp = stream.Read(buffer, r, count - r);
                    if (tmp == 0)
                        break;
                    r += tmp;
                }
            }

            return r;
        }

        /// <summary>
        /// Reads all bytes from a stream
        /// https://stackoverflow.com/questions/1080442/how-to-convert-an-stream-into-a-byte-in-c
        /// </summary>
        public static byte[] ReadAllBytes(this Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}