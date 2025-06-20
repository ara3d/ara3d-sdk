using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Ara3D.Geometry;
using Ara3D.Utils;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Ara3D.Studio.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct InstanceStruct
    {
        // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
        // Static properties 
        public static readonly uint Size = (uint)sizeof(InstanceStruct);

        // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
        // Static initializer - for debugging 
        static InstanceStruct()
            => Debug.Assert(Size == 64);

        // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
        // Fields 

        public Vector4 Row0;
        public Vector4 Row1;
        public Vector4 Row2;
        public uint ObjectIndex; //  4 bytes
        public uint SceneIndex;  //  4 bytes
        public uint PackedColor; // 4 bytes
        public uint MetallicRoughness; // (byte 0 == Metallic, byte 1 == Roughness, bytes 3-4 unused)

        // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
        // Constructor

        public InstanceStruct(Matrix4x4 transform,
            int objectIndex,
            int sceneIndex,
            Color color,
            float metallic,
            float roughness)
        {
            Matrix4x4 = transform;
            ObjectIndex = (uint)objectIndex;
            SceneIndex = (uint)sceneIndex;
            Color = color;
            Metallic = metallic;
            Roughness = roughness;
        }

        //==
        // Properties 

        public float Metallic
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MetallicRoughness.GetByte0().ToNormalizedFloat();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => MetallicRoughness = MetallicRoughness.SetByte0(value.ToByteFromNormalized());
        }

        public float Roughness
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MetallicRoughness.GetByte1().ToNormalizedFloat();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => MetallicRoughness = MetallicRoughness.SetByte1(value.ToByteFromNormalized());
        }

        public Color Color
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(
                PackedColor.GetByte0().ToNormalizedFloat(),
                PackedColor.GetByte1().ToNormalizedFloat(),
                PackedColor.GetByte2().ToNormalizedFloat(),
                PackedColor.GetByte3().ToNormalizedFloat());

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => PackedColor = PackedColor.SetBytes(
                    value.R.Value.ToByteFromNormalized(),
                    value.G.Value.ToByteFromNormalized(),
                    value.B.Value.ToByteFromNormalized(),
                    value.A.Value.ToByteFromNormalized());
        }

        public float Alpha 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => PackedColor.GetByte3().ToNormalizedFloat();
        }

        public bool Transparent
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Alpha < 0.99f;
        }

        public Matrix4x4 Matrix4x4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => throw new NotImplementedException("TODO");

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Row0 = new Vector4(value.M11, value.M21, value.M31, value.M41);
                Row1 = new Vector4(value.M12, value.M22, value.M32, value.M42);
                Row2 = new Vector4(value.M13, value.M23, value.M33, value.M43);
            }
        }
    }
}
