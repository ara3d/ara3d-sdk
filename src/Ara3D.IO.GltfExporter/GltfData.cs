namespace Ara3D.IO.GltfExporter;

/// <summary>
/// The json serializable glTF file format
/// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0.
/// </summary>
public class GltfData
{
    public GltfVersion asset = new();
    public List<GltfScene> scenes = new();
    public List<GltfNode> nodes = new();
    public List<GltfMesh> meshes = new();
    public List<GltfBuffer> buffers = new();
    public List<GltfBufferView> bufferViews = new();
    public List<GltfAccessor> accessors = new();
    public List<GltfMaterial> materials = new();
    public List<byte> bytes = new();
}