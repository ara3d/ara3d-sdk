namespace Ara3D.DoorClearance.Tests;

public static class TestPaths
{
    public const string TestKitFolder = @"C:\Users\cdigg\git\nrc-ifc-llm\IFC-Test-Kit";
    public static readonly string DuplexIfc = Path.Combine(TestKitFolder, "duplex.ifc");

    public static string RulesJson
        => Path.Combine(TestContext.CurrentContext.TestDirectory, "rules", "door-clearance-rules.json");

    /// <summary>
    /// Repo-relative artifacts folder, so generated files survive a rebuild of bin/. Nested under
    /// the project name because artifacts/ is also the NuGet package output path.
    /// </summary>
    public static string OutputFolder
    {
        get
        {
            var r = Path.Combine(RepoRoot, "artifacts", "Ara3D.DoorClearance.Tests");
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
        if (!File.Exists(DuplexIfc))
            Assert.Ignore($"Test kit not found at {TestKitFolder}");
    }
}
