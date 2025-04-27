using Plato;

namespace Ara3D.Data
{
    public class RenderScene : IRenderScene
    {
        public MemoryBlock<VertexStruct> VertexBlock { get; }
        public MemoryBlock<uint> IndexBlock { get; }
        public MemoryBlock<MeshSliceStruct> MeshBlock { get; }
        public MemoryBlock<InstanceStruct> InstanceBlock { get; }
        public MemoryBlock<InstanceGroupStruct> GroupBlock { get; }

        public IBuffer<VertexStruct> Vertices => VertexBlock;
        public IBuffer<uint> Indices => IndexBlock;
        public IBuffer<MeshSliceStruct> Meshes => MeshBlock;
        public IBuffer<InstanceStruct> Instances => InstanceBlock;
        public IBuffer<InstanceGroupStruct> Groups => GroupBlock;

        public RenderScene(
            MemoryBlock<VertexStruct> vertexBlock,
            MemoryBlock<uint> indexBlock,
            MemoryBlock<MeshSliceStruct> meshBlock,
            MemoryBlock<InstanceStruct> instanceBlock,
            MemoryBlock<InstanceGroupStruct> groupBlock)
        {
            VertexBlock = vertexBlock;
            IndexBlock = indexBlock;
            MeshBlock = meshBlock;
            InstanceBlock = instanceBlock;
            GroupBlock = groupBlock;
        }

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