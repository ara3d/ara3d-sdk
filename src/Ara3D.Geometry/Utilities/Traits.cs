namespace Ara3D.Geometry;

public interface IGeometryTraits<TGeometry> 
{
    TGeometry Geometry { get; }
    object Data { get; }
}

public interface IWithBounds<TGeometry> : IGeometryTraits<TGeometry>
{
    object IGeometryTraits<TGeometry>.Data => Bounds;
    Bounds3D Bounds { get; }
}

public record WithBounds<TGeometry>(TGeometry Geometry, Bounds3D Bounds) : IWithBounds<TGeometry>;

public interface IWithSelectedPoints<TGeometry> : IGeometryTraits<TGeometry>
{
    object IGeometryTraits<TGeometry>.Data => SelectedPoints;
    IReadOnlyList<bool> SelectedPoints { get; }
}

public record WithSelectedPoints<TGeometry>(TGeometry Geometry, IReadOnlyList<bool> SelectedPoints) : IWithSelectedPoints<TGeometry>;

public interface IWithPointWeights<TGeometry> : IGeometryTraits<TGeometry>
{
    object IGeometryTraits<TGeometry>.Data => Weights;
    IReadOnlyList<double> Weights { get; }
}

public record WithPointWeights<TGeometry>(TGeometry Geometry, IReadOnlyList<double> Weights) : IWithPointWeights<TGeometry>;

public interface IWithColors<TGeometry> : IGeometryTraits<TGeometry>
{
    object IGeometryTraits<TGeometry>.Data => Colors;
    IReadOnlyList<Vector3> Colors { get; }
}

public interface IWithTopology<TGeometry> : IGeometryTraits<TGeometry>
{
    object IGeometryTraits<TGeometry>.Data => Topology;
    Topology Topology { get; }
}

public record WithTopology<TGeometry>(TGeometry Geometry, Topology Topology) : IWithTopology<TGeometry>;

public interface IWithStats<TGeometry> : IGeometryTraits<TGeometry>
{
    object IGeometryTraits<TGeometry>.Data => Stats;
    MeshStatistics Stats { get; }
}

