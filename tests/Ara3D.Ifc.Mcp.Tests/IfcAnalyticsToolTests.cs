using System.Text.Json.Nodes;
using Ara3D.MCP;

namespace Ara3D.Ifc.Mcp.Tests;

/// <summary>Drives the analytics tools through tools/call. The BOS conversion is a whole-file
/// re-read, so the fixture shares one session cache across every test rather than paying it per
/// test.</summary>
[TestFixture]
public sealed class IfcAnalyticsToolTests
{
    private IfcSessionCache _cache = null!;
    private McpServer _mcp = null!;
    private string _path = null!;
    private string _scratch = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _path = TestModel.RequirePath(TestModel.FzkHaus);
        _cache = new IfcSessionCache();
        _mcp = IfcMcpServer.Create(_cache, McpTransport.Stdio);
        _scratch = Path.Combine(Path.GetTempPath(), "ara3d-ifc-mcp-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_scratch);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _mcp.Dispose();
        _cache.Dispose();
        try
        {
            Directory.Delete(_scratch, recursive: true);
        }
        catch (IOException)
        {
        }
    }

    [Test]
    public void ToolsList_CoversTheAnalyticsSurface()
    {
        var result = _mcp.HandlePost("""{"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}""");
        var names = JsonNode.Parse(result.JsonBody!)!["result"]!["tools"]!
            .AsArray()
            .Select(tool => tool!["name"]!.GetValue<string>())
            .ToList();

        Assert.That(names, Is.SupersetOf(new[] { "ifc_to_bos", "ifc_table", "ifc_sql", "ifc_sql_export" }));
    }

    [Test]
    public void ToBos_WritesAZipAndCopiesItWhereAsked()
    {
        var output = Path.Combine(_scratch, "fzk.bos");
        var data = CallData("ifc_to_bos", new JsonObject { ["path"] = _path, ["outputPath"] = output });

        Assert.That(data["bosBytes"]!.GetValue<long>(), Is.GreaterThan(0));
        Assert.That(data["savedTo"]!.GetValue<string>(), Is.EqualTo(output));
        Assert.That(File.Exists(output), Is.True);

        var header = new byte[2];
        using (var stream = File.OpenRead(output))
            Assert.That(stream.Read(header, 0, 2), Is.EqualTo(2));
        Assert.That(header, Is.EqualTo(new byte[] { (byte)'P', (byte)'K' }), "A .bos file is a zip.");
    }

    [Test]
    public void Table_ListsTablesWithColumnsAndRowCounts()
    {
        var data = CallData("ifc_table", new JsonObject { ["path"] = _path, ["take"] = 50 });
        var items = data["items"]!.AsArray();

        Assert.That(data["total"]!.GetValue<int>(), Is.GreaterThan(0));
        Assert.That(items, Is.Not.Empty);

        foreach (var item in items)
        {
            TestContext.Out.WriteLine(
                $"{item!["table"]} ({item["rowCount"]}): "
                + string.Join(", ", item["columns"]!.AsArray().Select(c => $"{c!["name"]} {c["type"]}")));
            Assert.That(item["columns"]!.AsArray(), Is.Not.Empty);
        }
    }

    [Test]
    public void Table_DescribesOneTableAndRejectsAnUnknownName()
    {
        var name = BiggestTable();
        var data = CallData("ifc_table", new JsonObject { ["path"] = _path, ["table"] = name });
        Assert.That(data["total"]!.GetValue<int>(), Is.EqualTo(1));
        Assert.That(data["items"]!.AsArray()[0]!["table"]!.GetValue<string>(), Is.EqualTo(name));

        var failure = Call("ifc_table", new JsonObject { ["path"] = _path, ["table"] = "NoSuchTable" });
        Assert.That(failure["ok"]!.GetValue<bool>(), Is.False);
        Assert.That(failure["type"]!.GetValue<string>(), Is.EqualTo("ArgumentException"));
    }

    [Test]
    public void Sql_PagesAndReportsTheUnpagedTotal()
    {
        var name = BiggestTable();
        var data = CallData("ifc_sql", new JsonObject
        {
            ["path"] = _path,
            ["sql"] = $"SELECT * FROM \"{name}\"",
            ["take"] = 5,
        });

        Assert.That(data["count"]!.GetValue<int>(), Is.EqualTo(5));
        Assert.That(data["total"]!.GetValue<long>(), Is.GreaterThan(5));
        Assert.That(data["columns"]!.AsArray(), Is.Not.Empty);
        Assert.That(data["rows"]!.AsArray()[0]!.AsArray(), Has.Count.EqualTo(data["columns"]!.AsArray().Count));
    }

    [Test]
    public void Sql_AggregatesAcrossTables()
    {
        var data = CallData("ifc_sql", new JsonObject
        {
            ["path"] = _path,
            ["sql"] = "SELECT count(*) AS n FROM information_schema.tables WHERE table_schema = 'main'",
        });

        Assert.That(data["rows"]!.AsArray()[0]!.AsArray()[0]!.GetValue<long>(), Is.GreaterThan(0));
    }

