using System.Text.Json;
using System.Text.Json.Serialization;
using Ara3D.Utils;

namespace Ara3D.Bowerbird;

/// <summary>
/// Reads and writes folder/bin/compile-cache.json after successful compiles.
/// </summary>
public static class CommandCompileCache
{
    public const string CacheFileName = "compile-cache.json";

    public static FilePath? TryGet(CommandDescriptor descriptor, CommandCompileCacheKey key)
    {
        var cachePath = descriptor.OutputFolder.RelativeFile(CacheFileName);
        if (!cachePath.Exists())
            return null;

        CompileCacheRecord record;
        try
        {
            record = JsonSerializer.Deserialize<CompileCacheRecord>(cachePath.ReadAllText(), JsonOptions);
        }
        catch
        {
            return null;
        }

        if (record == null || record.Fingerprint != key.Fingerprint)
            return null;

        var outputPath = record.OutputDll?.Trim();
        if (outputPath.IsNullOrWhiteSpace() || !File.Exists(outputPath))
            return null;

        return new FilePath(outputPath);
    }

    public static void Write(CommandDescriptor descriptor, CommandCompileCacheKey key, FilePath outputDll)
    {
        descriptor.OutputFolder.Create();
        var record = new CompileCacheRecord
        {
            Fingerprint = key.Fingerprint,
            OutputDll = outputDll.GetFullPath(),
            CompiledAt = DateTime.Now,
        };
        var cachePath = descriptor.OutputFolder.RelativeFile(CacheFileName);
        cachePath.WriteAllText(JsonSerializer.Serialize(record, JsonOptions));
    }

    sealed class CompileCacheRecord
    {
        public string Fingerprint { get; set; }
        public string OutputDll { get; set; }
        public DateTime CompiledAt { get; set; }
    }

    static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
}
