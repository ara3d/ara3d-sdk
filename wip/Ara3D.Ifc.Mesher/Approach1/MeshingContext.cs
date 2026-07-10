using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IO.StepParser;
using Ara3D.Models;

namespace Ara3D.Ifc.Mesher.Approach1;



/// <summary>Shared meshing state: resolver, unit scale, caches, and diagnostics.</summary>

public sealed class MeshingContext : IDisposable

{

    readonly Dictionary<int, Vector3> _cartesianPoints = new();

    readonly Dictionary<int, Vector3> _directions = new();

    internal readonly Dictionary<int, Frame3D> _axis2Placements = new();

    internal readonly Dictionary<int, Frame3D> _localPlacements = new();

    internal readonly Dictionary<int, Matrix4x4> _mappedTransforms = new();

    readonly Dictionary<int, PolygonWithHoles> _profiles = new();

    readonly Dictionary<int, TriangleMesh3D> _meshCache = new();

    readonly Dictionary<int, IReadOnlyList<Vector2>> _curve2DCache = new();

    readonly Dictionary<int, IReadOnlyList<Point3D>> _curve3DCache = new();

    Dictionary<int, Material>? _itemMaterials;

    StepDocument? _ownedDocument;



    public MeshingContext(IfcFile file, int circleSegments = 32, int arcSegments = 16)

    {

        File = file;

        Resolver = file.EntityResolver;

        CircleSegments = circleSegments;

        ArcSegments = arcSegments;

        LengthScale = Units.ResolveLengthScaleToMeters(this);

    }



    public MeshingContext(StepDocument document, double? lengthScale = null, int circleSegments = 32, int arcSegments = 16)

    {

        _ownedDocument = document;

        Resolver = new IfcEntityResolver(document);

        CircleSegments = circleSegments;

        ArcSegments = arcSegments;

        LengthScale = lengthScale ?? Units.ResolveLengthScaleToMeters(this);

    }



    public IfcFile? File { get; }

    public IfcEntityResolver Resolver { get; }

    public double LengthScale { get; }

    public int CircleSegments { get; }

    public int ArcSegments { get; }

    public MeshingDiagnostics Diagnostics { get; } = new();



    public float ScaleLength(double value) => (float)(value * LengthScale);



    public Dictionary<int, Vector3> PointCache => _cartesianPoints;

    public Dictionary<int, Vector3> DirectionCache => _directions;

    public Dictionary<int, PolygonWithHoles> ProfileCache => _profiles;

    public Dictionary<int, TriangleMesh3D> MeshCache => _meshCache;



    public IfcEntity GetEntity(int id) => Resolver.GetEntity(id);

    public IfcEntity? GetEntityOrDefault(int id) => Resolver.GetEntityOrDefault(id);

    /// <summary>Material from an <c>IfcStyledItem</c> that references this geometry item, if any.</summary>
    public Material? TryGetItemMaterial(int geometryItemId)
    {
        _itemMaterials ??= StyleResolver.BuildItemMaterialMap(this);
        return _itemMaterials.TryGetValue(geometryItemId, out var material) ? material : null;
    }



    public bool TryGetProfile(int id, out PolygonWithHoles profile) => _profiles.TryGetValue(id, out profile);

    public void CacheProfile(int id, PolygonWithHoles profile) => _profiles[id] = profile;



    public bool TryGetMesh(int id, out TriangleMesh3D mesh) => _meshCache.TryGetValue(id, out mesh);

    public void CacheMesh(int id, TriangleMesh3D mesh) => _meshCache[id] = mesh;



    public bool TryGetCurve2D(int id, out IReadOnlyList<Vector2> points) => _curve2DCache.TryGetValue(id, out points);

    public void CacheCurve2D(int id, IReadOnlyList<Vector2> points) => _curve2DCache[id] = points;



    public bool TryGetCurve3D(int id, out IReadOnlyList<Point3D> points) => _curve3DCache.TryGetValue(id, out points);

    public void CacheCurve3D(int id, IReadOnlyList<Point3D> points) => _curve3DCache[id] = points;

    public void Try(Action action, string entityName, string? context = null)
    {
        try { action(); }
        catch (Exception ex)
        {
            Diagnostics.RecordUnsupported(entityName, context is null ? ex.Message : $"{context}: {ex.Message}");
        }
    }

    public void Dispose()

    {

        _ownedDocument?.Dispose();

        _ownedDocument = null;

    }

}


