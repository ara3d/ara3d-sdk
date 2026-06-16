using Ara3D.Geometry;
using System.Runtime.CompilerServices;
using Ara3D.Utils;

namespace Ara3D.Models;

public record struct PackedMaterial(byte R, byte G, byte B, byte A, byte Metallic, byte Roughness)
{
    public override string ToString()
    {
        return $"R:{R:x} G:{G:x} B:{B:x} Me:{Metallic} Rg:{Roughness}";
    }
}

public record struct Material(Color Color, float Metallic, float Roughness)
{
    public static Color DefaultColor = new(0.85f, 0.85f, 0.95f, 1f);
    public static float DefaultMetallic = 0f;
    public static float DefaultRoughness = 0.7f;
    public static Material Default = new(DefaultColor, DefaultMetallic, DefaultRoughness);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Material WithColor(Color color) 
        => this with { Color = color };
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Material WithMetallic(float metallic) 
        => this with { Metallic = metallic };
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Material WithRoughness(float roughness) 
        => this with { Roughness = roughness };
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Material WithAlpha(float alpha) 
        => WithColor(Color.WithA(alpha));
    
    public float Red
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Color.R; 
    }

    public float Green
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Color.G;
    }

    public float Blue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Color.B;
    }

    public float Alpha
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Color.A;
    }

    public PackedMaterial AsPacked()
        => new(
            Color.R.Value.ToByteFromNormalized(),
            Color.G.Value.ToByteFromNormalized(),
            Color.B.Value.ToByteFromNormalized(),
            Color.A.Value.ToByteFromNormalized(),
            Metallic.ToByteFromNormalized(),
            Roughness.ToByteFromNormalized());

    public bool Transparent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Color.A <= 0.999f;
    }

    public bool Visible
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Color.A >= 0.001f;
    }

    public static implicit operator Material(Color c)
        => Default.WithColor(c);
}