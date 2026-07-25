using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.Studio.API;

/// <summary>
/// The "shoebox of instruction cards" (tracker studio-166): maps a flow value's type to the
/// <see cref="IFlowRenderable"/> that turns it into drawable geometry. The host render sink and
/// converters ask this instead of switching on type, so adding a renderable type never touches
/// their code.
///
/// Resolution rules, in order:
///  1. If a card is registered for the value's type, run the card and repeat (so Sdf3D →
///     VoxelizedField → Model3D chains automatically). Cards win over direct renderability:
///     Plato curves implement IGeometry yet must be sampled by their card, not passed through.
///  2. If the value is directly renderable, return it unchanged.
///  3. Otherwise draw a labelled placeholder box, so an unhandled type is visible, never a crash
///     and never silently missing.
/// Built-in cards register themselves in <see cref="BuiltinFlowRenderers"/> via the static ctor.
/// </summary>
public static class FlowRenderRegistry
{
    private static readonly Dictionary<Type, IFlowRenderable> Cards = new();

    static FlowRenderRegistry()
        => BuiltinFlowRenderers.RegisterAll();

    /// <summary>Registers a card, replacing any existing one for the same source type.</summary>
    public static void Register(IFlowRenderable card)
        => Cards[card.SourceType] = card;

    public static void Register<T>(Func<T, FlowRenderContext, object> realize)
        => Register(new DelegateFlowRenderable(typeof(T), (v, c) => realize((T)v, c)));

    /// <summary>
    /// True if the renderer can draw this type as-is (a mesh, model, or render data). These are
    /// exactly the cases the GL sink already handles, so a card's job is to reach one of them.
    /// </summary>
    public static bool IsDirectlyRenderable(Type type)
        => type == typeof(RenderModelData)
           || typeof(IModel3D).IsAssignableFrom(type)
           || typeof(IGeometry).IsAssignableFrom(type);

    /// <summary>
    /// Resolves a flowed value to directly-renderable geometry, chaining cards as needed and
    /// falling back to a placeholder box. Never returns null for a non-null input.
    /// </summary>
    public static object ToRenderable(object value, FlowRenderContext context)
    {
        // Guard against a card cycle (A→B→A); 8 hops is far more than any real chain.
        for (var hop = 0; hop < 8 && value != null; hop++)
        {
            var card = FindCard(value.GetType());
            if (card != null)
            {
                value = card.ToRenderable(value, context);
                continue;
            }

            if (IsDirectlyRenderable(value.GetType()))
                return value;

            return PlaceholderBox(value, context);
        }
        return value ?? PlaceholderBox(value, context);
    }

    private static IFlowRenderable? FindCard(Type type)
    {
        if (Cards.TryGetValue(type, out var exact))
            return exact;
        // Allow a card registered on a base type or interface (e.g. IVoxels) to match.
        foreach (var kv in Cards)
            if (kv.Key.IsAssignableFrom(type))
                return kv.Value;
        return null;
    }

    /// <summary>
    /// The universal fallback: a wireframe box at the context's default bounds. A type with no
    /// card still shows something locatable rather than vanishing. (A text label and a real
    /// per-type bounds are the planned refinement — studio-166.)
    /// </summary>
    public static LineMesh3D PlaceholderBox(object? value, FlowRenderContext context)
        => BoxWireframe(context.DefaultBounds);

    /// <summary>12-edge wireframe of a box, as a LineMesh3D the GL sink draws directly.</summary>
    public static LineMesh3D BoxWireframe(Bounds3D bounds)
    {
        var c = bounds.Corners;
        // Corner order (from Bounds3D.Corners): 0:mmm 1:Mmm 2:mMm 3:MMm 4:mmM 5:MmM 6:mMM 7:MMM
        Integer2[] edges =
        [
            (0, 1), (1, 3), (3, 2), (2, 0), // bottom (z = min)
            (4, 5), (5, 7), (7, 6), (6, 4), // top    (z = max)
            (0, 4), (1, 5), (2, 6), (3, 7), // verticals
        ];
        return new LineMesh3D(c, edges);
    }
}

/// <summary>Adapts a plain delegate into an <see cref="IFlowRenderable"/> card.</summary>
public sealed class DelegateFlowRenderable : IFlowRenderable
{
    private readonly Func<object, FlowRenderContext, object> _realize;
    public Type SourceType { get; }

    public DelegateFlowRenderable(Type sourceType, Func<object, FlowRenderContext, object> realize)
    {
        SourceType = sourceType;
        _realize = realize;
    }

    public object ToRenderable(object value, FlowRenderContext context)
        => _realize(value, context);
}
