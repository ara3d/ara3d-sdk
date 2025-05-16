namespace Ara3D.PropKit;

/// <summary>
/// A value associated with a property descriptor 
/// </summary>
public class PropValue
{
    public object Value { get; }
    public PropDescriptor Descriptor { get; }

    public PropValue(object value, PropDescriptor desc)
    {
        Value = desc.Validate(value);
        Descriptor = desc;
    }

    public PropValue(PropDescriptor desc)
        : this(desc.Default, desc)
    { }

    public bool Equals(PropValue value)
        => Value.Equals(value.Value) && SameDescriptor(value);
    
    public override bool Equals(object obj)
        => obj is PropValue value && Equals(value);

    public bool SameDescriptor(PropValue other)
        => Descriptor.Equals(other.Descriptor);

    public PropValue WithNewValue(object value)
        => new PropValue(value, Descriptor);

    public string Name => Descriptor.Name;
}