    [Test]
    public void Sql_RefusesAnythingButOneReadOnlyStatement()
    {
        foreach (var sql in new[] { "DROP TABLE Entities", "SELECT 1; SELECT 2", "INSERT INTO Strings VALUES ('x')" })
        {
            var failure = Call("ifc_sql", new JsonObject { ["path"] = _path, ["sql"] = sql });
            Assert.That(failure["ok"]!.GetValue<bool>(), Is.False, $"'{sql}' was accepted.");
            Assert.That(failure["type"]!.GetValue<string>(), Is.EqualTo("ArgumentException"));
        }
    }

    /// <summary>Every string and enum in BIM Open Schema is interned, so the raw tables hold
    /// integer indexes; without the views a query can see no names at all.</summary>
    [Test]
    public void Views_ResolveInternedIndexesToText()
    {
        var entities = CallData("ifc_sql", new JsonObject
        {
            ["path"] = _path,
            ["sql"] = "SELECT Category, count(*) AS n FROM EntityText GROUP BY Category ORDER BY n DESC",
            ["take"] = 5,
        });

        var categories = entities["rows"]!.AsArray().Select(row => row![0]!.GetValue<string>()).ToList();
        Assert.That(categories, Has.All.Matches<string>(
            category => category.StartsWith("IFC", StringComparison.OrdinalIgnoreCase)));

        var parameters = CallData("ifc_sql", new JsonObject
        {
            ["path"] = _path,
            ["sql"] = "SELECT Name, ValueType, Value FROM ParameterText WHERE Name = 'Height' AND Value IS NOT NULL",
            ["take"] = 5,
        });

        Assert.That(parameters["total"]!.GetValue<long>(), Is.GreaterThan(0));

        var relations = CallData("ifc_sql", new JsonObject
        {
            ["path"] = _path,
            ["sql"] = "SELECT DISTINCT RelationType FROM RelationText",
            ["take"] = 20,
        });

        var kinds = relations["rows"]!.AsArray().Select(row => row![0]!.GetValue<string>()).ToList();
        Assert.That(kinds, Is.Not.Empty);
        Assert.That(kinds, Has.All.Matches<string>(kind => kind.Length > 1 && char.IsLetter(kind[0])));
    }

    [Test]
    public void SqlExport_WritesEveryRowToCsv()
    {
        var name = BiggestTable();
        var output = Path.Combine(_scratch, "rows.csv");
        var data = CallData("ifc_sql_export", new JsonObject
        {
            ["path"] = _path,
            ["sql"] = $"SELECT * FROM \"{name}\"",
            ["outputPath"] = output,
        });

        var rows = data["rows"]!.GetValue<long>();
        Assert.That(rows, Is.GreaterThan(0));
        Assert.That(data["bytes"]!.GetValue<long>(), Is.GreaterThan(0));
        Assert.That(File.ReadLines(output).Count(), Is.EqualTo(rows + 1), "CSV export writes a header row.");
    }

    /// <summary>Spatial containers survive the conversion. IfcToBosConverter.HiddenIfcNames is a
    /// geometry-instance visibility flag, not an entity filter, so a query can see site, building
    /// and storey — and the BOS storey count has to agree with the entity tools.</summary>
    [Test]
    public void Bos_KeepsSpatialContainers()
    {
        var data = CallData("ifc_sql", new JsonObject
        {
            ["path"] = _path,
            ["sql"] = "SELECT upper(Category) AS c, count(*) AS n FROM EntityText "
                      + "WHERE upper(Category) IN ('IFCSITE', 'IFCBUILDING', 'IFCBUILDINGSTOREY') GROUP BY c",
        });

        var counts = data["rows"]!.AsArray()
            .ToDictionary(row => row![0]!.GetValue<string>(), row => row![1]!.GetValue<long>());

        Assert.That(counts.Keys, Is.EquivalentTo(new[] { "IFCSITE", "IFCBUILDING", "IFCBUILDINGSTOREY" }));

        var storeys = CallData("ifc_type_counts", new JsonObject { ["path"] = _path, ["take"] = 500 })["items"]!
            .AsArray()
            .First(item => item!["type"]!.GetValue<string>() == "IFCBUILDINGSTOREY")!["count"]!
            .GetValue<int>();

        Assert.That(counts["IFCBUILDINGSTOREY"], Is.EqualTo(storeys));
    }

    private string BiggestTable()
    {
        var data = CallData("ifc_table", new JsonObject { ["path"] = _path, ["take"] = 100 });
        return data["items"]!.AsArray()
            .OrderByDescending(item => item!["rowCount"]!.GetValue<long>())
            .First()!["table"]!.GetValue<string>();
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
}
