using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Generic, quick-filter-backed element collectors (P3). Every method materializes
/// once into an <see cref="IReadOnlyList{T}"/> — none hand back a live collector.
/// </summary>
public static class ElementCollectors
{
    /// <summary>Raw collector plumbing (§3.3 <c>Collect…</c>) — rarely public; prefer the typed queries below.</summary>
    public static FilteredElementCollector CollectElements(this Document doc)
        => new(doc);

    public static IReadOnlyList<T> GetElements<T>(this Document doc) where T : Element
        => doc.CollectElements().OfClass(typeof(T)).WhereElementIsNotElementType().Cast<T>().ToList();

    public static IReadOnlyList<T> GetElementTypes<T>(this Document doc) where T : ElementType
        => doc.CollectElements().OfClass(typeof(T)).WhereElementIsElementType().Cast<T>().ToList();

    public static IReadOnlyList<T> GetElementTypes<T>(this Document doc, BuiltInCategory category) where T : ElementType
        => doc.CollectElements().OfCategory(category).OfClass(typeof(T)).WhereElementIsElementType().Cast<T>().ToList();

    public static IReadOnlyList<Element> GetElements(this Document doc, BuiltInCategory category)
        => doc.CollectElements().OfCategory(category).WhereElementIsNotElementType().ToList();

    public static IReadOnlyList<T> GetElements<T>(this Document doc, BuiltInCategory category) where T : Element
        => doc.CollectElements().OfCategory(category).OfClass(typeof(T)).WhereElementIsNotElementType().Cast<T>().ToList();

    public static IReadOnlyList<Element> GetElementInstances(this Document doc)
        => doc.CollectElements().WhereElementIsNotElementType().ToList();

    public static IReadOnlyList<Element> GetElementTypes(this Document doc)
        => doc.CollectElements().WhereElementIsElementType().ToList();

    /// <summary>Raw collector scoped to the elements visible in <paramref name="view"/>.</summary>
    public static FilteredElementCollector CollectElements(this View view)
        => new(view.Document, view.Id);

    public static IReadOnlyList<T> GetElementsInView<T>(this View view) where T : Element
        => view.CollectElements().OfClass(typeof(T)).WhereElementIsNotElementType().Cast<T>().ToList();

    public static IReadOnlyList<Element> GetElementsInView(this View view, BuiltInCategory category)
        => view.CollectElements().OfCategory(category).WhereElementIsNotElementType().ToList();

    public static IReadOnlyList<T> GetElementsOfLevel<T>(this Level level) where T : Element
        => level.Document.CollectElements()
            .OfClass(typeof(T)).WherePasses(new ElementLevelFilter(level.Id)).WhereElementIsNotElementType()
            .Cast<T>().ToList();

    public static IReadOnlyList<Element> GetElementsOfLevel(this Level level, BuiltInCategory category)
        => level.Document.CollectElements()
            .OfCategory(category).WherePasses(new ElementLevelFilter(level.Id)).WhereElementIsNotElementType()
            .ToList();

    /// <summary>Instances passing an arbitrary <see cref="ElementFilter"/> (compose via <see cref="ElementFilters"/>).</summary>
    public static IReadOnlyList<T> GetElements<T>(this Document doc, ElementFilter filter) where T : Element
        => doc.CollectElements().OfClass(typeof(T)).WherePasses(filter).WhereElementIsNotElementType()
            .Cast<T>().ToList();

    /// <summary>Instance ids only — cheaper than materializing elements when only ids are needed.</summary>
    public static IReadOnlyList<ElementId> GetElementIds<T>(this Document doc) where T : Element
        => doc.CollectElements().OfClass(typeof(T)).WhereElementIsNotElementType().ToElementIds().ToList();
}
