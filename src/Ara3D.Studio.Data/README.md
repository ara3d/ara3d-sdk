# Ara3D.Studio.Data

This project contains the low-level data structures used for efficiently representing a scene for rendering. 

The primary data structure is `RenderScene`. It is designed so that it can be used efficiently with the OpenGL 
MultiDrawIndirect API, which allows for rendering all of the objects in the rendered scene in a single draw call.

> Technically there are two separate `RenderScene` instances used under the hood: one for transparent objects, and another for opaque objects. 

The `RenderScene` contains the following buffers:

```
IBuffer<VertexStruct> Vertices;
IBuffer<uint> Indices;
IBuffer<MeshSliceStruct> Meshes;
IBuffer<InstanceStruct> Instances;
IBuffer<InstanceGroupStruct> Groups; 
```

Buffers are low-level memory structures exposed by the `Ara3D.Memory` library. 
They are a more convenient replacements for `Span<T>` in that it exposes `IReadOnlyList` and can be stored in the heap. 

