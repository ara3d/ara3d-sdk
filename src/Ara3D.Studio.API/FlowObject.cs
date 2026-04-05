using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.Studio.API;

/// <summary>
/// This is the primary type of object that flows through the modifier stack and graphs.
/// It can have attachments and be transformed. 
/// </summary>
public sealed class FlowObject : ITransformable3D<FlowObject>
{
    public Type? Type { get; }
    public object? Value { get; }
    public bool IsNull => Value == null;
    public RenderSettings? RenderSettings { get; }
    public Material Material { get; }
    public bool OverrideMaterial { get; }

    // Attachments are workflow specific
    public IReadOnlyList<object> Attachments { get; }
    
    public FlowObject(object? value, RenderSettings? renderSettings, Material material, bool overrideMaterial, IReadOnlyList<object> attachments)
    {
        Type = value?.GetType();
        Value = value;
        RenderSettings = renderSettings;
        Material = material;
        OverrideMaterial = overrideMaterial;
        Attachments = attachments ?? [];
    }

    public FlowObject WithNewValue(object value)
        => new(value, RenderSettings, Material, OverrideMaterial, Attachments);

    public FlowObject WithNewRenderSettings(RenderSettings renderSettings)
        => new(Value, renderSettings, Material, OverrideMaterial, Attachments);

    public FlowObject WithMaterial(Material material)
        => new(Value, RenderSettings, material, true, Attachments);

    public FlowObject WithNewAttachments(IReadOnlyList<object> attachments)
        => new(Value, RenderSettings, Material, OverrideMaterial, attachments);

    public bool HasObject
        => Value != null;

    public FlowObject Transform(Transform3D t)
    {
        throw new NotImplementedException("Work in progress");
    }

    public IEnumerable<T> GetAttachments<T>()
        => Attachments.OfType<T>();

    public T? GetAttachment<T>()
        => GetAttachments<T>().FirstOrDefault();
}