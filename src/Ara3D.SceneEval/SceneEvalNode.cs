using System.ComponentModel;
using Ara3D.PropKit;
using Ara3D.Utils;

namespace Ara3D.SceneEval;

/// <summary>
/// Wraps an object that has an `Eval` method and an optional `Name` property.
/// The number of inputs must match the number of parameters in the `Eval` method.
/// All public fields and properties on that object are exposed as an `IPropContainer` 
/// </summary>
public class SceneEvalNode : IDisposable, INotifyPropertyChanged
{
    public SceneEvalGraph Graph { get; }
    public SceneEvalNode Input { get; set; }
    public PropProviderWrapper Properties { get; private set; }
    public object EvaluatableObject { get; private set; }
    public string Name { get; private set; }
    public event EventHandler Invalidated;
    private object _cached;
    private object[] _args;
    private Func<object[], object> _evalFunc;
    public event PropertyChangedEventHandler PropertyChanged;

    public SceneEvalNode(SceneEvalGraph graph, object evaluableObject)
    {
        Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        
        UpdateEvaluatableObject(evaluableObject);

        Properties = evaluableObject.GetBoundPropProvider();
        Properties.PropertyChanged += (s, e) => InvalidateCache();
    }

    public object GetCachedObject()
        => _cached;

    public object Eval(EvalContext context)
    {
        if (context.CancellationToken.IsCancellationRequested)
            throw new OperationCanceledException(context.CancellationToken);
        if (_cached == null)
            Interlocked.CompareExchange(ref _cached, EvalCore(context), null);
        return _cached;
    }

    public void InputInvalidated(object sender, EventArgs e)
    {
        InvalidateCache();
    }

    public void SetInput(SceneEvalNode input)
    {
        if (Input != null)
            input.Invalidated -= InputInvalidated;
        Input = input;
        Input.Invalidated += InputInvalidated;
        InvalidateCache();
        Graph.NotifyGraphChanged();
    }

    private object EvalCore(EvalContext context)
    {
        if (Input != null)
            _args[0] = Input.Eval(context);
        _args[^1] = context;
        return _cached = _evalFunc(_args);
    }

    public void InvalidateCache()
    {
        if (_cached == null)
            return;
        _cached = null;
        Invalidated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Looking at only primary dependencies can be adapted to provide a "stack" view of an object
    /// evaluation graph or a "tree" where the first item in the path is a parent and the rest are children.
    /// The first item in the list is the root.  
    /// </summary>
    public IReadOnlyList<SceneEvalNode> GetPrimaryDependencyPath()
    {
        var list = new List<SceneEvalNode>();
        var cur = this;
        while (cur != null)
        {
            list.Add(cur);
            cur = cur.Input; 
        }
        list.Reverse();
        return list;
    }

    public bool IsSource
        => Input == null;

    public SceneEvalNode GetRoot()
        => Graph.GetRoot(this);

    public void Dispose()
    {
        if (Input != null)
            Input.Invalidated -= InputInvalidated;
        Properties.Dispose();
    }

    public IEnumerable<SceneEvalNode> GetAllNodes()
        => GetPrimaryDependencyPath().Append(this);

    private string GetName(object obj)
    {
        var type = obj.GetType();
        var prop = type.GetProperty("Name");
        var name = prop?.GetValue(obj)?.ToString();
        return name ?? type.Name.SplitCamelCase();
    }

    private (object[], Func<object[], object>) GetArgsAndEvalFunction(object obj)
    {
        var type = obj.GetType();
        var func = type.GetMethod("Eval");
        if (func == null)
            throw new InvalidOperationException($"The object {obj} does not have an Eval method.");
        var args = new object[func.GetParameters().Length];
        return (args, (localArgs) => func.Invoke(EvaluatableObject, localArgs));
    }

    public void UpdateEvaluatableObject(object obj)
    {
        if (obj == null) throw new Exception("Evaluatable object cannot be null.");
        (_args, _evalFunc) = GetArgsAndEvalFunction(obj);
        Name = GetName(obj);
        var old = EvaluatableObject;
        EvaluatableObject = obj;
        var newWrapper = obj.GetBoundPropProvider();
        if (Properties != null)
        {
            newWrapper.CopyValuesFrom(Properties, old, obj);
            Properties.Dispose();
        }
        Properties = newWrapper;
        Properties.PropertyChanged += (s, e) => InvalidateCache();
    }
}