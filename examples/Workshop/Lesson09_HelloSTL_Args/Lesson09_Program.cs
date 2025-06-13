using Ara3D.Geometry;
using Ara3D.Collections;

TriangleMesh3D Fan(IReadOnlyList<Point3D> points, Point3D apex)
{
    var apexIndex = points.Count;
    var faces = new List<Integer3>();
    for (var i = 0; i < points.Count; i++)
    {
        var curIndex = i;
        var nextIndex = i + 1;
        if (nextIndex == points.Count)
            nextIndex = 0;
        var face = new Integer3(curIndex, nextIndex, apexIndex);
        faces.Add(face);
    }

    return new TriangleMesh3D(points.Append(apex), faces);
}

IReadOnlyList<Point3D> CirclePoints(Integer n, Number width)
{
    var r = new List<Point3D>();
    for (var i = 0; i < n; i++)
    {
        var frac = (Number)i / (Number)n;
        var pt = new Point3D(frac.Turns.Sin, frac.Turns.Cos, 0);
        pt *= width;
        r.Add(pt);
    }
    return r;
}

var count = int.Parse(args[0]);
var height = float.Parse(args[1]);
var width = float.Parse(args[2]);

var apex = new Point3D(0, 0, height);
var mesh = Fan(CirclePoints(count, width), apex);

WorkshopUtils.StlUtils.WriteMesh(mesh.Triangles, "fan");