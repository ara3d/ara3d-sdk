using Plato;
using Ara3D.Memory;
using Plato.Geometry;

namespace Ara3D.Models
{
    public sealed class MeshAdapter : IDisposable
    {
        public IMemoryOwner? IndexData { get; private set; }
        public IMemoryOwner? PointData { get; private set; }
        public TriangleMesh3D Mesh { get; }

        public MeshAdapter(IMemoryOwner pointData, IMemoryOwner indexData)
        {
            PointData = pointData;
            IndexData = indexData;
            var points = PointData.Reinterpret<Point3D>();
            var indices = IndexData.Reinterpret<Integer3>();
            Mesh = new TriangleMesh3D(points, indices);
        }

        public void Dispose()
        {
            PointData?.Dispose();
            IndexData?.Dispose();
            PointData = null;
            IndexData = null;
        }
    }
}
