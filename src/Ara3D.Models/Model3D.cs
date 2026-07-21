using Ara3D.Collections;
using Ara3D.Geometry;

namespace Ara3D.Models;

/// <summary>
/// A model is a collection of meshes and instances.
/// A mesh is a triangular mesh with vertices and indices. 
/// Instances are: transform, mesh index, material data, and an entity index. 
/// </summary>
public class Model3D 
    : ITransformable3D<Model3D>, IModel3D
{
    public Model3D(
        IReadOnlyList<TriangleMesh3D> meshes,
        IReadOnlyList<InstanceStruct> instances)
    {
        Meshes = meshes;
        Instances = instances;
    }

    public static Model3D Empty = new([], []);

    public IReadOnlyList<TriangleMesh3D> Meshes { get; }
    public IReadOnlyList<InstanceStruct> Instances { get; }

    public static Model3D Create(TriangleMesh3D mesh, Material material, Matrix4x4 matrix, byte flags)
        => new([mesh], [new(InstanceStruct.NoEntityIndex, matrix, meshIndex: 0, material, flags)]);

    public static Model3D Create(TriangleMesh3D mesh, Material material, IReadOnlyList<Matrix4x4> matrices)
        => new([mesh], matrices.Select(m => new InstanceStruct(InstanceStruct.NoEntityIndex, m, meshIndex: 0, material)));

    public static Model3D Create(TriangleMesh3D mesh, Material material)
        => Create(mesh, material, Matrix4x4.Identity, 0);

    public static Model3D Create(TriangleMesh3D mesh)
        => Create(mesh, Material.Default);

    public static implicit operator Model3D(TriangleMesh3D m)
        => Create(m);

    public IModel3D Transform(Transform3D t)
        => Model3DExtensions.Transform(this, t);

    Model3D ITransformable3D<Model3D>.Transform(Transform3D t)
        => Model3DExtensions.Transform(this, t);
}