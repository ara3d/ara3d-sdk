using System.Text.Json;
using Ara3D.Utils;

namespace Ara3D.Bowerbird;

/// <summary>
/// Loads and validates a command manifest from its folder.
/// </summary>
public static class CommandManifestReader
{
    public const string ManifestExtension = ".manifest.json";

    public static FilePath GetManifestPath(DirectoryPath commandFolder)
    {
        var folderName = commandFolder.GetFolderName();
        return commandFolder.RelativeFile($"{folderName}{ManifestExtension}");
    }

    public static bool TryRead(DirectoryPath commandFolder, out CommandManifest manifest, out string error)
    {
        manifest = null;
        error = null;

        var folderName = commandFolder.GetFolderName();
        var manifestPath = GetManifestPath(commandFolder);

        if (!manifestPath.Exists())
        {
            error = $"Missing manifest: {manifestPath}";
            return false;
        }

        try
        {
            var json = manifestPath.ReadAllText();
            manifest = JsonSerializer.Deserialize<CommandManifest>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            error = $"Failed to parse manifest {manifestPath}: {ex.Message}";
            return false;
        }

        if (manifest == null)
        {
            error = $"Manifest is empty: {manifestPath}";
            return false;
        }

        if (manifest.DisplayName.IsNullOrWhiteSpace())
        {
            error = $"Manifest missing displayName: {manifestPath}";
            return false;
        }

        if (manifest.TypeName.IsNullOrWhiteSpace())
        {
            error = $"Manifest missing typeName: {manifestPath}";
            return false;
        }

        if (!manifest.Kind.IsNullOrWhiteSpace() && !IsKnownKind(manifest.Kind))
        {
            error = $"Manifest has invalid kind '{manifest.Kind}': {manifestPath}";
            return false;
        }

        return true;
    }

    static bool IsKnownKind(string kind)
        => string.Equals(kind, "command", StringComparison.OrdinalIgnoreCase)
           || string.Equals(kind, "generator", StringComparison.OrdinalIgnoreCase)
           || string.Equals(kind, "modifier", StringComparison.OrdinalIgnoreCase);

    static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };
}
