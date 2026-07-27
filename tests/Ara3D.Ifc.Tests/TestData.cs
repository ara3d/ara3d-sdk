namespace Ara3D.Ifc.Tests;

public static class TestData
{
    public const string Folder = @"C:\Users\cdigg\git\nrc-ifc-llm\IFC-Test-Kit";
    public static readonly string DuplexIfc = Path.Combine(Folder, "duplex.ifc");
    public static readonly string AnalyticsCsvPath = Path.Combine(Folder, "analytics_dataset_with_levels.csv");

    /// <summary>
    /// Repo-relative artifacts folder, so generated IFC files survive a rebuild of bin/. Nested
    /// under the project name because artifacts/ is also the NuGet package output path.
    /// </summary>
    public static string OutputFolder
    {
        get
        {
            var r = Path.Combine(RepoRoot, "artifacts", "Ara3D.Ifc.Tests");
            Directory.CreateDirectory(r);
            return r;
        }
    }

    private static string RepoRoot
    {
        get
        {
            // .git is a file, not a directory, when the repo is checked out as a submodule.
            var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            while (dir != null && !Path.Exists(Path.Combine(dir.FullName, ".git")))
                dir = dir.Parent;
            return dir?.FullName ?? throw new DirectoryNotFoundException("No repository root above test directory");
        }
    }

    public static void RequireTestKit()
    {
        if (!File.Exists(DuplexIfc) || !File.Exists(AnalyticsCsvPath))
            Assert.Ignore($"Test kit not found at {Folder}");
    }
}
