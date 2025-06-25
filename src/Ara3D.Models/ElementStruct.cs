using Ara3D.Geometry;

namespace Ara3D.Models;

/// <summary>
/// An element is a part of a 3D model. It has a mesh, a material, and a transformation.
/// The element struct uses indices to reference the data stored in the parent model.
/// </summary>
public readonly struct ElementStruct
{
    public ElementStruct(int elementIndex, int materialIndex, int meshIndex, int transformIndex)
    {
        ElementIndex = elementIndex;
        MaterialIndex = materialIndex;
        MeshIndex = meshIndex;
        TransformIndex = transformIndex;
    }

    public int ElementIndex { get; }
    public int MaterialIndex { get; }
    public int MeshIndex { get; }
    public int TransformIndex { get; }
}