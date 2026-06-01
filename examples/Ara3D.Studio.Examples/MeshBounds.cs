using Ara3D.Studio.Samples.Generators;
using Cylinder = Ara3D.Geometry.Cylinder;
using System.Linq;

namespace Ara3D.Studio.Samples;

public class MeshBounds : IModifier
{
    public bool UseFrames = true;
    public bool Oriented = true;
    [Range(0,1)] public float OriginalTransparency = 0.2f;
    public bool UseCylinders = false;
    public bool UseCylindricalMatrix = false;
    public bool UseNormalsForCylinders = true;
    
    // TEMP:
    public bool UseCylinderFromLine = true;
    public bool RejectNonCylinders = true;
    public bool CheckAxialIntersection = true;
    public bool CheckLength = true;
    public bool CheckRadius = true;

    // TEMP:
    [Range(0, 3)] public int Magnitude = 2; 
    [Range(0f, 1f)] public float MinCylinderLength = 0.1f;
    [Range(0.001f, 50f)] public float MaxCylinderLength = 10f;
    [Range(0f, 1f)] public float MinCylinderRadius = 0.05f;
    [Range(0.001f, 50f)] public float MaxCylinderRadius = 10f;
    public bool KeepOriginalMesh = false;
    public GeometryFitting.CylinderRadiusFit CylinderFit { get; set; } = GeometryFitting.CylinderRadiusFit.Average;

    [Range(0.001f, 0.5f)] public float FrameSize = 0.1f;
    
    public QuadMesh3D GetFrameMesh()
        => new BoxFrameMeshBuilder(FrameSize).Mesh;

    public QuadMesh3D GetDisplayMesh()
        => UseFrames && !UseCylinders
            ? GetFrameMesh() 
            : UseCylinders
                ? GeometryUtil.UnitCylinder() 
                : PlatonicSolids.Cube;

    // NOTE: I can also try converting the thing into a line cylinder 

    public TriangleMesh3D ToBoundsMesh(TriangleMesh3D mesh, int i)
    {
        var bounds = Bounds[i];
        if (!Oriented)
            return GetDisplayMesh().FitToBounds(bounds).Triangulate();
        
        var obb = mesh.Points.FitOrientedBox();

        var fittedCylinder = UseNormalsForCylinders 
            ? obb.FitCylinder(mesh.GetFaceNormals(), CylinderFit)
            : obb.FitCylinder(CylinderFit);

        var matrix = UseCylindricalMatrix 
            ? fittedCylinder.ToMatrix() 
            : obb.ToMatrix();

        if (RejectNonCylinders && IsNonCylinder(mesh, fittedCylinder))
        {
            return ([], []);
        }

        if (UseCylinderFromLine)
        {
            return KeepOriginalMesh ? mesh : fittedCylinder.ToMesh();
        }

        return KeepOriginalMesh ? mesh : GetDisplayMesh().Transform(matrix).Triangulate();
    }

    public bool IsNonCylinder(TriangleMesh3D mesh, Cylinder cyl)
    {
        var mag = Math.Pow(10, Magnitude);
        
        if (CheckAxialIntersection && mesh.Triangles.AsEnumerable().Any(tri => cyl.Line.Intersects(tri)))
            return true;

        if (CheckRadius)
            if (cyl.Radius < MinCylinderRadius * mag || cyl.Radius > MaxCylinderRadius * mag)
                return true;
        
        if (CheckLength)
            if (cyl.Line.Length < MinCylinderLength * mag || cyl.Line.Length > MaxCylinderLength * mag)
                return true;
        
        return false;
    }

    public Bounds3D[] Bounds;

    public IModel3D Eval(IModel3D model)
    {
        if (Bounds == null) Bounds = model.Meshes.Select(b => b.Bounds).ToArray();
        var boundsAsMeshes = model.Meshes.Select(ToBoundsMesh).ToList();
        var r = model.WithMeshes(boundsAsMeshes);
        return r.Combine(model.MapInstances(inst => inst.WithAlpha(OriginalTransparency)));
    }
}

public static class Intersection3D
{
    
