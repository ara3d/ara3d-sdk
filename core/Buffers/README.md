# Ara3D.Memory

This is a collection of useful classes and interfaces in C# and .NET 8 for 
efficiently working with potentially large amounts of aligned low-level memory. 

The primary classes and structs are:

* `AlignedMemory` - A block of fixed memory that is aligned to a specific byte boundary. This makes casting between SIMD type (like Vector256) safe and efficient. It can be larger than 2GB. 
* `FixedArray` - A pointer to an array that is fixed in memory, and provides access via ByteSlices and Spans.  
* `ByteSlice` - A pointer to a region of memory, with a length. Similar to a `Span<byte>` except that it can be stored on the heap and can be longer than 2GB. Provides helpers for safe casting to unmanaged types. 
* `UnmanagedList<T>` - A dynamic array of unmanaged types which uses, and makes public, an aligned memory block. Can grow but not shrink. 
* `Buffer<T>` - A types-safe wrapper around a `ByteSlice` and that exposes an array-like interface for reading and writing. 

The interfaces are:

* `IBuffer` - A generic block of memory accessible as a slice. 
* `ITypedBuffer` - A generic interface for a buffer of unmanaged types. 
* `INamedBuffer` - A buffer with an associated name.
* `IBuffer<T>` - An array of unmanaged types. Implements `IReadOnlyList<T>`
* `INamedBuffer<T>` - An array of unmanaged types associated with a name.

