namespace Ara3D.Studio.API;

public class EvalContext
{
    public IEvalServices Services { get; }
    public double AnimationTime { get; }
    public FlowObject Input { get; }
    public CancellationToken Cancellation { get; }
    public IDerivedDataCache DerivedData => Services.DerivedDataCache;

    /// <summary>
    /// View-time sampling settings (see <see cref="RenderSettings.Resolution"/>), supplied by the
    /// host per evaluation like <see cref="AnimationTime"/>. A modifier that must discretize a
    /// parametric value should read its sample count from here. NOTE: node memoization caches by
    /// input reference, so an evaluator that reads these settings is only re-run when the host
    /// dirties it after a settings change.
    /// </summary>
    public RenderSettings RenderSettings => Services?.RenderSettings ?? RenderSettings.Default;

    public EvalContext(FlowObject input, IEvalServices services, double animationTime, CancellationToken cancellation = default)
    {
        Input = input;
        Services = services;
        AnimationTime = animationTime;
        Cancellation = cancellation;
    }

    public EvalContext WithInput(FlowObject newInput)
        => new(newInput, Services, AnimationTime, Cancellation);
}
