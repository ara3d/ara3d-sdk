using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ara3D.Buffers
{
    /// <summary>
    /// A concrete implementation of IBuffer
    /// </summary>
    public unsafe class Buffer<T> : IBuffer<T>
        where T : unmanaged
    {
        public Buffer(T[] data) => _data = data;
        public int ElementSize => sizeof(T);
        private readonly T[] _data;
        public int ElementCount => _data.Length;
        public int Count => _data.Length;

        public T this[int i]
        {
            get => _data[i];
            set => _data[i] = value;
        }

        public Span<T> Span()
            => Span<T>();

        public Span<T0> Span<T0>() where T0 : unmanaged
            => MemoryMarshal.Cast<T, T0>(_data);

        public Type ElementType => typeof(T);
        
        object IBuffer.this[int i]
        {
            get => this[i];
            set => this[i] = (T)value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i= 0; i < ElementCount; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}