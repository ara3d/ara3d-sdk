using Ara3D.Geometry;
using Ara3D.Memory;

namespace Ara3D.Studio.Data
{
    public class RenderScene(
        IMemoryOwner<Point3D> vertexBlock,
        IMemoryOwner<int> indexBlock,
        IMemoryOwner<MeshSliceStruct> meshBlock,
        IMemoryOwner<InstanceStruct> instanceBlock,
        IMemoryOwner<InstanceGroupStruct> groupBlock)
        : IRenderScene
    {
        public IMemoryOwner<Point3D> VertexBlock { get; } = vertexBlock;
        public IMemoryOwner<int> IndexBlock { get; } = indexBlock;
        public IMemoryOwner<MeshSliceStruct> MeshBlock { get; } = meshBlock;
        public IMemoryOwner<InstanceStruct> InstanceBlock { get; } = instanceBlock;
        public IMemoryOwner<InstanceGroupStruct> GroupBlock { get; } = groupBlock;

        public IBuffer<Point3D> Vertices => VertexBlock;
        public IBuffer<int> Indices => IndexBlock;
        public IBuffer<MeshSliceStruct> Meshes => MeshBlock;
        public IBuffer<InstanceStruct> Instances => InstanceBlock;
        public IBuffer<InstanceGroupStruct> InstanceGroups => GroupBlock;

        public void Dispose()
        {
            VertexBlock.Dispose();
            IndexBlock.Dispose();
            InstanceBlock.Dispose();
            MeshBlock.Dispose();
            GroupBlock.Dispose();
        }
    }
}