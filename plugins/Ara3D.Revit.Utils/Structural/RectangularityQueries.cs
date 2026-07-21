using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class RectangularityQueries
{
    private const double Tolerance = 1e-9;

    /// <summary>True when <paramref name="curves"/> is exactly 4 straight segments with consecutive edges mutually perpendicular.</summary>
    public static bool IsRectangular(this IReadOnlyList<Curve> curves)
    {
        if (curves.Count != 4)
            return false;
        for (var i = 0; i < 4; i++)
            if (curves[i] is not Line)
                return false;
        for (var i = 0; i < 4; i++)
        {
            var a = (Line)curves[i];
            var b = (Line)curves[(i + 1) % 4];
            var va = a.GetEndPoint(1) - a.GetEndPoint(0);
            var vb = b.GetEndPoint(1) - b.GetEndPoint(0);
            if (Math.Abs(va.DotProduct(vb)) > Tolerance)
                return false;
        }
        return true;
    }

    /// <summary>Convenience over the native boundary type returned by e.g. <c>AreaReinforcement.GetExteriorBoundary()</c>.</summary>
    public static bool IsRectangular(this CurveArray curves)
        => curves.Cast<Curve>().ToArray().IsRectangular();
}
