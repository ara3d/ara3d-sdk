using System.Text.Json.Nodes;
using Ara3D.MCP;

namespace Ara3D.Ifc.Mcp.Tests;

/// <summary>Drives the geometry tools through tools/call against the FZK-Haus sample. Meshing is a
/// whole-model build, so the fixture shares one session cache — and therefore one meshed model —
/// across every test rather than rebuilding it each time.</summary>
[TestFixture]
public sealed class IfcGeometryToolTests
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
    public void ToolsList_CoversTheGeometrySurface()
    {
        var result = _mcp.HandlePost("""{"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}""");
        var names = JsonNode.Parse(result.JsonBody!)!["result"]!["tools"]!
            .AsArray()
            .Select(tool => tool!["name"]!.GetValue<string>())
            .ToList();

        Assert.That(names, Is.SupersetOf(new[]
        {
            "ifc_mesh", "ifc_bounds", "ifc_volume", "ifc_export_glb", "ifc_meshing_diagnostics",
        }));
    }

    [Test]
    public void Mesh_ReportsTrianglesAndPerElementGeometry()
    {
        var data = CallData("ifc_mesh", new JsonObject { ["path"] = _path, ["take"] = 5 });

        Assert.That(data["model"]!["success"]!.GetValue<bool>(), Is.True);
        Assert.That(data["model"]!["triangleCount"]!.GetValue<int>(), Is.GreaterThan(0));

        var elements = data["elements"]!;
        Assert.That(elements["total"]!.GetValue<int>(), Is.GreaterThan(5));
        Assert.That(elements["count"]!.GetValue<int>(), Is.EqualTo(5));

        var first = elements["items"]!.AsArray()[0]!;
        Assert.That(first["triangleCount"]!.GetValue<int>(), Is.GreaterThan(0));
        Assert.That(first["type"]!.GetValue<string>(), Does.StartWith("IFC"));
        Assert.That(first["bounds"]!["size"], Is.Not.Null);
    }

    [Test]
    public void Mesh_IdsFilterRestrictsToTheGivenElements()
    {
        var all = CallData("ifc_mesh", new JsonObject { ["path"] = _path, ["take"] = 3 });
        var id = all["elements"]!["items"]!.AsArray()[0]!["id"]!.GetValue<int>();

        var one = CallData("ifc_mesh", new JsonObject { ["path"] = _path, ["ids"] = id.ToString() });
        var items = one["elements"]!["items"]!.AsArray();

        Assert.That(one["elements"]!["total"]!.GetValue<int>(), Is.EqualTo(1));
        Assert.That(items[0]!["id"]!.GetValue<int>(), Is.EqualTo(id));
    }

    [Test]
    public void Bounds_ReportsAModelBoxThatIsNonEmpty()
    {
        var data = CallData("ifc_bounds", new JsonObject { ["path"] = _path, ["take"] = 1 });
        var size = data["model"]!["size"]!;

        var extent = size["x"]!.GetValue<double>() + size["y"]!.GetValue<double>() + size["z"]!.GetValue<double>();
        Assert.That(extent, Is.GreaterThan(0));

        var min = data["model"]!["min"]!["x"]!.GetValue<double>();
        var max = data["model"]!["max"]!["x"]!.GetValue<double>();
        Assert.That(max, Is.GreaterThanOrEqualTo(min));
        Assert.That(data["elements"]!["total"]!.GetValue<int>(), Is.GreaterThan(0));
    }

    [Test]
    public void Volume_ReportsModelTotalsAndPerElementQuantities()
    {
        var data = CallData("ifc_volume", new JsonObject { ["path"] = _path, ["take"] = 100 });

        Assert.That(Math.Abs(data["model"]!["signedVolume"]!.GetValue<double>()), Is.GreaterThan(0));

        var withVolume = data["elements"]!["items"]!.AsArray()
            .Count(item => Math.Abs(item!["signedVolume"]!.GetValue<double>()) > 0);
        Assert.That(withVolume, Is.GreaterThan(0), "At least one element should enclose a volume.");

        var withArea = data["elements"]!["items"]!.AsArray()
            .Count(item => item!["surfaceArea"]!.GetValue<double>() > 0);
        Assert.That(withArea, Is.GreaterThan(0));
    }

    [Test]
    public void ExportGlb_WritesABinaryGltfFile()
    {
        var output = Path.Combine(_scratch, "fzk.glb");
        var data = CallData("ifc_export_glb", new JsonObject { ["path"] = _path, ["outputPath"] = output });

        Assert.That(data["bytes"]!.GetValue<long>(), Is.GreaterThan(0));
        Assert.That(data["instanceCount"]!.GetValue<int>(), Is.GreaterThan(0));
        Assert.That(File.Exists(output), Is.True);

        var magic = new byte[4];
        using (var stream = File.OpenRead(output))
            Assert.That(stream.Read(magic, 0, 4), Is.EqualTo(4));
        Assert.That(magic, Is.EqualTo(new byte[] { (byte)'g', (byte)'l', (byte)'T', (byte)'F' }), "A .glb starts with the glTF magic.");
    }

    [Test]
    public void ExportGlb_IdsFilterWritesFewerInstances()
    {
        var all = CallData("ifc_export_glb", new JsonObject
        {
            ["path"] = _path,
            ["outputPath"] = Path.Combine(_scratch, "all.glb"),
        });
        var id = CallData("ifc_mesh", new JsonObject { ["path"] = _path, ["take"] = 1 })
            ["elements"]!["items"]!.AsArray()[0]!["id"]!.GetValue<int>();

        var one = CallData("ifc_export_glb", new JsonObject
        {
            ["path"] = _path,
            ["ids"] = id.ToString(),
            ["outputPath"] = Path.Combine(_scratch, "one.glb"),
        });

        Assert.That(one["instanceCount"]!.GetValue<int>(), Is.LessThan(all["instanceCount"]!.GetValue<int>()));
        Assert.That(one["instanceCount"]!.GetValue<int>(), Is.GreaterThan(0));
    }

    [Test]
    public void MeshingDiagnostics_ReportsTheMesherAndItsMessages()
    {
        var data = CallData("ifc_meshing_diagnostics", new JsonObject { ["path"] = _path, ["take"] = 10 });

        Assert.That(data["mesherName"]!.GetValue<string>(), Is.EqualTo("Approach1"));
        Assert.That(data["success"]!.GetValue<bool>(), Is.True);
        Assert.That(data["instanceCount"]!.GetValue<int>(), Is.GreaterThan(0));
        Assert.That(data["errors"]!.AsArray(), Is.Empty);
    }

    [Test]
    public void Mesh_RejectsNonIntegerIds()
    {
        var failure = Call("ifc_mesh", new JsonObject { ["path"] = _path, ["ids"] = "173,notanumber" });
        Assert.That(failure["ok"]!.GetValue<bool>(), Is.False);
        Assert.That(failure["type"]!.GetValue<string>(), Is.EqualTo("ArgumentException"));
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
