namespace Ara3D.Bowerbird.Tests;

public class TestSampleCommandsTests
{
    internal static readonly string[] SampleFolderNames =
    [
        "Args",
        "Clock",
        "Counter",
        "Echo",
        "Environment",
        "HelloWorld",
        "HttpEcho",
        "JsonFormat",
        "MultiFile",
        "TempFile",
    ];

    [Test]
    public static void AllSamplesCompile()
    {
        Assert.That(TestSampleCommandCache.Service.Catalog.Commands.Count, Is.EqualTo(SampleFolderNames.Length));
        foreach (var folderName in SampleFolderNames)
            Assert.That(TestSampleCommandCache.Get(folderName), Is.Not.Null);
    }

    [Test]
    public static void HelloWorld_WritesGreeting()
    {
        var output = TestSampleCommandCache.CaptureOutput("HelloWorld");
        Assert.That(output, Does.Contain("Hello World!"));
    }

    [Test]
    public static void Echo_WritesGreeting()
    {
        var output = TestSampleCommandCache.CaptureOutput("Echo");
        Assert.That(output, Does.Contain("Hello from Echo!"));
    }

    [Test]
    public static void Args_WithNullParameter()
    {
        var output = TestSampleCommandCache.CaptureOutput("Args");
        Assert.That(output, Does.Contain("Parameter: (null)"));
    }

    [Test]
    public static void Args_WithParameter()
    {
        var output = TestSampleCommandCache.CaptureOutput("Args", "test-value");
        Assert.That(output, Does.Contain("Parameter: test-value"));
    }

    [Test]
    public static void Counter_IncrementsOnRepeatedRuns()
    {
        var first = TestSampleCommandCache.CaptureOutput("Counter");
        var second = TestSampleCommandCache.CaptureOutput("Counter");
        var firstCount = ParseCounterOutput(first);
        var secondCount = ParseCounterOutput(second);
        Assert.That(secondCount, Is.EqualTo(firstCount + 1));
    }

    static int ParseCounterOutput(string output)
    {
        var match = System.Text.RegularExpressions.Regex.Match(output, @"(\d+) time\(s\)");
        Assert.That(match.Success, Is.True, output);
        return int.Parse(match.Groups[1].Value);
    }

    [Test]
    public static void Environment_PrintsRuntimeInfo()
    {
        var output = TestSampleCommandCache.CaptureOutput("Environment");
        Assert.That(output, Does.Contain("OS:"));
        Assert.That(output, Does.Contain("Framework:"));
        Assert.That(output, Does.Contain("Working directory:"));
    }

    [Test]
    public static void MultiFile_UsesHelperClass()
    {
        var output = TestSampleCommandCache.CaptureOutput("MultiFile");
        Assert.That(output, Does.Contain("multi-file sample"));
        Assert.That(output, Does.Contain("Bowerbird"));
    }

    [Test]
    public static void Clock_PrintsTimestamps()
    {
        var output = TestSampleCommandCache.CaptureOutput("Clock");
        Assert.That(output, Does.Contain("ISO:"));
        Assert.That(output, Does.Contain("Local:"));
    }

    [Test]
    public static void TempFile_WritesAndReadsBack()
    {
        var output = TestSampleCommandCache.CaptureOutput("TempFile");
        Assert.That(output, Does.Contain("Path:"));
        Assert.That(output, Does.Contain("Content: Bowerbird temp file sample"));
    }

    [Test]
    public static void JsonFormat_SerializesPayload()
    {
        var output = TestSampleCommandCache.CaptureOutput("JsonFormat");
        Assert.That(output, Does.Contain("\"Name\": \"Bowerbird\""));
        Assert.That(output, Does.Contain("\"Count\": 42"));
        Assert.That(output, Does.Contain("compile"));
    }

    [Test]
    [Category("Slow")]
    [NonParallelizable]
    public static async Task HttpEcho_ServesResponse()
    {
        var output = TestSampleCommandCache.CaptureOutput("HttpEcho");
        Assert.That(output, Does.Contain("HttpEcho listening at http://127.0.0.1:"));

        var uriStart = output.IndexOf("http://127.0.0.1:", StringComparison.Ordinal);
        Assert.That(uriStart, Is.GreaterThanOrEqualTo(0));
        var uriLine = output[uriStart..].Split('\n', '\r')[0].Trim();
        Assert.That(uriLine, Does.StartWith("http://127.0.0.1:8765"));

        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var response = await client.GetStringAsync(uriLine);
        Assert.That(response, Does.Contain("Hello from HttpEcho"));
    }
}
