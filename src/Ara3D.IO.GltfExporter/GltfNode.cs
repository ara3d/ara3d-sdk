using System.Numerics;

namespace Ara3D.IO.GltfExporter;

/// <summary>
/// The nodes defining individual (or nested) elements in the scene
/// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#nodes-and-hierarchy.
/// </summary>
public class GltfNode
{
    public GltfNode(Matrix4x4 mat, int meshIndex, string name = null)
    {
        SetMatrix(mat);
        mesh = meshIndex;
        this.name = name;
    }

    /// <summary>
    /// Gets or sets the user-defined name of this object.
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// Gets or sets the index of the mesh in this node.
    /// </summary>
    public int? mesh { get; set; } = null;

    public static List<float> ToGltfArray(Matrix4x4 m)
        => [
            // column 1
            m.M11, m.M21, m.M31, m.M41,
            // column 2
            m.M12, m.M22, m.M32, m.M42,
            // column 3
            m.M13, m.M23, m.M33, m.M43,
            // column 4
            m.M14, m.M24, m.M34, m.M44
        ];

    public void SetMatrix(Matrix4x4 m)
        => matrix = ToGltfArray(m); 
    
    /// <summary>
    /// Gets or sets a floating-point 4x4 transformation matrix stored in column major order.
    /// </summary>
    public List<float> matrix { get; set; }

}