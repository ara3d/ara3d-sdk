Console.WriteLine("Hello from a project referencing Ara3D.SDK");
var asm = typeof(Ara3D.Geometry.Vector3).Assembly;
var types = asm.GetTypes().Where(t => t.IsPublic).OrderBy(t => t.Name).ToList();

Console.WriteLine($"Found {types.Count} types in the Ara3D.Geometry library");
foreach (var t in types) 
{
    Console.WriteLine($"Type: {t.Name}");
}