namespace Ara3D.IO.GltfExporter;

/// <summary>
/// The glTF PBR Material format
/// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#materials.
/// </summary>
public class GltfMaterial
{
    public string alphaMode { get; set; }

    public float? alphaCutoff { get; set; }

    public string name { get; set; }

    public GltfPbr pbrMetallicRoughness { get; set; }

    public bool doubleSided { get; set; }
}