    /// <summary>
    /// Returns true if the finite line segment intersects the triangle.
    /// If true, outputs the intersection point.
    /// </summary>
    public static bool Intersects(
        this Line3D line,
        Triangle3D triangle,
        out Point3D point,
        float tolerance = 1e-6f)
    {
        point = default;

        var origin = line.A;
        var segment = line.B - line.A;
        var length = segment.Length();

        if (length <= tolerance)
            return triangle.ContainsPoint(line.A, tolerance);

        var direction = segment / length;

        if (!RayTriangleIntersection(
                origin,
                direction,
                triangle,
                out var rayDistance,
                tolerance))
        {
            return false;
        }

        // Because this is a finite segment, the ray hit distance must be between 0 and segment length.
        if (rayDistance < -tolerance || rayDistance > length + tolerance)
            return false;

        point = origin + direction * rayDistance;
        return true;
    }

    /// <summary>
    /// Returns true if the finite line segment intersects the triangle.
    /// </summary>
    public static bool Intersects(
        this Line3D line,
        Triangle3D triangle,
        float tolerance = 1e-6f)
        => line.Intersects(triangle, out _, tolerance);

    /// <summary>
    /// Ray-triangle intersection.
    /// The ray is Origin + Direction * t, where Direction should be normalized.
    /// Returns the distance t along the ray.
    /// </summary>
    public static bool RayTriangleIntersection(
        Point3D origin,
        Vector3 direction,
        Triangle3D triangle,
        out float distance,
        float tolerance = 1e-6f)
    {
        distance = 0;

        var edge1 = triangle.B - triangle.A;
        var edge2 = triangle.C - triangle.A;

        var p = Vector3.Cross(direction, edge2);
        var det = Vector3.Dot(edge1, p);

        // Ray is parallel to triangle plane, or triangle is degenerate.
        if (MathF.Abs(det) <= tolerance)
            return false;

        var invDet = 1.0f / det;

        var t = origin - triangle.A;

        var u = Vector3.Dot(t, p) * invDet;
        if (u < -tolerance || u > 1.0f + tolerance)
            return false;

        var q = Vector3.Cross(t, edge1);

        var v = Vector3.Dot(direction, q) * invDet;
        if (v < -tolerance || u + v > 1.0f + tolerance)
            return false;

        distance = Vector3.Dot(edge2, q) * invDet;

        // For a ray, distance must be non-negative.
        if (distance < -tolerance)
            return false;

        return true;
    }

    /// <summary>
    /// Checks whether a point lies inside or on the boundary of the triangle.
    /// The point is expected to be close to the triangle plane.
    /// </summary>
    public static bool ContainsPoint(
        this Triangle3D triangle,
        Point3D point,
        float tolerance = 1e-6f)
    {
        var normal = triangle.Normal;

        if (normal.LengthSquared() <= tolerance * tolerance)
            return false;

        var distanceToPlane = Vector3.Dot(point - triangle.A, normal);

        if (MathF.Abs(distanceToPlane) > tolerance)
            return false;

        var bary = triangle.Barycentric(point);

        return bary.X >= -tolerance
            && bary.Y >= -tolerance
            && bary.Z >= -tolerance
            && bary.X <= 1.0f + tolerance
            && bary.Y <= 1.0f + tolerance
            && bary.Z <= 1.0f + tolerance;
    }

    /// <summary>
    /// Returns barycentric coordinates relative to triangle A/B/C.
    /// X corresponds to A, Y to B, Z to C.
    /// </summary>
    public static Vector3 Barycentric(
        this Triangle3D triangle,
        Point3D point)
    {
        var v0 = triangle.B - triangle.A;
        var v1 = triangle.C - triangle.A;
        var v2 = point - triangle.A;

        var d00 = Vector3.Dot(v0, v0);
        var d01 = Vector3.Dot(v0, v1);
        var d11 = Vector3.Dot(v1, v1);
        var d20 = Vector3.Dot(v2, v0);
        var d21 = Vector3.Dot(v2, v1);

        var denom = d00 * d11 - d01 * d01;

        if (MathF.Abs(denom) <= 1e-12f)
            return new Vector3(float.NaN, float.NaN, float.NaN);

        var v = (d11 * d20 - d01 * d21) / denom;
        var w = (d00 * d21 - d01 * d20) / denom;
        var u = 1.0f - v - w;

        return new Vector3(u, v, w);
    }
}