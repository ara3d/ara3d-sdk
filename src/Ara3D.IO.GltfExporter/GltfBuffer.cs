namespace Ara3D.IO.GltfExporter;

/// <summary>
/// A reference to the location and size of binary data
/// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#buffers-and-buffer-views.
/// </summary>
public class GltfBuffer
{
    /// <summary>
    /// Gets or sets the total byte length of the buffer.
    /// </summary>
    public int byteLength { get; set; }
}