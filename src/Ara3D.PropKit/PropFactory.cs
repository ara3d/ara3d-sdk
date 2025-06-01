using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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
        min = (float)(double)(rangeAttr?.Minimum ?? -1000.0);
        max = (float)(double)(rangeAttr?.Maximum ?? +1000.0);
        def = Math.Clamp(0, min, max);
    }

    public static IEnumerable<PropAccessor> CreateFromObject(object obj)
    {
        var type = obj.GetType();
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
            var isDeprecated = false;

            var displayNameAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null)
                displayName = displayNameAttr.DisplayName;

            var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
            
            Func<object> getter = () => prop.GetValue(obj);
            Action<object> setter = !isReadOnly ? val => prop.SetValue(obj, val) : null;

            if (prop.PropertyType == typeof(int))
            {
                GetRangeAsInt(rangeAttr, out var def, out var min, out var max);
                yield return new PropAccessor(
                    new TypedPropDescriptorInt(name, displayName, description, units, isReadOnly, isDeprecated, def, min, max),
                    getter, setter);
            }
            else if (prop.PropertyType == typeof(float))
            {
                GetRangeAsFloat(rangeAttr, out var def, out var min, out var max);
                yield return new PropAccessor(
                    new TypedPropDescriptorFloat(name, displayName, description, units, isReadOnly, isDeprecated, def, min, max),
                    getter, setter);
            }
            else if (prop.PropertyType == typeof(bool))
                yield return new PropAccessor(
                    new TypedPropDescriptorBool(name, displayName, description, units, isReadOnly, isDeprecated),
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
            var isDeprecated = false;

            var displayNameAttr = field.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null)
                displayName = displayNameAttr.DisplayName;

            var rangeAttr = field.GetCustomAttribute<RangeAttribute>();

            Func<object> getter = () => field.GetValue(obj);
            Action<object> setter = !isReadOnly ? val => field.SetValue(obj, val) : null;

            if (field.FieldType == typeof(int))
            {
                GetRangeAsInt(rangeAttr, out var def, out var min, out var max);
                yield return new PropAccessor(
                    new TypedPropDescriptorInt(name, displayName, description, units, isReadOnly, isDeprecated, def, min, max),
                    getter, setter);
            }
            else if (field.FieldType == typeof(float))
            {
                GetRangeAsFloat(rangeAttr, out var def, out var min, out var max);
                yield return new PropAccessor(
                    new TypedPropDescriptorFloat(name, displayName, description, units, isReadOnly, isDeprecated, def, min, max),
                    getter, setter);
            }
            else if (field.FieldType == typeof(bool))
                yield return new PropAccessor(
                    new TypedPropDescriptorBool(name, displayName, description, units, isReadOnly, isDeprecated),
                    getter, setter);
        }
    }
}