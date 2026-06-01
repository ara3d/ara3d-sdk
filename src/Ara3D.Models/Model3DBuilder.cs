using Ara3D.Geometry;
using Ara3D.Collections;

namespace Ara3D.Models;

public class Model3DBuilder 
{
    public List<TriangleMesh3D> Meshes { get; } = [];
    public List<InstanceStruct> Instances { get; } = [];
    
    public Model3D Build()
        => new(Meshes, Instances);

    public void AddInstance(int meshIndex, Matrix4x4 matrix)
        => AddInstance(meshIndex, matrix, Material.Default);

    public void AddInstance(int meshIndex, Material material)
        => AddInstance(meshIndex, Matrix4x4.Identity, material);

    public void AddInstance(InstanceStruct inst)
        => Instances.Add(inst);

    public void AddInstance(int meshIndex, Matrix4x4 matrix, Material material, int entityIndex = -1, byte flags = 0)
        => AddInstance(new InstanceStruct(entityIndex, matrix, meshIndex, material, flags));

    public void AddModel(IModel3D model)
    {
        var meshOffset = Meshes.Count;
        Meshes.AddRange(model.Meshes);
        foreach (var inst in model.Instances)
            Instances.Add(inst.WithMeshIndex(inst.MeshIndex + meshOffset));
    }

    public void AddInstance(TriangleMesh3D mesh, Material material)
        => AddInstance(mesh, Matrix4x4.Identity, material);

    public void AddInstance(TriangleMesh3D mesh, Matrix4x4 matrix, Material material)
        => AddInstance(AddMesh(mesh), matrix, material);

    public void AddInstance(TriangleMesh3D mesh)
        => AddInstance(mesh, Matrix4x4.Identity, Material.Default);

    public int AddMesh(TriangleMesh3D mesh)
    {
        var r = Meshes.Count;
        Meshes.Add(mesh);
        return r;
    }
}