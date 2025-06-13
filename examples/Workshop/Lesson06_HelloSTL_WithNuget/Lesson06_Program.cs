using Ara3D.Geometry;

// Helper functions.

void WriteVertex(Vector3 v)
{
    Console.WriteLine($"      vertex {v.X} {v.Y} {v.Z}");
}

void WriteFacet(Vector3 v0, Vector3 v1, Vector3 v2)
{
    Console.WriteLine("  facet normal 0 0 0");
    Console.WriteLine("    outer loop");
    WriteVertex(v0);
    WriteVertex(v1);
    WriteVertex(v2);
    Console.WriteLine("    endloop");
    Console.WriteLine("  endfacet");
}

void WriteTriangle(Triangle3D t)
{
    WriteFacet(t.A, t.B, t.C);
}

void WriteMesh(IReadOnlyList<Triangle3D> triangles, string name)
{
    Console.WriteLine($"solid {name}");
    foreach (var t in triangles)
        WriteTriangle(t);
    Console.WriteLine($"endsolid {name}");
}

// Our hard-coded pyramid mesh.

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

// The main worker function 

WriteMesh(mesh.Triangles, "pyramid");



