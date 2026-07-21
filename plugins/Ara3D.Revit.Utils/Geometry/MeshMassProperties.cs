using Ara3D.Geometry;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Signed volume and centroid of a closed triangle mesh via the divergence theorem
/// (each triangle contributes a signed tetrahedron to the origin). Pure sdk-typed math with
/// no Revit dependency, so it is unit-tested directly (P10). For an outward-oriented closed
/// mesh the volume is positive and equals the enclosed volume in the mesh's own units.
/// </summary>
public static class MeshMassProperties
{
    public static double Volume(this TriangleMesh3D mesh)
        => Integrate(mesh).volume / 6.0;

    public static Point3D Centroid(this TriangleMesh3D mesh)
    {
        var (v, x, y, z) = Integrate(mesh);
        if (v == 0)
            return default;
        var s = 1.0 / (4.0 * v);
        return new Point3D((float)(x * s), (float)(y * s), (float)(z * s));
    }

    private static (double volume, double x, double y, double z) Integrate(TriangleMesh3D mesh)
    {
        var tris = mesh.Triangles;
        double sv = 0, sx = 0, sy = 0, sz = 0;
        for (var i = 0; i < tris.Count; i++)
        {
            var t = tris[i];
            double ax = t.A.X.Value, ay = t.A.Y.Value, az = t.A.Z.Value;
            double bx = t.B.X.Value, by = t.B.Y.Value, bz = t.B.Z.Value;
            double cx = t.C.X.Value, cy = t.C.Y.Value, cz = t.C.Z.Value;

            var d = (ax * ((by * cz) - (cy * bz)))
                  + (ay * ((bz * cx) - (cz * bx)))
                  + (az * ((bx * cy) - (cx * by)));

            sv += d;
            sx += d * (ax + bx + cx);
            sy += d * (ay + by + cy);
            sz += d * (az + bz + cz);
        }

        return (sv, sx, sy, sz);
    }
}
