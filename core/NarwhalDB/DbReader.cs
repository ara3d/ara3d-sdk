using Ara3D.Buffers;
using Ara3D.Collections;

namespace Ara3D.NarwhalDB
{
    public readonly unsafe struct TypedSpan<T> 
        : IArray<T>
        where T : unmanaged
    {
        public int Count { get; }
        public readonly T* Data;
        public T this[int index] => Data[index];
        public IIterator<T> Iterator => new ArrayIterator<T>(this);
        public TypedSpan(ByteSpan span)
        {
            Data = (T*)span.Ptr;
            Count = span.Length / sizeof(T);
        }
    }
}