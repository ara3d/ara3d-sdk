using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ara3D.Graphics
{
    // https://en.wikipedia.org/wiki/BMP_file_format
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BitmapHeader
    {
        public ushort Magic; 
        public uint Size;
        public ushort Unused1;
        public ushort Unused2;
        public uint DataStartOffset;  
        
        public static uint StructSize = 14;

        public unsafe BitmapHeader(uint dataSize)
        {
            Magic = 0x4D42;
            DataStartOffset = StructSize + DibHeader.StructSize;
            Size = dataSize + DataStartOffset;
            Unused1 = default;
            Unused2 = default;
            Debug.Assert(sizeof(BitmapHeader) == StructSize);
        }
    }
}