using System;
using System.Collections.Generic;

namespace Ara3D.Buffers
{
    /// <summary>
    /// Represents a buffer associated with a string name. 
    /// </summary>
    public interface IBuffer<T> : IBuffer, IReadOnlyList<T> 
    {
        new T this[int i] { get; set; }
        Span<T> Span();
    }

    /// <summary>
    /// Provides an interface to an object that manages an array of unmanaged memory.
    /// </summary>
    public interface IBuffer
    {
        int ElementSize { get; }
        Type ElementType { get; }
        int ElementCount { get; }
        object this[int i] { get; set; }
        Span<T> Span<T>() where T : unmanaged;
    }
}