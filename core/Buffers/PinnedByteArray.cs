using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ara3D.Buffers
{
    /// <summary>
    /// Allocates fixed and aligned memory appropriate
    /// for usage with data structures up to "Width" bytes wide.
    /// Allocates extra space so we can iterate over the data using structures up to "Width" size.
    /// Also allocates extra space so that the data is safely aligned on a "Width" boundary.  
    /// </summary>
    public unsafe class PinnedByteArray : IDisposable
    {
        public readonly byte[] Bytes;
        public GCHandle Handle;
        public readonly byte* BytePtr;
        public readonly int Offset;
        public readonly int NumBytes;
        public readonly int NumVectors;
        public const int Width = 32;

        public PinnedByteArray(int numBytes)
        {
            NumBytes = numBytes;
            NumVectors = (numBytes / Width) + (numBytes % Width == 0 ? 0 : 1);

            // This is the amount of space we need 
            // We add 1 to number of vectors to account for space need to pad for alignment
            var space = (NumVectors + 1) * Width;
            Debug.Assert(space >= NumBytes + Width);

            // Allocate the array
            Bytes = new byte[space];

            // Create a pinned pointer
            Handle = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
            
            // Get the pointer
            BytePtr = (byte*)Handle.AddrOfPinnedObject();

            // Check if we are aligned
            var asLong = (long)BytePtr;
            var modulo = asLong % Width;
            // If not force alignment 
            if (modulo != 0)
                Offset = (int)(Width - modulo);
            Debug.Assert(Offset >= 0 && Offset < Width);
            Debug.Assert(Offset + NumBytes <= space);

            // Cast back to a pointer 
            Debug.Assert((asLong + Offset) % Width == 0);
            BytePtr = (byte*)(asLong + Offset);
        }

        public void Dispose()
            => Handle.Free();

        public ByteSpan ToByteSpan()
            => new ByteSpan(BytePtr, NumBytes);
    }
}