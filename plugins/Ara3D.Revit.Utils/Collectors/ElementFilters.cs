using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Composable slow-filter building blocks (P5). Quick filters (class/category/level) live on
/// the collector in <see cref="ElementCollectors"/>; these add parameter-value predicates the
/// quick filters can't express, kept as small named pieces so callers compose them instead of
/// re-deriving the <c>ParameterValueProvider</c>/<c>FilterRule</c> ceremony each time.
/// </summary>
public static class ElementFilters
{
    /// <summary>A slow filter matching elements whose string-valued <paramref name="parameter"/> equals <paramref name="value"/>.</summary>
    public static ElementFilter CreateParameterEqualsFilter(BuiltInParameter parameter, string value)
        => new ElementParameterFilter(
            new FilterStringRule(new ParameterValueProvider(new ElementId(parameter)), new FilterStringEquals(), value));

    /// <summary>Applies <see cref="CreateParameterEqualsFilter"/> to a collector (fluent).</summary>
    public static FilteredElementCollector WhereParameterEquals(this FilteredElementCollector collector,
        BuiltInParameter parameter, string value)
        => collector.WherePasses(CreateParameterEqualsFilter(parameter, value));

    /// <summary>Instances of <paramref name="category"/> and type <typeparamref name="T"/> whose string parameter equals <paramref name="value"/>.</summary>
    public static IReadOnlyList<T> GetElementsWithParameter<T>(this Document doc, BuiltInCategory category,
        BuiltInParameter parameter, string value) where T : Element
        => doc.CollectElements()
            .OfCategory(category)
            .OfClass(typeof(T))
            .WhereElementIsNotElementType()
            .WhereParameterEquals(parameter, value)
            .Cast<T>()
            .ToList();
}
