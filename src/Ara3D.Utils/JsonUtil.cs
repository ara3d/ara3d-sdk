using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Ara3D.Utils;

public static class JsonUtil
{
    public static FilePath WriteJson<T>(this FilePath filePath, T self, bool includeFields = false,
        bool writeIndented = true)
    {
        using var fs = filePath.CreateWrite();
        var options = new JsonSerializerOptions() { IncludeFields = includeFields, WriteIndented = writeIndented };
        JsonSerializer.Serialize(fs, self, options);
        return filePath;
    }

    public static T LoadJson<T>(this FilePath filePath)
    {
        using var fs = filePath.OpenRead();
        return JsonSerializer.Deserialize<T>(fs);
    }

    public static List<object> ToList(this JsonArray self)
        => self.Select(ToObject).ToList();

    public static Dictionary<string, object> ToDictionary(this JsonObject self)
        => self.ToDictionary(kv => kv.Key, kv => kv.Value.ToObject());

    public static List<object> AsList(this JsonElement je)
    {
        if (je.ValueKind  == JsonValueKind.Array)
            return Enumerable.Range(0, je.GetArrayLength()).Select(i => je[i].ToObject()).ToList();
        throw new Exception($"Not a Json Array, instead is a {je.ValueKind}");
    }

    public static Dictionary<string, object> AsDictionary(this JsonElement je)
    {
        if (je.ValueKind == JsonValueKind.Object)
            return je.EnumerateObject().ToDictionary(jp => jp.Name, jp => jp.Value.ToObject());
        throw new Exception($"Not a Json Object, instead is a {je.ValueKind}");
    }

    public static object ToObject(this JsonElement je)
    {
        switch (je.ValueKind)
        {
            case JsonValueKind.Undefined:
                return null;

            case JsonValueKind.Object:
                return je.AsDictionary();
            
            case JsonValueKind.Array:
                return je.AsList();
            
            case JsonValueKind.String:
                return je.GetString();

            case JsonValueKind.Number:
                return je.GetDouble();

            case JsonValueKind.True:
                return true;

            case JsonValueKind.False:
                return false;

            case JsonValueKind.Null:
                return null;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static object ToObject(this JsonNode node)
    {
        if (node == null)
            return null;

        switch (node)
        {
            case JsonArray jsonArray:
                return jsonArray.ToList();

            case JsonObject jsonObject:
                return jsonObject.ToDictionary();

            case JsonValue jsonValue:
                {
                    var k = jsonValue.GetValueKind();
                    switch (k)
                    {
                        case JsonValueKind.Undefined:
                            return null;

                        case JsonValueKind.Object:
                            throw new NotImplementedException("Should have been handled by earlier case");

                        case JsonValueKind.Array:
                            throw new NotImplementedException("Should have been handled by earlier case");

                        case JsonValueKind.String:
                            return jsonValue.GetValue<string>();

                        case JsonValueKind.Number:
                            return jsonValue.GetValue<double>();

                        case JsonValueKind.True:
                            return true;

                        case JsonValueKind.False:
                            return false;

                        case JsonValueKind.Null:
                            return null;

                        default:
                            return null;
                    }
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(node));
        }
    }
}