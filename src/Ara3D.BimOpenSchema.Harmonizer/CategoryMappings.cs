using System.Collections.Generic;

namespace Ara3D.BimOpenSchema.Harmonizer;

/// <summary>
/// Maps source-specific category vocabularies (Revit category names, IFC class names)
/// to a shared canonical category name.
/// </summary>
public static class CategoryMappings
{
    /// <summary>
    /// Canonical category name, the IFC class names that map to it, and the Revit category names that map to it.
    /// </summary>
    public record CategoryMapping(string Canonical, string[] IfcClassNames, string[] RevitCategoryNames);

    public static readonly CategoryMapping[] All =
    [
        new("Wall", ["IFCWALL", "IFCWALLSTANDARDCASE"], ["Walls"]),
        new("CurtainWall", ["IFCCURTAINWALL"], ["Curtain Systems"]),
        new("Door", ["IFCDOOR"], ["Doors"]),
        new("Window", ["IFCWINDOW"], ["Windows"]),
        new("Floor", ["IFCSLAB"], ["Floors"]),
        // NOTE: IFCCOVERING also represents floor and wall finishes; ceiling is the most common export.
        new("Ceiling", ["IFCCOVERING", "IFCCEILING"], ["Ceilings"]),
        new("Column", ["IFCCOLUMN"], ["Columns", "Structural Columns"]),
        new("Beam", ["IFCBEAM"], ["Structural Framing"]),
        new("Stair", ["IFCSTAIR", "IFCSTAIRFLIGHT"], ["Stairs"]),
        new("Railing", ["IFCRAILING"], ["Railings"]),
        new("Roof", ["IFCROOF"], ["Roofs"]),
        new("Space", ["IFCSPACE"], ["Rooms", "Spaces"]),
        new("Level", ["IFCBUILDINGSTOREY"], ["Levels"]),
        new("Site", ["IFCSITE"], ["Site", "Topography"]),
        new("Furniture", ["IFCFURNISHINGELEMENT", "IFCFURNITURE"], ["Furniture", "Furniture Systems"]),
        new("PlumbingFixture", ["IFCSANITARYTERMINAL", "IFCFLOWTERMINAL"], ["Plumbing Fixtures"]),
        new("LightingFixture", ["IFCLIGHTFIXTURE"], ["Lighting Fixtures"]),
    ];

    /// <summary>
    /// Returns a lookup from source category name to canonical category name for the given source.
    /// </summary>
    public static Dictionary<string, string> ForSource(SourceKind source)
    {
        var r = new Dictionary<string, string>();
        foreach (var m in All)
        {
            var names = source == SourceKind.Ifc ? m.IfcClassNames : m.RevitCategoryNames;
            foreach (var n in names)
                r[n] = m.Canonical;
        }
        return r;
    }
}
