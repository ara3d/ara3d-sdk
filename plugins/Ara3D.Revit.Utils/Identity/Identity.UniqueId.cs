using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static partial class Identity
{
    public static string GetUniqueId(this Element e)
        => e.UniqueId;

    /// <summary>
    /// Looks up an element by its stable <see cref="Element.UniqueId"/>. Returns
    /// <see langword="null"/> if no element with that id exists in the document (P8) —
    /// this passes through <see cref="Document.GetElement(string)"/>'s native behavior.
    /// Named <c>GetElementByUniqueId</c> rather than <c>GetElement</c>: <see cref="Document"/>
    /// already declares an instance <c>GetElement(string)</c> overload, which would silently
    /// shadow an identically-named/-signatured extension at every call site.
    /// </summary>
    public static Element? GetElementByUniqueId(this Document doc, string uniqueId)
        => doc.GetElement(uniqueId);
}
