using System.Collections.Generic;
using Ara3D.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.Revit.Utils;

public static class RebarCurveExtraction
{
    /// <summary>
    /// Centerline curves of every <see cref="Rebar"/> in the document, converted to sdk
    /// <see cref="Line3D"/> via C4 (curved segments are approximated by their endpoints —
    /// see C4 <c>ToLine3D</c>).
    /// </summary>
    public static IReadOnlyList<Line3D> GetRebarCurves(this Document doc)
    {
        var result = new List<Line3D>();
        foreach (var rebar in doc.GetElements<Rebar>())
        {
            var accessor = rebar.IsRebarShapeDriven() ? rebar.GetShapeDrivenAccessor() : null;
            var count = rebar.NumberOfBarPositions;
            for (var i = 0; i < count; i++)
            {
                var centerlines = rebar.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeAllMultiplanarCurves, i);
                var transform = accessor?.GetBarPositionTransform(i);
                foreach (var curve in centerlines)
                    result.Add((transform != null ? curve.CreateTransformed(transform) : curve).ToLine3D());
            }
        }
        return result;
    }
}
