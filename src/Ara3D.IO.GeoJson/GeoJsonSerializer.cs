using System.Text.Json;
using System.Text.Json.Serialization;
using Ara3D.Utils;

namespace Ara3D.IO.GeoJson;

public static class GeoJsonSerializer
{
    /// <summary>
    /// Polymorphic converter that inspects the "type" discriminator inside a geometry object
    /// and instantiates the correct derived geometry class.
    /// </summary>
    public sealed class GeoJsonFeatureConverter : JsonConverter<GeoJsonFeature>
    {
        public override GeoJsonFeature? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);

            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                throw new JsonException("GeoJSON feature must be an object.");

            var featureType = "";
            if (doc.RootElement.TryGetProperty("feature_type", out var featureTypeProp) ||
                featureTypeProp.ValueKind != JsonValueKind.String)
            {
                featureType = featureTypeProp.GetString();
            }

            return featureType switch
            {
                "unit" => doc.RootElement.Deserialize<ImdfUnit>(options),
                "opening" => doc.RootElement.Deserialize<ImdfOpening>(options),
                _ => doc.RootElement.Deserialize<GeoJsonFeature>(options)

            };
        }

        public override void Write(Utf8JsonWriter writer, GeoJsonFeature value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }

    public static GeoJsonFeature ToGeoJsonFeature(this JsonElement e)
    {
        var ft = e.GetProperty("feature_type").GetString();
        if (ft.Equals(ImdfUnit.FeatureType, StringComparison.OrdinalIgnoreCase))
            return e.Deserialize<ImdfUnit>();
        if (ft.Equals(ImdfOpening.FeatureType, StringComparison.OrdinalIgnoreCase))
            return e.Deserialize<ImdfOpening>();
        return e.Deserialize<GeoJsonFeature>();
    }

    public static GeoJsonFeatureCollection LoadFeatureCollection(this Stream stream)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
        options.Converters.Add(new GeoJsonFeatureConverter());

        using var doc = JsonDocument.Parse(stream);
        var root = doc.RootElement;

        var r = new GeoJsonFeatureCollection();

        var features = root.GetProperty("features");
        for (var i = 0; i < features.GetArrayLength(); i++)
        {
            r.features.Add(features[i].ToGeoJsonFeature());
        }

        foreach (var f in r.features)
        {
            foreach (var kv in f.properties.ToList())
            {
                if (kv.Value == null) continue;
                var je = (JsonElement)kv.Value;
                f.properties[kv.Key] = je.ToObject();
            }
        }

        return r;
    }

    public static GeoJsonFeatureCollection LoadFeatureCollection(this DirectoryPath dir)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
        options.Converters.Add(new GeoJsonFeatureConverter());

        var r = new GeoJsonFeatureCollection();
        foreach (var file in dir.GetFiles("*.json"))
        {
            var text = file.ReadAllText();
            using var doc = JsonDocument.Parse(text);
            var root = doc.RootElement;

            var feature = root.ToGeoJsonFeature();
            r.features.Add(feature);

            foreach (var kv in feature.properties.ToList())
            {
                if (kv.Value == null) continue;
                var je = (JsonElement)kv.Value;
                feature.properties[kv.Key] = je.ToObject();
            }
        }

        return r;
    }
}