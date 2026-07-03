using Ara3D.Bowerbird;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.Tests;

public class CommandCatalogTests
{
    [Test]
    public static void ScansTestSamples()
    {
        var catalog = new CommandCatalog(TestPaths.TestSamplesCommands);
        Assert.That(catalog.Commands.Count, Is.EqualTo(10));
        Assert.That(catalog.Commands.Select(c => c.DisplayName), Does.Contain("Echo"));
        Assert.That(catalog.Commands.Select(c => c.FolderName), Does.Contain("HelloWorld"));
        Assert.That(catalog.Commands.Select(c => c.FolderName), Does.Contain("HttpEcho"));
        Assert.That(catalog.Commands, Is.Ordered.By("DisplayName"));
    }

    [Test]
    public static void ResolvesByDisplayNameOrFolder()
    {
        var catalog = new CommandCatalog(TestPaths.TestSamplesCommands);
        Assert.That(catalog.ResolveByName("Echo")?.FolderName, Is.EqualTo("Echo"));
        Assert.That(catalog.ResolveByName("Counter")?.DisplayName, Is.EqualTo("Counter"));
    }

    [Test]
    public static void Scan_FindsValidCommands_SkipsInvalidFolders()
    {
        var root = TestCommandsRoot.Create(r =>
        {
            TestCommandsRoot.WriteCommand(r, "Alpha", "Alpha Command", "Ns.Alpha", "class Alpha {}");
            TestCommandsRoot.WriteCommand(r, "Beta", "Beta Command", "Ns.Beta", "class Beta {}");
            r.RelativeFolder("NoManifest").Create();
            r.RelativeFolder("BadName").Create();
            r.RelativeFolder("BadName").RelativeFile("Wrong.manifest.json").WriteAllText("""
                { "displayName": "X", "typeName": "Ns.X" }
                """);
        });

        var catalog = new CommandCatalog(root, Logger.Console);

        Assert.That(catalog.Commands.Count, Is.EqualTo(2));
        Assert.That(catalog.Commands.Select(c => c.DisplayName), Is.EqualTo(new[] { "Alpha Command", "Beta Command" }));
        Assert.That(catalog.Commands[0].SourceFiles.Count, Is.EqualTo(1));
    }
}
