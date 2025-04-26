using System;
using System.Collections;
using System.Collections.Generic;

namespace Ara3D.Buffers
{
    public unsafe class ReinterpretBuffer<T> : IBuffer<T>
        where T : unmanaged
    {
        public IBuffer Original;
        public int ElementSize => sizeof(T);
        public int ElementCount { get; }
        public int Count => ElementCount;

        public ReinterpretBuffer(IBuffer original)
        {
            Original = original;
            
            ElementCount = (int)(original.GetNumBytes() / ElementSize);

            if (this.GetNumBytes() != original.GetNumBytes())
                throw new Exception("The original buffer cannot be cast into the new buffer");
        }

        public Span<T0> Span<T0>() where T0 : unmanaged
            => Original.Span<T0>();

        public T this[int i]
        {
            get => Span()[i];
            set => Span()[i] = value;
        }

        public Span<T> Span()
            => Span<T>();

        public Type ElementType => typeof(T);

        object IBuffer.this[int i]
        {
            get => this[i];
            set => this[i] = (T)value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < ElementCount; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}