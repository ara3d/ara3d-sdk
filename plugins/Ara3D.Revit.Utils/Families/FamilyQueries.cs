using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Family instance/symbol queries (pure, P1). Category-scoped instance access
/// reuses C1's quick-filter collectors (P2); symbol lookups add the
/// element-type filter C1 doesn't expose in category-scoped form.
/// </summary>
public static class FamilyQueries
{
    public static IReadOnlyList<FamilyInstance> GetFamilyInstances(this Document doc, BuiltInCategory category)
        => doc.GetElements<FamilyInstance>(category);

    public static IReadOnlyList<FamilySymbol> GetFamilySymbols(this Document doc, BuiltInCategory category)
        => doc.GetElementTypes<FamilySymbol>(category);

    /// <summary>Exact family-name + type-name lookup; null when no symbol matches (P8).</summary>
    public static FamilySymbol? FindFamilySymbol(this Document doc, string familyName, string typeName)
        => doc.GetElementTypes<FamilySymbol>()
            .FirstOrDefault(s => s.FamilyName == familyName && s.Name == typeName);

    /// <summary>Throwing form of <see cref="FindFamilySymbol"/> (§3.3 <c>Get…</c> contract).</summary>
    public static FamilySymbol GetFamilySymbol(this Document doc, string familyName, string typeName)
        => doc.FindFamilySymbol(familyName, typeName)
           ?? throw new ArgumentException($"No family symbol '{familyName}: {typeName}' in the document.", nameof(typeName));
}
