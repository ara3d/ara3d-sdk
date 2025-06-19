using Ara3D.Geometry;
using Ara3D.Memory;

namespace Ara3D.Studio.Data
{
    public interface IRenderScene
        : IDisposable
    {
        IBuffer<Point3D> Vertices { get; }
        IBuffer<uint> Indices { get; }
        IBuffer<MeshSliceStruct> Meshes { get; }
        IBuffer<InstanceStruct> Instances { get; }
        IBuffer<InstanceGroupStruct> Groups { get; }
    }
}