using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.Studio.API;

/// <summary>
/// This is the object that flows through the modifier stack and graphs. It is usually a wrapper around an IGeometry.
/// It can have attributes, attachments, presentation options, and be transformed. 
/// </summary>
public sealed class FlowObject : ITransformable3D<FlowObject>
{
    public Type? Type { get; }
    public object? Content { get; }
    public bool IsNull => Content == null;
    public FlowPresentation Presentation { get; }
    public IReadOnlyList<FlowAttribute> Attributes { get; }
    public IReadOnlyList<object> Attachments { get; }
    
    public FlowObject(object? content, FlowPresentation? presentation, IReadOnlyList<FlowAttribute> attributes, IReadOnlyList<object> attachments)
    {
        Type = content?.GetType();
        Content = content;
        Presentation = presentation ?? new FlowPresentation(null, default, false);
        Attributes = attributes ?? [];
        Attachments = attachments ?? [];
    }

    /// <summary>
    /// Replaces the content, dropping attributes whose domain indexing the new content
    /// may have invalidated. By default all indexing-dependent attributes (vertex, face,
    /// edge, corner, instance domains) are dropped; a modifier that knows it preserved
    /// indexing (e.g. a transform or color change) passes the domains it kept.
    /// </summary>
    public FlowObject WithNewContent(object content, FlowAttribute.AttributeDomainMask preserved = FlowAttribute.AttributeDomainMask.None)
        => new(content, Presentation, FilterAttributes(Attributes, preserved), Attachments);

    private static IReadOnlyList<FlowAttribute> FilterAttributes(IReadOnlyList<FlowAttribute> attributes, FlowAttribute.AttributeDomainMask preserved)
    {
        if (attributes.Count == 0 || preserved == FlowAttribute.AttributeDomainMask.All)
            return attributes;
        var result = new List<FlowAttribute>(attributes.Count);
        for (var i = 0; i < attributes.Count; i++)
        {
            var dependence = attributes[i].DomainDependence;
            if (dependence == FlowAttribute.AttributeDomainMask.None || (dependence & preserved) == dependence)
                result.Add(attributes[i]);
        }
        return result;
    }

    public FlowObject WithNewPresentation(FlowPresentation presentation)
        => new(Content, presentation, Attributes, Attachments);

    public FlowObject WithNewAttributes(IReadOnlyList<FlowAttribute> attributes)
        => new(Content, Presentation, attributes, Attachments);

    public FlowObject WithNewAttachments(IReadOnlyList<object> attachments)
        => new(Content, Presentation, Attributes, attachments);

    public FlowObject WithNewRenderSettings(RenderSettings settings)
        => WithNewPresentation(Presentation with { RenderSettings = settings });

    public FlowObject WithNewMaterial(Material material)
        => WithNewPresentation(Presentation with { Material = material });

    /// <summary>
    /// Per-object <see cref="FlowPresentation.RenderSettings"/> when stamped; otherwise the
    /// host-global fallback (e.g. from <see cref="EvalContext.RenderSettings"/>).
    /// </summary>
    public RenderSettings EffectiveRenderSettings(RenderSettings global)
        => Presentation.RenderSettings ?? global;

    public bool HasObject
        => Content != null;

    /// <summary>
    /// Applies a rigid transform to the flowed content, dispatched by type through
    /// <see cref="FlowTransformRegistry"/> (studio-217). The content type is preserved (a curve
    /// stays a curve, a line mesh stays a line mesh); a rigid transform keeps every domain's
    /// indexing, so all attributes carry over. A content type with no transform card throws a
    /// named error, surfaced non-destructively by studio-221.
    /// </summary>
    public FlowObject Transform(Transform3D t)
        => Content == null
            ? this
            : WithNewContent(FlowTransformRegistry.Transform(Content, t), FlowAttribute.AttributeDomainMask.All);

    public IEnumerable<T> GetAttachments<T>()
        => Attachments.OfType<T>();

    public T? GetAttachment<T>()
        => GetAttachments<T>().FirstOrDefault();
}

public record FlowPresentation(
    RenderSettings? RenderSettings,
    Material Material,
    bool OverrideMaterial);