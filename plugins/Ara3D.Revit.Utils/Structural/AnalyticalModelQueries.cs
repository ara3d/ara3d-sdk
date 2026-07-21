using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.Revit.Utils;

public static class AnalyticalModelQueries
{
    /// <summary>
    /// The analytical element id associated with <paramref name="element"/>, or null if the
    /// document has no analytical-to-physical association manager, or no association exists.
    /// </summary>
    public static ElementId? TryGetAnalyticalElementId(this Element element)
        => AnalyticalToPhysicalAssociationManager
            .GetAnalyticalToPhysicalAssociationManager(element.Document)
            ?.GetAssociatedElementId(element.Id);
}
