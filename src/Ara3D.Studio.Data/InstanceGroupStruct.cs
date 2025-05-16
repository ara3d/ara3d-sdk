using System.Diagnostics;

namespace Ara3D.Data
{
    public unsafe struct InstanceGroupStruct
    {
        public static int Size => sizeof(InstanceGroupStruct);

        public uint MeshIndex;
        public uint BaseInstance;
        public uint InstanceCount;

        static InstanceGroupStruct()
        {
            Debug.Assert(Size == 12);
        }

        public override bool Equals(object? obj)
            => obj is InstanceGroupStruct other && Equals(other);

        public bool Equals(InstanceGroupStruct other)
            => MeshIndex == other.MeshIndex
               && BaseInstance == other.BaseInstance
               && InstanceCount == other.InstanceCount;
    }
}