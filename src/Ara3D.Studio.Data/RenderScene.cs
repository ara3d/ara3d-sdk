﻿using Ara3D.Memory;

namespace Ara3D.Studio.Data
{
    public class RenderScene(
        IMemoryOwner<VertexStruct> vertexBlock,
        IMemoryOwner<uint> indexBlock,
        IMemoryOwner<MeshSliceStruct> meshBlock,
        IMemoryOwner<InstanceStruct> instanceBlock,
        IMemoryOwner<InstanceGroupStruct> groupBlock)
        : IRenderScene
    {
        public IMemoryOwner<VertexStruct> VertexBlock { get; } = vertexBlock;
        public IMemoryOwner<uint> IndexBlock { get; } = indexBlock;
        public IMemoryOwner<MeshSliceStruct> MeshBlock { get; } = meshBlock;
        public IMemoryOwner<InstanceStruct> InstanceBlock { get; } = instanceBlock;
        public IMemoryOwner<InstanceGroupStruct> GroupBlock { get; } = groupBlock;

        public IBuffer<VertexStruct> Vertices => VertexBlock;
        public IBuffer<uint> Indices => IndexBlock;
        public IBuffer<MeshSliceStruct> Meshes => MeshBlock;
        public IBuffer<InstanceStruct> Instances => InstanceBlock;
        public IBuffer<InstanceGroupStruct> Groups => GroupBlock;

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