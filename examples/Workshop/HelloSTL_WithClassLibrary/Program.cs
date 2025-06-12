using Ara3D.Geometry;
using WorkshopUtils;

var points = new Point3D[]
{
    (-0.5f, -0.5f, 0),
    (0.5f, -0.5f, 0),
    (0.5f, 0.5f, 0),
    (-0.5f, 0.5f, 0),
    (0, 0, 1),
};

var faces = new Integer3[]
{
    (0, 1, 4),
    (1, 2, 4),
    (2, 3, 4),
    (3, 0, 4),
};

var mesh = new TriangleMesh3D(points, faces);
StlUtils.WriteMesh(mesh.Triangles, "pyramid");