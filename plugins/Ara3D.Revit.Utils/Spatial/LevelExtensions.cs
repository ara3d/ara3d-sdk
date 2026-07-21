using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Level queries and the generic element-to-level lookup. Elevations are Revit internal
/// units (feet, P7).
/// </summary>
public static class LevelExtensions
{
    public static IReadOnlyList<Level> GetLevels(this Document doc)
        => doc.GetElements<Level>();

    /// <summary>All levels ordered by elevation, lowest first.</summary>
    public static IReadOnlyList<Level> GetLevelsByElevation(this Document doc)
        => doc.GetLevels().OrderBy(l => l.Elevation).ToList();

    /// <summary>The element's associated level; throws if the element has none (§3.3 <c>Get…</c>).</summary>
    public static Level GetLevel(this Element element)
        => element.TryGetLevel(out var level)
            ? level
            : throw new InvalidOperationException($"Element {element.Id.ToLong()} has no associated level.");

    /// <summary>Non-throwing form of <see cref="GetLevel"/> — <c>false</c> when the element has no level (P8).</summary>
    public static bool TryGetLevel(this Element element, [NotNullWhen(true)] out Level? level)
    {
        level = element.LevelId.IsValid() ? element.Document.GetElement(element.LevelId) as Level : null;
        return level is not null;
    }

    /// <summary>The closest level above this one, or null when this is the top level (P8).</summary>
    public static Level? FindLevelAbove(this Level level)
        => level.Document.GetLevels()
            .Where(l => l.Elevation > level.Elevation)
            .OrderBy(l => l.Elevation)
            .FirstOrDefault();

    /// <summary>The closest level below this one, or null when this is the bottom level (P8).</summary>
    public static Level? FindLevelBelow(this Level level)
        => level.Document.GetLevels()
            .Where(l => l.Elevation < level.Elevation)
            .OrderByDescending(l => l.Elevation)
            .FirstOrDefault();

    /// <summary>The level whose elevation (feet) is closest to <paramref name="elevation"/>. Throws if the document has no levels.</summary>
    public static Level GetNearestLevel(this Document doc, double elevation)
        => doc.GetLevels()
               .OrderBy(l => Math.Abs(l.Elevation - elevation))
               .FirstOrDefault()
           ?? throw new InvalidOperationException("Document has no levels.");
}
