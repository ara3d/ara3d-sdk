using Ara3D.Models;
using Ara3D.SceneEval;
using Ara3D.Studio.API;

namespace Ara3D.Studio.Samples;

public class Merge : IModelModifier
{
    public Model3D Eval(Model3D m, EvalContext eval)
    {
        if (m.Nodes.Count == 0) return m;
        var mat = m.Nodes[0].Material;
        var mergedMesh = m.ToMesh();
        var node = (Model3DNode)mergedMesh;
        return node with { Material = mat };
    }
}