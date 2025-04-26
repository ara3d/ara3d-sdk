using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ara3D.Buffers;
using Ara3D.Logging;
using Ara3D.Serialization.BFAST;
using Ara3D.Utils;

namespace Ara3D.NarwhalDB
{
    public static class BFastReader2
    {
        public static unsafe IReadOnlyList<ByteSpan> UnpackStrings(this ByteSpan self)
        {
            var list = new List<ByteSpan>();
            var prev = self.Begin();
            for (var cur = prev; cur < self.End(); cur++)
            {
                if (*cur == 0)
                {
                    list.Add(new ByteSpan(prev, cur));
                    prev = cur + 1;
                }
            }

            if (prev < self.End())
            {
                list.Add(new ByteSpan(prev, self.End()));
            }

            return list;
        }

        public static unsafe IReadOnlyList<ByteSpanBuffer> Read(FilePath fp, ILogger logger)
        {
            logger.Log($"Reading {fp}");
            var mem = ReadBytesAligned(fp);
            logger.Log($"Read {PathUtil.BytesToString(mem.NumBytes)}");

            if (mem.NumBytes < BFastPreamble.Size)
                throw new Exception($"Not enough bytes {mem.NumBytes} to hold preamble for BFAST. Expected {BFastPreamble.Size}");

            logger.Log("Validating preamble");
            var preamble = *(BFastPreamble*)mem.BytePtr;
            preamble.Validate();
            logger.Log($"Preamble validated. Found {preamble.NumArrays} arrays");
            
            var ranges = new BFastRange[preamble.NumArrays];
            var rangeData = (BFastRange*)(mem.BytePtr + BFastPreamble.Size);
            for (var i = 0; i < ranges.Length; i++)
                ranges[i] = rangeData[i];
            var mainSpan = mem.ToByteSpan();

            if (ranges.Length == 0 || ranges.Length == 1)
                return Array.Empty<ByteSpanBuffer>();

            if (ranges.Any(r => r.Count > int.MaxValue))
                throw new Exception($"Maximum size of any range is 2GB");

            logger.Log($"Slicing arrays");
            var byteSpans = ranges.Select(range => mainSpan.Slice(range.Begin, (int)range.Count)).ToArray();

            logger.Log($"Unpacking buffer names");
            var nameData = byteSpans[0].ToArray();
            var names = nameData.UnpackStrings();

            if (names.Length != ranges.Length - 1)
                throw new Exception($"Expected number of names ({names.Length}) to be {ranges.Length - 1}");

            return byteSpans.Skip(1).Zip(names, ByteSpanBuffer.Create).ToList();
        }

        public static PinnedByteArray ReadBytesAligned(FilePath path, int bufferSize = 1024 * 1024)
        {
            using (var fs = new FileStream(path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize,
                false))
            {
                var fileLength = fs.Length;
                if (fileLength > int.MaxValue)
                    throw new IOException("File too big: > 2GB");

                var count = (int)fileLength;
                var r = new PinnedByteArray(count);
                var offset = r.Offset;
                while (count > 0)
                {
                    var n = fs.Read(r.Bytes, offset, count);
                    if (n == 0)
                        break;
                    count -= n;
                }

                Debug.Assert(count == 0);
                return r;
            }
        }
    }
}