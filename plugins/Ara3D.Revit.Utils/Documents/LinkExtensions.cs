using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Link instance enumeration and linked-document access. The single home for
/// <see cref="GetLinkedDocuments"/> — previously duplicated in both WB and RS reference code (P2).
/// </summary>
public static class LinkExtensions
{
    public static IReadOnlyList<RevitLinkInstance> GetLinks(this Document doc)
        => doc.GetElements<RevitLinkInstance>();

    /// <summary>Loaded linked documents; a link whose target is unloaded is skipped, not an error.</summary>
    public static IReadOnlyList<Document> GetLinkedDocuments(this Document doc)
        => doc.GetLinks()
            .Select(link => link.GetLinkDocument())
            .Where(linkDoc => linkDoc != null)
            .ToArray();

    /// <summary>Every non-type element of the linked document. Throws if the link is unloaded.</summary>
    public static IReadOnlyList<Element> GetLinkedElements(this RevitLinkInstance link)
        => (link.GetLinkDocument() ?? throw new InvalidOperationException($"Link '{link.Name}' is not loaded."))
            .GetElementInstances();

    /// <summary>
    /// The link's placement transform (including any shared-coordinate offset) as an sdk
    /// row-major matrix via C4 — carries points from the linked document into this one.
    /// </summary>
    public static Matrix4x4 GetLinkTransform(this RevitLinkInstance link)
        => link.GetTotalTransform().ToMatrix4x4();
}
