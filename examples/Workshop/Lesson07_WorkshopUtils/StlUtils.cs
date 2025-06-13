using Ara3D.Geometry;

namespace WorkshopUtils
{
    public static class StlUtils
    {
        public static void WriteVertex(Vector3 v)
        {
            Console.WriteLine($"      vertex {v.X} {v.Y} {v.Z}");
        }

        public static void WriteFacet(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            Console.WriteLine("  facet normal 0 0 0");
            Console.WriteLine("    outer loop");
            WriteVertex(v0);
            WriteVertex(v1);
            WriteVertex(v2);
            Console.WriteLine("    endloop");
            Console.WriteLine("  endfacet");
        }

        public static void WriteTriangle(Triangle3D t)
        {
            WriteFacet(t.A, t.B, t.C);
        }

        public static void WriteMesh(IReadOnlyList<Triangle3D> triangles, string name)
        {
            Console.WriteLine($"solid {name}");
            foreach (var t in triangles)
                WriteTriangle(t);
            Console.WriteLine($"endsolid {name}");
        }
    }
}
