using System;
using System.Linq;

namespace Ara3D.Buffers
{
    /// <summary>
    /// A concrete implementation of INamedBuffer with a specific type.
    /// </summary>
    public class NamedBuffer<T> : Buffer<T>, INamedBuffer<T> where T : unmanaged
    {
        public NamedBuffer(IBuffer<T> data, string name) : base(data.ToArray()) => Name = name;
        public NamedBuffer(T[] data, string name) : base(data) => Name = name;
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
        public int ElementSize => Buffer.ElementSize;
        public int ElementCount => Buffer.ElementCount;
        public Span<T> Span<T>() where T : unmanaged => Buffer.Span<T>();

        public Type ElementType => Buffer.ElementType;

        object IBuffer.this[int i]
        {
            get => Buffer[i];
            set => Buffer[i] = value;
        }
    }
}