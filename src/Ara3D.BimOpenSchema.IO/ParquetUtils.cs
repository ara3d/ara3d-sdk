using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ara3D.DataTable;
using Ara3D.Extras;
using Ara3D.Logging;
using Ara3D.Utils;
using Parquet;
using Parquet.Schema;
using DataColumn = Parquet.Data.DataColumn;

namespace Ara3D.BimOpenSchema.IO;

public static class ParquetUtils
{
    public static async Task WriteParquetAsync(
        this IDataTable table,
        FilePath filePath,
        CompressionLevel level = CompressionLevel.Optimal,
        CompressionMethod method = CompressionMethod.Brotli)
    {
        await using var fs = File.Create(filePath);
        await table.WriteParquetAsync(fs, level, method);
    }

    public static async Task WriteParquetAsync(
        this IDataTable table,
        Stream stream,
        CompressionLevel level = CompressionLevel.Optimal,
        CompressionMethod method = CompressionMethod.Brotli)
    {
        var dataFields = table.Columns.Select(c => new DataField(c.Descriptor.Name, c.Descriptor.Type)).ToList();
        var schema = new ParquetSchema(dataFields);

        await using var writer = await ParquetWriter.CreateAsync(schema, stream);
        writer.CompressionLevel = level;
        writer.CompressionMethod = method;
        using var rg = writer.CreateRowGroup();

        foreach (var c in table.Columns)
        {
            var df = dataFields[c.ColumnIndex];
            var array = Array.CreateInstance(c.Descriptor.Type, c.Count);
            for (var i = 0; i < c.Count; i++)
                array.SetValue(c[i], i);
            var dc = new DataColumn(df, array);
            await rg.WriteColumnAsync(dc);
        }
    }

    public static void WriteParquetToZip(
        this IDataSet set,
        FilePath zipPath,
        CompressionMethod parquetCompressionMethod = CompressionMethod.Brotli,
        CompressionLevel parquetCompressionLevel = CompressionLevel.Optimal,
        CompressionLevel zipCompressionLevel = CompressionLevel.NoCompression)
        => Task.Run(() => set.WriteParquetToZipAsync(zipPath, parquetCompressionMethod, parquetCompressionLevel, zipCompressionLevel))
            .GetAwaiter().GetResult();

