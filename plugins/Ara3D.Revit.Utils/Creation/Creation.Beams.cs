using Ara3D.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.Revit.Utils;

public static partial class Creation
{
    /// <summary>
    /// Creates a structural framing (beam) instance along <paramref name="line"/> hosted on
    /// <paramref name="level"/>, activating <paramref name="type"/> first if needed. Requires an
    /// open transaction.
    /// </summary>
    public static FamilyInstance CreateBeam(this Document doc, Line3D line, FamilySymbol type, Level level)
    {
        type.EnsureActive();
        return doc.Create.NewFamilyInstance(line.ToRevitLine(), type, level, StructuralType.Beam);
    }
}
