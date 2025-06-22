using Ara3D.Memory;
using Ara3D.Models;
using Ara3D.Geometry;
using Ara3D.Logging;

namespace Ara3D.Studio.Data
{
    public class RenderSceneBuilder : IRenderScene
    {
        public UnmanagedList<Point3D> VertexList = new();
        public UnmanagedList<int> IndexList = new();
        public UnmanagedList<MeshSliceStruct> MeshList = new();
        public UnmanagedList<InstanceStruct> InstanceList = new();
        public UnmanagedList<InstanceGroupStruct> InstanceGroupList = new();

        public IBuffer<Point3D> Vertices => VertexList;
        public IBuffer<int> Indices => IndexList;
        public IBuffer<MeshSliceStruct> Meshes => MeshList;
        public IBuffer<InstanceStruct> Instances => InstanceList;
        public IBuffer<InstanceGroupStruct> InstanceGroups => InstanceGroupList;

        public int AddMesh(TriangleMesh3D mesh)
        {
            // Create a new mesh slice 
            var meshSlice = new MeshSliceStruct()
            {
                FirstIndex = (uint)IndexList.Count,
                IndexCount = (uint)mesh.FaceIndices.Count * 3,
                BaseVertex = (int)VertexList.Count,
            };

            var meshIndex = MeshList.Count;
            MeshList.Add(meshSlice);
            VertexList.AddRange(mesh.Points);
            IndexList.AddRange(mesh.CornerIndices().Map(i => i.Value));
            return meshIndex;
        }

        public void AddModel(Model3D model, ILogger logger)
        {
            logger.LogDebug("Adding a model");

            var meshOffset = MeshList.Count;
            
            var newPointCount = model.Meshes.Sum(m => m.Points.Count);
            var newIndexCount = model.Meshes.Sum(m => m.FaceIndices.Count * 3);

            logger.Log($"Adding {model.Meshes.Count} meshes with a total of {newPointCount} more points, and {newIndexCount} more indices");
            VertexList.AccomodateMore(newPointCount);
            IndexList.AccomodateMore(newIndexCount);


            foreach (var mesh in model.Meshes)
                AddMesh(mesh);

            logger.LogDebug("Computing instances");
            var instanceGroups = model.Meshes.Count.MapRange(_ => new List<InstanceStruct>()).ToArray();
            
            foreach (var node in model.ElementStructs)
            {
                var transform = model.Transforms[node.TransformIndex];
                var material = model.Materials[node.MaterialIndex];
                var instance = new InstanceStruct(transform, 0, 0, material.Color, material.Metallic, material.Roughness);
                if (instanceGroups[node.MeshIndex] == null)
                    instanceGroups[node.MeshIndex] = new List<InstanceStruct>();
                instanceGroups[node.MeshIndex].Add(instance);
            }

            logger.LogDebug("Computing instance groups");
            foreach (var group in instanceGroups)
            {
                var meshIndex = meshOffset++;
                if (group == null || group.Count == 0)
                    continue;

                var instanceOffset = Instances.Count;
                foreach (var instance in group)
                {
                    InstanceList.Add(instance);
                }   
                var instanceGroupStruct = new InstanceGroupStruct()
                {
                    BaseInstance = (uint)instanceOffset,
                    InstanceCount = (uint)group.Count,
                    MeshIndex = (uint)meshIndex,
                };

                InstanceGroupList.Add(instanceGroupStruct);
            }

            logger.LogDebug("Completed creating scene");
        }

        public void Dispose()
        {
            MeshList.Dispose();
            IndexList.Dispose();
            VertexList.Dispose();
            InstanceList.Dispose();
            InstanceGroupList.Dispose();
        }
    }
}