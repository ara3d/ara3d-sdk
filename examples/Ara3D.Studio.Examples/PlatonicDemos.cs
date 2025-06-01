using System.ComponentModel.DataAnnotations;
using Ara3D.Models;
using Ara3D.SceneEval;
using Ara3D.Studio.API;
using Plato.Geometry;

namespace Ara3D.Studio.Samples;

public class PlatonicShape : IModelGenerator
{
    [Range(0, 4)] public int Shape;
    [Range(0f, 1f)] public float Red = 0.2f;
    [Range(0f, 1f)] public float Green = 0.8f;
    [Range(0f, 1f)] public float Blue = 0.1f;
    [Range(0f, 1f)] public float Alpha = 1f;

    public Model3D Eval(EvalContext context)
        => PlatonicSolids
            .GetMesh(Shape)
            .ToNode()
            .WithColor((Red, Green, Blue, Alpha));
}