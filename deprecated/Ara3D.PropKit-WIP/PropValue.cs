namespace Ara3D.PropKit;

/// <summary>
/// A value associated with a property descriptor 
/// </summary>
public class PropValue
{
    public object Value { get; }
    public Prop Prop { get; }
    public PropDescriptor Descriptor => Prop.Descriptor;
    public IPropAccessor Accessor => Prop.Accessor;
    public IPropValidator Validator => Prop.Validator;

    public PropValue(object value, Prop prop)
    {
        if (!prop.Descriptor.HasValidType(value))
            throw new Exception(
                $"Type of {value} is {value?.GetType()} which is not compatible with {Descriptor.GetType()}");
        Prop = prop;
        Value = Validator?.Coerce(value) ?? value;
    }

    public PropValue(Prop prop)
        : this(prop.Constraints.Default, prop)
    { }

    public bool Equals(PropValue value)
        => Value.Equals(value.Value) && SameDescriptor(value);
    
    public override bool Equals(object obj)
        => obj is PropValue value && Equals(value);

    public bool SameDescriptor(PropValue other)
        => Descriptor.Equals(other.Descriptor);

    public PropValue WithNewValue(object value)
        => new PropValue(value, Prop);

    public string Name => Descriptor.Name;
}