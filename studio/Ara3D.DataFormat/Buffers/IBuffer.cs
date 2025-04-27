namespace Ara3D.Data
{
    /// <summary>
    /// Represents a pointer to a range of unmanaged memory.
    /// </summary>
    public unsafe interface IBuffer<T> 
        where T: unmanaged
    {
        long Count { get; }
        T* Pointer { get; }
        ref T this[long i] => ref Pointer[i];
    }
}