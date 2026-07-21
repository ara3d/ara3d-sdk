using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Centralizes every ElementId/UniqueId/Reference version difference (P7). This is the
/// only place in the library that reads <see cref="ElementId.Value"/> or would read
/// <c>IntegerValue</c> — Revit 2025 targets the 2024+ 64-bit <c>Value</c> form.
/// </summary>
public static partial class Identity
{
    public static long ToLong(this ElementId id)
        => id.Value;

    public static ElementId ToElementId(long value)
        => new(value);

    public static bool IsValid(this ElementId id)
        => id is not null && id != ElementId.InvalidElementId;
}
