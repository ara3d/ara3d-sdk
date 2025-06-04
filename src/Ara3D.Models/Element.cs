using Ara3D.Geometry;

namespace Ara3D.Models;

/// <summary>
/// Bundles a mesh, a material, and a transformation matrix.
/// </summary>
public class Element
{
    public TriangleMesh3D Mesh { get; }
    public Material Material { get; }
    public Matrix4x4 Transform { get; }

    public Element(TriangleMesh3D mesh, Material material)
        : this(mesh, material, Matrix4x4.Identity) 
    { }

    public Element(TriangleMesh3D mesh)
        : this(mesh, Material.Default, Matrix4x4.Identity)
    { }

    public Element(TriangleMesh3D mesh, Matrix4x4 transform)
        : this(mesh, Material.Default, transform)
    { }

    public Element(TriangleMesh3D mesh, Material material, Matrix4x4 transform)
    {
        Mesh = mesh;
        Material = material;
        Transform = transform;
    }
}