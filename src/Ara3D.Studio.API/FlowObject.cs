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

    // TODO: maybe this is where the attributes might get discarded if no longer valid. 
    public FlowObject WithNewContent(object content)
        => new(content, Presentation, Attributes, Attachments);

    public FlowObject WithNewPresentation(FlowPresentation presentation)
        => new(Content, Presentation, Attributes, Attachments);

    public FlowObject WithNewAttributes(IReadOnlyList<FlowAttribute> attributes)
        => new(Content, Presentation, attributes, Attachments);

    public FlowObject WithNewAttachments(IReadOnlyList<object> attachments)
        => new(Content, Presentation, Attributes, attachments);

    public FlowObject WithNewRenderSettings(RenderSettings settings)
        => WithNewPresentation(Presentation with { RenderSettings = settings });

    public FlowObject WithNewMaterial(Material material)
        => WithNewPresentation(Presentation with { Material = material });
    
    public bool HasObject
        => Content != null;

    public FlowObject Transform(Transform3D t)
    {
        // TODO: this needs to be completed. To implement it easily "ITransformable3D" should exist without it requiring an argument
        // The "challenge" with that is that I can't easily chain implementations together. Maybe? 
        // Or I have both ... ITransformable3D and ITransformable3D<T>, 
        throw new NotImplementedException("Work in progress");
    }

    public IEnumerable<T> GetAttachments<T>()
        => Attachments.OfType<T>();

    public T? GetAttachment<T>()
        => GetAttachments<T>().FirstOrDefault();
}

public record FlowPresentation(
    RenderSettings? RenderSettings,
    Material Material,
    bool OverrideMaterial);