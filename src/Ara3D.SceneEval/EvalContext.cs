using Ara3D.Logging;

namespace Ara3D.SceneEval;

public class EvalContext
{
    public CancellationToken CancellationToken { get; } 
    public DateTime Started = DateTime.Now;
    public double AnimationTime { get; }
    public ILogger Logger { get; }

    public EvalContext(CancellationToken cancellationToken, double animationTime, ILogger logger)
    {
        CancellationToken = cancellationToken;
        AnimationTime = animationTime;
        Logger = logger;
    }
}