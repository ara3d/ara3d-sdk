using System;
using System.Runtime.InteropServices;

namespace Ara3D.Buffers
{
    /// <summary>
    /// Allocates fixed and aligned memory appropriate
    /// for usage with data structures up to "Width" bytes wide.  
    /// </summary>
    public unsafe class AlignedMemory : IDisposable
    {
        public readonly byte* BytePtr;
        public readonly IntPtr AllocPtr;
        public readonly int NumBytes;
        public readonly int NumVectors;
        public const int Width = 32;
        public byte* End => BytePtr + NumBytes;

        public AlignedMemory(int numBytes)
        {
            NumBytes = numBytes;
            NumVectors = (numBytes / Width) + (numBytes % Width == 0 ? 0 : 1);

            // This is the amount of space we need for the vectors
            // We add 1 to number of vectors to account for alignment 
            var space = (NumVectors + 1) * Width;
            AllocPtr = Marshal.AllocHGlobal(space + Width);

            BytePtr = (byte*)(((long)AllocPtr + Width - 1) & ~(Width - 1));
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(AllocPtr);
        }
    }
}