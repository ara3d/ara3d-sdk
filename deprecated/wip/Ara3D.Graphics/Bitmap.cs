
using Ara3D.Mathematics;
using Ara3D.Memory;

namespace Ara3D.Graphics
{
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