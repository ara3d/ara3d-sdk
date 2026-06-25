using Ara3D.Utils;

namespace Ara3D.PropKit;

public class ComputedRangeAttribute : Attribute
{
    public string RangeFactoryFunction { get; }

    public ComputedRangeAttribute(string rangeFactoryFunction)
    {
        RangeFactoryFunction = rangeFactoryFunction;
    }

    public int GetRange(object obj)
    {
        var result = obj.GetFieldOrPropOrInvokeMethod(RangeFactoryFunction);
        if (result == null)
            return 0;
        if (result is not int range)
            throw new Exception($"Method {RangeFactoryFunction} must return an int.");
        return range;
    }
}