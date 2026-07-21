using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Dependent and hosted element lookups (P1: pure queries). Id resolution reuses C1's
/// <see cref="ElementIdQueries.GetElements(Document, IReadOnlyList{ElementId})"/> (P2).
/// </summary>
public static class DependentElementQueries
{
    /// <summary>
    /// Elements dependent on <paramref name="e"/> (optionally narrowed by <paramref name="filter"/>).
    /// Named apart from Revit's own <c>Element.GetDependentElements(ElementFilter)</c> — an
    /// extension sharing that name+signature would be silently shadowed (decision 017); the
    /// zero-or-more, never-throw contract also matches §3.3 <c>Find…</c> better than <c>Get…</c>.
    /// </summary>
    public static IReadOnlyList<Element> FindDependentElements(this Element e, ElementFilter? filter = null)
        => e.Document.GetElements(e.GetDependentElements(filter).ToList());

    /// <summary>Elements hosted by <paramref name="host"/> (inserts: openings, fixtures, etc.).</summary>
    public static IReadOnlyList<Element> GetHostedElements(this HostObject host)
        => host.Document.GetElements(host.FindInserts(true, false, true, true).ToList());
}
