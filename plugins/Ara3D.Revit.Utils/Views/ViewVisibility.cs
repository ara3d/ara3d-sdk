using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Pure element/view visibility predicates (P1) — no transaction, no mutation. Combine the
/// exact per-element check with the cheaper category-only and phase-aware checks as needed.
/// </summary>
public static class ViewVisibility
{
    /// <summary>True if this exact element instance is drawn in <paramref name="view"/> (view-scoped collector — exact, but not free).</summary>
    public static bool IsVisible(this Element element, View view)
        => new FilteredElementCollector(view.Document, view.Id)
            .WhereElementIsNotElementType()
            .WherePasses(new ElementIdSetFilter(new List<ElementId> { element.Id }))
            .Any();

    /// <summary>
    /// True if the element's category is non-null, non-analytical, and not hidden by V/G in
    /// <paramref name="view"/>. Cheaper than <see cref="IsVisible"/> but coarser — a category
    /// check, not a per-element one.
    /// </summary>
    public static bool IsCategoryVisibleInView(this Element element, View view)
    {
        var category = element.Category;
        if (category is null || category.CategoryType == CategoryType.AnalyticalModel)
            return false;
        return !view.CanCategoryBeHidden(category.Id) || !view.GetCategoryHidden(category.Id);
    }

    /// <summary>The view's phase (VIEW_PHASE), or null if the view doesn't carry one.</summary>
    public static Phase? GetViewPhase(this View view)
    {
        if (!view.TryGetParameter(BuiltInParameter.VIEW_PHASE, out var parameter)
            || parameter.StorageType != StorageType.ElementId)
            return null;
        var phaseId = parameter.AsElementId();
        return phaseId == ElementId.InvalidElementId ? null : view.Document.GetElement(phaseId) as Phase;
    }

    /// <summary>
    /// True if the element exists at <paramref name="viewPhase"/>: created on or before it and,
    /// if demolished, demolished after it. Elements that don't support phasing, or a null
    /// phase, are always considered present.
    /// </summary>
    public static bool IsInPhaseRange(this Element element, Phase? viewPhase)
        => viewPhase is null || !element.ArePhasesModifiable() || element.ExistsInPhase(viewPhase);
}
