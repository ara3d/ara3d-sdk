using System.Numerics;

var corners = new Vector3[]
{
    new(-0.5f, -0.5f, 0), // base vertices
    new(0.5f, -0.5f, 0),
    new(0.5f, 0.5f, 0),
    new(-0.5f, 0.5f, 0),
};

var apex = new Vector3(0, 0, 1);

void WriteVertex(Vector3 v)
{
    Console.WriteLine($"      vertex {v.X} {v.Y} {v.Z}");
}

Console.WriteLine("solid pyramid");
Console.WriteLine("  facet normal 0 0 0");
Console.WriteLine("    outer loop");
WriteVertex(corners[0]);
WriteVertex(corners[1]);
WriteVertex(apex);
Console.WriteLine("    endloop");
Console.WriteLine("  endfacet");
Console.WriteLine("  facet normal 0 0 0");
Console.WriteLine("    outer loop");
WriteVertex(corners[1]);
WriteVertex(corners[2]);
WriteVertex(apex);
Console.WriteLine("    endloop");
Console.WriteLine("  endfacet");
Console.WriteLine("  facet normal 0 0 0");
Console.WriteLine("    outer loop");
WriteVertex(corners[2]);
WriteVertex(corners[3]);
WriteVertex(apex);
Console.WriteLine("    endloop");
Console.WriteLine("  endfacet");
Console.WriteLine("  facet normal 0 0 0");
Console.WriteLine("    outer loop");
WriteVertex(corners[3]);
WriteVertex(corners[0]);
WriteVertex(apex);
Console.WriteLine("    endloop");
Console.WriteLine("  endfacet");
Console.WriteLine("endsolid pyramid");