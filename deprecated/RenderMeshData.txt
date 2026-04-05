using Ara3D.Memory;

namespace Ara3D.Models;

/// <summary>
/// This is a long-lived data structure that contains the data used for rendering point, lines, and triangles. 
/// </summary>
public class RenderMeshData
{
    public int PrimSize { get; }
    public bool Indexed { get; } 
    public UnmanagedList<float> Vertices { get; private set; } = new();
    public UnmanagedList<int> Indices { get; private set; } = new();

    public RenderMeshData(int primSize, bool indexed)
    {
        if (primSize < 1 || primSize > 4)
            throw new Exception("Only primitives of size 1 to 4 can be supported");
        PrimSize = primSize;
        Indexed = indexed;
    }

    public void Dispose()
    {
        Vertices?.Dispose();
        Indices?.Dispose();
        Vertices = null;
        Indices = null;
    }

    public int PrimitiveCount => Indexed
        ? Indices?.Count ?? 0 / PrimSize
        : Vertices?.Count ?? 0 / PrimSize;

    public void Update(IBuffer<float> vertices, IBuffer<int> indices = null)
    {
        if (Indexed)
        {
            if (indices != null && indices.Count % PrimSize != 0)
                throw new Exception($"Number of indices ({indices.Count}) is not a multiple of {PrimSize}");
        }
        else
        {
            if (vertices.Count % PrimSize != 0)
                throw new Exception($"Number of vertices ({vertices.Count}) is not a multiple of {PrimSize}");
        }

        Vertices.CopyFrom(vertices);

        if (indices != null) 
            Indices.CopyFrom(indices);
    }
}