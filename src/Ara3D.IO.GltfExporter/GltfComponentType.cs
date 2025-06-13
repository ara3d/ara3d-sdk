namespace Ara3D.IO.GltfExporter;

/// <summary>
/// Magic numbers to differentiate array buffer component types
/// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#accessor-element-size.
/// </summary>
public enum GltfComponentType
{
    BYTE = 5120,
    UNSIGNED_BYTE = 5121,
    SHORT = 5122,
    UNSIGNED_SHORT = 5123,
    UNSIGNED_INT = 5125,
    FLOAT = 5126,
}