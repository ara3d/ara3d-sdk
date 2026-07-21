using System.Reflection;
using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.Studio.API;

/// <summary>
/// Shared reflection-based invoker for objects exposing an `Eval` method. Every host
/// (Studio scene graph, WPF controls, headless runners) must delegate to this so that
/// argument binding, input conversion, the mesh-map fast path, and FlowObject wrapping
/// behave identically everywhere.
/// </summary>
public sealed class EvalInvoker
{
    public object Evaluator { get; }
    public MethodInfo Method { get; }
    public Type ReturnType => Method.ReturnType;
    public IReadOnlyList<Type> ArgTypes { get; }
    public Type? InputType { get; }
    public int ContextArgPosition { get; } = -1;
    public int InputArgPosition { get; } = -1;

    private readonly object?[] _args;

    /// <summary>
    /// True if the evaluator can observe the pipeline input: either it takes a data
    /// argument, or it takes an EvalContext (which exposes Input). Only a truly
    /// argument-less Eval() is input-independent.
    /// </summary>
    public bool ConsumesInput
        => InputArgPosition >= 0 || ContextArgPosition >= 0;

    public EvalInvoker(object evaluator)
    {
        Evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        Method = evaluator.GetType().GetMethod("Eval")
            ?? throw new InvalidOperationException($"Type {evaluator.GetType().Name} has no Eval method.");
        var argTypes = Method.GetParameters().Select(p => p.ParameterType).ToArray();
        ArgTypes = argTypes;
        for (var i = 0; i < argTypes.Length; i++)
        {
            if (argTypes[i] == typeof(EvalContext))
                ContextArgPosition = i;
            else
            {
                InputType = argTypes[i];
                InputArgPosition = i;
            }
        }
        _args = new object?[argTypes.Length];
    }

    /// <summary>
    /// Binds the context and (converted) input arguments and invokes Eval.
    /// A TriangleMesh3D-to-TriangleMesh3D modifier fed a model is mapped over every
    /// mesh in mesh-local space, preserving instances, instead of baking the model
    /// to a single world-space mesh via conversion.
    /// </summary>
    public object? Invoke(EvalContext context)
    {
        if (ContextArgPosition >= 0)
            _args[ContextArgPosition] = context;

        if (InputArgPosition < 0)
            return Method.Invoke(Evaluator, _args);

        if (InputType == typeof(TriangleMesh3D)
            && ReturnType == typeof(TriangleMesh3D)
            && context.Input.Content is IModel3D model)
        {
            var meshes = new List<TriangleMesh3D>();
            foreach (var mesh in model.Meshes)
            {
                _args[InputArgPosition] = mesh;
                meshes.Add((TriangleMesh3D)Method.Invoke(Evaluator, _args)!);
            }
            return model.WithMeshes(meshes);
        }

        _args[InputArgPosition] = FlowObjectConverters.Convert(context.Input, InputType!);
        return Method.Invoke(Evaluator, _args);
    }

    /// <summary>
    /// Wraps a raw Eval result as the FlowObject passed to the next node: FlowObject
    /// results pass through, assets contribute their value and attachments, and any
    /// other result replaces the content of the incoming FlowObject.
    /// </summary>
    public FlowObject? ToFlowObject(object? result, EvalContext context)
    {
        if (ReturnType == typeof(FlowObject))
            return result as FlowObject;

        if (result is IAsset asset)
            return context.Input
                .WithNewContent(asset.Value)
                .WithNewAttachments(asset.Attachments);

        return context.Input.WithNewContent(result!);
    }

    public FlowObject? Evaluate(EvalContext context)
        => ToFlowObject(Invoke(context), context);
}
