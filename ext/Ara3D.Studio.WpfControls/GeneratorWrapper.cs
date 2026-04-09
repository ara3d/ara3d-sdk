using Ara3D.PropKit;
using Ara3D.Studio.API;

namespace Ara3D.Studio.WpfControls;

public class GeneratorWrapper
{
    public IGenerator Generator => (IGenerator)Evaluator.Evaluator;
    public IBoundPropContainer Props => Evaluator.PropProvider;
    public EvaluatorWrapper Evaluator { get; }
    public IHostApplication DummyHostApplication { get; } = new DefaultHostApplication(); 
    
    public GeneratorWrapper(IGenerator obj)
        => Evaluator = new(obj);

    public object Evaluate()
        => Evaluate(DummyHostApplication);

    public object Evaluate(IHostApplication host)
        => Evaluate(new EvalContext(null, host, 0));

    public object Evaluate(EvalContext context)
        => Evaluator.Evaluate(context);
}