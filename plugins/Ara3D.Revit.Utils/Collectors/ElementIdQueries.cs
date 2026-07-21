using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Element-id ⇄ element resolution. <see cref="GetRequiredElement"/> and
/// <see cref="TryGetElement"/> are named apart from Revit's own
/// <c>Document.GetElement(ElementId)</c> (which silently returns null on a miss) so the
/// throw/no-throw contract (P8) is explicit and actually reachable at the call site —
/// an extension method sharing that exact signature would be shadowed and never called.
/// </summary>
public static class ElementIdQueries
{
    public static IReadOnlyList<Element> GetElements(this Document doc, IReadOnlyList<ElementId> ids)
        => ids.Select(doc.GetRequiredElement).ToList();

    /// <summary>Typed form of <see cref="GetElements"/>; throws if any id is missing or not a <typeparamref name="T"/>.</summary>
    public static IReadOnlyList<T> GetElements<T>(this Document doc, IReadOnlyList<ElementId> ids) where T : Element
        => ids.Select(doc.GetRequiredElement<T>).ToList();

    /// <summary>The elements' ids, in order.</summary>
    public static IReadOnlyList<ElementId> ToElementIds<T>(this IReadOnlyList<T> elements) where T : Element
        => elements.Select(e => e.Id).ToList();

    public static IReadOnlyDictionary<long, T> ToIdMap<T>(this IReadOnlyList<T> elements) where T : Element
        => elements.ToDictionary(e => e.Id.ToLong(), e => e);

    public static bool TryGetElement(this Document doc, ElementId id, [NotNullWhen(true)] out Element? element)
    {
        element = doc.GetElement(id);
        return element is not null;
    }

    public static Element GetRequiredElement(this Document doc, ElementId id)
        => doc.TryGetElement(id, out var element)
            ? element
            : throw new ArgumentException($"No element exists with id {id.ToLong()} in the document.", nameof(id));

    /// <summary>Typed form of <see cref="GetRequiredElement"/>; also throws when the element is not a <typeparamref name="T"/>.</summary>
    public static T GetRequiredElement<T>(this Document doc, ElementId id) where T : Element
        => doc.GetRequiredElement(id) as T
           ?? throw new ArgumentException($"Element {id.ToLong()} is not a {typeof(T).Name}.", nameof(id));
}
