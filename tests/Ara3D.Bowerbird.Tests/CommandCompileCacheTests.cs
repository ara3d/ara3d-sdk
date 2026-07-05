using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.Tests;

[NonParallelizable]
public class CommandCompileCacheTests
{
    [Test]
    public static void Fingerprint_StableWhenUnchanged()
    {
        var descriptor = TestBowerbirdHost.Resolve(TestBowerbirdHost.Create(), "HelloWorld");
        var refs = new ReferenceResolver().ResolveFingerprintRefs(descriptor.Folder, null);
        var first = CommandCompileCacheKey.Compute(descriptor, refs);
        var second = CommandCompileCacheKey.Compute(descriptor, refs);
        Assert.That(second.Fingerprint, Is.EqualTo(first.Fingerprint));
    }

    [Test]
    public static void Fingerprint_ChangesWhenSourceEdited()
    {
        var root = TestCommandsRoot.Create(r =>
            TestCommandsRoot.WriteCommand(r, "FpTest", "Fp", "Ns.Fp", "// v1"));

        var descriptor = new CommandCatalog(root).FindByFolderName("FpTest");
        var refs = new ReferenceResolver().ResolveFingerprintRefs(descriptor.Folder, null);
        var first = CommandCompileCacheKey.Compute(descriptor, refs);

        descriptor.Folder.RelativeFile($"{descriptor.FolderName}.cs").WriteAllText("// v2");
        var second = CommandCompileCacheKey.Compute(descriptor, refs);

        Assert.That(second.Fingerprint, Is.Not.EqualTo(first.Fingerprint));
    }

    [Test]
    public static void CacheMiss_WhenCachedDllDeleted()
    {
        var root = TestCommandsRoot.Create(r =>
            TestCommandsRoot.WriteCommand(r, "CacheTest", "Cache", "Isolated.CacheCommand", """
                using Ara3D.Bowerbird;
                namespace Isolated;
                public class CacheCommand : NamedCommand { public override void Execute() { } }
                """));

        var options = new BowerbirdOptions("Bowerbird.Tests", root);
        var compiler = new CommandCompiler(Logger.Console);
        var descriptor = new CommandCatalog(root).FindByFolderName("CacheTest");

        var first = compiler.Compile(descriptor, options);
        Assert.That(first.Compilation?.Success, Is.True);
        Assert.That(first.FromCache, Is.False);

        var second = compiler.Compile(descriptor, options);
        Assert.That(second.FromCache, Is.True);
        Assert.That(second.OutputDll, Is.EqualTo(first.OutputDll));

        first.OutputDll.Delete();
        Assert.That(first.OutputDll.Exists(), Is.False);

        var third = compiler.Compile(descriptor, options);
        Assert.That(third.FromCache, Is.False);
        Assert.That(third.OutputDll.Exists(), Is.True);
    }

    [Test]
    public static void EnableCompileCacheFalse_ForcesRecompile()
    {
        var options = new BowerbirdOptions("Bowerbird.Tests", TestPaths.TestSamplesCommands)
        {
            EnableCompileCache = false,
        };
        var service = new BowerbirdService(options, Logger.Console);
        var descriptor = TestBowerbirdHost.Resolve(service, "HelloWorld");

        var first = service.RunCommand(descriptor);
        Assert.That(first.Success, Is.True, TestBowerbirdHost.FormatDiagnostics(first));
        Assert.That(first.FromCache, Is.False);

        var second = service.RunCommand(descriptor);
        Assert.That(second.Success, Is.True, TestBowerbirdHost.FormatDiagnostics(second));
        Assert.That(second.FromCache, Is.False);
        Assert.That(second.OutputDll, Is.Not.EqualTo(first.OutputDll));
    }
}
