using System.Text;
using System.Text.Json.Nodes;

namespace Ara3D.BimOpenSchema.Tests;

public static class IfcBosJsonTestHelpers
{
    public static string WriteTempIfc(string prefix, string content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"ara3d-{prefix}-{Guid.NewGuid():N}.ifc");
        File.WriteAllText(path, content, Encoding.ASCII);
        return path;
    }

    public static void AssertEntity(JsonObject entity, string name, string globalId, long localId)
    {
        Assert.That(entity["name"]!.GetValue<string>(), Is.EqualTo(name));
        Assert.That(entity["globalId"]!.GetValue<string>(), Is.EqualTo(globalId));
        Assert.That(entity["localId"]!.GetValue<long>(), Is.EqualTo(localId));
        Assert.That(entity["id"]!.GetValue<int>(), Is.GreaterThanOrEqualTo(0));
    }

    public static void AssertRelation(JsonObject relation, string type, string targetName, string targetCategory)
    {
        Assert.That(relation["type"]!.GetValue<string>(), Is.EqualTo(type));

        var target = relation["entity"]!.AsObject();
        Assert.That(target["name"]!.GetValue<string>(), Is.EqualTo(targetName));
        Assert.That(target["category"]!.GetValue<string>(), Is.EqualTo(targetCategory));
        Assert.That(target["id"]!.GetValue<int>(), Is.GreaterThanOrEqualTo(0));
    }
}
