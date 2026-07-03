
using Ara3D.Memory;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Ara3D.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack= 4)]
    [DataContract]
    public readonly partial struct ColorRGBA
        : IEquatable<ColorRGBA>
    {
        [DataMember]
        public readonly byte R;
        [DataMember]
        public readonly byte G;
        [DataMember]
        public readonly byte B;
        [DataMember]
        public readonly byte A;
        public ColorRGBA((byte r, byte g, byte b, byte a) tuple) : this(tuple.r, tuple.g, tuple.b, tuple.a) { }
        public ColorRGBA(byte r, byte g, byte b, byte a) { R = r; G = g; B = b; A = a; }
        public static ColorRGBA Create(byte r, byte g, byte b, byte a) => new ColorRGBA(r, g, b, a);
        public static ColorRGBA Create((byte r, byte g, byte b, byte a) tuple) => new ColorRGBA(tuple);
        public override bool Equals(object obj) => obj is ColorRGBA x && Equals(x);
        public override int GetHashCode() => HashCode.Combine(R, G, B, A);
        public override string ToString() => $"ColorRGBA(R = {R}, G = {G}, B = {B}, A = {A})";
        public void Deconstruct(out byte r, out byte g, out byte b, out byte a) { r = R; g = G; b = B; a = A; }
        public bool Equals(ColorRGBA x) => R == x.R && G == x.G && B == x.B && A == x.A;
        public static bool operator ==(ColorRGBA x0, ColorRGBA x1) => x0.Equals(x1);
        public static bool operator !=(ColorRGBA x0, ColorRGBA x1) => !x0.Equals(x1);
        public static implicit operator ColorRGBA((byte r, byte g, byte b, byte a) tuple) => new ColorRGBA(tuple);
        public static implicit operator (byte r, byte g, byte b, byte a)(ColorRGBA self) => (self.R, self.G, self.B, self.A);

        public static ColorRGBA Zero = new ColorRGBA(default, default, default, default);
        public static ColorRGBA MinValue = new ColorRGBA(byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue);
        public static ColorRGBA MaxValue = new ColorRGBA(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        public ColorRGBA SetR(byte x) => new ColorRGBA(x, G, B, A);
        public ColorRGBA SetG(byte x) => new ColorRGBA(R, x, B, A);
        public ColorRGBA SetB(byte x) => new ColorRGBA(R, G, x, A);
        public ColorRGBA SetA(byte x) => new ColorRGBA(R, G, B, x);
    }

    public class Bitmap : IBitmap
    {
        public int Height { get; }
        public int Width { get; }
        
        public FixedArray<ColorRGBA> PixelBuffer { get; }
        
        public Bitmap(int width, int height)
        {
            Height = height;
            Width = width;
            PixelBuffer = new(new ColorRGBA[Width * Height]);
        }
        
        public int GetNumPixels()
            => Width * Height;

        public void SetPixel(int x, int y, ColorRGBA color)
            => SetPixel(x + y * Width, color);

        public void SetPixel(int i, ColorRGBA color)
            => PixelBuffer[i] = color;

        public ColorRGBA Eval(int x, int y)
            => GetPixel(x + y * Width);

        public ColorRGBA GetPixel(int i)
            => PixelBuffer[i];
    }
}