using System;
using System.Collections.Generic;
using Ara3D.Collections;
using Ara3D.Geometry;

namespace Ara3D.Bowerbird.RevitSamples;

public struct Color32
{
    public readonly byte R;
    public readonly byte G;
    public readonly byte B;
    public readonly byte A;
    public Color32(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
}

public static class RenderExtensions
{
    public static double Clamp(this double value, double min, double max)
        => Math.Max(Math.Min(value, max), min);

    public static double Clamp(this double value)
        => value.Clamp(0, 1);

    public static double Round(this double value)
        => Math.Round(value);

    public static byte ScaleToByte(this double value)
    {
        // Scale from [0.0, 1.0] to [0, 255], round to nearest integer, and clamp to [0, 255]
        return (byte)(value * 255).Round().Clamp(0, 255);
    }

    public static uint SingleToUInt32Bits(float f)
    {
        var bytes = BitConverter.GetBytes(f); // Convert float to 4 bytes
        return (uint)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24));
    }

    public static ushort ToHalf(this float f)
    {
        var floatBits = SingleToUInt32Bits(f);

        var sign = (floatBits >> 31) & 0x1;        // Extract the sign (1 bit)
        var exponent = (floatBits >> 23) & 0xFF;   // Extract the exponent (8 bits)
        var mantissa = floatBits & 0x7FFFFF;       // Extract the mantissa (23 bits)

        // Initialize the 16-bit result
        ushort half = 0;

        if (exponent == 255) // Special cases: NaN or Infinity
        {
            // NaN or Infinity
            half = (ushort)((sign << 15) | (0x1F << 10)); // Set exponent to max for half-float (all 1s)
            if (mantissa != 0) half |= (ushort)(mantissa >> 13); // Preserve mantissa for NaN
        }
        else if (exponent > 112) // Normal number
        {
            // Exponent bias adjustment: from 127 for float to 15 for half-float
            exponent = exponent - 127 + 15;
            if (exponent >= 0x1F)
            {
                // Overflow, return infinity
                half = (ushort)((sign << 15) | (0x1F << 10));
            }
            else
            {
                // Normalized number in half precision
                half = (ushort)((sign << 15) | (exponent << 10) | (mantissa >> 13));
            }
        }
        else if (exponent >= 103) // Subnormal number
        {
            // Subnormal numbers are represented with a leading 0 in the exponent and a scaled mantissa
            mantissa = (mantissa | 0x800000) >> (int)(113 - exponent); // Recover the hidden leading bit and shift
            half = (ushort)((sign << 15) | (mantissa >> 13)); // No exponent, just the mantissa
        }
        else
        {
            // Too small to be represented, return 0
            half = (ushort)(sign << 15);
        }

        return half;
    }

    public static RenderMesh ToRenderMesh(this TriangleMesh3D mesh, Color32 color)
        => mesh.ToRenderMesh(color.Repeat(mesh.Points.Count));

    public static RenderMesh ToRenderMesh(this TriangleMesh3D mesh, IReadOnlyList<Color32> colors = null)
        => RenderMesh.Create(mesh.Points, mesh.CornerIndices().Map(i => i.Value), null, null, colors);
}