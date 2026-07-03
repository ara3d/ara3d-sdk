using System.Text.Json.Serialization;

namespace Ara3D.Bowerbird;

// TODO: support multiple tools per manifest (tool groups).
/// <summary>
/// JSON manifest describing one command folder.
/// </summary>
public record CommandManifest(
    [property: JsonPropertyName("displayName")] string DisplayName,
    [property: JsonPropertyName("typeName")] string TypeName,
    [property: JsonPropertyName("description")] string Description = null,
    [property: JsonPropertyName("longDescription")] string LongDescription = null);
