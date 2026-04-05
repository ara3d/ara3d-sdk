namespace Ara3D.Studio.API;

/// <summary>
/// Eventually may provide a "CancelToken"
/// </summary>
public class EvalContext
{
    public IHostApplication Application { get; }
    public double AnimationTime { get; }
    public FlowObject Input { get; }

    public EvalContext(FlowObject input, IHostApplication application, double animationTime)
    {
        Input = input;
        Application = application;
        AnimationTime = animationTime;
    }

    public EvalContext WithInput(FlowObject newInput)
        => new(newInput, Application, AnimationTime);
}