using System.Text.RegularExpressions;

namespace Ara3D.PropKit;

/// <summary>
/// Describe characteristic of a run-time modifiable property.
/// Contains a type, description, name, and more.
/// Replaces the System.Component.PropertyDescriptor.
/// </summary>
public abstract class PropDescriptor
{
    public string Name { get; }
    public string DisplayName { get; }
    public IReadOnlyDictionary<string, string> Tags { get; }
    public Type Type { get; }
    public string Description { get; }
    public string Units { get; }
    public bool IsReadOnly { get; }
    public bool IsDeprecated { get; }

    protected PropDescriptor(string name, Type type, string displayName = "", string description = "", string units = "",
        bool isReadOnly = false, bool isDeprecated = false, Dictionary<string, string> tags = null)
    {
        Name = name;
        DisplayName = displayName ?? name;
        Type = type;
        Description = description;
        Units = units;
        IsReadOnly = isReadOnly;
        IsDeprecated = isDeprecated;
        Tags = tags ?? [];
    }

    public static Regex RegexUppercaseAcronym = new ("([A-Z]+)([A-Z][a-z])", RegexOptions.Compiled); 
    public static Regex RegexUppercase = new ("([a-z0-9])([A-Z])", RegexOptions.Compiled);

    public static string SplitCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        var result = RegexUppercaseAcronym.Replace(input, "$1 $2");
        result = RegexUppercase.Replace(result, "$1 $2");
        result = char.ToUpper(result[0]) + result.Substring(1);
        return result;
    }

    public abstract object Update(object value, PropUpdateType propUpdate);
    public abstract bool IsValid(object value);
    public abstract object Validate(object value);
    public abstract bool IsValidString(string value);
    public abstract bool AreEqual(object value1, object value2);
    public abstract object FromString(string value);
    public abstract string ToString(object value);

    public object Default => Update(default, PropUpdateType.Default);
    public object Min => Update(default, PropUpdateType.Min);
    public object Max => Update(default, PropUpdateType.Max);
    
    public override string ToString()
        => $"{Name}[\"{DisplayName}\"]";

    public PropValue DefaultPropValue => new(Default, this);
    public PropValue MinPropValue => new(Min, this);
    public PropValue MaxPropValue => new(Max, this);
}

