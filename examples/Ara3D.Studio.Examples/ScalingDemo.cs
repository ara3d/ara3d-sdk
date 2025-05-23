using System.ComponentModel.DataAnnotations;
using Ara3D.Models;
using Ara3D.Studio.API;

namespace Ara3D.Studio.Samples;

public class ScalingDemo : IModelOperator
{
    [Range(0.01f, 10f)]
    public float Scale { get; set; } = 2f;

    public Model Evaluate(Model model)
        => model.Scale(Scale);
}