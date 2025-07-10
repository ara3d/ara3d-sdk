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

    public static PropAccessor CreatePropAccessor(Type type, object hostObj, RangeAttribute rangeAttr, OptionsAttribute optionsAttr, string name, string displayName, string description, string units, Func<object, object> getter, Action<object, object> setter)
    {
        var isReadOnly = setter == null;
            
        if (type.IsEnum)
        {
            var names = Enum.GetNames(type);
            if (names.Length == 0)
            {
                type = Enum.GetUnderlyingType(type);
            }
            else
            {
                return new PropAccessor(
                    new PropDescriptorStringList(names, name, displayName, description, units, isReadOnly),
                    getter, setter);
            }
        }

        if (type == typeof(int))
        {
            if (optionsAttr != null)
            {
                var options = optionsAttr.GetOptions(hostObj);
                return new PropAccessor(
                    new PropDescriptorStringList(options, name, displayName, description, units, isReadOnly),
                    getter, setter);
            }
            else
            {
                GetRangeAsInt(rangeAttr, out var def, out var min, out var max);
                return new PropAccessor(
                    new PropDescriptorInt(name, displayName, description, units, isReadOnly, def, min, max),
                    getter, setter);
            }
        }
        else if (type == typeof(long))
        {
            GetRangeAsInt(rangeAttr, out var def, out var min, out var max);
            return new PropAccessor(
                new PropDescriptorLong(name, displayName, description, units, isReadOnly, def, min, max),
                getter, setter);
        }
        else if (type == typeof(float))
        {
            GetRangeAsFloat(rangeAttr, out var def, out var min, out var max);
            return new PropAccessor(
                new PropDescriptorFloat(name, displayName, description, units, isReadOnly, def, min, max),
                getter, setter);
        }
        else if (type == typeof(double))
        {
            GetRangeAsDouble(rangeAttr, out var def, out var min, out var max);
            return new PropAccessor(
                new PropDescriptorDouble(name, displayName, description, units, isReadOnly, def, min, max),
                getter, setter);
        }
        else if (type == typeof(bool))
        {
            return new PropAccessor(
                new PropDescriptorBool(name, displayName, description, units, isReadOnly),
                getter, setter);
        }
        else if (type == typeof(string))
        {
            return new PropAccessor(
                new PropDescriptorString(name, displayName, description, units, isReadOnly),
                getter, setter);
        }
        else
        {
            return new PropAccessor(
                new GenericPropDescriptor(null, type, name, displayName, description, units, isReadOnly),
                getter, setter);
        }
    }

    public static PropAccessor CreatePropAccessor(Type type, object hostObj, MemberInfo mi, Func<object, object> getter,
        Action<object, object> setter)
    {
        var name = mi.Name;
        var displayName = mi.Name;
        var description = "";
        var units = "";

        var displayNameAttr = mi.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttr != null)
            displayName = displayNameAttr.DisplayName;

        var rangeAttr = mi.GetCustomAttribute<RangeAttribute>();
        var optionsAttr = mi.GetCustomAttribute<OptionsAttribute>();
        
        return CreatePropAccessor(type, hostObj, rangeAttr, optionsAttr, name, displayName,
            description, units, getter, setter);
    }

    public static IEnumerable<PropAccessor> GetPropAccessors(this Type type, object hostObj = null)
    {
        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var prop in props)
        {
            if (!prop.CanRead)
                continue;

            var isReadOnly = !prop.CanWrite;

            Func<object, object> getter = prop.GetValue;
            Action<object, object> setter = !isReadOnly ? prop.SetValue : null;

            yield return CreatePropAccessor(prop.PropertyType, hostObj, prop, getter, setter);
        }

        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        foreach (var field in fields)
        {
            var isReadOnly = field.IsInitOnly;

            Func<object, object> getter = field.GetValue;
            Action<object, object> setter = !isReadOnly ? field.SetValue : null;

            yield return CreatePropAccessor(field.FieldType, hostObj, field, getter, setter);

        }
    }
}