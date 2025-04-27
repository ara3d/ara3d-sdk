using System.Numerics;
using Ara3D.Utils;

namespace Ara3D.Data
{
  
    public class RenderSceneBuilder : IDisposable, IRenderScene
    {
        public UnmanagedList<VertexStruct> VertexList = new();
        public UnmanagedList<uint> IndexList = new();
        public UnmanagedList<MeshSliceStruct> MeshList = new();
        public UnmanagedList<InstanceStruct> InstanceList = new();
        public UnmanagedList<InstanceGroupStruct> InstanceGroupList = new();
        
        public IBuffer<VertexStruct> Vertices => VertexList;
        public IBuffer<uint> Indices => IndexList;
        public IBuffer<MeshSliceStruct> Meshes => MeshList;
        public IBuffer<InstanceStruct> Instances => InstanceList;
        public IBuffer<InstanceGroupStruct> Groups => InstanceGroupList;

        public static InstanceStruct ToInstance(MeshSliceStruct mesh)
            => new InstanceStruct
            {
                Bounds = mesh.Bounds,
                Radius = mesh.Bounds.Radius,
                PosX = 0,
                PosY = 0,
                PosZ = 0,
                Color = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                Orientation = Quaternion.Identity,
                Scale = Vector3.One,
                Metallic = 0,
                Roughness = 0.5f,
            };

        public void AddMesh(IEnumerable<Vector3> positions, IEnumerable<int> indices, bool addInstance)
            => AddMesh((ReadOnlySpan<Vector3>)positions.ToArray(), (ReadOnlySpan<int>)indices.ToArray(), addInstance);

        public void AddMesh(IEnumerable<Vector3> positions, IEnumerable<Vector3> normals, IEnumerable<int> indices, bool addInstance)
            => AddMesh(
                (ReadOnlySpan<Vector3>)positions.ToArray(),
                (ReadOnlySpan<Vector3>)normals.ToArray(),
                (ReadOnlySpan<int>)indices.ToArray(), addInstance);

        public void AddMesh(Vector3[] positions, int[] indices, bool addInstance)
            => AddMesh((ReadOnlySpan<Vector3>)positions, (ReadOnlySpan<int>)indices, addInstance);

        public void AddMesh(ReadOnlySpan<Vector3> positions, ReadOnlySpan<int> indices, bool addInstance)
            => AddMesh(positions, ToNormals(positions, indices), indices, addInstance);

        public void AddMesh(ReadOnlySpan<Vector3> positions, ReadOnlySpan<Vector3> normals, ReadOnlySpan<int> indices, bool addInstance)
        {
            // Create a new mesh slice 
            var meshSlice = new MeshSliceStruct()
            {
                FirstIndex = (uint)IndexList.Count,
                IndexCount = (uint)indices.Length,
                BaseVertex = (int)VertexList.Count,
            };

            // Get the index of this mesh 
            var meshIndex = MeshList.Count;
            MeshList.Add(meshSlice);

            // Add the instance group 
            var group = new InstanceGroupStruct()
            {
                BaseInstance = (uint)InstanceList.Count,
                InstanceCount = 1,
                MeshIndex = (uint)meshIndex
            };
            InstanceGroupList.Add(group);

            // Add the vertices (i.e., positions and normals)
            for (var i = 0; i < positions.Length; i++)
                AddVertex(positions[i], normals[i]);

            // Add the indices 
            foreach (var index in indices)
                IndexList.Add((uint)index);

            ComputeBounds(ref meshSlice);

            if (addInstance)
            {
                InstanceList.Add(ToInstance(meshSlice));
            }
        }

        public void AddVertex(Vector3 pos, Vector3 normal)
            => VertexList.Add(new VertexStruct(pos, normal));

        public void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            var normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
            IndexList.Add((uint)VertexList.Count);
            AddVertex(a, normal);
            IndexList.Add((uint)VertexList.Count);
            AddVertex(b, normal);
            IndexList.Add((uint)VertexList.Count);
            AddVertex(c, normal);
        }

        public static InstanceStruct CreateInstance(MeshSliceStruct mesh)
            => new InstanceStruct
            {
                Bounds = mesh.Bounds,
                Radius = mesh.Bounds.Radius,
                PosX = 0,
                PosY = 0,
                PosZ = 0,
                Color = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                Orientation = Quaternion.Identity,
                Scale = Vector3.One,
                Metallic = 0,
                Roughness = 0.5f,
            };

        public void AddInstancesAndGroups(UnmanagedList<InstanceStruct> instances)
        {
            // TODO: a more efficient multi-dictionary would be nice.             
            var multiDict = new MultiDictionary<int, InstanceStruct>();
            for (var i = 0; i < instances.Count; i++)
            {             
                var instanceStruct = instances[i];
                multiDict.Add(instanceStruct.MeshIndex, instanceStruct);
            }
            foreach (var (meshIndex, instanceStructs) in multiDict)
            {
                var group = new InstanceGroupStruct()
                {
                    BaseInstance = (uint)InstanceList.Count,
                    InstanceCount = (uint)instanceStructs.Count,
                    MeshIndex = (uint)meshIndex
                };
                InstanceGroupList.Add(group);
                foreach (var instanceStruct in instanceStructs)
                {
                    InstanceList.Add(instanceStruct);
                }
            }
        }

        public static Vector3[] ToNormals(ReadOnlySpan<Vector3> positions, ReadOnlySpan<int> indices)
        {
            // TODO: this ia a naive normal computation algorithm .... Normals should be angle based for best accuracy. 
            var normals = new Vector3[positions.Length];
            for (var i = 0; i < indices.Length; i += 3)
            {
                var a = indices[i];
                var b = indices[i + 1];
                var c = indices[i + 2];
                var pa = positions[a];
                var pb = positions[b];
                var pc = positions[c];
                var n = Vector3.Normalize(Vector3.Cross(pb - pa, pc - pa));
                normals[a] += n;
                normals[b] += n;
                normals[c] += n;
            }

            return normals;
        }

        public void Dispose()
        {
            MeshList.Dispose();
            IndexList.Dispose();
            VertexList.Dispose();
            InstanceList.Dispose();
            InstanceGroupList.Dispose();
        }

        //=======================================================================================================
        // Bounds computation algorithms
        //=======================================================================================================

        public void ComputeBounds(ref MeshSliceStruct mesh)
        {
            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);
            for (var i = mesh.FirstIndex; i < mesh.FirstIndex + mesh.IndexCount; i++)
            {
                var index = IndexList[i];
                var vertex = VertexList[index];
                min = Vector3.Min(min, vertex.Position);
                max = Vector3.Max(max, vertex.Position);
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
                ComputeBounds(ref Instances[i], mesh.Bounds);
        }

        public void ComputeAllInstanceBounds()
        {
            for (var i=0; i < InstanceGroupList.Count; i++)
                ComputeAllInstanceBounds(ref InstanceGroupList[i]);
        }

        public void ComputeAllMeshAndInstanceBounds()
        {
            ComputeAllMeshBounds();
            ComputeAllInstanceBounds();
        }
    }
}