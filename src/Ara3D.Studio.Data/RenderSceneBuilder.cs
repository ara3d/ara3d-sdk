using System.Runtime.InteropServices;
using Ara3D.Memory;
using Ara3D.Models;
using Ara3D.Utils;
using Ara3D.Geometry;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace Ara3D.Studio.Data
{
    // TODO: this could be SO much better.
    // Store positions. Store indices. Compute normals at the end. 
    // Do the grouping of the instances at the end. 
    // Do all of the bounds computation at the end. 
    // Use different structures for building the scene.
    // Make this VERY fast. 
    // All for the data to be manipulated!
    // ComputeNormals()
    // ComputeBounds()
    public class RenderSceneBuilder :  IRenderScene
    {
        public UnmanagedList<Point3D> VertexList = new();
        public UnmanagedList<uint> IndexList = new();
        public UnmanagedList<MeshSliceStruct> MeshList = new();
        public UnmanagedList<InstanceStruct> InstanceList = new();
        public UnmanagedList<InstanceGroupStruct> GroupList = new();
        
        public IBuffer<Point3D> Vertices => VertexList;
        public IBuffer<uint> Indices => IndexList;
        public IBuffer<MeshSliceStruct> Meshes => MeshList;
        public IBuffer<InstanceStruct> Instances => InstanceList;
        public IBuffer<InstanceGroupStruct> Groups => GroupList;

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

        public void AddMesh(IEnumerable<Vector3> positions, IEnumerable<int> indices)
            => AddMesh(positions.ToArray().AsSpan(), indices.ToArray().AsSpan());

        public static IEnumerable<int> GetIndices(TriangleMesh3D self)
        {
            foreach (var fi in self.FaceIndices)
            {
                yield return fi.A;
                yield return fi.B;
                yield return fi.C;
            }
        }

        // TODO: ideally we would use reinterpret casts 
        public void AddMesh(TriangleMesh3D mesh)
            => AddMesh(
                mesh.Points.Select(p => new Vector3(p.X, p.Y, p.Z)), 
                GetIndices(mesh));

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

        public void AddInstancesAndGroups(System.Collections.Generic.IReadOnlyList<InstanceStruct> instances)
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
                GroupList.Add(group);
                foreach (var instanceStruct in instanceStructs)
                {
                    InstanceList.Add(instanceStruct);
                }
            }
        }

        public static Vector3[] ToNormals(IReadOnlyList<Vector3> positions, IReadOnlyList<int> indices)
        {
            var normals = new Vector3[positions.Count];
            for (var i = 0; i < indices.Count; i += 3)
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

        public void AddModel(Model3D model)
        {
            var meshOffset = MeshList.Count;

            foreach (var mesh in model.Meshes)
                AddMesh(mesh);

            var instances = new List<InstanceStruct>();
            foreach (var node in model.ElementStructs)
            {
                var meshIndex = meshOffset + node.MeshIndex;
                var transform = model.Transforms[node.TransformIndex];
                var (translation, quaternion, scale, success) = transform.Decompose;
                var material = model.Materials[node.MaterialIndex];
                var color = material.Color;
                var instance = new InstanceStruct()
                {
                    MeshIndex = meshIndex,
                    Color = new Vector4(color.R, color.G, color.B, color.A),
                    Roughness = material.Roughness,
                    Metallic = material.Metallic,
                    Orientation = quaternion,
                    Scale = scale,
                    Position = translation
                };
                instances.Add(instance);
            }
            AddInstancesAndGroups(instances);
        }

        public void AddMesh(ReadOnlySpan<Vector3> positions, ReadOnlySpan<int> indices)
        {
#if DEBUG
            foreach (var index in indices)
            {
                if (index < 0 || index >= positions.Length)
                    throw new Exception($"Index out of range is {index} expected max {positions.Length}");
            }
#endif 

            // Create a new mesh slice 
            var meshSlice = new MeshSliceStruct()
            {
                FirstIndex = (uint)IndexList.Count,
                IndexCount = (uint)indices.Length,
                BaseVertex = (int)VertexList.Count,
            };

            MeshList.Add(meshSlice);

            VertexList.AddRange(MemoryMarshal.Cast<Vector3, Point3D>(positions));
            IndexList.AddRange(MemoryMarshal.Cast<int, uint>(indices));

            ComputeBounds(ref meshSlice);
        }


        public void Dispose()
        {
            MeshList.Dispose();
            IndexList.Dispose();
            VertexList.Dispose();
            InstanceList.Dispose();
            GroupList.Dispose();
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
}