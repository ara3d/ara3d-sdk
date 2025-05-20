using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ara3D.Memory;

public unsafe sealed class FixedArray<T> : IMemoryOwner<T>
    where T: unmanaged
{
    private bool _disposed;
    public T[] Array { get; private set; }
    public GCHandle Handle { get; private set; }
    public ByteSlice Bytes { get; }

    public FixedArray(T[] array)
    {
        Array = array;
        Handle = GCHandle.Alloc(Array, GCHandleType.Pinned);
        Bytes = new((byte*)Handle.AddrOfPinnedObject().ToPointer(), Array.Length * Marshal.SizeOf<T>());
    }

    public static FixedArray<T> Empty 
        = new([]);

    public Type Type => typeof(T);

    public IEnumerator<T> GetEnumerator()
        => (IEnumerator<T>)Array.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => Array.GetEnumerator();

    public int Count 
        => Array.Length;

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Array[index];
    }

    T IReadOnlyList<T>.this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this[index];
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        if (Handle.IsAllocated)
            Handle.Free();
        Handle.Free();
        Handle = default;
        Array = null;
        GC.SuppressFinalize(this);
    }

    ~FixedArray()
    {
        Dispose();
    }
}