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

    public static IEnumerable<IPropAccessor> GetPropAccessors(this object obj)
        => obj.GetType().GetPropAccessors(obj);

    public static PropProvider GetPropProvider(this Type type)
        => new(GetPropAccessors(type));

    public static IPropAccessor CreatePropAccessor(
        this PropDescriptor descriptor,
        Type targetType, Type valueType,
        Delegate getterRef, Delegate? setterRef)
    {
        var open = typeof(PropAccessor<,>);
        var closed = open.MakeGenericType(targetType, valueType);
        return (IPropAccessor)Activator.CreateInstance(closed, descriptor, getterRef, setterRef)!;
    }
    

    public static IPropAccessor CreatePropAccessor(this Type type, object hostObj, Type targetType, RangeAttribute rangeAttr,
        OptionsAttribute optionsAttr, string name, string displayName, string description, string units,
        Delegate getter, Delegate setter)
    {
        var isReadOnly = setter == null;
        var underlyingType = type;
        if (type.IsEnum)
        {
            var names = Enum.GetNames(type);
            if (names.Length == 0)
            {
                underlyingType = Enum.GetUnderlyingType(type);
            }
            else
            {
                return CreatePropAccessor(
                    new PropDescriptorStringList(names, name, displayName, description, units, isReadOnly),
                    targetType, type, getter, setter);
            }
        }

        if (type == typeof(int) || underlyingType == typeof(int))
        {
            if (optionsAttr != null)
            {
                return CreatePropAccessor(
                    new PropDescriptorDynamicStringList(() => optionsAttr.GetOptions(hostObj), name, displayName, description, units, isReadOnly),
                    targetType, type, getter, setter);
            }
            else
            {
                GetRangeAsInt(rangeAttr, out var def, out var min, out var max);
                return CreatePropAccessor(
                    new PropDescriptorInt(name, displayName, description, units, isReadOnly, def, min, max),
                    targetType, type, getter, setter);
            }
        }
        else if (type == typeof(long) || underlyingType == typeof(long))
        {
            GetRangeAsInt(rangeAttr, out var def, out var min, out var max);
            return CreatePropAccessor(
                new PropDescriptorLong(name, displayName, description, units, isReadOnly, def, min, max),
                targetType, type, getter, setter);
        }
        else if (type == typeof(float))
        {
            GetRangeAsFloat(rangeAttr, out var def, out var min, out var max);
            return CreatePropAccessor(
                new PropDescriptorFloat(name, displayName, description, units, isReadOnly, def, min, max),
                targetType, type, getter, setter);
        }
        else if (type == typeof(double))
        {
            GetRangeAsDouble(rangeAttr, out var def, out var min, out var max);
            return CreatePropAccessor(
                new PropDescriptorDouble(name, displayName, description, units, isReadOnly, def, min, max),
                targetType, type, getter, setter);
        }
        else if (type == typeof(bool))
        {
            return CreatePropAccessor(
                new PropDescriptorBool(name, displayName, description, units, isReadOnly),
                targetType, type, getter, setter);
        }
        else if (type == typeof(string))
        {
            return CreatePropAccessor(
                new PropDescriptorString(name, displayName, description, units, isReadOnly),
                targetType, type, getter, setter);
        }
        else if (type == typeof(System.Numerics.Vector2) || type == typeof(Ara3D.Geometry.Vector2))
        {
            return CreatePropAccessor(
                new PropDescriptorVector2(name, displayName, description, units, isReadOnly),
                targetType, type, getter, setter);
        }
        else if (type == typeof(System.Numerics.Vector3) || type == typeof(Ara3D.Geometry.Vector3))
        {
            return CreatePropAccessor(
                new PropDescriptorVector3(name, displayName, description, units, isReadOnly),
                targetType, type, getter, setter);
        }
        else if (type == typeof(System.Numerics.Vector4) || type == typeof(Ara3D.Geometry.Vector4))
        {
            return CreatePropAccessor(
                new PropDescriptorVector4(name, displayName, description, units, isReadOnly),
                targetType, type, getter, setter);
        }
        else if (type == typeof(Action))
        {
            return CreatePropAccessor(
                new PropDescriptorAction(name, displayName, description),
                targetType, type, getter, setter);
        }
        else
        {
            return CreatePropAccessor(
                new GenericPropDescriptor(null, type, name, displayName, description, units, isReadOnly),
                targetType, type, getter, setter);
        }
    }

    public static IPropAccessor CreatePropAccessor(Type type, object hostObj, Type targetType, MemberInfo mi, Delegate getter, Delegate setter)
    {
        var name = mi.Name;
        var displayName = mi.Name.SplitCamelCase();
        var description = "";
        var units = "";

        var displayNameAttr = mi.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttr != null)
            displayName = displayNameAttr.DisplayName;

        var rangeAttr = mi.GetCustomAttribute<RangeAttribute>();
        var optionsAttr = mi.GetCustomAttribute<OptionsAttribute>();

        return CreatePropAccessor(type, hostObj, targetType, rangeAttr, optionsAttr, name, displayName,
            description, units, getter, setter);
    }

    public static IEnumerable<IPropAccessor> GetPropAccessors(this Type type, object hostObj = null)
    {
        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var prop in props)
        {
            if (!prop.CanRead)
                continue;
            if (prop.GetIndexParameters().Length != 0)
                continue;

            var setMeth = prop.GetSetMethod(false);
            var isReadOnly = !prop.CanWrite || setMeth == null || setMeth.IsPrivate;

            var getter = prop.GetFastGetter();
            var setter = !isReadOnly ? prop.GetFastSetter() : null;

            yield return CreatePropAccessor(prop.PropertyType, hostObj, type, prop, getter, setter);
        }

        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        foreach (var field in fields)
        {
            var isReadOnly = field.IsInitOnly;

            var getter = field.GetFastGetter();
            var setter = !isReadOnly ? field.GetFastSetter() : null;

            yield return CreatePropAccessor(field.FieldType, hostObj, type, field, getter, setter);

        }
    }
}