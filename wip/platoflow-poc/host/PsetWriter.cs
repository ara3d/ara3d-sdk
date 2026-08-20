using System.Text.Json;
using System.Text.Json.Nodes;
using Ara3D.Ifc.Tests;
using Ara3D.Utils;

namespace PlatoFlow.Host;

/// <summary>Writes computed values back into a real IFC file as a property set, using the
/// byte-exact append/diff machinery from ara3d-sdk: everything not explicitly added stays
/// byte-identical, so the diff is the complete story of what the graph changed.</summary>
public static class PsetWriter
{
    public static JsonObject Append(ModelCatalog catalog, JsonNode? request)
    {
        var model = catalog.Require(request?["model"]?.GetValue<string>());
        if (model.IfcFile == null)
            throw new ArgumentException($"Model '{model.Id}' has no source IFC, so psets cannot be written to it.");

        var psetName = request?["psetName"]?.GetValue<string>() ?? "Ara3D_Analytics";
        var rows = request?["rows"] as JsonArray
                   ?? throw new ArgumentException("'rows' must be an array of {globalId, props}.");

        using var source = IfcSourceFile.Load(new FilePath(catalog.PathOf(model.IfcFile)));
        var lookup = source.GlobalIdToEntityId();
        var builder = new IfcPropertySetBuilder(source.MaxId + 1, source.FirstIdOfType("IFCOWNERHISTORY"));

        var skipped = new JsonArray();
        var written = 0;
        foreach (var row in rows.OfType<JsonObject>())
        {
            var globalId = row["globalId"]?.GetValue<string>();
            if (globalId == null || !lookup.TryGetValue(globalId, out var entityId))
            {
                skipped.Add(globalId ?? "(null)");
                continue;
            }

            var props = Properties(row["props"] as JsonObject);
            if (props.Count == 0)
            {
                skipped.Add(globalId);
                continue;
            }

            builder.AddPropertySet(entityId, psetName, props, $"{psetName}:{globalId}");
            written++;
        }

        if (builder.Lines.Count == 0)
            throw new ArgumentException("Nothing to write: no row matched an element in the model.");

        Directory.CreateDirectory(catalog.OutDir);
        var outPath = Path.Combine(catalog.OutDir, $"{model.Id}-enriched.ifc");
        File.WriteAllBytes(outPath, IfcPatcher.Append(source, builder.Lines));

        using var modified = IfcSourceFile.Load(new FilePath(outPath));
        var diff = IfcDiff.Compare(source, modified);

        return new JsonObject
        {
            ["outPath"] = outPath,
            ["entitiesAdded"] = builder.Ids.Count,
            ["diffSummary"] = diff.ToString(),
            ["diff"] = new JsonObject
            {
                ["added"] = diff.Added.Count,
                ["deleted"] = diff.Deleted.Count,
                ["changed"] = diff.Changed.Count,
            },
            ["psetName"] = psetName,
            ["elementsWritten"] = written,
            ["skipped"] = skipped,
        };
    }

    /// <summary>Maps a JSON property bag onto IFC measure literals. Numbers become IFCREAL; short
    /// strings IFCLABEL and long ones IFCTEXT, because IfcLabel is capped at 255 characters.</summary>
    private static List<IfcPropertyValue> Properties(JsonObject? props)
    {
        var result = new List<IfcPropertyValue>();
        if (props == null)
            return result;

        foreach (var (name, value) in props)
        {
            if (value is not JsonValue jsonValue)
                continue;

            result.Add(jsonValue.GetValueKind() switch
            {
                JsonValueKind.Number => IfcPropertyValue.Real(name, jsonValue.GetValue<double>()),
                JsonValueKind.True or JsonValueKind.False => IfcPropertyValue.Boolean(name, jsonValue.GetValue<bool>()),
                _ => Text(name, jsonValue.ToString()),
            });
        }

        return result;
    }

    private static IfcPropertyValue Text(string name, string value)
        => value.Length > 255 ? IfcPropertyValue.Text(name, value) : IfcPropertyValue.Label(name, value);
}
