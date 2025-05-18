using System;
using System.Collections.Generic;

namespace Ara3D.Memory
{
    /// <summary>
    /// Provides an interface to an object that manages an array of unmanaged memory.
    /// </summary>
    public interface IBuffer
    {
        ByteSlice Bytes { get; }
    }

    /// <summary>
    /// Represents a buffer associated with a string name. 
    /// </summary>
    public interface INamedBuffer : IBuffer
    {
        string Name { get; }
    }

    /// <summary>
    /// Represents a buffer with a known run-time type.
    /// </summary>
    public interface ITypedBuffer : IBuffer
    {
        Type Type { get; }
    }

    /// <summary>
    /// Represents a buffer with a known run-time type and a name.
    /// </summary>
    public interface ITypedNamedBuffer : ITypedBuffer, INamedBuffer
    {
    }

    /// <summary>
    /// Represents a buffer associated with a string name. 
    /// </summary>
    public interface IBuffer<T> : ITypedBuffer, IReadOnlyList<T> 
    {
        new ref T this[int i] { get; }
    }

    /// <summary>
    /// Represents a buffer associated with a string name. 
    /// </summary>
    public interface INamedBuffer<T> : IBuffer<T>, INamedBuffer
    { }

    /// <summary>
    /// A block of memory is a buffer that owns the actual memory and is responsible for cleaning it up 
    /// </summary>
    public interface IMemoryOwner : IBuffer, IDisposable 
    { }

    /// <summary>
    /// A block of memory is a buffer that owns the actual memory and is responsible for cleaning it up 
    /// </summary>
    public interface IMemoryOwner<T> : IBuffer<T>, IMemoryOwner
    { }

    /// <summary>
    /// A named block of memory is a buffer
    /// that owns the actual memory and is responsible for cleaning it up 
    /// </summary>
    public interface INamedMemoryOwner : INamedBuffer, IMemoryOwner
    { }
}