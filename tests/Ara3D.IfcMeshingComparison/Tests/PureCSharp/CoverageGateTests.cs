using System.Reflection;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Tests.Support;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
[Category("IfcMesherCorrectness")]
public sealed class CoverageGateTests
{
    [Test]
    public void Coverage_DispatcherArmsAreInBacklog()
    {
        var known = GeometryCreationBacklog.KnownItems
            .Select(i => i.EntityName)
            .ToHashSet(StringComparer.Ordinal);
        var missing = GeometryDispatcher.DispatchedEntityNames
            .Where(n => !known.Contains(n))
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();
        Assert.That(missing, Is.Empty,
            "Dispatcher arms missing from GeometryCreationBacklog.KnownItems: " + string.Join(", ", missing));
    }

    [Test]
    public void Coverage_SupportedItemsHaveLinkedTestsOrExemption()
    {
        var covered = DiscoverGeometryCoverageEntities();
        var exemptions = GeometryCreationBacklog.CoverageExemptions
            .ToHashSet(StringComparer.Ordinal);
        var missing = GeometryCreationBacklog.KnownItems
            .Where(i => i.Support == GeometryCreationSupport.Supported)
            .Where(i => !exemptions.Contains(i.EntityName))
            .Where(i => !covered.Contains(i.EntityName))
            .Select(i => i.EntityName)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();
        Assert.That(missing, Is.Empty,
            "Supported backlog items lack [GeometryCoverage] and are not exempt: " + string.Join(", ", missing));
    }

    [Test]
    public void Coverage_GeometryCoverageAttributesAreKnownEntities()
    {
        var known = GeometryCreationBacklog.KnownItems
            .Select(i => i.EntityName)
            .ToHashSet(StringComparer.Ordinal);
        var unknown = DiscoverGeometryCoverageEntities()
            .Where(n => !known.Contains(n))
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();
        Assert.That(unknown, Is.Empty,
            "[GeometryCoverage] names not in backlog: " + string.Join(", ", unknown));
    }

    static HashSet<string> DiscoverGeometryCoverageEntities()
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        var asm = typeof(CoverageGateTests).Assembly;
        foreach (var type in asm.GetTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static))
            {
                foreach (var attr in method.GetCustomAttributes<GeometryCoverageAttribute>(inherit: false))
                    result.Add(attr.EntityName);
            }
        }
        return result;
    }
}
