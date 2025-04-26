using System.IO;
using System.IO.Compression;

namespace Ara3D.Utils
{
    public static class ZipUtil
    {
        public static byte[] UnGzipIfNeeded(this byte[] bytes)
        {
            //https://superuser.com/questions/115902/tell-if-a-gz-file-is-really-gzipped
            if (bytes.IsGzipped())
            {
                using (var mem = new MemoryStream(bytes))
                using (var zip = new GZipStream(mem, CompressionMode.Decompress))
                {
                    return zip.ReadAllBytes();
                }
            }

            return bytes;
        }

        public static bool IsGzipped(this byte[] bytes)
        {
            const byte GzipMagicByte0 = 0x1F;
            const byte GzipMagicByte1 = 0x8B;
            return bytes != null && bytes.Length > 2 && bytes[0] == GzipMagicByte0 && bytes[1] == GzipMagicByte1;
        }

        /// <summary>
        /// Zips a file and places the result into a newly created file in the temporary directory
        /// </summary>
        public static FilePath Zip(this FilePath filePath)
            => Zip(filePath, Path.GetTempFileName());

        /// <summary>
        /// Zips a file and places the result into a newly created file in the temporary directory
        /// </summary>
        public static FilePath Zip(this FilePath filePath, FilePath outputFile)
        {
            using (var za = new ZipArchive(File.OpenWrite(outputFile), ZipArchiveMode.Create))
            {
                var zae = za.CreateEntry(Path.GetFileName(filePath) ?? "");
                using (var outputStream = zae.Open())
                using (var inputStream = File.OpenRead(filePath))
                    inputStream.CopyTo(outputStream);
            }

            return outputFile;
        }

        /// <summary>
        /// Unzips the first entry in an archive to a designated file, returning that file path.
        /// </summary>
        public static FilePath Unzip(this FilePath zipFilePath, FilePath outputFile)
        {
            using (var za = new ZipArchive(File.OpenRead(zipFilePath), ZipArchiveMode.Read))
            {
                var zae = za.Entries[0];
                using (var inputStream = zae.Open())
                using (var outputStream = File.OpenWrite(outputFile))
                    inputStream.CopyTo(outputStream);
            }

            return outputFile;
        }

        /// <summary>
        /// Unzips the first entry in an archive into a temp generated file, returning that file path
        /// </summary>
        public static FilePath Unzip(this FilePath zipFilePath)
            => Unzip(zipFilePath, Path.GetTempFileName());

        public static StreamWriter CreateAndOpenEntry(this ZipArchive archive, string entryName)
        {
            var entry = archive.CreateEntry(entryName);
            return new StreamWriter(entry.Open());
        }

        // TODO: there could be a bug in this code, when I used it I seemed to have some problems with sporadic Zip creation
        public static void CreateEntryFromText(this ZipArchive archive, string entryName, string content)
        {
            using (var sw = archive.CreateAndOpenEntry(entryName))
            {
                sw.Write(content);
                sw.Flush();
                sw.Close();
            }
        }
    }
}