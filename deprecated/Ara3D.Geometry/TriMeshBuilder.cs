using Ara3D.Collections;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    [Mutable]
    public class TriMeshBuilder
    {
        private ArrayBuilder<Vector3> _vertices = new ArrayBuilder<Vector3>();
        private ArrayBuilder<Int3> _indices = new ArrayBuilder<Int3>();

        public TriMeshBuilder Add(Vector3 v)
        {
            _vertices.Add(v);
            return this;
        }
        public TriMeshBuilder AddFace(int a, int b, int c)
        {
            _indices.Add((a, b, c));
            return this;
        }

        public TriMesh ToMesh()
        {
            var r = new TriMesh(_vertices.ToIArray(), _indices.ToIArray());
            _vertices = null;
            _indices = null;
            return r;
        }

        public TriMeshBuilder Add(ITriMesh m)
        {
            var offset = _vertices.Count;
            _vertices.AddRange(m.Points);
            _indices.AddRange(m.FaceIndices.Select(i => i + offset));
            return this;
        }
    }
}