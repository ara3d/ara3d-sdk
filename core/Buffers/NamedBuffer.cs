using System;
using System.Linq;

namespace Ara3D.Memory
{
    /// <summary>
    /// A concrete implementation of INamedBuffer with a specific type.
    /// </summary>
    public class NamedBuffer<T> : Buffer<T>, ITypedNamedBuffer, INamedBuffer<T> where T : unmanaged
    {
        public NamedBuffer(IBuffer<T> data, string name) : base(data.Bytes) => Name = name;
        public string Name { get; }
    }

    /// <summary>
    /// A concrete implementation of INamedBuffer
    /// </summary>
    public class NamedBuffer : INamedBuffer
    {
        public NamedBuffer(IBuffer buffer, string name) => (Buffer, Name) = (buffer, name);
        public IBuffer Buffer { get; }
        public string Name { get; }
        public ByteSlice Bytes => Buffer.Bytes;
    }
}