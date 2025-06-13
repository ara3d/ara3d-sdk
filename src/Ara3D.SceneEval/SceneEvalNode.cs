using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
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
    public IPropContainer Properties { get; }
    public object EvaluatableObject { get; }
    public string Name { get; }
    public event EventHandler Invalidated;
    private object _cached;
    private object[] _args;
    private readonly Func<object[], object> _evalFunc;
    public event PropertyChangedEventHandler PropertyChanged;

    public SceneEvalNode(SceneEvalGraph graph, object evaluableObject)
    {
        Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        EvaluatableObject = evaluableObject ?? throw new ArgumentNullException(nameof(evaluableObject));
        
        var type = EvaluatableObject.GetType();

        var prop = type.GetProperty("Name");
        if (prop != null)
            Name = prop.GetValue(EvaluatableObject)?.ToString();
        Name ??= type.Name.SplitCamelCase();

        var func = type.GetMethod("Eval");
        if (func == null)
            throw new InvalidOperationException($"The object {evaluableObject} does not have an Eval method.");

        _args = new object[func.GetParameters().Length];
        _evalFunc = (args) => func.Invoke(EvaluatableObject, args);

        Properties = new PropContainerWrapper(evaluableObject);
        Properties.PropertyChanged += (s, e) => InvalidateCache();
    }

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
    }

    public IEnumerable<SceneEvalNode> GetAllNodes()
        => GetPrimaryDependencyPath().Append(this);

    /*
    // TODO: this needs to be tested and validated.
    /// <summary>
    /// Builds a delegate that calls <paramref name="method"/>.  
    /// The delegate accepts an <c>object[]</c> with the actual arguments
    /// *and* returns the boxed result (or <c>null</c> for <c>void</c>).
    ///
    /// • **Instance methods** → pass the target object in slot 0, followed by the real parameters.  
    /// • **Static methods**   → slot 0 is *not* reserved, all elements are parameters.
    /// </summary>
    public static Func<object[], object> BuildArrayInvoker(MethodInfo method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));

        // parameter: object[] args
        var arrParam = Expression.Parameter(typeof(object[]), "args");

        // Handle instance target (args[0]) if necessary
        Expression instance = null;
        int argOffset = 0;

        if (!method.IsStatic)
        {
            if (method.DeclaringType == null)
                throw new InvalidOperationException("Instance method without declaring type.");

            instance = Expression.Convert(
                Expression.ArrayIndex(arrParam, Expression.Constant(0)),
                method.DeclaringType);

            argOffset = 1; // parameter list starts after the target
        }

        // Convert each element of the array to the parameter’s declared type
        var paramInfos = method.GetParameters();
        var callArgs = new Expression[paramInfos.Length];

        for (int i = 0; i < paramInfos.Length; i++)
        {
            var valueObj =
                Expression.ArrayIndex(arrParam, Expression.Constant(i + argOffset));

            callArgs[i] = Expression.Convert(valueObj, paramInfos[i].ParameterType);
        }

        // Build the call expression
        Expression call = method.IsStatic
            ? Expression.Call(method, callArgs)
            : Expression.Call(instance!, method, callArgs);

        // If the method is void, return null; otherwise box the result
        Expression body = method.ReturnType == typeof(void)
            ? Expression.Block(call, Expression.Constant(null, typeof(object)))
            : Expression.Convert(call, typeof(object));

        // Compile: Func<object?[], object?>
        return Expression.Lambda<Func<object[], object>>(body, arrParam)
            .Compile();
    }
    */
}