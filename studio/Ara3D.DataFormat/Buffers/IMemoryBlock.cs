namespace Ara3D.Data
{
    /// <summary>
    /// Represents a pointer to a range of unmanaged memory.
    /// This might be a fixed array, or an IntPtr returned by an external API,
    /// or some memory that has been allocated by the user.
    /// It must be disposed when no longer used, or when it is known to be no longer valid.
    /// If it is external then Dispose will do nothing
    /// </summary>
    public interface IMemoryBlock : IDisposable 
    {
        /// <summary>
        /// Gets the pointer to the start of the memory block.
        /// </summary>
        IntPtr IntPtr { get; }

        /// <summary>
        /// Gets the size of the memory block in bytes.
        /// </summary>
        long NumBytes { get; }
    }
}