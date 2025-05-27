namespace Ara3D.SceneEval;

public class SceneEvalContext
{
    public CancellationToken CancellationToken { get; } 
    public DateTime Started = DateTime.Now;
    public SceneEvalContext(CancellationToken cancellationToken)
        => CancellationToken = cancellationToken;
}