using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ara3D.Memory;

/// <summary>
/// Wrap an IMemoryOwner in a typed wrapper. 
/// </summary>
/// <typeparam name="T"></typeparam>
[SkipLocalsInit]
public class MemoryOwner<T> : IMemoryOwner<T>
    where T : unmanaged
{
    public IMemoryOwner Memory { get; private set; }
    public IBuffer<T> Buffer { get; private set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryOwner(IMemoryOwner memory)
    {
        Memory = memory;
        Buffer = memory.Reinterpret<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        Memory.Dispose();
        Memory = null;
        Buffer = null;
    }

    ~MemoryOwner()
    {
        Dispose();
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Buffer.Count; 
    }


    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Buffer[index];
    }

    T IReadOnlyList<T>.this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this[index];
    }


    public ByteSlice Bytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        get => Buffer.Bytes;
    }

    public Type Type
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Buffer.Type; 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<T> GetEnumerator()
        => Buffer.GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)Buffer).GetEnumerator();
}