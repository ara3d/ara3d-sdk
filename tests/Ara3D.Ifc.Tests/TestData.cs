namespace Ara3D.Ifc.Tests;

public static class TestData
{
    public const string Folder = @"C:\Users\cdigg\git\nrc-ifc-llm\IFC-Test-Kit";
    public static readonly string DuplexIfc = Path.Combine(Folder, "duplex.ifc");
    public static readonly string AnalyticsCsvPath = Path.Combine(Folder, "analytics_dataset_with_levels.csv");

    public static string OutputFolder
    {
        get
        {
            var r = Path.Combine(TestContext.CurrentContext.WorkDirectory, "output");
            Directory.CreateDirectory(r);
            return r;
        }
    }

    public static void RequireTestKit()
    {
        if (!File.Exists(DuplexIfc) || !File.Exists(AnalyticsCsvPath))
            Assert.Ignore($"Test kit not found at {Folder}");
    }
}
