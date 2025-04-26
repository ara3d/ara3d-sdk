namespace Ara3D.Buffers
{
    /// <summary>
    /// Represents a buffer associated with a string name. 
    /// </summary>
    public interface INamedBuffer<T> : IBuffer<T>, INamedBuffer
    {
    }

    /// <summary>
    /// Represents a buffer associated with a string name. 
    /// </summary>
    public interface INamedBuffer : IBuffer
    {
        string Name { get; }
    }
}