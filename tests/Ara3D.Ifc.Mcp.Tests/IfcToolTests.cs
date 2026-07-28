using System.Text.Json.Nodes;
using Ara3D.MCP;

namespace Ara3D.Ifc.Mcp.Tests;

/// <summary>Drives the tools the way a client does — through tools/call — against the FZK-Haus
/// sample, an IFC4 model with a single storey.</summary>
[TestFixture]
public sealed class IfcToolTests
{
    private IfcSessionCache _cache = null!;
    private McpServer _mcp = null!;
    private string _path = null!;

    [SetUp]
    public void SetUp()
    {
        _path = TestModel.RequirePath(TestModel.FzkHaus);
        _cache = new IfcSessionCache();
        _mcp = IfcMcpServer.Create(_cache, McpTransport.Stdio);
    }

    [TearDown]
    public void TearDown()
    {
        _mcp.Dispose();
        _cache.Dispose();
    }

    [Test]
    public void ToolsList_CoversTheDataSurface()
    {
        var names = ListToolNames();
        Assert.That(names, Is.SupersetOf(new[]
        {
            "ifc_open", "ifc_close", "ifc_models", "ifc_header", "ifc_type_counts", "ifc_search",
            "ifc_entity", "ifc_entities_of_type", "ifc_attributes",
            "ifc_properties", "ifc_quantities", "ifc_property_sets",
            "ifc_relations", "ifc_spatial_tree", "ifc_spatial_contents", "ifc_element_containment",
        }));
    }

    [Test]
    public void Open_ReportsSchemaAndEntityCount()
    {
        var data = CallData("ifc_open", new JsonObject { ["path"] = _path });
        Assert.That(data["schema"]!.GetValue<string>(), Is.EqualTo("IFC4"));
        Assert.That(data["entityCount"]!.GetValue<int>(), Is.GreaterThan(1000));
    }

    [Test]
    public void TypeCounts_AreSortedDescendingAndTotalIsUnpaged()
    {
        var data = CallData("ifc_type_counts", new JsonObject { ["path"] = _path, ["take"] = 3 });
        Assert.That(data["count"]!.GetValue<int>(), Is.EqualTo(3));
        Assert.That(data["total"]!.GetValue<int>(), Is.GreaterThan(3));

        var counts = data["items"]!.AsArray().Select(item => item!["count"]!.GetValue<int>()).ToList();
        Assert.That(counts, Is.Ordered.Descending);
    }

    [Test]
    public void SpatialTree_NestsProjectSiteBuildingStorey()
    {
        var data = CallData("ifc_spatial_tree", new JsonObject { ["path"] = _path });
        var project = data["roots"]!.AsArray()[0]!;
        Assert.That(project["type"]!.GetValue<string>(), Is.EqualTo("IFCPROJECT"));

        var site = project["children"]!.AsArray()[0]!;
        Assert.That(site["type"]!.GetValue<string>(), Is.EqualTo("IFCSITE"));

        var building = site["children"]!.AsArray()[0]!;
        Assert.That(building["type"]!.GetValue<string>(), Is.EqualTo("IFCBUILDING"));

        var storey = building["children"]!.AsArray()[0]!;
        Assert.That(storey["type"]!.GetValue<string>(), Is.EqualTo("IFCBUILDINGSTOREY"));
        Assert.That(storey["elementCount"]!.GetValue<int>(), Is.GreaterThan(0));
    }

    [Test]
    public void ElementContainment_WalksFromWallUpToProject()
    {
        var wallId = FirstIdOfType("IFCWALLSTANDARDCASE");
        var data = CallData("ifc_element_containment", new JsonObject { ["path"] = _path, ["id"] = wallId });
        var types = data["containers"]!.AsArray().Select(item => item!["type"]!.GetValue<string>()).ToList();
        Assert.That(types, Is.EqualTo(new[] { "IFCBUILDINGSTOREY", "IFCBUILDING", "IFCSITE", "IFCPROJECT" }));
    }

    /// <summary>The value of a typed property lives in a token after the one the attribute list
    /// exposes; without unwrapping it, every property reads back as its own measure type.</summary>
    [Test]
    public void Properties_UnwrapTypedValues()
    {
        var wallId = FirstIdOfType("IFCWALLSTANDARDCASE");
        var data = CallData("ifc_properties", new JsonObject { ["path"] = _path, ["id"] = wallId, ["take"] = 200 });
        var items = data["properties"]!["items"]!.AsArray();
        Assert.That(items, Is.Not.Empty);

        foreach (var item in items)
        {
            var measure = item!["measureType"]!.GetValue<string>();
            var value = item["value"]!.GetValue<string>();
            Assert.That(value, Is.Not.EqualTo(measure), $"Property '{item["name"]}' reported its type as its value.");
        }
    }

