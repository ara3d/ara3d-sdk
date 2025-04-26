using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace Ara3D.Serialization.BFAST
{
    public static class MemoryMappedFileExtensions
    {
        public static MemoryMappedView CreateView(this MemoryMappedFile self, long offset, long size)
            => new MemoryMappedView(self, offset, size);

        public static MemoryMappedView CreateSubView(this MemoryMappedView self, long offset, long size)
            => new MemoryMappedView(self.File, self.Offset + offset, size);

        public static unsafe T[] ReadArray<T>(this MemoryMappedView self)
            where T: unmanaged
        {
            var elementSize = Marshal.SizeOf(typeof(T));
            var cnt = self.Size / elementSize;
            if (self.Size % elementSize != 0)
                throw new Exception($"Size of view {self.Size} does not divide evenly by size of elements {elementSize}");

            var r = new T[cnt];

            // https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/unmanagedmemoryaccessor.cs#L624
            // https://github.com/microsoft/referencesource/blob/master/mscorlib/system/runtime/interopservices/safebuffer.cs#L233
            // NOTE: assumes alignment!
            var buffer = self.Accessor.SafeMemoryMappedViewHandle;
            
            fixed (T* pResult = r)
            {
                byte* pBuffer = null;
                try
                {
                    buffer.AcquirePointer(ref pBuffer);
                    pBuffer += self.Accessor.PointerOffset;
                    Buffer.MemoryCopy(pBuffer, pResult, self.Size, self.Size);
                }
                finally
                {
                    buffer.ReleasePointer();
                }
            }

            // The following is not "unsafe" but is a bit slower. 
            var tmp = self.Accessor.ReadArray(0, r, 0, (int)cnt);
            if (tmp != cnt)
                throw new Exception($"Failed to read {cnt} items, instead only read {tmp}");
            

            return r;
        }

        public static byte[] ReadBytes(this MemoryMappedView self)
            => self.ReadArray<byte>();

        public static string ReadString(this MemoryMappedView self)
            => Encoding.UTF8.GetString(self.ReadBytes());
    }
}