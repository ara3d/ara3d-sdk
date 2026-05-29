
/*
using Ara3D.IO.GeoJson;

namespace Ara3D.Studio.Samples;
public class GeoJsonDemo : IGenerator
{
    [Range(0f, 2f)] public float Thickness = 0.1f;
    [Range(0f, 10f)] public float Height = 2;
    public bool ShowFloors = false;

    public static string Dir = @"C:\Users\cdigg\data\temp";

    private DirectoryWatcher dw;

    public static IReadOnlyList<Point3D> ToPoints(IReadOnlyList<Vector3> vectors)
        => vectors.Select(v => new Point3D(v.X, v.Y, v.Z));

    public static IReadOnlyList<Line3D> LoopToLines(IReadOnlyList<Point3D> points)
        => points.Select((pt, i) => new Line3D(pt, points.ElementAtModulo(i + 1)));

    public IHostApplication App;

    public void Init(IHostApplication app)
    {
        App = app;
        dw = new DirectoryWatcher(Dir, "*.*", DirChanged);
    }

    public void DirChanged()
    {
        App.Invalidate(this);
    }

    public IModel3D Eval(EvalContext context)
    {
        var fc = GeoJsonSerializer.LoadFeatureCollection(Dir);
        var mb = new Model3DBuilder();
        mb.AddMesh(PlatonicSolids.TriangulatedCube);

        var groups = fc.features.GroupBy(f => f["level_id"]);
        var roomIndex = 0;
        foreach (var group in groups)
        {
            foreach (var feature in group)
            {
                if (feature is ImdfUnit imdfUnit)
                {
                    var geo = imdfUnit.geometry;
                    var loops = geo.coordinates.ToVector3Arrays();
                    foreach (var loop in loops)
                    {
                        var lines = LoopToLines(ToPoints(loop));
                        foreach (var line in lines)
                        {
                            var mat = line.ToBoxTransform(Thickness, Height) * Matrix4x4.CreateTranslation((0, 0, Height / 2));
                            mb.AddInstance(0, mat);
                        }
                    }

                    if (loops.Length == 0 || loops[0].Length == 0) continue;
                    var height = loops[0][0].Z;

                    var loops2D = loops.Select(loop => loop.Select(v => v.To2D));
                    if (loops2D.Count > 0 && ShowFloors)
                    {
                        var outerLoop = loops2D[0];
                        var innerLoops = loops2D.Skip();
                        var floorPolygon = outerLoop.ToPolygonWithHoles(innerLoops.ToArray());
                        if (outerLoop.Count < 3) continue;
                        var newMesh = floorPolygon.TrianglesFan().ToMesh();
                        var newMeshIndex = mb.AddMesh(newMesh);
                        var mat = Matrix4x4.CreateTranslation(new(0, 0, height));
                        mb.AddInstance(newMeshIndex, mat);
                    }

                    roomIndex++;
                }
            }
        }

        return mb.Build();
    }
}
*/