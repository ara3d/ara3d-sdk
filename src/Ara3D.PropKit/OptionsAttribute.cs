using Ara3D.Utils;

namespace Ara3D.PropKit;

public class OptionsAttribute : Attribute
{
    public string OptionsFactoryFunction { get; }

    public OptionsAttribute(string optionsFactoryFunction)
    {
        OptionsFactoryFunction = optionsFactoryFunction;
    }

    public List<string> GetOptions(object obj)
    {
        var result = obj.GetFieldOrPropOrInvokeMethod(OptionsFactoryFunction);
        if (result is not IEnumerable<string> options)
            throw new Exception($"Method {OptionsFactoryFunction} must return an IEnumerable<string>.");
        return options.ToList();
    }
}