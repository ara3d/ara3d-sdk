using System;
using System.Collections.Generic;
using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Curve-to-points sampling used to cross room/space boundaries to sdk <see cref="Point3D"/>
/// (P2 — bridges through C4's <c>ToPoint3D</c>, does not reimplement point conversion). Lines
/// return their two endpoints; other curves are evaluated at <paramref name="n"/> normalized
/// parameters (cyclic curves skip the duplicate closing point).
/// </summary>
internal static class CurveSampling
{
    internal static IReadOnlyList<Point3D> SamplePoints(this Curve curve, int n)
    {
        if (curve is Line line)
            return new[] { line.GetEndPoint(0).ToPoint3D(), line.GetEndPoint(1).ToPoint3D() };

        var count = Math.Max(2, n);
        var denominator = curve.IsCyclic ? count : count - 1;
        var points = new List<Point3D>(count);
        for (var i = 0; i < count; i++)
            points.Add(curve.Evaluate((double)i / denominator, true).ToPoint3D());
        return points;
    }
}
