using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.Revit.Utils;

public static class StructuralLoadQueries
{
    public static LoadUsage? FindLoadUsageByName(this Document doc, string name)
        => doc.FindByName<LoadUsage>(name);

    public static LoadCase? FindLoadCaseByName(this Document doc, string name)
        => doc.FindByName<LoadCase>(name);
}
