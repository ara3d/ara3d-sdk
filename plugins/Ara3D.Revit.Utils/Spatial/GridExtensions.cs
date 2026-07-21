using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>Grid queries.</summary>
public static class GridExtensions
{
    public static IReadOnlyList<Grid> GetGrids(this Document doc)
        => doc.GetElements<Grid>();

    /// <summary>The grid with an exact name match (e.g. "A", "1"), or null (P8).</summary>
    public static Grid? FindGridByName(this Document doc, string name)
        => doc.FindByName<Grid>(name);

    /// <summary>Throwing form of <see cref="FindGridByName"/> (§3.3 <c>Get…</c> contract).</summary>
    public static Grid GetGridByName(this Document doc, string name)
        => doc.GetByName<Grid>(name);
}
