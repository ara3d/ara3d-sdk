namespace Ara3D.Studio.Samples;

[Category(Cat.Display)]
[Description("Overlays the input mesh's bounding box, drawn as edges, frames, or cylinders, leaving the original visible.")]
public class DisplayMeshBounds : IModifier
{
    public bool UseFrames = true;
    public bool Oriented = true;
    [Range(0,1)] public float OriginalTransparency = 0.2f;
    public bool UseCylinders = false;
    public bool UseCylindricalMatrix = false;
    public bool UseNormalsForCylinders = true;
    [Range(0.001f, 0.5f)] public float FrameSize = 0.1f;
    GeometryFitting.CylinderRadiusFit CylinderFit = GeometryFitting.CylinderRadiusFit.Average;

    private IModel3D _model;
    private MeshStatistics[] _stats;
    
    public QuadMesh3D GetFrameMesh()
        => new BoxFrameMeshBuilder(FrameSize).Mesh;

    public QuadMesh3D GetDisplayMesh()
        => UseFrames && !UseCylinders
            ? GetFrameMesh() 
            : UseCylinders
                ? GeometryUtil.UnitCylinder() 
                : PlatonicSolids.Cube;

    // NOTE: I can also try converting the thing into a line cylinder 

    public TriangleMesh3D ToBoundsMesh(TriangleMesh3D mesh, int i)
    {
        var stats = _stats[i];
        if (!Oriented)
            return GetDisplayMesh().FitToBounds(stats.Bounds).Triangulate();
        
        var obb = mesh.Points.FitOrientedBox();

        var fittedCylinder = UseNormalsForCylinders 
            ? obb.FitCylinder(stats.Attributes.Faces.Normals, CylinderFit)
            : obb.FitCylinder(CylinderFit);

        var matrix = UseCylindricalMatrix 
            ? fittedCylinder.ToMatrix() 
            : obb.ToMatrix();

        return GetDisplayMesh().Transform(matrix).Triangulate();
    }

    public Action Recompute 
        => RecomputeImpl;

    public void RecomputeImpl()
    {
        if (_model == null)
        {
            _stats = null;
            return;
        }

        var welded = true;
        _stats = _model.Meshes.Select(m => new MeshStatistics(m, welded)).ToArray();
    }

    public IModel3D Eval(IModel3D model)
    {
        _model = model;
        if (_stats == null) RecomputeImpl();
        var meshes = _model.Meshes.Select(ToBoundsMesh);
        var r = _model.WithMeshes(meshes);
        return r.Combine(model.MapInstances(inst => inst.WithAlpha(OriginalTransparency)));
    }
}