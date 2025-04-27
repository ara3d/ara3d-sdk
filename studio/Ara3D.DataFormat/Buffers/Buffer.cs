namespace Ara3D.Data
{
    public unsafe class Buffer<T>
        : IBuffer<T> where T : unmanaged
    {
        private long _count;
        private T* _pointer;
        public long Count => _count;
        public T* Pointer => _pointer;
        public Buffer(long count, T* pointer)
        {
            _count = count;
            _pointer = pointer;
        }
    }
}