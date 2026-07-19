namespace Ara3D.Studio.API;

public class EvalContext
{
    public IEvalServices Services { get; }
    public double AnimationTime { get; }
    public FlowObject Input { get; }
    public CancellationToken Cancellation { get; }
    public IDerivedDataCache DerivedData => Services.DerivedDataCache;

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
