using Ara3D.Collections;
using Ara3D.Geometry;

namespace Ara3D.SDK.NuGet.Tests;

public static class NuGetPackageTests
{
    static string RepoRoot
        => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

    static string ArtifactsFolder
        => Path.Combine(RepoRoot, "artifacts");

    [Test, Category("Slow")]
    public static void RequiredPackages_Exist_In_Artifacts()
    {
        Assert.That(Directory.Exists(ArtifactsFolder), Is.True, $"Missing artifacts folder: {ArtifactsFolder}");

        foreach (var packageId in new[] { "Ara3D.SDK", "Ara3D.SDK.Core", "Ara3D.Collections" })
        {
            var matches = Directory.GetFiles(ArtifactsFolder, $"{packageId}.*.nupkg");
            Assert.That(matches, Is.Not.Empty, $"Missing {packageId} package in {ArtifactsFolder}");
        }
    }

    [Test, Category("Slow")]
    public static void MetaPackage_Provides_Geometry_Types()
    {
        Assert.That(typeof(Vector3).Assembly.GetName().Name, Is.EqualTo("Ara3D.Geometry"));
        Assert.That(new Vector3(1, 2, 3), Is.Not.EqualTo(default));
    }

    [Test, Category("Slow")]
    public static void Standalone_Collections_Package_Works()
    {
        IReadOnlyList<int> values = new[] { 1, 2, 3 };
        Assert.That(values.IsEmpty(), Is.False);
        Assert.That(values.Last(), Is.EqualTo(3));
    }
}
