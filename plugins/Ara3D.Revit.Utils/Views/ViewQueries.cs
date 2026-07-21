using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Views and sheets as plain queries — filtering, lookup, inventory. No mutation, no
/// transaction (P1); reuses C1 collectors (P2).
/// </summary>
public static class ViewQueries
{
    /// <summary>Every non-template view in the document (includes sheets and schedules).</summary>
    public static IReadOnlyList<View> GetViews(this Document doc)
        => doc.GetElements<View>().Where(v => !v.IsTemplate).ToList();

    /// <summary>Non-template 3D views.</summary>
    public static IReadOnlyList<View3D> Get3DViews(this Document doc)
        => doc.GetElements<View3D>().Where(v => !v.IsTemplate).ToList();

    /// <summary>Non-template views of a given <see cref="ViewType"/> (plan, section, schedule, …).</summary>
    public static IReadOnlyList<View> GetViewsOfType(this Document doc, ViewType viewType)
        => doc.GetViews().Where(v => v.ViewType == viewType).ToList();

    /// <summary>The first non-template view with an exact name match, or null (P8).</summary>
    public static View? FindViewByName(this Document doc, string name)
        => doc.GetViews().FirstOrDefault(v => v.Name == name);

    /// <summary>Throwing form of <see cref="FindViewByName"/> (§3.3 <c>Get…</c> contract).</summary>
    public static View GetViewByName(this Document doc, string name)
        => doc.FindViewByName(name)
           ?? throw new ArgumentException($"No view named '{name}' in the document.", nameof(name));

    /// <summary>The view named "{3D}" if present, else any non-template 3D view. Throws if the document has none.</summary>
    public static View3D GetDefault3DView(this Document doc)
        => doc.Get3DViews().FirstOrDefault(v => v.Name == "{3D}")
           ?? doc.Get3DViews().FirstOrDefault()
           ?? throw new InvalidOperationException("Document has no 3D view.");

    public static IReadOnlyList<ViewSheet> GetSheets(this Document doc)
        => doc.GetElements<ViewSheet>();

    /// <summary>Title block family symbols loaded in the document.</summary>
    public static IReadOnlyList<FamilySymbol> GetTitleBlockTypes(this Document doc)
        => doc.GetElementTypes<FamilySymbol>(BuiltInCategory.OST_TitleBlocks);

    /// <summary>
    /// Views that can host a viewport (excludes sheets, schedules and templates) and are not
    /// already placed on a sheet.
    /// </summary>
    public static IReadOnlyList<View> GetPlaceableViews(this Document doc)
        => doc.GetViews()
            .Where(v => v is not ViewSheet && v is not ViewSchedule
                        && v.GetPlacementOnSheetStatus() == ViewPlacementOnSheetStatus.NotPlaced)
            .ToList();

    /// <summary>The 3D/plan/section views in which every instance of <paramref name="element"/> is visible.</summary>
    public static IReadOnlyList<View> FindViewsShowing(this Element element)
        => element.Document.GetViews()
            .Where(v => v is View3D or ViewPlan or ViewSection)
            .Where(element.IsVisible)
            .ToList();
}
