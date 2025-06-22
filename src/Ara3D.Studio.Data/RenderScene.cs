using Ara3D.Geometry;
using Ara3D.Memory;

namespace Ara3D.Studio.Data
{
    public class RenderScene(
        IBuffer<Point3D> vertices,
        IBuffer<int> indices,
        IBuffer<MeshSliceStruct> meshes,
        IBuffer<InstanceStruct> instances,
        IBuffer<InstanceGroupStruct> groups)
        : IRenderScene
    {
        public IBuffer<Point3D> Vertices { get; } = vertices;
        public IBuffer<int> Indices { get; } = indices;
        public IBuffer<MeshSliceStruct> Meshes { get; } = meshes;
        public IBuffer<InstanceStruct> Instances { get; } = instances;
        public IBuffer<InstanceGroupStruct> InstanceGroups { get; } = groups;
    }
}