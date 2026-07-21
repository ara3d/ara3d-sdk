using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Type↔instance navigation (P1: pure queries). Resolution reuses C1's
/// <see cref="ElementIdQueries.GetRequiredElement"/>/<see cref="ElementIdQueries.TryGetElement"/>
/// and C6's <see cref="Identity.IsValid(ElementId)"/> — no re-querying (P2).
/// </summary>
public static class ElementTypeNavigation
{
    /// <summary>The instance's type. Throws if <paramref name="e"/> has none (§3.3 <c>Get…</c> contract).</summary>
    public static ElementType GetElementType(this Element e)
    {
        var typeId = e.GetTypeId();
        if (!typeId.IsValid())
            throw new InvalidOperationException($"Element {e.Id.ToLong()} has no associated type.");
        return e.Document.GetRequiredElement<ElementType>(typeId);
    }

    /// <summary>Non-throwing form of <see cref="GetElementType"/> — <c>false</c> when <paramref name="e"/> has no type (P8).</summary>
    public static bool TryGetElementType(this Element e, [NotNullWhen(true)] out ElementType? type)
    {
        var typeId = e.GetTypeId();
        if (typeId.IsValid() && e.Document.TryGetElement(typeId, out var element))
        {
            type = element as ElementType;
            return type is not null;
        }
        type = null;
        return false;
    }

    /// <summary>Every instance in the document whose type is <paramref name="type"/>.</summary>
    public static IReadOnlyList<Element> GetInstancesOfType(this ElementType type)
        => type.Document.FindElements<Element>(e => e.GetTypeId() == type.Id);
}
