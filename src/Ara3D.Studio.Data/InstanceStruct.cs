using System.Diagnostics;
using System.Drawing;
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
        // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
        // Static properties 
        public static readonly uint Size = (uint)sizeof(InstanceStruct2);

        // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
        // Static initializer - for debugging 
        static InstanceStruct2()
            => Debug.Assert(Size == 64);

        // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
        // 16-byte block #1: position + pad
        public Vector3 Pos;      // 12 bytes
        private float _pad0;   //  4 bytes (to align the next 16-byte field)

        // 16-byte block #2: orientation
        public Quaternion Orientation; // 16 bytes

        // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
        // 16-byte block #3: scale + pad
        public Vector3 Scale;    // 12 bytes
        private float _pad1;   //  4 bytes (to align the next 16-byte field)

        // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
        // 16-byte block #4: scene/object indices
        public uint ObjectIndex; //  4 bytes
        public uint SceneIndex;  //  4 bytes
        public uint Color; // 4 bytes
        public uint MetalRoughness; // 4 bytes
    }
}
