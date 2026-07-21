using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Read-only snapshot of an element's identity and naming — for logging, display, and
/// MCP-style discovery. Values are copied at call time, not a live view.
/// </summary>
public readonly record struct ElementInfo(
    long Id,
    string UniqueId,
    string Name,
    string? CategoryName,
    string? TypeName);

public static class ElementInfoExtensions
{
    /// <summary>Best-effort summary — category and type are omitted (not thrown) when absent.</summary>
    public static ElementInfo GetElementInfo(this Element e)
        => new(
            e.Id.ToLong(),
            e.GetUniqueId(),
            e.Name,
            e.Category?.Name,
            e.TryGetElementType(out var type) ? type.Name : null);
}
