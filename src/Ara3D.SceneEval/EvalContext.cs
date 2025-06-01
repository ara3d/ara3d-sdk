namespace Ara3D.SceneEval;

public class EvalContext
{
    public CancellationToken CancellationToken { get; } 
    public DateTime Started = DateTime.Now;
    public double AnimationTime { get; }

    public EvalContext(CancellationToken cancellationToken, double animationTime)
    {
        CancellationToken = cancellationToken;
        AnimationTime = animationTime;
    }
}