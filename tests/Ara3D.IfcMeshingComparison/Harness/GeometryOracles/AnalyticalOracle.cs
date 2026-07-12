using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;

namespace Ara3D.IfcMeshingComparison.Harness.GeometryOracles;

/// <summary>Closed-form volume checks for micro fixtures.</summary>
public static class AnalyticalOracle
{
    public static double AbsVolume(TriangleMesh3D mesh)
        => Math.Abs(MeshHelpers.SignedVolume(mesh));

    public static bool MatchesVolume(TriangleMesh3D mesh, double expected, double relativeTol = 0.01)
    {
        var actual = AbsVolume(mesh);
        if (expected <= 0)
            return actual <= 1e-8;
        return Math.Abs(actual - expected) <= expected * relativeTol;
    }

    public static double BoxVolume(double xDim, double yDim, double depth)
        => xDim * yDim * depth;

    public static double CylinderVolume(double radius, double height)
        => Math.PI * radius * radius * height;

    public static double HollowRectangleVolume(
        double outerX, double outerY, double wallThickness, double depth)
    {
        var innerX = outerX - 2 * wallThickness;
        var innerY = outerY - 2 * wallThickness;
        if (innerX <= 0 || innerY <= 0)
            return BoxVolume(outerX, outerY, depth);
        return (BoxVolume(outerX, outerY, depth) - BoxVolume(innerX, innerY, depth));
    }

    public static bool MatchesHalfSpaceClipFraction(
        TriangleMesh3D solid,
        TriangleMesh3D clipped,
        double keptFraction,
        double relativeTol = 0.05)
    {
        var v0 = AbsVolume(solid);
        if (v0 <= 1e-12)
            return false;
        var expected = v0 * keptFraction;
        return Math.Abs(AbsVolume(clipped) - expected) <= expected * relativeTol + 1e-6;
    }
}
