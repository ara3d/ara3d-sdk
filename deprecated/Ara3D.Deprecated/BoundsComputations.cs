using System.Numerics;

namespace Ara3D.Studio.Data;

public static class BoundsComputations
{
    //=======================================================================================================
    // Bounds computation algorithms
    //=======================================================================================================

    public void ComputeBounds(ref MeshSliceStruct mesh)
    {
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);
        for (var i = mesh.FirstIndex; i < mesh.FirstIndex + mesh.IndexCount; i++)
        {
            var index = IndexList[(int)i];
            var vertex = VertexList[(int)index];
            min = Vector3.Min(min, vertex.Vector3);
            max = Vector3.Max(max, vertex.Vector3);
        } 
        mesh.Bounds = new Bounds(min, max);
    }

    public void ComputeAllMeshBounds()
    {
        for (var i=0; i < Meshes.Count; i++)
            ComputeBounds(ref Meshes[i]);
    }

    public static void ComputeBounds(ref InstanceStruct instance, in Bounds meshBounds)
        => instance.Bounds = meshBounds.ApplyTransform(instance.Transform);

    public void ComputeAllInstanceBounds(ref InstanceGroupStruct group)
    {
        ref var mesh = ref Meshes[(int)group.MeshIndex];
        for (var i=group.BaseInstance; i < group.BaseInstance + group.InstanceCount; i++)
            ComputeBounds(ref Instances[(int)i], mesh.Bounds);
    }

    public void ComputeAllInstanceBounds()
    {
        for (var i=0; i < GroupList.Count; i++)
            ComputeAllInstanceBounds(ref GroupList[i]);
    }

    public void ComputeAllMeshAndInstanceBounds()
    {
        ComputeAllMeshBounds();
        ComputeAllInstanceBounds();
    }
}