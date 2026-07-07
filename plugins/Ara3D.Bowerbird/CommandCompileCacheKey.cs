using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ara3D.Utils;

namespace Ara3D.Bowerbird;

/// <summary>
/// Fingerprint of command sources, manifest, refs, and resolved references for compile caching.
/// </summary>
public readonly struct CommandCompileCacheKey
{
    public const string SchemaVersion = "3";

    public CommandCompileCacheKey(string fingerprint) => Fingerprint = fingerprint;

    public string Fingerprint { get; }

    public static CommandCompileCacheKey Compute(CommandDescriptor descriptor, IReadOnlyList<FilePath> refs)
    {
        var parts = new List<string> { SchemaVersion };

        var sourceFiles = descriptor.SourceFiles
            .OrderBy(f => f.FullPath, StringComparer.OrdinalIgnoreCase)
            .Select(ToSourceFileRecord)
            .ToList();
        parts.Add(HashJson(sourceFiles));

        parts.Add(ToFileHash(descriptor.ManifestPath));

        var refsFile = descriptor.Folder.RelativeFile(ReferenceResolver.RefsFileName);
        if (refsFile.Exists())
            parts.Add(ToFileHash(refsFile));

        foreach (var fp in refs.OrderBy(r => r.FullPath, StringComparer.OrdinalIgnoreCase))
            parts.Add($"{fp.FullPath}|{fp.GetModifiedTime().Ticks}|{fp.GetFileSize()}");

        var combined = string.Join("\n", parts);
        var fingerprint = Sha256.Compute(Encoding.UTF8.GetBytes(combined)).ToHex();
        return new CommandCompileCacheKey(fingerprint);
    }

    static string ToFileHash(FilePath file)
        => Convert.ToHexString(file.SHA256Hash()).ToLowerInvariant();

    static SourceFileRecord ToSourceFileRecord(FilePath file)
        => new(file.GetFileName(), ToFileHash(file));

    static string HashJson<T>(T value)
        => Sha256.Compute(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, JsonOptions))).ToHex();

    sealed record SourceFileRecord(
        [property: JsonPropertyName("fileName")] string FileName,
        [property: JsonPropertyName("sha256")] string Sha256);

    static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };
}
