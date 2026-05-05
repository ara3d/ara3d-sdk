using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Ara3D.Geometry;
using Ara3D.Utils;

namespace Ara3D.Models;

/// <summary>
/// An instance references a mesh by index, and contains the global transform
/// matrix (a 3 x 4 matrix) and material data (color, metallic, roughness) 
/// It fits into 64 bytes, for efficient copies and manipulation.
/// It is passed as-is to the renderer.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct InstanceStruct
{
    // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
    // Constant flags
    public const byte HiddenFlag = 0x1;

    // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
    // Static properties 
    public static readonly uint Size = (uint)sizeof(InstanceStruct);

    // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
    // Static initializer - for debugging 
    static InstanceStruct()
        => Debug.Assert(Size == 64);

    // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
    // Fields 

    public Vector4 Column0;
    public Vector4 Column1;
    public Vector4 Column2;

    // Index of the mesh 
    public int MeshIndex; //  4 bytes
    
    // Used for looking up which entity this instance is associated with.
    // Multiple instances may refer to the same entity: as they represent 
    // different parts of a whole. 
    public int EntityIndex;  //  4 bytes

    public uint PackedColor; // 4 bytes: R, G, B, A

    public byte Unused; 
    public byte Flags;

    public byte RoughnessByte;
    public byte MetallicByte;

    // –––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
    // Constructors

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InstanceStruct
    (
        int entityIndex,
        Matrix4x4 transform,
        int meshIndex,
        Color color,
        float metallic,
        float roughness,
        byte flags = 0
    )
    {
        EntityIndex = entityIndex;
        Matrix4x4 = transform;
        MeshIndex = meshIndex;
        Color = color;
        Metallic = metallic;
        Roughness = roughness;
        Flags = flags;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InstanceStruct
    (
        int entityIndex,
        Matrix4x4 transform,
        int meshIndex,
        Material mat,
        byte flags = 0
    )
        : this
        (
            entityIndex,
            transform, 
            meshIndex, 
            mat.Color, 
            mat.Metallic, 
            mat.Roughness,
            flags
        )
    { }

    //==
    // Properties 

    public float Metallic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MetallicByte.ToNormalizedFloat();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => RoughnessByte = value.ToByteFromNormalized();
    }

    public float Roughness
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MetallicByte.ToNormalizedFloat();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => RoughnessByte = value.ToByteFromNormalized();
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Color = Color.WithA(value);
    }

    public bool Transparent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Alpha < 0.99f;
    }

    public Matrix4x4 Matrix4x4
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(
            Column0.X, Column1.X, Column2.X, 0f,
            Column0.Y, Column1.Y, Column2.Y, 0f,
            Column0.Z, Column1.Z, Column2.Z, 0f,
            Column0.W, Column1.W, Column2.W, 1f);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set 
        {
            Column0 = (value.M11, value.M21, value.M31, value.M41);
            Column1 = (value.M12, value.M22, value.M32, value.M42);
            Column2 = (value.M13, value.M23, value.M33, value.M43);
        }
    }

    public Vector3 Translation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Column0.W, Column1.W, Column2.W);
    }

    public Vector3 Scale
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Matrix4x4.Decompose().X2;
    }

    public Quaternion Rotation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Matrix4x4.Decompose().X1;
    }

    public Material Material
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Color, Metallic, Roughness);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            Color = value.Color;
            Metallic = value.Metallic;
            Roughness = value.Roughness;
        }
    }

    //------------
    // With functions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InstanceStruct WithMatrix(Matrix4x4 matrix)
    {
        var r = this;
        r.Matrix4x4 = matrix;
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InstanceStruct WithColor(Color color)
    {
        var r = this;
        r.Color = color;
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InstanceStruct WithAlpha(float alpha) 
    {
        var r = this;
        r.Alpha = alpha;
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InstanceStruct WithRoughness(float roughness)
    {
        var r = this;
        r.Roughness = roughness;
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InstanceStruct WithMetallic(float metallic)
    {
        var r = this;
        r.Metallic = metallic;
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InstanceStruct WithMaterial(Material material)
    {
        var r = this;
        r.Material = material;
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InstanceStruct WithMeshIndex(int meshIndex)
    {
        var r = this;
        r.MeshIndex = meshIndex;
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InstanceStruct WithFlags(byte flags)
    {
        var r = this;
        r.Flags = flags;
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InstanceStruct WithVisibility(bool vis)
        => WithFlags(vis ? (byte)(Flags & ~HiddenFlag) : (byte)(Flags | HiddenFlag));

    //------------
    // Helper function

    public InstanceStruct Transform(Matrix4x4 matrix)
        => WithMatrix(matrix * Matrix4x4);

    //---------------
    // 

    public bool Equals(InstanceStruct other) =>
        other.EntityIndex == EntityIndex &&
        other.MeshIndex == MeshIndex &&
        other.Material.Equals(Material) &&
        other.Matrix4x4.Equals(Matrix4x4) && 
        other.Flags.Equals(Flags);

    public override int GetHashCode()
        => HashCode.Combine(EntityIndex, Material, MeshIndex, Matrix4x4, Flags);
   
    public override bool Equals(object? obj)
        => obj is InstanceStruct other && Equals(other);

    public override string ToString()
        => $"Mesh={MeshIndex},Entity={EntityIndex},Material={Material}";

    public bool IsVisible
        => (Flags & HiddenFlag) == 0 && MeshIndex >= 0;
}