    /// <summary>IfcPropData mis-parses IFCELEMENTQUANTITY, so quantities are read from the entity
    /// directly; a wall in this model carries dozens of them.</summary>
    [Test]
    public void Quantities_AreFoundAndNamed()
    {
        var wallId = FirstIdOfType("IFCWALLSTANDARDCASE");
        var data = CallData("ifc_quantities", new JsonObject { ["path"] = _path, ["id"] = wallId, ["take"] = 200 });
        var items = data["properties"]!["items"]!.AsArray();
        Assert.That(items, Is.Not.Empty);

        var names = items.Select(item => item!["name"]!.GetValue<string>()).ToList();
        Assert.That(names, Does.Contain("Height"));
        Assert.That(names, Does.Contain("Width"));

        var sets = items.Select(item => item!["propertySet"]!.GetValue<string>()).Distinct().ToList();
        Assert.That(sets, Does.Contain("BaseQuantities"));
    }

    [Test]
    public void PropertySets_NameQuantitySetsRatherThanTheirGuid()
    {
        var wallId = FirstIdOfType("IFCWALLSTANDARDCASE");
        var data = CallData("ifc_property_sets", new JsonObject { ["path"] = _path, ["id"] = wallId });
        var quantitySets = data["propertySets"]!.AsArray()
            .Where(set => set!["isQuantitySet"]!.GetValue<bool>())
            .ToList();

        Assert.That(quantitySets, Is.Not.Empty);
        foreach (var set in quantitySets)
        {
            Assert.That(set!["memberCount"]!.GetValue<int>(), Is.GreaterThan(0));
            Assert.That(set["name"]!.GetValue<string>(), Has.Length.Not.EqualTo(22));
        }
    }

    [Test]
    public void Search_MatchesNameCaseInsensitively()
    {
        var data = CallData("ifc_search", new JsonObject
        {
            ["path"] = _path,
            ["text"] = "fenster",
            ["type"] = "IFCWINDOW",
        });

        Assert.That(data["total"]!.GetValue<int>(), Is.GreaterThan(0));
        foreach (var item in data["items"]!.AsArray())
            Assert.That(item!["type"]!.GetValue<string>(), Is.EqualTo("IFCWINDOW"));
    }

    [Test]
    public void UnknownEntity_FailsInsideTheEnvelope()
    {
        var payload = Call("ifc_entity", new JsonObject { ["path"] = _path, ["id"] = 99999999 });
        Assert.That(payload["ok"]!.GetValue<bool>(), Is.False);
        Assert.That(payload["type"]!.GetValue<string>(), Is.EqualTo("KeyNotFoundException"));
    }

    [Test]
    public void MissingFile_FailsInsideTheEnvelope()
    {
        var payload = Call("ifc_header", new JsonObject { ["path"] = "C:/no-such-model.ifc" });
        Assert.That(payload["ok"]!.GetValue<bool>(), Is.False);
        Assert.That(payload["type"]!.GetValue<string>(), Is.EqualTo("FileNotFoundException"));
    }

    private int FirstIdOfType(string type)
    {
        var data = CallData("ifc_entities_of_type", new JsonObject
        {
            ["path"] = _path,
            ["type"] = type,
            ["take"] = 1,
        });

        return data["items"]!.AsArray()[0]!["id"]!.GetValue<int>();
    }

    private JsonNode CallData(string tool, JsonObject arguments)
    {
        var payload = Call(tool, arguments);
        Assert.That(payload["ok"]!.GetValue<bool>(), Is.True, payload["error"]?.GetValue<string>());
        return payload["data"]!;
    }

    private JsonObject Call(string tool, JsonObject arguments)
    {
        var request = new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["id"] = 1,
            ["method"] = "tools/call",
            ["params"] = new JsonObject { ["name"] = tool, ["arguments"] = arguments },
        };

        var result = _mcp.HandlePost(request.ToJsonString());
        var response = JsonNode.Parse(result.JsonBody!)!;
        Assert.That(response["error"], Is.Null, response["error"]?.ToJsonString());
        var text = response["result"]!["content"]![0]!["text"]!.GetValue<string>();
        return (JsonObject)JsonNode.Parse(text)!;
    }

    private IReadOnlyList<string> ListToolNames()
    {
        var result = _mcp.HandlePost("""{"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}""");
        return JsonNode.Parse(result.JsonBody!)!["result"]!["tools"]!
            .AsArray()
            .Select(tool => tool!["name"]!.GetValue<string>())
            .ToList();
    }
}
