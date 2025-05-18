using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Ara3D.Studio.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct InstanceStruct
    {
        // 32 bytes
        public double PosX;
        public double PosY;
        public double PosZ;
        public double Pos_Unused;

        // 32 bytes
        public Quaternion Orientation;
        public Vector3 Scale; 
        public float _ScalePadding;

        // 32 bytes (not easily loaded into a shader)
        public Bounds Bounds;
        public float Radius;
        public float _BoundsPadding;

        // 32 bytes
        public Vector4 Color;
        public float Metallic;
        public float Roughness;
        public int MeshIndex;
        public uint Padding1;

        // Computed properties
        public Transform Transform => new(Position, Orientation, Scale);
        public Vector3 Position
        {
            get => new((float)PosX, (float)PosY, (float)PosZ); 
            set => (PosX, PosY, PosZ) = (value.X, value.Y, value.Z);
        }

        public bool Transparent => Color.W < 0.95f;

        // Static properties 
        public static readonly uint Size = (uint)sizeof(InstanceStruct);

        static InstanceStruct()
        {
            Debug.Assert(Size == 128);
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct InstanceStruct2
    {
        // 12 bytes
        public float Pos;

        // 16 bytes
        public Quaternion Orientation;

        // 12 bytes
        public Vector3 Scale;

        // 12 bytes 
        public Bounds Bounds;

        // 4 bytes
        public int Color;

        // 2 bytes
        public byte Metallic;
        public byte Roughness;
    }
}
