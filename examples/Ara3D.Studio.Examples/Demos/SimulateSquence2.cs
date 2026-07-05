namespace Ara3D.Studio.Samples.Demos;

[Category(nameof(Categories.Demos))]
public class SimulateSequence2 : IModifier
{
    public List<byte> OriginalFlags { get; private set; }
    public List<Matrix4x4> OriginalTransforms { get; private set; }
    public List<float> StartTimes { get; private set; }
    public List<Bounds3D> InstanceBounds { get; private set; }
    public Bounds3D TotalBounds { get; private set; }
    [Range(0f,100f)] public float LerpAmount { get; set; }
    [Range(0f, 0.2f)] public float TimeToPosition { get; set; } = 0.05f;
    [Range(0, 100)] public int DistanceZ { get; set; } = 50;
    [Range(0, 20)] public float XYMultiplier { get; set; } = 3f;

    public int NumObjects => OriginalTransforms?.Count ?? 0;

    public float GetStartTime(Bounds3D localBounds, Bounds3D totalBounds)
        => localBounds.Max.Z.InverseLerp(totalBounds.Min.Z, totalBounds.Max.Z);

    /// <summary>
    /// Returns two positions: the start position, and an intermediate position
    /// The start position is far from the center and down.
    /// The desire is for object to move up into position, and then move towards the inside until
    /// it arrives in position
    /// </summary>
    public (Matrix4x4 A, Matrix4x4 B) GetPositions(Bounds3D bounds, Matrix4x4 o)
    {
        var p = o.Translation;
        var c = bounds.Center;
        var xOffset = p.X - c.X;
        var yOffset = p.Y - c.Y;
        var x0 = c.X + xOffset * XYMultiplier;
        var y0 = c.Y + yOffset * XYMultiplier;
        var m1 = o * Matrix4x4.CreateTranslation(new(x0, y0, -DistanceZ));
        var m2 = o * Matrix4x4.CreateTranslation(new(x0, y0, 0));
        return (m1, m2);
    }
    
    public FlowObject Eval(FlowObject obj, EvalContext context)
    {
        if (obj.Content is not RenderModelData rmd)
            return obj;

        if (OriginalTransforms == null)
        {
            InstanceBounds = rmd.InstanceBoundsData.ToList();
            OriginalTransforms = rmd.InstanceData.Select(i => i.Matrix4x4).ToList();
            OriginalFlags = rmd.InstanceData.Select(i => i.Flags).ToList();
            TotalBounds = rmd.TotalBounds;
            StartTimes = InstanceBounds.Select(b => GetStartTime(b, TotalBounds)).ToList();
        }

        var lerpAmount = (LerpAmount / 100f) * (1f + TimeToPosition);
        for (var i=0; i < rmd.InstanceData.Count; i++)
        {
            var start = StartTimes[i];
            var end = start + TimeToPosition;

            if (lerpAmount < start)
            {
                rmd.InstanceData[i].Flags = 1;
            }
            else 
            {
                rmd.InstanceData[i].Flags = OriginalFlags[i];
            }

            var dest = OriginalTransforms[i];

            if (lerpAmount >= end)
            {
                rmd.InstanceData[i] = rmd.InstanceData[i].WithMatrix(dest);
            }
            else
            {
                var (srcA, srcB) = GetPositions(TotalBounds, dest);
                var localLerpAmount = (lerpAmount - start) / TimeToPosition;

                if (localLerpAmount < 0.5f)
                {
                    var lerpedMatrix = srcA.Lerp(srcB, localLerpAmount * 2f);
                    rmd.InstanceData[i] = rmd.InstanceData[i].WithMatrix(lerpedMatrix);
                }
                else
                {
                    var lerpedMatrix = srcB.Lerp(dest, (localLerpAmount - 0.5f) * 2f);
                    rmd.InstanceData[i] = rmd.InstanceData[i].WithMatrix(lerpedMatrix);
                }
            }
        }

        return obj;
    }
}
