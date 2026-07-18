namespace Ara3D.PropKit;

/// <summary>
/// Presents an enum-valued member as an int index into its list of names,
/// so it can be edited with a string-list ComboBox (PropDescriptorStringList).
/// Enum.GetValues is aligned with Enum.GetNames (the ComboBox options), so the
/// index mapping is correct even for non-contiguous or non-zero-based enums.
/// </summary>
public sealed class EnumPropAccessor : IPropAccessor
{
    private readonly IPropAccessor _inner;
    private readonly Array _values;

    public EnumPropAccessor(PropDescriptor descriptor, Type enumType, IPropAccessor inner)
    {
        Descriptor = descriptor;
        _inner = inner;
        _values = Enum.GetValues(enumType);
    }

    public PropDescriptor Descriptor { get; }
    public bool HasSetter => _inner.HasSetter;

    public object GetValue(object host)
    {
        var index = Array.IndexOf(_values, _inner.GetValue(host));
        return index < 0 ? 0 : index;
    }

    public void SetValue(ref object host, object value)
    {
        var index = Math.Clamp(Convert.ToInt32(value), 0, _values.Length - 1);
        _inner.SetValue(ref host, _values.GetValue(index)!);
    }
}
