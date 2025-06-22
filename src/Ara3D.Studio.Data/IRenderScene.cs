using Ara3D.Geometry;
using Ara3D.Memory;

namespace Ara3D.Studio.Data
{
    public interface IRenderScene
    {
        IBuffer<int> Indices { get; }
        IBuffer<Point3D> Vertices { get; }
        IBuffer<MeshSliceStruct> Meshes { get; }
        IBuffer<InstanceStruct> Instances { get; }
        IBuffer<InstanceGroupStruct> InstanceGroups { get; }
    }
}