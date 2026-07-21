using System.Collections.Generic;
using Ara3D.Geometry;
using Autodesk.Revit.DB;
using RevitMesh = Autodesk.Revit.DB.Mesh;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Revit meshes and solids to the sdk's <see cref="TriangleMesh3D"/>. Coordinates stay in
/// Revit internal units (feet); winding follows Revit's outward orientation.
/// </summary>
public static class MeshConverters
{
    public static TriangleMesh3D ToTriangleMesh(this RevitMesh mesh)
    {
        if (mesh == null)
            return new([], []);

        var verts = mesh.Vertices;
        var points = new Point3D[verts.Count];
        for (var i = 0; i < verts.Count; i++)
            points[i] = verts[i].ToPoint3D();

        var n = mesh.NumTriangles;
        var faces = new Integer3[n];
        for (var i = 0; i < n; i++)
        {
            var tri = mesh.get_Triangle(i);
            faces[i] = ((int)tri.get_Index(0), (int)tri.get_Index(1), (int)tri.get_Index(2));
        }

        return new(points, faces);
    }

    public static TriangleMesh3D ToTriangleMesh(this Solid solid)
    {
        if (solid == null || solid.Faces.IsEmpty || !SolidUtils.IsValidForTessellation(solid))
            return new([], []);

        var tess = solid.Tessellate();
        var points = new List<Point3D>();
        var faces = new List<Integer3>();
        for (var s = 0; s < tess.ShellComponentCount; s++)
        {
            var comp = tess.GetShellComponent(s);
            var offset = points.Count;
            for (var v = 0; v < comp.VertexCount; v++)
                points.Add(comp.GetVertex(v).ToPoint3D());
            for (var t = 0; t < comp.TriangleCount; t++)
            {
                var tri = comp.GetTriangle(t);
                faces.Add((offset + tri.VertexIndex0, offset + tri.VertexIndex1, offset + tri.VertexIndex2));
            }
        }

        return new(points, faces);
    }

    // Tessellation controls tuned by the revit-ifc exporter; balances fidelity and count.
    private static TriangulatedSolidOrShell Tessellate(this Solid solid)
        => SolidUtils.TessellateSolidOrShell(solid, new SolidOrShellTessellationControls
        {
            Accuracy = 0.5,
            LevelOfDetail = 0.4,
            MinAngleInTriangle = 0.13,
            MinExternalAngleBetweenTriangles = 0.55
        });
}
