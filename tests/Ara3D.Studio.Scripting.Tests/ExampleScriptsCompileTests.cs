using System.IO;
using System.Reflection;
using Ara3D.Utils;
using Ara3D.Utils.Roslyn;

namespace Ara3D.Studio.Scripting.Tests;

/// <summary>
/// Guards that the example scripts always compile through the same Roslyn path the running
/// host uses (Ara3D.Studio.Scripting.CompilerService), and — crucially — that they compile
/// with only the assemblies the host actually makes available.
///
/// The host does not hand Roslyn a curated reference list; it snapshots whatever assemblies
/// happen to be loaded (RoslynUtils.LoadedAssemblyLocations), after force-loading a fixed set
/// in ServiceManager.CreateCompilerService. Framework assemblies that are only JIT-loaded on
/// first use — System.ComponentModel.Primitives (for the [Category] attribute) and
/// System.Text.Json — are therefore absent from the snapshot unless force-loaded. That is the
/// failure mode this suite exists to catch: a script fails to compile with "CategoryAttribute
/// could not be found" and every other script in the folder dies with it.
///
/// This suite mirrors the host mechanism exactly: it loads the example project's full
/// reference closure, then compiles the raw sources against the set of currently-loaded
/// assemblies — no ref-pack facades, no native binaries, one version of each assembly.
/// </summary>
[TestFixture]
public class ExampleScriptsCompileTests
{
    // Framework assemblies the examples need but the runtime only loads lazily. These MUST be
    // force-loaded by the host — keep in sync with the framework ForceLoadType calls in
    // src/Ara3D.Studio/Ara3DStudio/ServiceManager.cs (CreateCompilerService).
    static readonly Type[] LazyFrameworkTypes =
    {
        typeof(System.ComponentModel.CategoryAttribute), // System.ComponentModel.Primitives
        typeof(System.Text.Json.Nodes.JsonArray),        // System.Text.Json
    };

    // Files Ara3D.Studio.Samples.csproj excludes from compilation.
    static readonly string[] ExcludedSources =
        { "Lakehouse", "SimulateSequence.cs", "SimulateSquence2.cs", "GeoJsonDemo2.cs" };

    const string SamplesAssembly = "Ara3D.Studio.Samples";

    [OneTimeSetUp]
    public void LoadReferenceClosure()
    {
        // Pull the whole dependency graph of the example assembly into the AppDomain (the WPF
        // host has it loaded ambiently), plus the lazily-loaded framework assemblies the host
        // force-loads, so the loaded-assembly snapshot below is complete and deterministic.
        LoadClosure(typeof(Ara3D.Studio.Samples.Cat).Assembly);
        foreach (var t in LazyFrameworkTypes)
            _ = t.Assembly;
    }

    static void LoadClosure(Assembly root)
    {
        var seen = new HashSet<string>();
        var queue = new Queue<Assembly>();
        queue.Enqueue(root);
        while (queue.Count > 0)
        {
            var a = queue.Dequeue();
            if (a.FullName == null || !seen.Add(a.FullName))
                continue;
            foreach (var r in a.GetReferencedAssemblies())
            {
                try { queue.Enqueue(Assembly.Load(r)); }
                catch { /* unmanaged / unresolvable satellite — irrelevant to the compile */ }
            }
        }
    }

    static DirectoryPath ExamplesDir()
    {
        var dir = new DirectoryPath(AppContext.BaseDirectory);
        for (var i = 0; i < 12 && dir != null; i++, dir = dir.GetParent())
        {
            var candidate = dir.RelativeFolder("examples", "Ara3D.Studio.Examples");
            if (candidate.Exists())
                return candidate;
        }
        throw new DirectoryNotFoundException(
            "Could not locate examples/Ara3D.Studio.Examples above " + AppContext.BaseDirectory);
    }

    static List<FilePath> ExampleSources()
        => ExamplesDir().GetAllFilesRecursively()
            .Where(f => f.HasExtension(".cs"))
            .Where(f => !f.Value.Contains(@"\obj\") && !f.Value.Contains(@"\bin\"))
            .Where(f => !ExcludedSources.Any(e => f.Value.Contains(e)))
            .ToList();

    // The reference set the host presents: every currently-loaded managed assembly (exactly
    // RoslynUtils.LoadedAssemblyLocations), minus the compiled example assembly itself
    // (referencing it while compiling its sources would duplicate every type — CS0121) and
    // minus any assemblies named in <drop>.
    static List<FilePath> References(params string[] drop)
        => AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location) && File.Exists(a.Location))
            .Select(a => a.Location)
            .Where(loc => !Path.GetFileNameWithoutExtension(loc).Equals(SamplesAssembly, StringComparison.OrdinalIgnoreCase))
            .Where(loc => !drop.Any(d => Path.GetFileNameWithoutExtension(loc).Equals(d, StringComparison.OrdinalIgnoreCase)))
            .GroupBy(loc => Path.GetFileName(loc), StringComparer.OrdinalIgnoreCase)
            .Select(g => new FilePath(g.First()))
            .ToList();

    static CompilerOutput Compile(IReadOnlyList<FilePath> refs)
    {
        var options = new CompilerOptions(refs, RoslynUtils.GenerateNewDllFileName(), true);
        var input = new CompilerInput(ExampleSources(), options, refs);
        return new Compilation(input, null, default).Output;
    }

    [Test]
    public void AllExampleScripts_CompileCleanly()
    {
        var output = Compile(References());
        Assert.That(output, Is.Not.Null, "compilation produced no output");
        Assert.That(output!.Success, Is.True,
            "Example scripts failed to compile:\n" + string.Join("\n", output.Errors.Take(40)));
    }

    // Necessity: each lazy assembly really is needed — dropping it breaks the compile. If one
    // stops being required, the matching host force-load can be removed too.
    [TestCaseSource(nameof(LazyFrameworkAssemblyNames))]
    public void DroppingLazyFrameworkAssembly_BreaksCompile(string assemblyName)
    {
        var output = Compile(References(assemblyName));
        Assert.That(output!.Success, Is.False,
            $"Expected '{assemblyName}' to be required by the example scripts, but they compiled without it. " +
            "If it is no longer needed, drop the matching ForceLoadType in ServiceManager.");
    }

    // Sufficiency: with every lazy assembly dropped, re-adding only the ones named by
    // LazyFrameworkTypes restores a clean compile — i.e. that list (and the matching host
    // force-loads) covers the gap. Remove an entry and this fails, exactly as the host would.
    [Test]
    public void ForceLoadedFrameworkTypes_CoverTheLazyGap()
    {
        var dropped = LazyFrameworkAssemblyNames().ToArray();
        var refs = References(dropped).ToList();
        refs.AddRange(LazyFrameworkTypes.Select(t => new FilePath(t.Assembly.Location)));

        var output = Compile(refs);
        Assert.That(output!.Success, Is.True,
            "LazyFrameworkTypes does not cover every lazily-loaded assembly the examples need:\n" +
            string.Join("\n", output.Errors.Take(40)));
    }

    static IEnumerable<string> LazyFrameworkAssemblyNames()
        => LazyFrameworkTypes.Select(t => t.Assembly.GetName().Name!).Distinct();
}
