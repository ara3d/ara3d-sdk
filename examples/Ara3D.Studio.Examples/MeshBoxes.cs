using Ara3D.Studio.Samples.Generators;

namespace Ara3D.Studio.Samples;

public class MeshBoxes : IModifier
{
    public bool UseFrames = true;
    public bool Oriented = true;
    public bool Disabled = false;
    public bool ApplyRotation = true;
    public bool KeepOriginal = false;
    public bool UseCylinders = false;
    [Range(0f, 0.4f)] public float FrameSize;
    
    public QuadMesh3D UnitCylinder 
        => 20.GetCircularPoints(0.5f).Translate((0,0,0.5f)).Extrude(1f).ToQuadMesh3D();

    public QuadMesh3D GetFrameMesh()
    {
        var bldr = new BoxFrameMeshBuilder(FrameSize);
        return bldr.Mesh;
    }
    public TriangleMesh3D ToBoundsMesh(TriangleMesh3D mesh)
    {
        var bounds = mesh.Bounds;

        var cube = PlatonicSolids.Cube;
        var displayMesh = UseFrames ? GetFrameMesh() : UseCylinders ? UnitCylinder : cube;

        if (!Oriented)
            return displayMesh.Triangulate().FitToBounds(bounds);

        var obb = mesh.Points.FitOrientedBox();
        var q = obb.Frame.Rotation;
        Debug.Assert(!float.IsNaN(q.X), "bad quaternion – matrix not orthonormal?");

        var test = Vector3.UnitX * (obb.Size.X * 0.5f);
        var rotated = test.Transform(q);
        Debug.Assert(Math.Abs(rotated.Length() - test.Length()) < 1e-3f, "scale/shear sneaking in");

        return ApplyRotation 
            ? displayMesh.Triangulate().Scale(obb.Size).Rotate(q).Translate(obb.Frame.Origin)
            : displayMesh.Triangulate().Scale(obb.Size).Translate(obb.Frame.Origin);
    }

    public IModel3D Eval(IModel3D model)
    {
        if (Disabled)
            return model;
        var boundsAsMeshes = model.Meshes.Select(ToBoundsMesh).ToList();
        var r = model.WithMeshes(boundsAsMeshes);
        if (KeepOriginal)
            return r.Combine(model);
        return r;
    }
}