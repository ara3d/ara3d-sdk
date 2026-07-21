using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class MarkExtensions
{
    /// <summary>
    /// Sets the "Mark" parameter sequentially across elements, starting at start. Requires an
    /// open transaction.
    /// </summary>
    public static void SetMarks(this IReadOnlyList<Element> elements, int start = 1)
    {
        for (var i = 0; i < elements.Count; i++)
            elements[i].SetParameter("Mark", (start + i).ToString());
    }
}
