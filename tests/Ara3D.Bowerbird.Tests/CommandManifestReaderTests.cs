using Ara3D.Utils;

namespace Ara3D.Bowerbird.Tests;

public class CommandManifestReaderTests
{
    [Test]
    public static void ReadsHelloWorldManifest()
    {
        var folder = TestPaths.TestSamplesCommands.RelativeFolder("HelloWorld");
        Assert.That(CommandManifestReader.TryRead(folder, out var manifest, out var error), Is.True, error);
        Assert.That(manifest.DisplayName, Is.EqualTo("Hello World"));
        Assert.That(manifest.TypeName, Is.EqualTo("Ara3D.Bowerbird.TestSamples.HelloWorld.HelloWorldCommand"));
        Assert.That(manifest.Description, Is.EqualTo("Prints a greeting"));
    }

    [Test]
    public static void RejectsMissingManifest()
    {
        var folder = TestPaths.TestSamplesCommands.RelativeFolder("MissingCommand");
        Assert.That(CommandManifestReader.TryRead(folder, out _, out var error), Is.False);
        Assert.That(error, Does.Contain("Missing manifest"));
    }
}
