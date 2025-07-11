using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Ara3D.Utils;
using BIMOpenSchema;
using MessagePack;
using MessagePack.Resolvers;
using System.Text.Json;

namespace Ara3D.DataSetBrowser.WPF;

public class SerializationStats
{
    public TimeSpan Elapsed;
    public string Path;
    public long Size;
}

public static class Serialization
{
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

    public static BIMData LoadBimDataFromJsonZip(FilePath fp)
        => LoadBimDataFromJson(new GZipStream(fp.OpenRead(), CompressionMode.Decompress));

    public static BIMData LoadBimDataFromJson(FilePath fp)
        => LoadBimDataFromJson(fp.OpenRead());

    public static BIMData LoadBimDataFromJson(Stream stream)
        => JsonSerializer.Deserialize<BIMData>(stream);

    public static BIMData ReadBimDataFromMessagePack(FilePath fp)
    {
        var bytes = fp.ReadAllBytes();
        var options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);
        var obj = MessagePackSerializer.Typeless.Deserialize(bytes, options);
        return MessagePackDynamicConverter.ConvertTo<BIMData>(obj);
    }

    public static void WriteBIMDataToJson(BIMData data, FilePath fp, bool withIndenting, bool withZip)
    {
        using var stream = fp.OpenWrite();
        if (!withZip)
        {
            JsonSerializer.Serialize(stream, data, new JsonSerializerOptions() { WriteIndented = withIndenting });
        }
        else
        {
            var zipStream = new GZipStream(stream, CompressionMode.Compress);
            JsonSerializer.Serialize(zipStream, data, new JsonSerializerOptions() { WriteIndented = withIndenting });
        }
    }

    public static void WriteBIMDataToMessagePack(BIMData data, FilePath fp, bool useLz4, bool useGZip)
    {
        // Choose resolver + (optional) LZ4 compression inside MessagePack itself
        var opts = MessagePackSerializerOptions.Standard
            .WithResolver(ContractlessStandardResolver.Instance);
        if (useLz4) opts = opts.WithCompression(MessagePackCompression.Lz4Block);

        var memStream = new MemoryStream();

        using var file = fp.OpenWrite();
        Stream target = file;

        if (useGZip)
            target = new GZipStream(file, CompressionMode.Compress, leaveOpen: false);

        MessagePackSerializer.Serialize(target, data, opts);   // write direct
        target.Flush();
    }

    public static void WriteDuckDB(BIMData data, FilePath fp)
        => data.ToDataSet().WriteToDuckDB(fp);
}