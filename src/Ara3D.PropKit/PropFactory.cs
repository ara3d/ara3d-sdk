using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Ara3D.Utils;

namespace Ara3D.PropKit;

/// <summary>
/// Creates property descriptors and accessors from the fields and properties of a type.
/// In the future we will look at attributes for additional clues.
/// </summary>
public static class PropFactory
{
    public static void GetRangeAsInt(RangeAttribute? rangeAttr, out int def, out int min, out int max)
    {
        min = (int)(rangeAttr?.Minimum ?? -1000);
        max = (int)(rangeAttr?.Maximum ?? +1000);
        def = Math.Clamp(0, min, max);
    }

    public static void GetRangeAsFloat(RangeAttribute? rangeAttr, out float def, out float min, out float max)
    {
        min = (float)(rangeAttr?.Minimum.CastToDouble() ?? -1000.0);
        max = (float)(rangeAttr?.Maximum.CastToDouble() ?? +1000.0);
        def = Math.Clamp(0, min, max);
    }

    public static void GetRangeAsDouble(RangeAttribute? rangeAttr, out double def, out double min, out double max)
    {
        min = (rangeAttr?.Minimum.CastToDouble() ?? -1000.0);
        max = (rangeAttr?.Maximum.CastToDouble() ?? +1000.0);
        def = Math.Clamp(0, min, max);
    }

    public static PropProviderWrapper GetBoundPropProvider(this object obj)
        => new(obj, new PropProvider(obj.GetPropAccessors()));

    public static IEnumerable<PropAccessor> GetPropAccessors(this object obj)
        => obj.GetType().GetPropAccessors(obj);

    public static PropProvider GetPropProvider(this Type type)
        => new(GetPropAccessors(type));

    public static IEnumerable<PropAccessor> GetPropAccessors(this Type type, object hostObj = null)
    {
        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var prop in props)
        {
            if (!prop.CanRead)
                continue;
            var name = prop.Name;
            var displayName = prop.Name;
            var description = "";
            var units = "";
            var isReadOnly = !prop.CanWrite;

            var displayNameAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null)
                displayName = displayNameAttr.DisplayName;

            var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
            var optionsAttr = prop.GetCustomAttribute<OptionsAttribute>();

            Func<object, object> getter = prop.GetValue;
            Action<object, object> setter = !isReadOnly ? prop.SetValue : null;

            if (prop.PropertyType == typeof(int))
            {
                if (optionsAttr != null)
                {
                    var options = optionsAttr.GetOptions(hostObj);
                    yield return new PropAccessor(
                        new PropDescriptorStringList(options, name, displayName, description, units, isReadOnly),
                        getter, setter);
                }
                else
                {
                    GetRangeAsInt(rangeAttr, out var def, out var min, out var max);
                    yield return new PropAccessor(
                        new PropDescriptorInt(name, displayName, description, units, isReadOnly, def, min,
                            max),
                        getter, setter);
                }
            }
            else if (prop.PropertyType == typeof(float))
            {
                GetRangeAsFloat(rangeAttr, out var def, out var min, out var max);
                yield return new PropAccessor(
                    new PropDescriptorFloat(name, displayName, description, units, isReadOnly, def, min, max),
                    getter, setter);
            }
            else if (prop.PropertyType == typeof(bool))
                yield return new PropAccessor(
                    new PropDescriptorBool(name, displayName, description, units, isReadOnly),
                    getter, setter);
        }

        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        foreach (var field in fields)
        {
            var name = field.Name;
            var displayName = field.Name;
            var description = "";
            var units = "";
            var isReadOnly = field.IsInitOnly;

            var displayNameAttr = field.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null)
                displayName = displayNameAttr.DisplayName;

            var rangeAttr = field.GetCustomAttribute<RangeAttribute>();
            var optionsAttr = field.GetCustomAttribute<OptionsAttribute>();

            Func<object, object> getter = field.GetValue;
            Action<object, object> setter = !isReadOnly ? field.SetValue : null;

            if (field.FieldType == typeof(int))
            {
                if (optionsAttr != null)
                {
                    var options = optionsAttr.GetOptions(hostObj);
                    yield return new PropAccessor(
                        new PropDescriptorStringList(options, name, displayName, description, units, isReadOnly),
                        getter, setter);
                }
                else
                {
                    GetRangeAsInt(rangeAttr, out var def, out var min, out var max);
                    yield return new PropAccessor(
                        new PropDescriptorInt(name, displayName, description, units, isReadOnly, def,
                            min, max),
                        getter, setter);
                }
            }
            else if (field.FieldType == typeof(float))
            {
                GetRangeAsFloat(rangeAttr, out var def, out var min, out var max);
                yield return new PropAccessor(
                    new PropDescriptorFloat(name, displayName, description, units, isReadOnly, def, min, max),
                    getter, setter);
            }
            else if (field.FieldType == typeof(bool))
                yield return new PropAccessor(
                    new PropDescriptorBool(name, displayName, description, units, isReadOnly),
                    getter, setter);
        }
    }
}