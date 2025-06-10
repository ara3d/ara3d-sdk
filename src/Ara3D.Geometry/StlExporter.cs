using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ara3D.Geometry
{
    public static class StlExporter
    {
        public static IEnumerable<string> StlLines(IEnumerable<Triangle3D> triangles)
        {
            foreach (var triangle in triangles)
            {
                var n = triangle.Normal;
                yield return $"facet normal {n.X} {n.Y} {n.Z}";
                yield return $"outer loop";
                foreach (var v in triangle.Points)
                    yield return $"vertex {v.X} {v.Y} {v.Z}";
                yield return $"end loop";
                yield return $"end facet";
            }
        }

        public static void WriteStl(this TriangleMesh3D mesh, string filePath)
            => File.WriteAllLines(filePath, StlLines(mesh.Triangles));
    
    }
}
