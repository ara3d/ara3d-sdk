using System.ComponentModel.DataAnnotations;
using Ara3D.Models;
using Ara3D.SceneEval;
using Ara3D.Studio.API;
using Ara3D.Utils;

namespace Ara3D.Studio.Samples;

public class Transform : IModelModifier
{
    [Range(0.01f, 10f)] public float Scale = 1f;

    [Range(-100f, 100f)] public float XOffset;
    [Range(-100f, 100f)] public float YOffset; 
    [Range(-100f, 100f)] public float ZOffset;

    [Range(-360, 360f)] public float Yaw;
    [Range(-360f, 360f)] public float Pitch;
    [Range(-360f, 360f)] public float Roll;

    public Model3D Eval(Model3D model3D, EvalContext context)
        => model3D
            .Translate((XOffset, YOffset, ZOffset))
            .Rotate(Yaw.ToRadians(), Pitch.ToRadians(), Roll.ToRadians())
            .Scale(Scale);
}