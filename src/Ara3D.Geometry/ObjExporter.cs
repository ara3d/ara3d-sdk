using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ara3D.Geometry
{
    /// <summary>
    /// This is a simple ObjExporter
    /// https://en.wikipedia.org/wiki/Wavefront_.obj_file
    /// https://paulbourke.net/dataformats/obj/
    /// https://fegemo.github.io/cefet-cg/attachments/obj-spec.pdf
    /// </summary>
    public static class ObjExporter
    {
        public static IEnumerable<string> ObjLines(TriangleMesh3D mesh)
        {
            // Write the vertices 
            foreach (var v in mesh.Points)
                yield return $"v {v.X} {v.Y} {v.Z}";

            // Write the faces 
            foreach (var f in mesh.FaceIndices)
            {
                var a = f.A + 1;
                var b = f.B + 1;
                var c = f.C + 1;
                yield return $"f {a} {b} {c}";
            }
        }

        public static void WriteObj(this TriangleMesh3D mesh, string filePath)
            => File.WriteAllLines(filePath, ObjLines(mesh));
    }
}
