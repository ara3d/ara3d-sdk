using Ara3D.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Family symbol load/activate/place. Every member here requires an already
/// open transaction (P1) — none opens one; Revit throws if there isn't one.
/// </summary>
public static class FamilyMutations
{
    /// <summary>
    /// Loads a symbol from an .rfa file and returns it; requires an open transaction. Throws if the load fails.
    /// Named apart from the native <c>Document.LoadFamilySymbol(string, string)</c> (returns <c>bool</c>) —
    /// an identical-signature extension is silently shadowed (decision 017).
    /// </summary>
    public static FamilySymbol LoadFamilySymbolFromFile(this Document doc, string rfaPath, string typeName)
        => doc.LoadFamilySymbol(rfaPath, typeName, out FamilySymbol symbol)
            ? symbol
            : throw new System.InvalidOperationException(
                $"Failed to load family symbol '{typeName}' from '{rfaPath}'.");

    /// <summary>
    /// Activates the symbol if not already active; requires an open transaction.
    /// Named apart from the native <c>FamilySymbol.Activate()</c>/<c>IsActive</c> —
    /// an identical-signature extension would be silently shadowed (decision 017).
    /// </summary>
    public static void EnsureActive(this FamilySymbol symbol)
    {
        if (!symbol.IsActive)
            symbol.Activate();
    }

    /// <summary>Places a new instance of <paramref name="symbol"/> at a point on a level; requires an open transaction and an already-active symbol.</summary>
    public static FamilyInstance PlaceInstance(this Document doc, FamilySymbol symbol, Point3D at, Level level)
        => doc.Create.NewFamilyInstance(at.ToXyz(), symbol, level, StructuralType.NonStructural);
}
