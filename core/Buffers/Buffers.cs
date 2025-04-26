using System;
using System.Runtime.InteropServices;

namespace Ara3D.Buffers
{
    // TODO: this needs to be removed, along with ByteSpan 
    public unsafe class ByteSpanBuffer 
    {
        public readonly ByteSpan ByteSpan;

        public ByteSpanBuffer(ByteSpan byteSpan, string name)
        {
            ByteSpan = byteSpan;
            Name = name;
        }

        public void WithPointer(Action<IntPtr> action)
            => action(new IntPtr(ByteSpan.Ptr));

        public int ElementSize => 1;
        public int Count => ByteSpan.Length;
        public Span<T> Span<T>() where T : unmanaged
            => MemoryMarshal.Cast<byte, T>(ByteSpan.ToSpan());

        public string Name { get; }

        public static ByteSpanBuffer Create(ByteSpan span, string name)
            => new ByteSpanBuffer(span, name);
    }
}
