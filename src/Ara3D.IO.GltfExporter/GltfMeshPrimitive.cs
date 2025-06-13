namespace Ara3D.IO.GltfExporter;

/// <summary>
/// Properties defining where the GPU should look to find the mesh and material data
/// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#meshes.
/// </summary>
public class GltfMeshPrimitive
{
    // We only use the position attribute 
    public GltfAttribute attributes { get; set; } = new();

    // The index of the accessor for indices
    public int indices { get; set; }

    public int? material { get; set; } = null;

    public int mode { get; set; } = 4; // 4 is triangles

    public GltfMeshPrimitive(int vertexAccessor, int indexAccessor, int material)
    {
        indices = indexAccessor;
        attributes.POSITION = vertexAccessor;
        this.material = material;
    }
}