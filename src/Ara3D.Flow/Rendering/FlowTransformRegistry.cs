using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.Studio.API;

/// <summary>
/// The transform analogue of <see cref="FlowRenderRegistry"/> (tracker studio-217): maps a flowed
/// value's type to a function that applies a rigid <see cref="Transform3D"/> and returns a value of
/// the same (or a naturally compatible) type. <see cref="FlowObject.Transform"/> dispatches here
/// instead of switching on type, so teaching a new flow type to transform is one registration, not
/// a host edit.
///
/// Resolution mirrors <see cref="FlowRenderRegistry"/>: an exact type match wins; otherwise the
/// first registered card whose key is assignable from the value's type (so <c>ICurve3D</c> covers
/// every analytic curve, <c>IModel3D</c> covers every model). A value with no card throws a named,
/// user-readable error — surfaced non-destructively by studio-221 rather than clearing the viewport.
/// </summary>
public static class FlowTransformRegistry
{
    private static readonly Dictionary<Type, Func<object, Transform3D, object>> Cards = new();

    static FlowTransformRegistry()
        => RegisterBuiltins();

    /// <summary>Registers a transform for a concrete flow type, replacing any existing card.</summary>
    public static void Register<T>(Func<T, Transform3D, object> transform)
        => Cards[typeof(T)] = (v, t) => transform((T)v, t);

    /// <summary>
    /// Sugar for a type that already implements <see cref="ITransformable3D{Self}"/>: registers its
    /// own Transform, erasing the generic at registration so the dispatch table stays non-generic
    /// (this is what lets <see cref="FlowObject.Transform"/> avoid the non-generic-interface redesign).
    /// </summary>
    public static void RegisterTransformable<T>() where T : ITransformable3D<T>
        => Register<T>((v, t) => v.Transform(t));

    /// <summary>Registers a type that has no meaningful 3D transform: applying one is a named error.</summary>
    public static void RegisterUnsupported<T>()
        => Register<T>((_, _) => throw new NotSupportedException($"Transform does not apply to {typeof(T).Name}"));

    /// <summary>Applies a rigid transform to a flowed value, or throws a named error if unsupported.</summary>
    public static object Transform(object value, Transform3D t)
        => (FindCard(value.GetType())
            ?? throw new NotSupportedException($"Transform does not apply to {value.GetType().Name}"))(value, t);

    private static Func<object, Transform3D, object>? FindCard(Type type)
    {
        if (Cards.TryGetValue(type, out var exact))
            return exact;
        foreach (var kv in Cards)
            if (kv.Key.IsAssignableFrom(type))
                return kv.Value;
        return null;
    }

    private static void RegisterBuiltins()
    {
        // Meshes and models — already ITransformable3D<Self>. IModel3D covers Model3D and
        // RenderModelData (both implement it) via the assignable-from fallback.
        RegisterTransformable<TriangleMesh3D>();
        RegisterTransformable<QuadMesh3D>();
        RegisterTransformable<LineMesh3D>();
        RegisterTransformable<InstanceStruct>();
        RegisterTransformable<IModel3D>();
        RegisterTransformable<ColoredTriangleMesh3D>();

        // Deformable generated geometry that stops short of ITransformable3D<Self>: a rigid
        // transform is just deforming every point through it.
        Register<QuadGrid3D>((g, t) => g.Deform(t.TransformPoint));
        Register<PolyLine3D>((p, t) => p.Deform(t.TransformPoint));

        // Parametric / procedural geometry.
        RegisterTransformable<Curve3D>();
        RegisterTransformable<ParametricSurface>();
        RegisterTransformable<Sdf3D>();
        // Any analytic curve (Helix, Spiral, ...) becomes an explicit transformed Curve3D so the
        // result stays a first-class, samplable curve.
        Register<ICurve3D>((c, t) => new Curve3D(x => t.TransformPoint(c.Eval(x))));

        // Flow container types.
        RegisterTransformable<Polyline3D>();
        // Compose on the right, matching InstanceStruct.Transform (WithMatrix(Matrix4x4 * t)) and
        // Model3DExtensions.Transform, so a TransformList's instances move exactly like a model's.
        Register<TransformList>((list, t) =>
            new TransformList(list.Transforms.Select(m => m * t.Matrix4x4).ToList()));

        // Types with no 3D transform: a named error, kept visible by studio-221's passthrough.
        RegisterUnsupported<Profile2D>();
        RegisterUnsupported<Bitmap2D>();
        RegisterUnsupported<ScalarGrid3D>();
        RegisterUnsupported<VoxelizedField>();
    }
}
