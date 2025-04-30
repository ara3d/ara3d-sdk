using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ara3D.Memory;

namespace Ara3D.BFAST
{
    /// <summary>
    /// Anything that can be added to a BFAST must have a size and write to a stream.
    /// </summary>
    public interface IBFastComponent
    {
        long GetSize();
        void Write(Stream stream);
    }

    /// <summary>
    /// A wrapper around a buffer so that it can be used as a BFAST component 
    /// </summary>
    public class BufferAsBFastComponent : IBFastComponent
    {
        public BufferAsBFastComponent(IBuffer buffer)
            => Buffer = buffer;
        public IBuffer Buffer { get; }
        public void Write(Stream stream) => stream.WriteBuffer(Buffer);
        public long GetSize() => Buffer.Length();
    }

    /// <summary>
    /// Used to build BFASTs incrementally that contain named buffers and/or other BFASTs. 
    /// </summary>
    public class BFastBuilder : IBFastComponent
    {
        public BFastHeader Header { get; private set; }
        public long GetSize() => GetOrComputeHeader().Preamble.DataEnd;

        public List<(string, IBFastComponent)> Children { get; } = new List<(string, IBFastComponent)>();

        public void Write(Stream stream)
            => stream.Write(GetOrComputeHeader(),
                BufferNames().ToArray(),
                BufferSizes().ToArray(),
                OnBuffer);

        public void Write(string filePath)
        {
            using (var stream = File.OpenWrite(filePath))
                Write(stream);
        }

        public long OnBuffer(Stream stream, int index, string name, long size)
        {
            var (bufferName, x) = Children[index];
            Debug.Assert(name == bufferName);
            Debug.Assert(size != GetSize());
            Debug.Assert(size == x.GetSize());
            x.Write(stream);
            return size;
        }

        public BFastHeader GetOrComputeHeader()
            => Header ?? (Header = BFast.CreateBFastHeader(
                BufferSizes().ToArray(), BufferNames().ToArray()));

        private BFastBuilder _add(string name, IBFastComponent component)
        {
            Header = null;
            Children.Add((name, component));
            return this;
        }

        public BFastBuilder Add(string name, IBFastComponent component)
            => _add(name, component);

        public BFastBuilder Add(string name, IBuffer buffer)
            => _add(name, new BufferAsBFastComponent(buffer));

        public BFastBuilder Add(INamedBuffer buffer)
            => Add(buffer.Name, buffer);

        public BFastBuilder Add(IEnumerable<INamedBuffer> buffers)
            => buffers.Aggregate(this, (x, y) => x.Add(y));

        public BFastBuilder Add(string name, IEnumerable<INamedBuffer> buffers)
            => Add(name, new BFastBuilder().Add(buffers));

        public IEnumerable<string> BufferNames()
            => Children.Select(x => x.Item1);

        public IEnumerable<long> BufferSizes()
            => Children.Select(x => x.Item2.GetSize());
    }
}