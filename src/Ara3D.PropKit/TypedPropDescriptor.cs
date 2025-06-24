namespace Ara3D.PropKit;

public abstract class TypedPropDescriptor<T> : PropDescriptor
{
    protected TypedPropDescriptor(string name, string displayName, string description, string units, bool isReadOnly, Dictionary<string, string> tags = null)
        : base(typeof(T), name, displayName, description, units, isReadOnly, tags) { }

    public abstract T Update(T value, PropUpdateType propUpdate);
    public abstract T Validate(T value);
    public abstract bool IsValid(T value);
    public abstract bool AreEqual(T value1, T value2);
    public abstract string ToString(T value);

    public override object Update(object value, PropUpdateType propUpdate)
        => Update((T)value, propUpdate);

    public override bool IsValid(object value)
        => value is T v && IsValid(v);

    public override bool AreEqual(object value1, object value2)
        => value1 is T v1 && value2 is T v2 && AreEqual(v1, v2);

    public override object FromString(string value)
        => Validate(FromString(value));

    public override string ToString(object value)
        => ToString((T)value);

    public override object Validate(object value)
    {
        var r = Validate((T)value);
        if (!IsValid(r))
            throw new Exception($"Unable to validate {value}");
        return r;
    }

    public override bool IsValidString(string value) => TryParse(value, out var parsed) && IsValid(parsed);
    protected abstract bool TryParse(string value, out T parsed);

    public new T Default => Update(default, PropUpdateType.Default);
    public new T Min => Update(default, PropUpdateType.Min);
    public new T Max => Update(default, PropUpdateType.Max);
}