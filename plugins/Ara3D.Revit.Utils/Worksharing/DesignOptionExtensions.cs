using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class DesignOptionExtensions
{
    public static IReadOnlyList<DesignOption> GetDesignOptions(this Document doc)
        => doc.GetElements<DesignOption>();
}
