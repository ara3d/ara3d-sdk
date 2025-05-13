using Ara3D.BFAST;
using Ara3D.Logging;
using Ara3D.Serialization.VIM;
using Ara3D.Utils;

namespace Ara3D.SDK.Tests
{
    public static class FileTests
    {
        public static DirectoryPath DataFolder => PathUtil.GetCallerSourceFolder().RelativeFolder("..", "..", "data");

        public static FilePath Residence => DataFolder.RelativeFile("residence.vim");

        [Test]
        public static void DataFiles()
        {
            foreach (var file in DataFolder.GetFiles())
            {
                Console.WriteLine($"File {file} exists");
            }
        }
        
        public static void OutputVimData(SerializableDocument vim)
        {
            foreach (var table in vim.EntityTables)
            {
                Console.WriteLine($"{table.Name} #{table.ColumnNames.Count()} columns");
            }

            var g = vim.Geometry;
            Console.WriteLine($"# meshes = {g.Meshes.Count}");
            Console.WriteLine($"# indices = {g.Indices.Count}");
            Console.WriteLine($"# vertices = {g.Vertices.Count}");
            Console.WriteLine($"# submeshes = {g.SubmeshIndexCount.Count}");
            Console.WriteLine($"# instances = {g.InstanceMeshes.Count}");
        }

        [Test]
        public static void OpenVIM()
        {
            var f = Residence;
            var logger = Logger.Console;
            logger.Log($"Opening {f}");
            var vim = VimSerializer.Deserialize(Residence);
            logger.Log("Loaded VIM");
            OutputVimData(vim);
            logger.Log("Completed test");
        }

        [Test]
        public static void OpenBFast()
        {
            var f = Residence;
            var logger = Logger.Console;
            logger.Log($"Opening {f}");
            var buffers = BFastReader.ReadBuffers(f);
            logger.Log("Loaded BFAST");
            foreach (var (buffer, name) in buffers)
            {
                logger.Log($"Buffer {name} has {buffer.NumBytes} bytes");
            }
        }
    }
}