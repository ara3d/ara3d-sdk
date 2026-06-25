using Ara3D.Utils;

namespace Ara3D.PropKit;

public record PropDescriptor
{
    public Type Type { get; init; }
    public string Name { get; init; }
    public bool IsReadOnly { get; init; }
    public string DisplayName { get; init; }
    public string Description { get; init; }
    public IReadOnlyList<Attribute> Attributes { get; init; }

    public PropDescriptor(Type type, string name, bool isReadOnly,
        string displayName = null, string description = null, IReadOnlyList<Attribute> attributes = null)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name required") : name;
        IsReadOnly = isReadOnly;
        DisplayName = displayName ?? name.SplitCamelCase();
        Description = description ?? "";
        Attributes = attributes ?? [];
    }

    public bool CanSupportNull 
        => !Type.IsValueType;

    public bool HasValidType(object o)
    {
        if (o == null) return CanSupportNull;
        return o.GetType() == Type;
    }
}