namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
public class RadialAnalyzer : IModifier
{ 
    private List<RadialObjectAnalysis> _analysis;

    [Range(0, 1)] public float Cutoff = 0.75f;
    public Action Recompute => RecomputeImpl;
    public bool Invert = false;
    public bool UseTransparency = false;

    private IModel3D _model;

    public static RadialObjectAnalysis Analyze(TriangleMesh3D mesh)
    {
        return RadialObjectAnalyzer.Analyze(mesh.Triangles);
    }

    public bool IsRadial(InstanceStruct inst)
    {
        var analysis = _analysis?.ElementAtOrDefault(inst.MeshIndex);
        if (analysis == null) return Invert;
        return analysis.IsProbablyRadial(Cutoff) ? !Invert : Invert;
    }

    public InstanceStruct CheckIsRadial(InstanceStruct inst)
    {
        if (IsRadial(inst))
            return inst;
        return UseTransparency ? inst.WithAlpha(0.2f) : inst.WithFlags(1);
    }


    public void RecomputeImpl()
    {
        if (_model == null)
            return;
        _analysis = _model.Meshes.Select(Analyze).ToList();
    }

    public IModel3D Eval(IModel3D model)
    {
        _model = model;

        if (_analysis == null || _analysis.Count == 0)
            return model;

        var instances = model.Instances.Map(CheckIsRadial);
        return model.WithInstances(instances);
    }
}