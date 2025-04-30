using Ara3D.Memory;

namespace Ara3D.Data
{
    public interface IRenderScene
        : IDisposable
    {
        IBuffer<VertexStruct> Vertices { get; }
        IBuffer<uint> Indices { get; }
        IBuffer<MeshSliceStruct> Meshes { get; }
        IBuffer<InstanceStruct> Instances { get; }
        IBuffer<InstanceGroupStruct> Groups { get; }
    }
}