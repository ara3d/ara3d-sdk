namespace Ara3D.IO.GltfExporter;

/// <summary>
/// Magic numbers to differentiate scalar and vector array buffers
/// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#buffers-and-buffer-views.
/// </summary>
public enum GltfTargets
{
    NONE = 0,
    ARRAY_BUFFER = 34962, // signals vertex data
    ELEMENT_ARRAY_BUFFER = 34963, // signals index or face data
}