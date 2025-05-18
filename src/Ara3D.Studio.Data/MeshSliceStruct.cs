using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ara3D.Studio.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MeshSliceStruct
    {
        public static readonly int Size = sizeof(MeshSliceStruct);

        public int BaseVertex;
        public uint FirstIndex;
        public uint IndexCount;
        public uint padding0;
        public Bounds Bounds;
        public float padding1;
        public float padding2;

        static MeshSliceStruct()
        {
            Debug.Assert(Size == 48);
        }

        public override bool Equals(object? obj)
            => obj is MeshSliceStruct other && Equals(other);

        public bool Equals(MeshSliceStruct other)
            => BaseVertex == other.BaseVertex 
               && FirstIndex == other.FirstIndex 
               && IndexCount == other.IndexCount;
    }
}