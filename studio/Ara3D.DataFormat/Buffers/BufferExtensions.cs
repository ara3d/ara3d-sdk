namespace Ara3D.Data
{
    public static class BufferExtensions
    {
        public static unsafe void WithBuffer<T>(this T[] data, Action<IBuffer<T>> onBuffer)
            where T : unmanaged
        {
            fixed (T* p = data)
            {
                onBuffer(new Buffer<T>(data.Length, p));
            }
        }

        public static unsafe void MemoryCopy<T0, T1>(this IBuffer<T0> src, IBuffer<T1> dest) where T0 : unmanaged where T1: unmanaged
        {
            Buffer.MemoryCopy(src.Pointer, dest.Pointer, dest.GetNumBytes(), src.GetNumBytes());
        }

        public static MemoryBlock<T> Copy<T>(this IBuffer<T> self) where T : unmanaged
        {
            var r = new MemoryBlock<T>(self.Count);
            self.MemoryCopy(r);
            return r;
        }

        public static IEnumerable<T> Enumerate<T>(this IBuffer<T> self) where T : unmanaged
        {
            for (var i = 0; i < self.Count; ++i)
                yield return self[i];
        }
    }
}