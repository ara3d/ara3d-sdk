using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;

namespace Ara3D.Revit.Utils;

/// <summary>MEP space queries.</summary>
public static class SpaceExtensions
{
    public static IReadOnlyList<Space> GetSpaces(this Document doc)
        => doc.GetElements<Space>();
}
