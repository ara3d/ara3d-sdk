using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Sheet and viewport creation. Every member requires an open transaction; none is opened
/// here (P1) — callers use <c>doc.Run("name", d => ...)</c> from the transactions module (C3).
/// </summary>
public static class SheetCreation
{
    /// <summary>requires an open transaction</summary>
    public static ViewSheet CreateSheet(this Document doc, FamilySymbol titleBlockType, string? name = null, string? number = null)
    {
        var sheet = ViewSheet.Create(doc, titleBlockType.Id);
        if (name is not null)
            sheet.Name = name;
        if (number is not null)
            sheet.SheetNumber = number;
        return sheet;
    }

    /// <summary>
    /// Places <paramref name="view"/> on <paramref name="sheet"/> at <paramref name="at"/>
    /// (sheet space, feet). requires an open transaction
    /// </summary>
    public static Viewport PlaceViewOnSheet(this ViewSheet sheet, View view, Point2D at)
    {
        var uv = at.ToUv();
        return Viewport.Create(sheet.Document, sheet.Id, view.Id, new XYZ(uv.U, uv.V, 0));
    }
}
