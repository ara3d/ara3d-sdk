using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ara3D.Studio.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MeshSliceStruct
    {
        public int BaseVertex;
        public uint FirstIndex;
        public uint IndexCount;

        public override bool Equals(object? obj)
            => obj is MeshSliceStruct other && Equals(other);

        public bool Equals(MeshSliceStruct other)
            => BaseVertex == other.BaseVertex 
               && FirstIndex == other.FirstIndex 
               && IndexCount == other.IndexCount;
    }
}