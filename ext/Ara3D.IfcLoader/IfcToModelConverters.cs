using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.IfcLoader;

public static unsafe class IfcToModelConverters
{
    public static (double X, double Y, double Z) ComputeMedianCenter(this IfcFile file)
    {
        var xs = new List<double>();
        var ys = new List<double>();
        var zs = new List<double>();

        foreach (var g in file.Model.GetGeometries())
        {
            foreach (var mesh in g.GetMeshes())
            {
                var m = (double*)mesh.Transform;

                var x = m[12];
                var y = -m[14];
                var z = m[13];
                
                xs.Add(x);
                ys.Add(y);
                zs.Add(z);
            }
        }

        xs.Sort();
        ys.Sort();
        zs.Sort();

        if (xs.Count == 0)
            return (0,0,0);

        return (xs[xs.Count / 2], ys[ys.Count / 2], zs[zs.Count / 2]);
    }

    public static Model3D ToModel3D(this IfcFile file)
    {
        //var center = file.ComputeMedianCenter();
        //return file.ToModel3D(center);
        return file.ToModel3D((0, 0, 0));
    }

    public static Model3D ToModel3D(this IfcFile file, (double X, double Y, double Z) origin)
    {
        var mb = new Model3DBuilder();

        var meshes = new Dictionary<uint, int>();
        
        foreach (var g in file.Model.GetGeometries())
        {
            foreach (var mesh in g.GetMeshes())
            {
                var m = (double*)mesh.Transform;
                var c = (IfcColor*)mesh.Color;
                var color = new Color((float)c->R, (float)c->G, (float)c->B, (float)c->A);
                var mat = new Material(color, 0.1f, 0.5f);

                var x = m[12] - origin.X;
                var y = -m[14] - origin.Y;
                var z = m[13] - origin.Z;     

                var matrix = new Matrix4x4(
                    (float)m[0], -(float)m[2], (float)m[1], (float)m[3],
                    (float)m[4], -(float)m[6], (float)m[5], (float)m[7],
                    (float)m[8], -(float)m[10], (float)m[9], (float)m[11],
                    (float)x, (float)y, (float)z, (float)m[15]);

                if (!meshes.ContainsKey(mesh.Id))
                {
                    var triMesh = mesh.ToTriangleMesh();
                    var index = mb.Meshes.Count;
                    mb.Meshes.Add(triMesh);
                    meshes.Add(mesh.Id, index);
                }

                // NOTE: the geometry ID has to be recreated. 
                mb.AddInstance(meshes[mesh.Id], matrix, mat, (int)g.Id);
                //mb.AddInstance(meshes[mesh.Id], matrix, mat, (int)mesh.Id);
            }
        }

        return mb.Build();
    }

    // Copies the data from the C++ layer into arrays.
    public static TriangleMesh3D ToTriangleMesh(this IfcMesh m)
    {
        var vertexPtr = (IfcVertex*)m.Vertices;
        var vertices = new Point3D[m.NumVertices];
        for (var i = 0; i < m.NumVertices; i++)
        {
            var v = vertexPtr[i];
            vertices[i] = new Vector3(
                (float)v.PX, (float)v.PY, (float)v.PZ);
        }
        var indexPtr = (Integer*)m.Indices;
        var indices = new Integer3[m.NumIndices / 3];
        for (var i = 0; i < m.NumIndices / 3; i++)
        {
            indices[i] = new Integer3(
                indexPtr[i * 3], indexPtr[i * 3 + 1], indexPtr[i * 3 + 2]);
        }
        return new TriangleMesh3D(vertices, indices);
    }

}