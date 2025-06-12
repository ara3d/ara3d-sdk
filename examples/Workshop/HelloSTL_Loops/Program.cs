using System.Numerics;

var corners = new Vector3[]
{
    new(-0.5f, -0.5f, 0), 
    new(0.5f, -0.5f, 0),
    new(0.5f, 0.5f, 0),
    new(-0.5f, 0.5f, 0),
};

var apex = new Vector3(0, 0, 1);

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

Console.WriteLine("solid pyramid");
for (var i=0; i < 4; i++)
{
    var v0 = corners[i];
    var v1 = corners[(i + 1) % 4];
    WriteFacet(v0, v1, apex);
}
Console.WriteLine("endsolid pyramid");