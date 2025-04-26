using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ara3D.Buffers
{
    public class SlicedBuffer<T> : IBuffer<T>
        where T : unmanaged
    {
        public IBuffer<T> Original;
        public int Offset;
        public int ElementCount { get; }
        public int Count => ElementCount;

        public int ElementSize => Original.ElementSize;
        public SlicedBuffer(IBuffer<T> original, int offset, int count)
        {
            Original = original;
            Offset = offset;
            Debug.Assert(count >= 0);
            Debug.Assert(count <= original.Count);
            ElementCount = count;
        }

        public T this[int i]
        {
            get => Original[i + Offset];
            set => Original[i + Offset] = value;
        }

        public Span<T0> Span<T0>() where T0 : unmanaged
            => Original.Span<T0>().Slice(Offset, ElementCount);

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
            for (var i=0; i < ElementCount; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}