    public static async Task WriteParquetToZipAsync(this IDataSet set, FilePath zipPath,
            CompressionMethod parquetCompressionMethod = CompressionMethod.Brotli,
            CompressionLevel parquetCompressionLevel = CompressionLevel.Optimal,
            CompressionLevel zipCompressionLevel = CompressionLevel.NoCompression)
    {
        await using var fs = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None);
        using var zip = new ZipArchive(fs, ZipArchiveMode.Create, leaveOpen: false);
        await WriteParquetToZipAsync(set, zip, parquetCompressionMethod, parquetCompressionLevel, zipCompressionLevel);
    }

    public static void WriteParquetToZip(
        this IDataSet set,
        ZipArchive zip,
        CompressionMethod parquetCompressionMethod,
        CompressionLevel parquetCompressionLevel,
        CompressionLevel zipCompressionLevel)
        => Task.Run(() =>
                set.WriteParquetToZipAsync(zip, parquetCompressionMethod, parquetCompressionLevel, zipCompressionLevel))
            .GetAwaiter().GetResult();

    public static async Task WriteParquetToZipAsync(
        this IDataSet set,
        ZipArchive zip,
        CompressionMethod parquetCompressionMethod,
        CompressionLevel parquetCompressionLevel,
        CompressionLevel zipCompressionLevel)
    {
        foreach (var table in set.Tables)
        {
            var entryName = $"{table.Name}.parquet";
            var entry = zip.CreateEntry(entryName, zipCompressionLevel);
            await using var parquetBuffer = new MemoryStream();
            await table.WriteParquetAsync(parquetBuffer, parquetCompressionLevel, parquetCompressionMethod);
            parquetBuffer.Position = 0;
            await using var entryStream = entry.Open();
            await parquetBuffer.CopyToAsync(entryStream);
        }
    }

    public static async Task<IDataTable> ReadParquetAsync(this FilePath filePath, string? name = null)
    {
        name ??= filePath.GetFileNameWithoutExtension();
        var reader = await ParquetReader.CreateAsync(filePath);
        var parquetColumns = await reader.ReadEntireRowGroupAsync();
        var araColumns = parquetColumns.Select((c, i) => new ParquetColumnAdapter(c, i)).ToList();
        return new DataTable.DataTable(name, araColumns, null);
    }

    public static async Task<ParquetTable<T>> ReadParquetAsync<T>(this FilePath filePath, Func<object[], T> ctor, string? name = null)
    {
        name ??= filePath.GetFileNameWithoutExtension();
        var reader = await ParquetReader.CreateAsync(filePath);
        var parquetColumns = await reader.ReadEntireRowGroupAsync();
        return new ParquetTable<T>(name, parquetColumns, ctor);
    }

    public static async Task<IDataTable> ReadParquetAsync(this Stream stream, string name)
    {
        var reader = await ParquetReader.CreateAsync(stream);
        var parquetColumns = await reader.ReadEntireRowGroupAsync();
        var araColumns = parquetColumns.Select((c, i) => new ParquetColumnAdapter(c, i)).ToList();
        return new DataTable.DataTable(name, araColumns, null);
    }

    public static async Task<ParquetColumn<T>> ReadParquetColumnAsync<T>(this Stream stream)
    {
        var reader = await ParquetReader.CreateAsync(stream);
        var parquetColumns = await reader.ReadEntireRowGroupAsync();
        if (parquetColumns.Length != 1) throw new Exception("Expected exactly one column");
        return new ParquetColumn<T>(parquetColumns[0]);
    }

    public static async Task<ParquetTable<T>> ReadParquetAsync<T>(this Stream stream, string name, Func<object[], T> ctor)
    {
        var reader = await ParquetReader.CreateAsync(stream);
        var parquetColumns = await reader.ReadEntireRowGroupAsync();
        return new ParquetTable<T>(name, parquetColumns, ctor);
    }

    /// <summary>
    /// Reads every "*.parquet" entry from <paramref name="zipPath"/>
    /// and returns them as a list of tables.
    /// </summary>
    public static async Task<IDataSet> ReadParquetFromZipAsync(this FilePath zipPath)
    {
        var tables = new List<IDataTable>();

        await using var fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var zip = new ZipArchive(fs, ZipArchiveMode.Read, leaveOpen: false);

        foreach (var entry in zip.Entries
                     .Where(e => e.Name.EndsWith(".parquet", StringComparison.OrdinalIgnoreCase))
                     .OrderBy(e => e.FullName))
        {
            await using var entryStream = entry.Open();
            await using var ms = new MemoryStream();
            await entryStream.CopyToAsync(ms);

            ms.Position = 0;
            var table = await ReadParquetAsync(ms, Path.GetFileNameWithoutExtension(entry.Name));
            tables.Add(table);
        }

        return tables.ToDataSet();
    }

    public static IDataSet ReadParquetFromZip(this FilePath filePath)
        => Task.Run(() => filePath.ReadParquetFromZipAsync()).GetAwaiter().GetResult();
    
    public static void WriteParquetToZip(this BimGeometry bg, FilePath file,
        CompressionMethod parquetCompressionMethod = CompressionMethod.Brotli,
        CompressionLevel parquetCompressionLevel = CompressionLevel.Optimal,
        CompressionLevel zipCompressionLevel = CompressionLevel.NoCompression)
        => Task.Run(() => WriteParquetToZipAsync(bg, file, parquetCompressionMethod, parquetCompressionLevel, zipCompressionLevel)).GetAwaiter().GetResult();

    public static async Task WriteParquetToZipAsync(this BimGeometry bg, FilePath file, 
        CompressionMethod parquetCompressionMethod = CompressionMethod.Brotli, 
        CompressionLevel parquetCompressionLevel = CompressionLevel.Optimal,
        CompressionLevel zipCompressionLevel = CompressionLevel.NoCompression)
    {
        await using var fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
        using var zip = new ZipArchive(fs, ZipArchiveMode.Create, leaveOpen: false);
        await WriteParquetToZipAsync(bg, zip, parquetCompressionMethod, parquetCompressionLevel);
    }

    public static void WriteParquetToZip(this BimGeometry bg, ZipArchive zip,
        CompressionMethod parquetCompressionMethod = CompressionMethod.Brotli,
        CompressionLevel parquetCompressionLevel = CompressionLevel.Optimal,
        CompressionLevel zipCompressionLevel = CompressionLevel.NoCompression)
        => Task.Run(() =>
                WriteParquetToZipAsync(bg, zip, parquetCompressionMethod, parquetCompressionLevel, zipCompressionLevel))
            .GetAwaiter().GetResult();

    public static async Task WriteParquetToZipAsync(this BimGeometry bg, ZipArchive zip,
        CompressionMethod parquetCompressionMethod = CompressionMethod.Brotli,
        CompressionLevel parquetCompressionLevel = CompressionLevel.Optimal,
        CompressionLevel zipCompressionLevel = CompressionLevel.NoCompression)
    {
        var builders = bg.ToParquet();
        foreach (var builder in builders)
        {
            var entryName = $"{builder.Name}.parquet";
            // Quickly compress data
            var entry = zip.CreateEntry(entryName, CompressionLevel.Fastest);
            await using var parquetBuffer = new MemoryStream();
            await builder.SaveToStream(parquetBuffer, parquetCompressionMethod, parquetCompressionLevel);
            parquetBuffer.Position = 0;
            await using var entryStream = entry.Open();
            await parquetBuffer.CopyToAsync(entryStream);
        }
    }

    public static List<ParquetBuilder> ToParquet(this BimGeometry bg)
    {
        var r = new List<ParquetBuilder>();
        {
            var pb = new ParquetBuilder(BimGeometry.MaterialTableName);
            pb.Add(bg.MaterialRed, nameof(bg.MaterialRed));
            pb.Add(bg.MaterialGreen, nameof(bg.MaterialGreen));
            pb.Add(bg.MaterialBlue, nameof(bg.MaterialBlue));
            pb.Add(bg.MaterialAlpha, nameof(bg.MaterialAlpha));
            pb.Add(bg.MaterialMetallic, nameof(bg.MaterialMetallic));
            pb.Add(bg.MaterialRoughness, nameof(bg.MaterialRoughness));
            r.Add(pb);
        }
        {
            var pb = new ParquetBuilder(BimGeometry.VertexTableName);
            pb.Add(bg.VertexX, nameof(bg.VertexX));
            pb.Add(bg.VertexY, nameof(bg.VertexY));
            pb.Add(bg.VertexZ, nameof(bg.VertexZ));
            r.Add(pb);
        }
        {
            var pb = new ParquetBuilder(BimGeometry.IndexTableName);
            pb.Add(bg.IndexBuffer, nameof(bg.IndexBuffer));
            r.Add(pb);
        }
        {
            var pb = new ParquetBuilder(BimGeometry.InstanceTableName);
            pb.Add(bg.InstanceEntityIndex, nameof(bg.InstanceEntityIndex));
            pb.Add(bg.InstanceMaterialIndex, nameof(bg.InstanceMaterialIndex));
            pb.Add(bg.InstanceMeshIndex, nameof(bg.InstanceMeshIndex));
            pb.Add(bg.InstanceTransformIndex, nameof(bg.InstanceTransformIndex));
            pb.Add(bg.InstanceFlags, nameof(bg.InstanceFlags));
            r.Add(pb);
        }
        {
            var pb = new ParquetBuilder(BimGeometry.MeshTableName);
            pb.Add(bg.MeshIndexOffset, nameof(bg.MeshIndexOffset));
            pb.Add(bg.MeshVertexOffset, nameof(bg.MeshVertexOffset));
            r.Add(pb);
        }
        {
            var pb = new ParquetBuilder(BimGeometry.TransformTableName);
            pb.Add(bg.TransformTX, nameof(bg.TransformTX));
            pb.Add(bg.TransformTY, nameof(bg.TransformTY));
            pb.Add(bg.TransformTZ, nameof(bg.TransformTZ));
            pb.Add(bg.TransformQX, nameof(bg.TransformQX));
            pb.Add(bg.TransformQY, nameof(bg.TransformQY));
            pb.Add(bg.TransformQZ, nameof(bg.TransformQZ));
            pb.Add(bg.TransformQW, nameof(bg.TransformQW));
            pb.Add(bg.TransformSX, nameof(bg.TransformSX));
            pb.Add(bg.TransformSY, nameof(bg.TransformSY));
            pb.Add(bg.TransformSZ, nameof(bg.TransformSZ));
            r.Add(pb);
        }
        return r;
    }

    public static async Task<BimGeometry> ReadBimGeometryFromParquetZipAsync(this FilePath fp)
        => (await fp.ReadParquetFromZipAsync()).ToBimGeometry();

    public static BimGeometry ReadBimGeometryFromParquetZip(this FilePath fp)
        => Task.Run(() => fp.ReadBimGeometryFromParquetZipAsync()).GetAwaiter().GetResult();

    /// <summary>
    /// Reads every "*.parquet" entry from <paramref name="zipPath"/>
    /// and returns them as a list of tables.
    /// </summary>
    public static async Task<BimData> ReadBimDataFromParquetZipAsync(this FilePath zipPath, ILogger logger = null)
    {
        var geometryTables = new List<IDataTable>();

        await using var fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var zip = new ZipArchive(fs, ZipArchiveMode.Read, leaveOpen: false);

        var entries = zip.Entries
            .Where(e => e.Name.EndsWith(".parquet", StringComparison.OrdinalIgnoreCase))
            .OrderBy(e => e.FullName)
            .ToList();

        logger?.Log("Creating memory streams");
        var streams = new List<MemoryStream>();
        var names = new List<string>();
        foreach (var entry in entries)
        {
            await using var entryStream = entry.Open();
            var ms = new MemoryStream();
            await entryStream.CopyToAsync(ms);
            streams.Add(ms);
            names.Add(entry.Name);
            ms.Position = 0;
        }
        logger?.Log("Creating data table reading tasks");

        var tables = new IDataTable[streams.Count];
        var dop = Math.Max(1, Environment.ProcessorCount - 1);

        using var sem = new SemaphoreSlim(dop);

        var bimData = new BimData();

        async Task ReadOneAsync(int i)
        {
            await sem.WaitAsync().ConfigureAwait(false);

            var stream = streams[i];

            try
            {
                var name = Path.GetFileNameWithoutExtension(names[i]);

                if (stream.CanSeek)
                    stream.Position = 0;

                var ctor = GetTableCtor(name);

                if (ctor == null)
                {
                    tables[i] = await ReadParquetAsync(stream, name).ConfigureAwait(false);
                }
                else
                {
                    await ctor(stream, bimData).ConfigureAwait(false);
                }
            }
            finally
            {
                try
                {
                    await stream.DisposeAsync().ConfigureAwait(false);
                }
                finally
                {
                    sem.Release();
                }
            }
        }

        var tasks = Enumerable.Range(0, streams.Count)
            .Select(ReadOneAsync)
            .ToArray();

        logger?.Log("Executing tasks");
        await Task.WhenAll(tasks).ConfigureAwait(false);

        logger?.Log("Create BIM geometry from geometry data tables");
        foreach (var table in tables)
        {
            if (table == null)
                continue;

            if (BimGeometry.TableNames.Contains(table.Name))
                geometryTables.Add(table);
            else
                Debug.WriteLine($"Unexpected table {table.Name}");
        }
        var geometryDataSet = geometryTables.ToDataSet();
        var bimGeometry = geometryDataSet.ToBimGeometry();
        bimData.Geometry = bimGeometry;
        return bimData;
    }


    public static Diagnostic ToDiagnostic(object[] row)
        => new((DiagnosticType)I32(row[0]), (DocumentIndex)I32(row[1]), (EntityIndex)I32(row[2]), (StringIndex)I32(row[3]));

    public static Point ToPoint(object[] row)
        => new(F32(row[0]), F32(row[1]), F32(row[2]));

    public static float ToNumber(object[] row)
        => F32(row[0]);

    public static Parameter ToParameter(object[] row)
        => new((EntityIndex)I32(row[0]), (DescriptorIndex)I32(row[1]), I32(row[2]));

    public static EntityRelation ToRelation(object[] row)
        => new((EntityIndex)I32(row[0]), (EntityIndex)I32(row[1]), (RelationType)I32(row[2]));

    public static ParameterDescriptor ToDescriptor(object[] row)
        => new((StringIndex)I32(row[0]), (StringIndex)I32(row[1]), (StringIndex)I32(row[2]), (ParameterType)I32(row[3]));

    public static Document ToDocument(object[] row)
        => new((StringIndex)I32(row[0]), (StringIndex)I32(row[1]));

    public static Entity ToEntity(object[] row)
        => new(I64(row[0]), (StringIndex)I32(row[1]), (DocumentIndex)I32(row[2]), (StringIndex)I32(row[3]), (EntityIndex)I32(row[4]), (EntityIndex)I32(row[5]));

    static int I32(object value) => Convert.ToInt32(value);
    static long I64(object value) => Convert.ToInt64(value);
    static float F32(object value) => Convert.ToSingle(value);


    public static Func<Stream, BimData, Task> GetTableCtor(string name)
    {
        switch (name)
        {
            // Tables with single columns
            case nameof(BimData.Strings): return async (stream, data) => data.Strings = (await ReadParquetColumnAsync<string>(stream)).ToArray();
            case nameof(BimData.Numbers): return async (stream, data) => data.Numbers = (await ReadParquetColumnAsync<float>(stream)).ToArray();
                    
            // Compound tables
            case nameof(BimData.Diagnostics): return async (stream, data) => data.Diagnostics = (await ReadParquetAsync(stream, name, ToDiagnostic)).ToArray();
            case nameof(BimData.Documents): return async (stream, data) => data.Documents = (await ReadParquetAsync(stream, name, ToDocument)).ToArray();
            case nameof(BimData.Points): return async (stream, data) => data.Points = (await ReadParquetAsync(stream, name, ToPoint)).ToArray();
            case nameof(BimData.Parameters): return async (stream, data) => data.Parameters = (await ReadParquetAsync(stream, name, ToParameter)).ToArray();
            case nameof(BimData.Relations): return async (stream, data) => data.Relations = (await ReadParquetAsync(stream, name, ToRelation)).ToArray();
            case nameof(BimData.Descriptors): return async (stream, data) => data.Descriptors = (await ReadParquetAsync(stream, name, ToDescriptor)).ToArray();
            case nameof(BimData.Entities): return async (stream, data) => data.Entities = (await ReadParquetAsync(stream, name, ToEntity)).ToArray();

            // Everything else 
            default: return null;
        }
    }

    public static BimData ReadBimDataFromParquetZip(this FilePath fp)
        => Task.Run(() => fp.ReadBimDataFromParquetZipAsync()).GetAwaiter().GetResult();

    public static async Task WriteToParquetZipAsync(this IBimData data, FilePath fp)
        => await data.ToDataSet().WriteParquetToZipAsync(fp);

    public static void WriteToParquetZip(this IBimData data, FilePath fp)
        => Task.Run(() => data.WriteToParquetZipAsync(fp)).GetAwaiter().GetResult();

    public static void WriteBimOpenSchema(this BimDataBuilder bdb, FilePath fp, CompressionLevel compressionLevel)
    {
        var dataSet = bdb.Build().ToDataSet();
        var fs = new FileStream(fp, FileMode.Create, FileAccess.Write, FileShare.None);
        using var zip = new ZipArchive(fs, ZipArchiveMode.Create, leaveOpen: false);

        // Default: Optimal
        var parquetCompressionMethod = CompressionMethod.Brotli;
        var parquetCompressionLevel = CompressionLevel.Optimal;
        var zipCompressionLevel = CompressionLevel.NoCompression;

        if (compressionLevel == CompressionLevel.NoCompression)
        {
            parquetCompressionMethod = CompressionMethod.None;
            parquetCompressionLevel = CompressionLevel.NoCompression;
            zipCompressionLevel = CompressionLevel.NoCompression;
        }
        else if (compressionLevel == CompressionLevel.SmallestSize)
        {
            // VERY SLOW but very fast
            parquetCompressionMethod = CompressionMethod.Brotli;
            parquetCompressionLevel = CompressionLevel.SmallestSize;
            zipCompressionLevel = CompressionLevel.SmallestSize;
        }
        else if (compressionLevel == CompressionLevel.Fastest)
        {
            parquetCompressionMethod = CompressionMethod.Snappy;
            parquetCompressionLevel = CompressionLevel.Fastest;
            zipCompressionLevel = CompressionLevel.Fastest;
        }

        dataSet.WriteParquetToZip(
            zip,
            parquetCompressionMethod,
            parquetCompressionLevel,
            zipCompressionLevel);

        bdb.Geometry.WriteParquetToZip(
            zip,
            parquetCompressionMethod,
            parquetCompressionLevel,
            zipCompressionLevel);
    }
}