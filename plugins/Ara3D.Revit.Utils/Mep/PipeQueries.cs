using System;
using Ara3D.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace Ara3D.Revit.Utils;

/// <summary>Pipe direction and sizing queries. Direction bridges to the sdk via C4 (P2); sizes stay in internal units (feet, P7).</summary>
public static class PipeQueries
{
    public const double DefaultParallelToleranceRadians = 0.01;

    public static Vector3 GetDirection(this Pipe pipe)
    {
        var curve = ((LocationCurve)pipe.Location).Curve;
        return (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize().ToVector3();
    }

    public static bool IsParallelTo(this Pipe a, Pipe b, double toleranceRadians = DefaultParallelToleranceRadians)
    {
        var lineA = (Line)((LocationCurve)a.Location).Curve;
        var lineB = (Line)((LocationCurve)b.Location).Curve;
        return Math.Sin(lineA.Direction.AngleTo(lineB.Direction)) < toleranceRadians;
    }

    /// <summary>Pipe wall thickness (outer radius minus inner radius), internal units (feet).</summary>
    public static double GetWallThickness(this Pipe pipe)
    {
        var inner = pipe.GetParameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM).AsDouble();
        var outer = pipe.GetParameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble();
        return 0.5 * (outer - inner);
    }
}
