using System.Diagnostics;

namespace Ara3D.Studio.Data
{
    public unsafe class SceneCounts
    {
        public readonly int NumVertices;
        public readonly int NumIndices;
        public readonly int NumMeshes;
        public readonly int NumInstances;
        public readonly int NumGroups;

        public readonly long VerticesOffset;
        public readonly long IndicesOffset;
        public readonly long MeshesOffset;
        public readonly long InstancesOffset;
        public readonly long GroupsOffset;

        public SceneCounts(int numVertices, int numIndices, int numMeshes, int numInstances, int numGroups)
        {
            NumVertices = numVertices;
            NumIndices = numIndices;
            NumMeshes = numMeshes;
            NumInstances = numInstances;
            NumGroups = numGroups;

            VerticesOffset = 0;
            IndicesOffset = VerticesOffset + VerticesSize;
            MeshesOffset = IndicesOffset + IndicesSize;
            InstancesOffset = MeshesOffset + MeshesSize;
            GroupsOffset = InstancesOffset + InstancesOffset;

            Debug.Assert(GroupsOffset + GroupsSize == TotalSize);

            Debug.Assert(VerticesOffset % RequiredAlignment == 0);
            Debug.Assert(IndicesOffset % RequiredAlignment == 0);
            Debug.Assert(MeshesOffset % RequiredAlignment == 0);
            Debug.Assert(InstancesOffset % RequiredAlignment == 0);
            Debug.Assert(GroupsOffset % RequiredAlignment == 0);
        }

        public const int RequiredAlignment = 8;

        // Make sure that the number of bytes for indices is divisible by 8 
        public static int AlignIndexCounts(int indices)
            => indices % 2 == 1 ? indices + 1 : indices;

        public long VerticesSize => NumVertices * VertexStruct.Size;
        public long IndicesSize => AlignIndexCounts(NumIndices) * sizeof(int);
        public long InstancesSize => NumInstances * InstanceStruct.Size;
        public long MeshesSize => NumMeshes * MeshSliceStruct.Size;
        public long GroupsSize => NumGroups * InstanceGroupStruct.Size;

        public long TotalSize => VerticesSize + IndicesSize + InstancesSize + MeshesSize + GroupsSize;

        public Span<VertexStruct> GetVertexSpan(byte* ptr) => new((VertexStruct*)(ptr + VerticesOffset), NumVertices);
        public Span<uint> GetIndexSpan(byte* ptr) => new((uint*)(ptr + IndicesOffset), NumIndices);
        public Span<InstanceStruct> GetInstanceSpan(byte* ptr) => new((InstanceStruct*)(ptr + InstancesOffset), NumInstances);
        public Span<MeshSliceStruct> GetMeshSpan(byte* ptr) => new((MeshSliceStruct*)(ptr + MeshesOffset), NumMeshes);
        public Span<InstanceGroupStruct> GetGroupSpan(byte* ptr) => new((InstanceGroupStruct*)(ptr + GroupsOffset), NumGroups);
    }
}