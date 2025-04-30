using System.IO.MemoryMappedFiles;

namespace Ara3D.MemoryMappedFiles
{
    /// <summary>
    /// This address a limitation of memory mapped view accessor,
    /// in that you can't create sub-views from it. You need access 
    /// to the original memory mapped file. 
    /// </summary>
    public class MemoryMappedView : IDisposable
    {
        public MemoryMappedFile File { get; }
        public long Offset { get; }
        public long Size { get; }
        public MemoryMappedViewAccessor Accessor { get; }

        public MemoryMappedView(MemoryMappedFile file, long offset, long size)
        {
            File = file;
            Offset = offset;
            Size = size;
            Accessor = file.CreateViewAccessor(offset, size);
        }

        public void Dispose()
            => Accessor.Dispose();

        public static void ReadFile(string filePath, Action<MemoryMappedView> action)
        {
            var fi = new FileInfo(filePath);
            using (var mmf = MemoryMappedFile.CreateFromFile(filePath))
                using (var view = new MemoryMappedView(mmf, 0, fi.Length))
                    action(view);
        }
    }
}