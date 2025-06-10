namespace Ara3D.IO.GltfExporter;

/// <summary>
/// A reference to a subsection of a BufferView containing a particular data type
/// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#accessors.
/// </summary>
public class GltfAccessor
{
    public GltfAccessor(int bufferView, int byteOffset, GltfComponentType gltfComponentType, int count, string type, 
        List<float> min, List<float> max, string name)
    {
        this.bufferView = bufferView;
        this.byteOffset = byteOffset;
        this.GltfComponentType = gltfComponentType;
        this.count = count;
        this.type = type;
        this.min = min;
        this.max = max;
        this.name = name;
    }

    /// <summary>
    /// Gets or sets the index of the bufferView.
    /// </summary>
    public int bufferView { get; set; }

    /// <summary>
    /// Gets or sets the offset relative to the start of the bufferView in bytes.
    /// </summary>
    public int byteOffset { get; set; }

    /// <summary>
    /// Gets or sets the datatype of the components in the attribute.
    /// </summary>
    public GltfComponentType GltfComponentType { get; set; }

    /// <summary>
    /// Gets or sets the number of attributes referenced by this accessor.
    /// </summary>
    public int count { get; set; }

    /// <summary>
    /// Gets or sets the specifies if the attribute is a scala, vector, or matrix.
    /// </summary>
    public string type { get; set; }

    /// <summary>
    /// Gets or sets the maximum value of each component in this attribute.
    /// </summary>
    public List<float> max { get; set; }

    /// <summary>
    /// Gets or sets the minimum value of each component in this attribute.
    /// </summary>
    public List<float> min { get; set; }

    /// <summary>
    /// Gets or sets a user defined name for this accessor.
    /// </summary>
    public string name { get; set; }
}