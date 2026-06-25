using Ara3D.Utils;
using System.Diagnostics;
using System.IO.Compression;
using Ara3D.BimOpenSchema;
using Ara3D.BimOpenSchema.IO;
using Ara3D.BimOpenSchema.Tests;
using Ara3D.DataTable;

namespace Ara3D.BIMOpenSchema.Tests
{
    public static class Tests
    {
        public class SerializationStats
        {
            public TimeSpan Elapsed;
            public string Path;
            public long Size;
        }

        public static T Read<T>(Func<FilePath, T> f, FilePath fp, string description, out SerializationStats stats)
        {
            var sw = Stopwatch.StartNew();
            var r = f(fp);
            stats = new SerializationStats()
            {
                Path = fp,
                Elapsed = sw.Elapsed,
                Size = fp.GetFileSize(),
            };
            return r;
        }

        public static SerializationStats Write<T>(T value, Action<FilePath, T> f, FilePath fp)
        {
            var sw = Stopwatch.StartNew();
            f(fp, value);
            return new SerializationStats()
            {
                Path = fp,
                Elapsed = sw.Elapsed,
                Size = fp.GetFileSize(),
            };
        }

        public static DirectoryPath DataFolder => PathUtil.GetCallerSourceFolder().RelativeFolder("..", "..", "data");
        public static DirectoryPath OutputFolder => DataFolder.RelativeFolder("output");

        public static FilePath InputFile => DataFolder.RelativeFile("rac_basic_sample_project-2025.bos");

        [Test]
        public static void TestInputFileExists()
        {
            Assert.IsTrue(InputFile.Exists());
            OutputFileDetails(InputFile);
        }

        public static IBimData GetTestInputData()
            => InputFile.ReadBimDataFromParquetZip();

        public static void OutputFileDetails(FilePath fp)
        {
            Console.WriteLine($"File: {fp}");
            Console.WriteLine($"Has size: {fp.GetFileSizeAsString()}");
        }

        [Test, Category("Slow")]
        public static void TestReadInputFile()
        {
            var bd = GetTestInputData();
            OutputBimData(bd);
        }

        public static void TestWriteData(IBimData data, string ext, Action<IBimData, FilePath> writer)
        {
            var outputFile = InputFile.ChangeDirectoryAndExt(OutputFolder, ext);
            var sw = Stopwatch.StartNew();
            writer.Invoke(data, outputFile);
            var sz = outputFile.GetFileSizeAsString();
            Console.WriteLine($"Wrote {sz} to {outputFile.GetFileName()} in {sw.Elapsed.Seconds:F} seconds");
        }

        [Test, Category("Slow")]
        public static void TestWriter()
        {
            var sw = Stopwatch.StartNew();
            var bimData = GetTestInputData();
            Console.WriteLine($"Loaded {InputFile.GetFileSizeAsString()} of BIM data in {sw.Elapsed.Seconds:F} seconds");

            TestWriteData(bimData, "xlsx", (bd, f) => bd.WriteToExcel(f));
            TestWriteData(bimData, "parquet.zip", (bd, f) => bd.WriteToParquetZip(f));
        }

        public static void OutputBimData(IBimData bd)
        {
            Console.WriteLine($"# documents = {bd.Documents.Length}");
            Console.WriteLine($"# entities = {bd.Entities.Length}");
            Console.WriteLine($"# descriptors = {bd.Descriptors.Length}");
            Console.WriteLine($"# points = {bd.Points.Length}");
            Console.WriteLine($"# string = {bd.Strings.Length}");
            Console.WriteLine($"# numbers = {bd.Numbers.Length}");
            Console.WriteLine($"# parameters = {bd.Parameters.Length}");
            Console.WriteLine($"# relations = {bd.Relations.Length}");
        }

        public static void TestParameterStatistics()
        {
            var bimData = GetTestInputData();
            var d = bimData.GetStatistics();
            var stats = d.Values.OrderBy(ps => ps.Index).ToList();
            var dt = stats.ToDataTable("parameters");
            var outputFile = OutputFolder.RelativeFile("parameters.xlsx");
            dt.WriteToExcel(outputFile);
        }

        [Test, Category("Slow")]
        public static void BimDataObjectModel()
        {
            var bimData = GetTestInputData();
            var model = new BimObjectModel(bimData, true);
            Console.WriteLine($"# documents = {model.Documents.Count}");
            Console.WriteLine($"# entities = {model.Entities.Count}");
            Console.WriteLine($"# descriptors = {model.Descriptors.Count}");
            var cats = model.Entities
                .Select(e => e.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
            foreach (var cat in cats)
                Console.WriteLine($"Category: {cat}");
        }

        public static void OutputDataSet(IDataSet set)
        {
            Console.WriteLine($"# tables = {set.Tables.Count}");
            foreach (var t in set.Tables)
            {
                Console.WriteLine($"Table {t.Name} # columns = {t.Columns.Count}, # rows = {t.Rows.Count}");
                for (var i = 0; i < t.Columns.Count; i++)
                {
                    var cd = t.Columns[i].Descriptor;
                    Console.WriteLine($" Column {i} = {cd.Name} {cd.Type}");
                }
            }
        }
    }
}