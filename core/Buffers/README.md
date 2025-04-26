# Ara3D.Buffers

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.Buffers)](https://www.nuget.org/packages/Ara3D.Buffers)

Utilities for working with large arrays of unmanaged types. 

## IBuffer

The `IBuffer` interface is presents a simple interface for an 
array of unmanaged types, where the type is not known at compile-time. 

This allows us to work more easily with collections of buffers where the 
underlying type can vary at run-time. 
 
 ```csharp
    public interface IBuffer
    {
        Array Data { get; }
        void WithPointer(Action<IntPtr> action);
        int ElementSize { get; }
    }
```

The total number of bytes is determined by multiply the data array by the element size.
This is provided in an extension function

```csharp
    public static long GetNumBytes(this IBuffer buffer)
        => (long)buffer.NumElements() * buffer.ElementSize;
```

Accessing the raw bytes can be done using a pattern similar to:

```csharp
    public static unsafe void Write(this Stream stream, IBuffer buffer)
        => buffer.WithPointer(ptr => {
            stream.WriteBytesBuffered((byte*)ptr.ToPointer(), buffer.GetNumBytes());
        });
```

## INamedBuffer

Frequently a buffer is associated with a name or identifier. 
The `INamedBuffer` introduces a string name property, for convenience.   

```csharp
    public interface INamedBuffer : IBuffer
    {
        string Name { get; }
    }
```

## Generic Versions

There exist generic versions of the interfaces as well, which provide access to the underlying data array.  

```csharp
    public interface IBuffer<out T> : IBuffer
        where T: unmanaged
    {
        T[] GetTypedData();
    }

    public interface INamedBuffer<out T> 
        : INamedBuffer, IBuffer<T> where T: unmanaged
    {
    }
```

## Use Case

This structure is particularly useful when serializing and manipulating raw memory such 
as attribute buffers associated with 3D geometry, ZIP files (or other binary archive formats), 
or color data associated with bitmaps. 

## Documentation 

This is a very small library, and we recommend reading the source code and looking at some projects 
that use it:

* Ara3D.Graphics
* Ara3D.Serialization.BFAST
* Ara3D.Serialization.G3D
* Ara3D.Serialization.VIM

## Size Limitations

Due to limitations in C# the array can have at most 2^31 
elements (approximately 2 billion). 

## About ByteSpan 

`ByteSpan` is designed specifically for working with pointers to unmanaged (e.g. pinned) memory. 

A `ByteSpan` consists of a literal pointer to a byte, and the number of bytes. 
It doesn’t have the `ref struct` limitations of `Span<byte>` data type and is faster 
for memory accessing (just grab the pointer and address it). 

The only caveat is you have to pin memory before using it, and be careful about avoiding accessing 
memory access overrun. 

Pinning memory prevents the garbage collector from moving it or freeing it, putting the responsibility of lifetime 
management of the memory on the developer. 

Pinning memory involves a call to `GCHandle.Alloc(<your array>, GCHandlerType.Pinned)` which returns a handle, 
and provides raw accesss to memory via `AddrOfPinnedObject()`.

While this is unusual for a C# developer, this is common practice for C/C++ developers.

It just means that as software developers, we need to consider how and when we deallocate memory in a reasonable way. 
It is no different than the requirements of any disposable resource (e.g., file handles, or window contexts). 



