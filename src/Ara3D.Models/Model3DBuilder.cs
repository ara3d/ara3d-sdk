using Ara3D.Geometry;

namespace Ara3D.Models;

public class Model3DBuilder 
{
    public List<TriangleMesh3D> Meshes { get; } = [];
    public List<InstanceStruct> Instances { get; } = [];
    
    public Model3D Build()
        => new(Meshes, Instances);

    public int AddInstance(int meshIndex, Matrix4x4 matrix)
        => AddInstance(meshIndex, matrix, Material.Default);

    public int AddInstance(int meshIndex, Material material)
        => AddInstance(meshIndex, Matrix4x4.Identity, material);

    public int AddInstance(InstanceStruct inst)
    {
        Instances.Add(inst);
        return Instances.Count - 1;
    }

    public int AddInstance(int meshIndex, Matrix4x4 matrix, Material material, int entityIndex = -1, byte flags = 0)
        => AddInstance(new InstanceStruct(entityIndex, matrix, meshIndex, material, flags));

    public void AddModel(IModel3D model)
    {
        var meshOffset = Meshes.Count;
        Meshes.AddRange(model.Meshes);
        foreach (var inst in model.Instances)
            Instances.Add(inst.WithMeshIndex(inst.MeshIndex + meshOffset));
    }

    public int AddInstance(TriangleMesh3D mesh, Material material)
        => AddInstance(mesh, Matrix4x4.Identity, material);

    public int AddInstance(TriangleMesh3D mesh, Matrix4x4 matrix, Material material)
        => AddInstance(AddMeshWithoutInstance(mesh), matrix, material);

    public int AddInstance(TriangleMesh3D mesh, Matrix4x4 matrix)
        => AddInstance(mesh, matrix,  Material.Default);

    public int AddInstance(TriangleMesh3D mesh)
        => AddInstance(mesh, Matrix4x4.Identity, Material.Default);

    public int AddMeshWithoutInstance(TriangleMesh3D mesh)
    {
        var r = Meshes.Count;
        Meshes.Add(mesh);
        return r;
    }

    public void AddInstanceAndRemapMesh(InstanceStruct inst, IModel3D other, Dictionary<int, int> meshRemap)
    {
        if (inst.MeshIndex < 0)
        {
            Instances.Add(inst);
        }
        else if (meshRemap.ContainsKey(inst.MeshIndex))
        {
            Instances.Add(inst.WithMeshIndex(meshRemap[inst.MeshIndex]));
        }
        else
        {
            var otherMesh = other.Meshes[inst.MeshIndex];
            var newMeshIndex = AddMeshWithoutInstance(otherMesh);
            meshRemap.Add(inst.MeshIndex, newMeshIndex);
            Instances.Add(inst.WithMeshIndex(newMeshIndex));
        }
    }
}