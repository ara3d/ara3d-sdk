using Ara3D.DataSetBrowser.WPF;
using Ara3D.Utils;
using BIMOpenSchema;

namespace Ara3D.BIMOpenSchema.Tests
{
    public static class Tests
    {
        public static DirectoryPath InputFolder = PathUtil.GetCallerSourceFolder().RelativeFolder("..", "input");
        public static DirectoryPath OutputFolder = PathUtil.GetCallerSourceFolder().RelativeFolder("..", "output");

        [Test]
        public static void TestDuckDB()
        {
            var outputFile = OutputFolder.RelativeFile("bimdata.duckdb");
            var data = GetData();
            var stats = Serialization.Write(data,
                (fp, bd) => Serialization.WriteDuckDB(bd, fp), outputFile);
            OutputStats(data, stats);
        }

        [Test]
        public static void TestJsonWithZip()
        {
            var outputFile = OutputFolder.RelativeFile("bimdata.json.zip");
            var data = GetData();
            var stats = Serialization.Write(data, 
                (fp, bd) => Serialization.WriteBIMDataToJson(bd, fp, true, true), outputFile);
            OutputStats(data, stats);
        }

        [Test]
        public static void TestJson()
        {
            var outputFile = OutputFolder.RelativeFile("bimdata.json");
            var data = GetData();
            var stats = Serialization.Write(data, 
                (fp, bd) => Serialization.WriteBIMDataToJson(bd, fp, true, false), outputFile);
            OutputStats(data, stats);
        }

        [Test]
        public static void TestMessagePack()
        {
            var outputFile = OutputFolder.RelativeFile("bimdata.mp");
            var data = GetData();
            var stats = Serialization.Write(data, 
                (fp, bd) => Serialization.WriteBIMDataToMessagePack(bd, fp, false, false), outputFile);
            OutputStats(data, stats);
        }

        [Test]
        public static void TestMessagePackWithZip()
        {
            var outputFile = OutputFolder.RelativeFile("bimdata.mp.zip");
            var data = GetData();
            var stats = Serialization.Write(data,
                (fp, bd) => Serialization.WriteBIMDataToMessagePack(bd, fp, false, true), outputFile);
            OutputStats(data, stats);
        }

        [Test]
        public static void TestMessagePackWithCompression()
        {
            var outputFile = OutputFolder.RelativeFile("bimdata.mpz");
            var data = GetData();
            var stats = Serialization.Write(data,
                (fp, bd) => Serialization.WriteBIMDataToMessagePack(bd, fp, true, false), outputFile);
            OutputStats(data, stats);
        }

        [Test]
        public static void TestMessagePackWithCompressionAndZip()
        {
            var outputFile = OutputFolder.RelativeFile("bimdata.mpz.zip");
            var data = GetData();
            var stats = Serialization.Write(data,
                (fp, bd) => Serialization.WriteBIMDataToMessagePack(bd, fp, true, true), outputFile);
            OutputStats(data, stats);
        }

        public static void OutputStats(BIMData bd, SerializationStats stats)
        {
            Console.WriteLine($"# documents = {bd.Documents.Count}");
            Console.WriteLine($"# entities = {bd.Entities.Count}");
            Console.WriteLine($"# descriptors = {bd.Descriptors.Count}");
            Console.WriteLine($"Wrote {PathUtil.BytesToString(stats.Size)}");
            Console.WriteLine($"Took {stats.Elapsed.Seconds:F} seconds");
            Console.WriteLine($"File name is {stats.Path}");
        }

        public static BIMData GetData()
        {
            var inputFile = InputFolder.RelativeFile("bimdata.mpz");
            return Serialization.ReadBimDataFromMessagePack(inputFile);
        }
    }
}