using Ara3D.Utils;

namespace Ara3D.PropKit;

/// <summary>
/// A class that combines a property descriptor with functions for retrieving or values.
/// </summary>
public class PropAccessor<TTarget, TValue> : IPropAccessor
{
    public PropAccessor(IPropValidator validator, Delegate getter, Delegate? setter = null)
    {
        Validator = validator;
        Getter = (Getter<TTarget, TValue>)getter;
        Setter = (Setter<TTarget, TValue>)setter;
    }

    public IPropValidator Validator { get; }
    public Getter<TTarget, TValue> Getter { get; }
    public Setter<TTarget, TValue> Setter { get; }
    public bool HasSetter => Setter != null;

    public object GetValue(object host)
        => Getter((TTarget)host);

    public void SetValue(ref object host, object value)
    {
        if (Setter == null)
            throw new Exception("No setter provided");
        value = Validator?.Coerce(value) ?? value;
        if (typeof(TTarget).IsValueType)
        {
            var t = (TTarget)host;
            Setter(ref t, (TValue)value);
            host = t!;
        }
        else
        {
            var t = (TTarget)host;
            Setter(ref t, (TValue)value);
        }
    }
}