using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ara3D.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct VertexStruct
    {
        public static readonly uint Size = (uint)sizeof(VertexStruct);

        public static readonly nint PositionOffset = Marshal.OffsetOf<VertexStruct>(nameof(Position));
        public Vector3 Position;

        public static readonly nint NOffset = Marshal.OffsetOf<VertexStruct>(nameof(N));
        public uint N;

        public Vector3 Normal 
            => NormalEncoder.DecodeNormal(N);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public VertexStruct(Vector3 pos, Vector3 normal)
            : this(pos, NormalEncoder.EncodeNormal(normal))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public VertexStruct(Vector3 pos, uint normal)
        {
            Position = pos;
            N = normal;
        }

        static VertexStruct()
        {
            Debug.Assert(Size == 16);
        }

        public override bool Equals(object? obj)
            => obj is VertexStruct other && Equals(other);

        public bool Equals(VertexStruct other)
            => Position == other.Position && N == other.N;
    }
}