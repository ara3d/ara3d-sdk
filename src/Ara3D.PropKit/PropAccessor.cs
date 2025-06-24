namespace Ara3D.PropKit;

/// <summary>
/// A property descriptor with functions for retrieving or values.
/// </summary>
public record PropAccessor(
    PropDescriptor Descriptor,
    Func<object, object> Getter,
    Action<object, object>? Setter)
{
    public PropValue GetValue(object host) 
        => new(Getter(host), Descriptor);

    public void SetValue(object host, object value)
    {
        if (Descriptor.IsReadOnly)
            throw new Exception("Read only accessor");
        if (Setter == null)
            throw new Exception("No setter provided");
        Setter(host, Descriptor.Validate(value));
    }

    public void SetValue(object host, PropValue propValue)
    {
        if (propValue.Descriptor != Descriptor)
            throw new Exception("Incorrect descriptor");
        SetValue(host, propValue.Value);
    }
}