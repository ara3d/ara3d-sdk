using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ara3D.Graphics
{
    // https://en.wikipedia.org/wiki/BMP_file_format
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DibHeader
    {
        public uint biSize;
        public uint biWidth;
        public uint biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public uint biXPelsPerMeter;
        public uint biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;

        public const uint StructSize = 40;

        // Uncompressed 32 bit. 
        public DibHeader(int width, int height, uint dpi = 600)
        {
            const double inchPerMeter = 39.3701;
            uint pixPerMeter = (uint)(dpi * inchPerMeter);
            biSize = StructSize;
            biWidth = (uint)width;
            biHeight = (uint)height;
            biPlanes = 1;
            biBitCount = 32;
            biCompression = 0;
            biSizeImage = (uint)width * (uint)height * 4;
            biXPelsPerMeter = pixPerMeter;
            biYPelsPerMeter = pixPerMeter;
            biClrUsed = 0;
            biClrImportant = 0;
            Debug.Assert(sizeof(DibHeader) == StructSize);
        }
    }
}