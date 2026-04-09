using System.ComponentModel;
using System.Reflection;
using Ara3D.PropKit;
using Ara3D.Studio.API;
using Ara3D.Utils;

namespace Ara3D.Studio.WpfControls;

// NOTE: this could be the new base class for SceneEvalNode 
// It is doing some generally useful things. 
// The important stuff is that it make "Ara 3D components" executable outside of Ara 3D Studio.
// THAT will be wildly interesting and important. 

public class EvaluatorWrapper
{
    public bool IsAnimated { get; private set; } = false;
    public PropProviderWrapper PropProvider { get; private set; }
    public object Evaluator { get; private set; }
    public Type EvaluatorType { get; private set; }
    public Attribute[] EvaluatorAttributes { get; private set; }
    public string Name { get; private set; }
    private object[] _args;
    private Func<object[], object> _evalFunc;
    public Type ReturnType { get; private set; }
    public Type InputType { get; private set; }
    public Type[] ArgTypes { get; private set; }
    public int ContextArgPosition { get; private set; }
    public event PropertyChangedEventHandler PropertyChanged;
    private bool _enabled = true;
    private bool _errorState = false;
    private string _errorMessage = "";
    private bool _needsEvaluation = true;
    public bool NeedsEvaluation => _needsEvaluation && _enabled;

    public bool InvalidateOnPropChange { get; set; }

    public EvaluatorWrapper(object evaluableObject)
    {
        UpdateEvaluatableObject(evaluableObject);
    }

    public override string ToString()
        => $"{GetType().Name}:{Name}";

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value)
                return;
            _enabled = value;
            RefreshUI();
        }
    }

    public bool ErrorState
        => _errorState;

    public string ErrorMessage
        => _errorMessage;

    public void ClearErrorState()
    {
        if (_errorState)
        {
            _errorState = false;
            _errorMessage = "";
        }
    }

    public void SetErrorState(string message = "")
    {
        if (_errorState != true || _errorMessage != message)
        {
            _errorState = true;
            _errorMessage = message;
            RefreshUI();
        }
    }

    public object Evaluate(EvalContext context)
    {
        try
        {
            _needsEvaluation = false;
            ClearErrorState();
            if (ContextArgPosition == 0)
            {
                if (_args.Length != 1)
                    throw new Exception("Expected one argument");
                _args[ContextArgPosition] = context;
            }
            else if (ContextArgPosition == 1)
            {
                if (_args.Length != 2)
                    throw new Exception("Expected two arguments");
                _args[ContextArgPosition] = context;
                _args[0] = FlowObjectConverters.Convert(context.Input, ArgTypes[0]);
            }
            else if (_args.Length == 1)
            {
                _args[0] = FlowObjectConverters.Convert(context.Input, ArgTypes[0]);
            }
            else if (_args.Length == 0)
            {
                // No arguments
            }
            else
            {
                throw new Exception($"Expected maximum of 2 arguments, not {_args.Length}");
            }

            return _evalFunc(_args);
        }
        catch (Exception ex)
        {
            SetErrorState(ex.Message);
            return null;
        }
    }

    public void RefreshUI()
    {
        PropProvider.NotifyPropertyChanged();
    }
    
    public void Dispose()
    {
        PropProvider.Dispose();
    }

    private (object[], Func<object[], object>) GetArgsAndEvalFunction(object obj)
    {
        var type = obj.GetType();
        var func = type.GetMethod("Eval");
        if (func == null)
            throw new InvalidOperationException($"The object {obj} does not have an Eval method.");
        ReturnType = func.ReturnType;
        ArgTypes = Enumerable.ToArray(func.GetParameters().Select(p => p.ParameterType));
        InputType = ArgTypes.Length > 0 && ArgTypes[0] != typeof(EvalContext)
            ? ArgTypes[0] : null;
        var args = new object[func.GetParameters().Length];
        return (args, (localArgs) => func.Invoke(Evaluator, localArgs));
    }

    public void UpdateEvaluatableObject(object obj)
    {
        if (obj == null) throw new Exception("Evaluatable object cannot be null.");

        Evaluator = obj;
        EvaluatorAttributes = [];

        (_args, _evalFunc) = GetArgsAndEvalFunction(obj);
        ContextArgPosition = Array.IndexOf(ArgTypes, typeof(EvalContext));

        var newWrapper = obj.GetBoundPropProvider();

        // Remove the old property provider, but first copy the values from it
        if (PropProvider != null)
        {
            var props = PropProvider.GetPropValues();
            foreach (var prop in props)
                if (!prop.Descriptor.IsReadOnly)
                    newWrapper.TrySetValue(prop.Descriptor, prop.Value);

            PropProvider.Dispose();
        }

        // Get the evaluatable object type 
        EvaluatorType = obj.GetType();

        // Get the name of the object (from a property if present, or the type if not).
        var nameProp = EvaluatorType.GetProperty("Name");
        Name = nameProp?.GetValue(obj)?.ToString() ?? EvaluatorType.Name.SplitCamelCase();

        InvalidateOnPropChange = EvaluatorType.GetCustomAttribute(typeof(OnDemandAttribute)) == null;

        IsAnimated = Evaluator is IAnimated
                     || EvaluatorType.GetCustomAttribute(typeof(AnimatedAttribute)) != null;

        PropProvider = newWrapper;
        PropProvider.PropertyChanged += PropProviderOnPropertyChanged;
    }

    private void PropProviderOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (InvalidateOnPropChange)
        {
            _needsEvaluation = true;
        }
    }
}