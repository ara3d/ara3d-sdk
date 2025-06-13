using Newtonsoft.Json;

namespace Ara3D.IO.GltfExporter;

/// <summary>
/// A reference to a subsection of a buffer containing either vector or scalar data.
/// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#buffers-and-buffer-views
/// </summary>
public class GltfBufferView
{
    public GltfBufferView(int buffer, int byteOffset, int byteLength, GltfTargets target, string name)
    {
        this.buffer = buffer;
        this.byteOffset = byteOffset;
        this.byteLength = byteLength;
        this.target = target;
        this.name = name;
    }

    [JsonProperty("buffer")]
    public int buffer { get; set; }

    [JsonProperty("byteOffset")]
    public int byteOffset { get; set; }

    [JsonProperty("byteLength")]
    public int byteLength { get; set; }

    [JsonIgnore] // Ignore the raw enum field so we can customize serialization
    public GltfTargets target { get; set; }

    [JsonProperty("target")]
    public int? TargetValue =>
        target switch
        {
            GltfTargets.ARRAY_BUFFER => (int)target,
            GltfTargets.ELEMENT_ARRAY_BUFFER => (int)target,
            _ => null
        };

    public bool ShouldSerializeTargetValue()
    {
        return TargetValue != null;
    }

    [JsonProperty("name")]
    public string name { get; set; }
}