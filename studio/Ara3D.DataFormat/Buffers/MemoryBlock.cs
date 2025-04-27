using System.Runtime.InteropServices;

namespace Ara3D.Data
{
    /// <summary>
    /// Represents a block of unmanaged memory with a specified alignment.
    /// Can only represent blocks up 
    /// </summary>
    public unsafe class MemoryBlock<T> : IDisposable, IBuffer<T>
        where T : unmanaged 
    {
        /// <summary>
        /// Safe IntPtr 
        /// </summary>
        public IntPtr IntPtr => (IntPtr)Pointer;

        /// <summary>
        /// Number of bytes in the memory block.
        /// </summary>
        public long NumBytes
            => Count * sizeof(T);

        /// <summary>
        /// Gets the aligned pointer to the start of the memory block.
        /// </summary>
        public T* Pointer { get; private set; }

        /// <summary>
        /// The number of items in the memory block.
        /// </summary>
        public long Count { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryBlock"/> class with the specified size and alignment.
        /// </summary>
        public MemoryBlock(long count, uint alignment = 512)
        {
            Count = count;
            Pointer = (T*)NativeMemory.AlignedAlloc((nuint)NumBytes, alignment);
        }

        /// <summary>
        /// Releases the unmanaged memory.
        /// </summary>
        public void Dispose()
        {
            if (Pointer != null)
                NativeMemory.AlignedFree(Pointer);
            Pointer = null;
        }

        /// <summary>
        /// Implicit conversion to a pointer
        /// </summary>
        /// <param name="block"></param>
        public static implicit operator T*(MemoryBlock<T> block) 
            => block.Pointer;
        
        /// <summary>
        /// Implicit conversion to an IntPtr
        /// </summary>
        /// <param name="block"></param>
        public static implicit operator IntPtr(MemoryBlock<T> block) 
            => (IntPtr)block.Pointer;

        /// <summary>
        /// Implicit conversion to a span
        /// </summary>
        /// <param name="block"></param>
        public static implicit operator Span<T>(MemoryBlock<T> block)
        {
            if (block.Count > int.MaxValue)
                throw new Exception("Memory block is too large to convert to a Span");
            return new Span<T>(block.Pointer, (int)block.Count);
        }
    }
}