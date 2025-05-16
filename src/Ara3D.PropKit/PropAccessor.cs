namespace Ara3D.PropKit;

/// <summary>
/// A property descriptor with functions for retrieving or values.
/// </summary>
public record PropAccessor(
    PropDescriptor Descriptor,
    Func<object> Getter,
    Action<object>? Setter)
{
    public PropValue GetValue() 
        => new(Getter(), Descriptor);

    public void SetValue(object value)
    {
        if (Descriptor.IsReadOnly)
            throw new Exception("Read only accessor");
        if (Setter == null)
            throw new Exception("No setter provided");
        Setter(Descriptor.Validate(value));
    }

    public void SetValue(PropValue propValue)
    {
        if (propValue.Descriptor != Descriptor)
            throw new Exception("Incorrect descriptor");
        SetValue(propValue.Value);
    }
}