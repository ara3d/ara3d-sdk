using System.Text.Json.Nodes;
using Ara3D.MCP;

namespace Ara3D.Ifc.Mcp.Tests;

/// <summary>Exercises the parameter tools against FZK-Haus. The model is an ArchiCAD export: its
/// walls carry only AC_Pset_* parameters, while the standard Pset_*Common sets appear on beams and
/// stairs, so "missing" columns below are the model's shape and not a lookup failure.</summary>
[TestFixture]
public sealed class IfcParameterToolTests
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
    public void ToolsList_CoversTheParameterSurface()
    {
        var result = _mcp.HandlePost("""{"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}""");
        var names = JsonNode.Parse(result.JsonBody!)!["result"]!["tools"]!
            .AsArray()
            .Select(tool => tool!["name"]!.GetValue<string>())
            .ToList();

        Assert.That(names, Is.SupersetOf(new[]
        {
            "ifc_parameters", "ifc_parameter_values", "ifc_find_by_parameter", "ifc_parameter_table",
        }));
    }

    [Test]
    public void Parameters_CoverBothPropertiesAndQuantities()
    {
        var data = CallData("ifc_parameters", new JsonObject { ["path"] = _path, ["take"] = 1 });
        Assert.That(data["parameterCount"]!.GetValue<int>(), Is.GreaterThan(500));
        Assert.That(data["elementsWithParameters"]!.GetValue<int>(), Is.GreaterThan(50));

        var kinds = Items("ifc_parameters", new JsonObject { ["path"] = _path, ["take"] = 2000 }, "parameters")
            .Select(item => item!["isQuantity"]!.GetValue<bool>())
            .Distinct()
            .ToList();

        Assert.That(kinds, Is.EquivalentTo(new[] { true, false }));
    }

    /// <summary>IfcPropData leaves set and property names in their STEP encoding, so an unfixed
    /// index reports "H\X2\00F6\X0\he" and nobody can search for "Höhe".</summary>
    [Test]
    public void ParameterNames_AreIfcDecoded()
    {
        var items = Items("ifc_parameters", new JsonObject { ["path"] = _path, ["take"] = 2000 }, "parameters");
        foreach (var item in items)
        {
            Assert.That(item!["name"]!.GetValue<string>(), Does.Not.Contain(@"\X2\"));
            Assert.That(item["propertySet"]!.GetValue<string>(), Does.Not.Contain(@"\X2\"));
        }

        var names = items.Select(item => item!["name"]!.GetValue<string>()).ToList();
        Assert.That(names, Does.Contain("Höhe"));
    }

    [Test]
    public void Parameters_FilterByNameAndSet()
    {
        var items = Items(
            "ifc_parameters",
            new JsonObject { ["path"] = _path, ["name"] = "bearing", ["propertySet"] = "Common", ["take"] = 50 },
            "parameters");

        Assert.That(items, Is.Not.Empty);
        foreach (var item in items)
        {
            Assert.That(item!["name"]!.GetValue<string>(), Does.Contain("Bearing").IgnoreCase);
            Assert.That(item["propertySet"]!.GetValue<string>(), Does.Contain("Common").IgnoreCase);
        }
    }

    /// <summary>A type filter re-counts against that type, so the count drops and parameters no
    /// element of the type carries fall out entirely.</summary>
    [Test]
    public void Parameters_TypeFilterNarrowsTheCatalogue()
    {
        var all = Items("ifc_parameters", new JsonObject { ["path"] = _path, ["take"] = 2000 }, "parameters").Count;
        var windows = Items(
            "ifc_parameters",
            new JsonObject { ["path"] = _path, ["type"] = "IFCWINDOW", ["take"] = 2000 },
            "parameters");

        Assert.That(windows, Is.Not.Empty);
        Assert.That(windows.Count, Is.LessThan(all));
        foreach (var item in windows)
            Assert.That(item!["elementCount"]!.GetValue<int>(), Is.GreaterThan(0));
    }

    [Test]
    public void ParameterValues_TallyDistinctValuesByElementCount()
    {
        var data = CallData("ifc_parameter_values", new JsonObject
        {
            ["path"] = _path,
            ["name"] = "Pset_BeamCommon.LoadBearing",
            ["take"] = 10,
        });

        Assert.That(data["propertySets"]!.AsArray().Select(set => set!.GetValue<string>()),
            Is.EqualTo(new[] { "Pset_BeamCommon" }));

        var items = data["values"]!["items"]!.AsArray();
        Assert.That(items[0]!["value"]!.GetValue<string>(), Is.EqualTo(".T."));
        Assert.That(items[0]!["elementCount"]!.GetValue<int>(), Is.EqualTo(45));
    }

    /// <summary>A bare name spans every set that uses it; qualifying it narrows to one.</summary>
    [Test]
    public void ParameterValues_BareNameSpansSetsAndQualifiedNarrows()
    {
        var bare = CallData("ifc_parameter_values", new JsonObject { ["path"] = _path, ["name"] = "IsExternal" });
        var qualified = CallData("ifc_parameter_values", new JsonObject
        {
            ["path"] = _path,
            ["name"] = "Pset_StairCommon.IsExternal",
        });

        Assert.That(bare["propertySets"]!.AsArray(), Has.Count.GreaterThan(1));
        Assert.That(qualified["propertySets"]!.AsArray(), Has.Count.EqualTo(1));

        var bareCount = bare["values"]!["items"]!.AsArray()[0]!["elementCount"]!.GetValue<int>();
        var oneCount = qualified["values"]!["items"]!.AsArray()[0]!["elementCount"]!.GetValue<int>();
        Assert.That(bareCount, Is.GreaterThan(oneCount));
    }

    [Test]
    public void FindByParameter_OrdersNumericallyAndRespectsTypeFilter()
    {
        var data = CallData("ifc_find_by_parameter", new JsonObject
        {
            ["path"] = _path,
            ["name"] = "Height",
            ["op"] = "gt",
            ["value"] = "1",
            ["type"] = "IFCWINDOW",
            ["take"] = 50,
        });

        var items = data["matches"]!["items"]!.AsArray();
        Assert.That(items, Is.Not.Empty);
        foreach (var item in items)
        {
            Assert.That(item!["entity"]!["type"]!.GetValue<string>(), Is.EqualTo("IFCWINDOW"));
            Assert.That(double.Parse(item["value"]!.GetValue<string>()), Is.GreaterThan(1));
        }

        var ids = items.Select(item => item!["entity"]!["id"]!.GetValue<int>()).ToList();
        Assert.That(ids, Is.Ordered);
        Assert.That(ids, Is.Unique);
    }

    /// <summary>Equality compares as numbers when both sides read as numbers, so a differently
    /// written form of the same value still matches.</summary>
    [Test]
    public void FindByParameter_EqualityIsNumericWhenBothSidesAre()
    {
        var plain = Matches("Height", "eq", "1.2", "IFCWINDOW");
        var padded = Matches("Height", "eq", "1.20", "IFCWINDOW");
        Assert.That(plain, Is.GreaterThan(0));
        Assert.That(padded, Is.EqualTo(plain));
    }

    [Test]
    public void FindByParameter_TextEqualityAndExistsDisagreeAsExpected()
    {
        Assert.That(Matches("IsExternal", "eq", ".F.", null), Is.EqualTo(46));
        Assert.That(Matches("IsExternal", "eq", ".T.", null), Is.EqualTo(0));
        Assert.That(Matches("IsExternal", "ne", ".T.", null), Is.EqualTo(46));
        Assert.That(Matches("IsExternal", "exists", null, null), Is.EqualTo(46));
    }

    [Test]
    public void ParameterTable_GivesEveryElementOfATypeARow()
    {
        var data = CallData("ifc_parameter_table", new JsonObject
        {
            ["path"] = _path,
            ["names"] = "IsExternal,LoadBearing",
            ["type"] = "IFCWALLSTANDARDCASE",
            ["take"] = 50,
        });

        Assert.That(data["columns"]!.AsArray().Select(column => column!.GetValue<string>()),
            Is.EqualTo(new[] { "IsExternal", "LoadBearing" }));

        var rows = data["rows"]!["items"]!.AsArray();
        Assert.That(rows, Is.Not.Empty);
        foreach (var row in rows)
        {
            Assert.That(row!["entity"]!["type"]!.GetValue<string>(), Is.EqualTo("IFCWALLSTANDARDCASE"));
            Assert.That(row["values"]!.AsArray(), Has.Count.EqualTo(2));
        }
    }

    [Test]
    public void ParameterTable_RestrictsToRequestedIds()
    {
        var beam = Items("ifc_find_by_parameter", new JsonObject
        {
            ["path"] = _path,
            ["name"] = "Pset_BeamCommon.LoadBearing",
            ["take"] = 2,
        }, "matches");

        var ids = beam.Select(item => item!["entity"]!["id"]!.GetValue<int>()).ToList();
        var data = CallData("ifc_parameter_table", new JsonObject
        {
            ["path"] = _path,
            ["names"] = "LoadBearing",
            ["ids"] = string.Join(",", ids),
        });

        var rows = data["rows"]!["items"]!.AsArray();
        Assert.That(rows.Select(row => row!["entity"]!["id"]!.GetValue<int>()), Is.EqualTo(ids));
        foreach (var row in rows)
            Assert.That(row!["values"]!.AsArray()[0]!.GetValue<string>(), Is.EqualTo(".T."));
    }

    [Test]
    public void Paging_ReportsTheUnpagedTotal()
    {
        var data = CallData("ifc_parameters", new JsonObject { ["path"] = _path, ["take"] = 5 });
        var page = data["parameters"]!;
        Assert.That(page["count"]!.GetValue<int>(), Is.EqualTo(5));
        Assert.That(page["total"]!.GetValue<int>(), Is.GreaterThan(5));
    }

    /// <summary>The index is derived from the property data the session already holds, so no
    /// parameter question triggers the expensive BIM Open Schema conversion.</summary>
    [Test]
    public void ParameterQueries_DoNotBuildTheBosDatabase()
    {
        CallData("ifc_parameters", new JsonObject { ["path"] = _path, ["take"] = 5 });
        CallData("ifc_find_by_parameter", new JsonObject
        {
            ["path"] = _path,
            ["name"] = "IsExternal",
            ["value"] = ".F.",
        });

        Assert.That(_cache.Get(_path).BosIsBuilt, Is.False);
    }

    [Test]
    public void UnknownParameter_FailsInsideTheEnvelope()
    {
        var payload = Call("ifc_parameter_values", new JsonObject { ["path"] = _path, ["name"] = "NoSuchParameter" });
        Assert.That(payload["ok"]!.GetValue<bool>(), Is.False);
        Assert.That(payload["type"]!.GetValue<string>(), Is.EqualTo("KeyNotFoundException"));
        Assert.That(payload["error"]!.GetValue<string>(), Does.Contain("ifc_parameters"));
    }

    [Test]
    public void OrderingOperatorAgainstNonNumericValue_FailsInsideTheEnvelope()
    {
        var payload = Call("ifc_find_by_parameter", new JsonObject
        {
            ["path"] = _path,
            ["name"] = "IsExternal",
            ["op"] = "gt",
            ["value"] = "yes",
        });

        Assert.That(payload["ok"]!.GetValue<bool>(), Is.False);
        Assert.That(payload["type"]!.GetValue<string>(), Is.EqualTo("ArgumentException"));
    }

    [Test]
    public void UnknownOperator_FailsInsideTheEnvelope()
    {
        var payload = Call("ifc_find_by_parameter", new JsonObject
        {
            ["path"] = _path,
            ["name"] = "IsExternal",
            ["op"] = "approximately",
            ["value"] = ".F.",
        });

        Assert.That(payload["ok"]!.GetValue<bool>(), Is.False);
        Assert.That(payload["type"]!.GetValue<string>(), Is.EqualTo("ArgumentException"));
    }

    private int Matches(string name, string op, string? value, string? type)
    {
        var arguments = new JsonObject { ["path"] = _path, ["name"] = name, ["op"] = op, ["take"] = 1 };
        if (value != null)
            arguments["value"] = value;
        if (type != null)
            arguments["type"] = type;

        return CallData("ifc_find_by_parameter", arguments)["matches"]!["total"]!.GetValue<int>();
    }

    private IReadOnlyList<JsonNode?> Items(string tool, JsonObject arguments, string field)
        => CallData(tool, arguments)[field]!["items"]!.AsArray().ToList();

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
}
