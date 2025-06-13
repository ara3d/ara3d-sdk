using Newtonsoft.Json;

namespace Ara3D.IO.GltfExporter;

/// <summary>
/// The list of accessors available to the renderer for a particular mesh
/// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#meshes.
/// </summary>
public class GltfAttribute
{
    /// <summary>
    /// Gets or sets the index of the accessor for position data.
    /// </summary>
    public int POSITION { get; set; }

    /// <summary>
    /// Gets or sets the index of the accessor for normal data.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? NORMAL { get; set; }

    /// <summary>
    /// Gets or sets the index of the UV Coordinates of the material's textures.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? TEXCOORD_0 { get; set; }
}