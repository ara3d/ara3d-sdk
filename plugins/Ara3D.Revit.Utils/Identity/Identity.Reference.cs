using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static partial class Identity
{
    public static Reference GetReference(this Element e)
        => new(e);

    /// <summary>
    /// Stable string form of a <see cref="Reference"/>. Revit's own
    /// <see cref="Reference.ConvertToStableRepresentation"/> requires the owning
    /// <see cref="Document"/> (the representation encodes link context), so the document
    /// parameter is load-bearing here, not optional plumbing.
    /// </summary>
    public static string ToStableString(this Reference r, Document doc)
        => r.ConvertToStableRepresentation(doc);

    /// <summary>
    /// Resolves a reference back to the element it points at in <paramref name="doc"/>.
    /// Uses <see cref="Reference.ElementId"/> — the host-document element only; it does
    /// not follow references into linked documents (see <see cref="Reference.LinkedElementId"/>).
    /// </summary>
    public static Element? GetReferencedElement(this Reference r, Document doc)
        => doc.GetElement(r.ElementId);
}
