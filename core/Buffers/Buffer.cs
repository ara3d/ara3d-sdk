using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ara3D.Memory
{
    public unsafe class Buffer<T> : IBuffer<T>
        where T : unmanaged
    {
        public ByteSlice Bytes { get; }
        public int Count { get; }

        private readonly T* _pointer;

        public Buffer(ByteSlice bytes)
        {
            Bytes = bytes;
            _pointer = bytes.GetPointer<T>();
            Count = (int)(bytes.Length / sizeof(T));
        }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _pointer[index];
        }

        T IReadOnlyList<T>.this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[index];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i=0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public Type Type => typeof(T);

    }
}