using Ara3D.Geometry;
using Ara3D.Memory;

namespace Ara3D.Models;

public interface IModel3D : ITransformable3D<IModel3D>
{
    IReadOnlyList<TriangleMesh3D> Meshes { get; }
    IReadOnlyList<InstanceStruct> Instances { get; }
}

public interface IHasBounds3D
{
    Bounds3D Bounds { get; }
}

public interface IBoundedModel3D 
    : IModel3D, IHasBounds3D, ITransformable3D<IBoundedModel3D>
{
    IReadOnlyList<Bounds3D> MeshBounds { get; }
    IReadOnlyList<Bounds3D> InstanceBounds { get; }
}

public interface IRenderableModel3D 
    : IBoundedModel3D, ITransformable3D<IRenderableModel3D>, IDeformable3D<IRenderableModel3D>
{
    IBuffer<float> VertexBuffer { get; }
    IBuffer<uint> IndexBuffer { get; }
    IBuffer<MeshSliceStruct> MeshSliceBuffer { get; }
    IBuffer<InstanceStruct> InstanceBuffer { get; }
    IBuffer<Bounds3D> MeshBoundsBuffer { get; }
    IBuffer<Bounds3D> InstanceBoundsBuffer { get; }
}
