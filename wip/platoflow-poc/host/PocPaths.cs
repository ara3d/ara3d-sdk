namespace PlatoFlow.Host;

/// <summary>Locates the PoC tree from wherever the exe happens to run. The marker is CONTRACTS.md
/// at the root of <c>ara3d-sdk/wip/platoflow-poc</c>, so <c>dotnet run</c>, a published exe and a test
/// runner all find the same <c>data/</c>.</summary>
public static class PocPaths
{
    public const string RootEnvVar = "PLATOFLOW_POC_ROOT";

    public static string FindRoot()
    {
        var fromEnv = Environment.GetEnvironmentVariable(RootEnvVar);
        if (!string.IsNullOrWhiteSpace(fromEnv) && File.Exists(Path.Combine(fromEnv, "CONTRACTS.md")))
            return Path.GetFullPath(fromEnv);

        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        for (var i = 0; i < 10 && dir != null; i++, dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "CONTRACTS.md")))
                return dir.FullName;

        throw new DirectoryNotFoundException(
            $"Could not find the PoC root (a folder containing CONTRACTS.md) above {AppContext.BaseDirectory}. "
            + $"Set {RootEnvVar} to ara3d-sdk/wip/platoflow-poc.");
    }

    /// <summary>Source data that lives outside this repo. Missing files are reported, never fatal.</summary>
    public static class Source
    {
        public const string DuplexIfc = @"C:\Users\cdigg\git\nrc-ifc-llm\IFC-Test-Kit\duplex.ifc";
        public const string CarbonCsv = @"C:\Users\cdigg\git\nrc-ifc-llm\IFC-Test-Kit\analytics_dataset_with_levels.csv";
        public const string RacBasicBos = @"C:\Users\cdigg\git\ara3d-webgl\examples\public\rac_basic_sample_project-2025.bos";
        public const string SnowdonIfc = @"C:\Users\cdigg\git\3d-format-shootout - Copy\data\misc\Snowdon-IFC\Snowdon Towers Sample Architectural.ifc";
        public const string SnowdonHvacIfc = @"C:\Users\cdigg\git\3d-format-shootout - Copy\data\misc\Snowdon-IFC\Snowdon Towers Sample HVAC.ifc";
        public const string SnowdonStructIfc = @"C:\Users\cdigg\git\3d-format-shootout - Copy\data\misc\Snowdon-IFC\Snowdon Towers Sample Structural.ifc";
    }
}
