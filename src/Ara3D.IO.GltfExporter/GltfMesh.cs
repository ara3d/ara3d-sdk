namespace Ara3D.IO.GltfExporter;

/// <summary>
/// The array of primitives defining the mesh of an object
/// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#meshes.
/// </summary>
public class GltfMesh
{
    public List<GltfMeshPrimitive> primitives { get; set; }

    public string name { get; set; }
}