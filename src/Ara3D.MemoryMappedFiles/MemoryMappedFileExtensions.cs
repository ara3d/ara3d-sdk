using System.IO.MemoryMappedFiles;
using Ara3D.Memory;

namespace Ara3D.MemoryMappedFiles
{
    public static class MemoryMappedFileExtensions
    {
        public static MemoryMappedView CreateView(this MemoryMappedFile self, long offset, long size)
            => new MemoryMappedView(self, offset, size);

        public static MemoryMappedView CreateSubView(this MemoryMappedView self, long offset, long size)
            => new MemoryMappedView(self.File, self.Offset + offset, size);

        public static unsafe AlignedMemory ReadBytes(this MemoryMappedView self)
        {
            var r = new AlignedMemory(self.Size);
            var buffer = self.Accessor.SafeMemoryMappedViewHandle;
            byte* pBuffer = null;
            try
            {
                buffer.AcquirePointer(ref pBuffer);
                pBuffer += self.Accessor.PointerOffset;
                Buffer.MemoryCopy(pBuffer, r.GetPointer(), self.Size, self.Size);
            }
            finally
            {
                buffer.ReleasePointer();
            }
            return r;
        }
    }
}