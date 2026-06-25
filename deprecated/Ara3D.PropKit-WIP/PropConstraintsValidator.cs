namespace Ara3D.PropKit;

public class PropConstraintsValidator : IPropValidator
{
    public PropConstraintsValidator(PropConstraints constraints)
    {
        Constraints = constraints;
    }

    public PropConstraints Constraints { get; init; }

    public object Coerce(object value)
    {
        if (value is IComparable c1 && Constraints.Min != null)
        {
            if (c1.CompareTo(Constraints.Min) < 0)
                value = Constraints.Min;
        }

        if (value is IComparable c2 && Constraints.Max != null)
        {
            if (c2.CompareTo(Constraints.Max) > 0)
                value = Constraints.Max;
        }

        return value;
    }

    public bool IsValid(object value)
    {
        if (value is IComparable c1 && Constraints.Min != null)
        {
            if (c1.CompareTo(Constraints.Min) < 0)
                return false;
        }

        if (value is IComparable c2 && Constraints.Max != null)
        {
            if (c2.CompareTo(Constraints.Max) > 0)
                return false;
        }

        return true;
    }
}