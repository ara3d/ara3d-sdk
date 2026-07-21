using Autodesk.Revit.DB;
using Color = Ara3D.Geometry.Color;

namespace Ara3D.Revit.Utils;

/// <summary>
/// View-scoped graphic overrides (color, transparency). All mutations here require an
/// already-open transaction (P1) — none is opened in this module.
/// </summary>
public static class GraphicOverrides
{
    /// <summary>Overrides projection- and cut-line and pattern colors for <paramref name="e"/> in <paramref name="view"/>. Requires an open transaction.</summary>
    public static void SetColorOverride(this View view, Element e, Color color)
    {
        var revitColor = color.ToRevitColor();
        var ogs = view.GetElementOverrides(e.Id);
        ogs.SetProjectionLineColor(revitColor);
        ogs.SetCutLineColor(revitColor);
        ogs.SetSurfaceForegroundPatternColor(revitColor);
        ogs.SetCutForegroundPatternColor(revitColor);
        view.SetElementOverrides(e.Id, ogs);
    }

    /// <summary>Sets surface transparency (0..100) for <paramref name="e"/> in <paramref name="view"/>. Requires an open transaction.</summary>
    public static void SetTransparency(this View view, Element e, int percent)
    {
        var ogs = view.GetElementOverrides(e.Id);
        ogs.SetSurfaceTransparency(percent);
        view.SetElementOverrides(e.Id, ogs);
    }

    /// <summary>Resets <paramref name="e"/> to no graphic overrides in <paramref name="view"/>. Requires an open transaction.</summary>
    public static void ClearOverrides(this View view, Element e)
        => view.SetElementOverrides(e.Id, new OverrideGraphicSettings());
}
