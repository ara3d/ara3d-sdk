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
    public static Prop CreateProp(this PropertyInfo pi, object hostObj)
    {
        var desc = pi.CreateDescriptor();
        var acc = pi.CreatePropAccessor(hostObj);
        var val = pi.CreateValidator(hostObj);
        var con = pi.CreateConstraints(hostObj);
        return new Prop(desc, acc, val, null, con, null);
    }

    public static Prop CreateProp(this FieldInfo fi, object hostObj)
    {
        var desc = fi.CreateDescriptor();
        var acc = fi.CreatePropAccessor(hostObj);
        var val = fi.CreateValidator(hostObj);
        var con = fi.CreateConstraints(hostObj);
        return new Prop(desc, acc, val, null, con, null);
    }

    // TODO: get default constraints
    // TODO: get default steppers
    // TODO: get default codec 

    public static PropConstraints CreateConstraints(this MemberInfo mi, object val)
    {
        var rangeAttr = mi.GetCustomAttribute<RangeAttribute>();
        if (rangeAttr == null) return default;
        return new(val, rangeAttr?.Minimum, rangeAttr?.Maximum);
    }

    public static PropConstraints CreateConstraints(this FieldInfo fi, object host)
    {
        return fi.CreateConstraints(fi.GetValue(host));
    }

    public static PropConstraints CreateConstraints(this PropertyInfo pi, object host)
    {
        return pi.CreateConstraints(pi.GetValue(host));
    }

    public static IPropValidator CreateValidator(this PropertyInfo pi, object host)
    {
        var constraints = pi.CreateConstraints(host);
        return constraints.HasMinMax ? new PropConstraintsValidator(constraints) : null;
    }

    public static IPropValidator CreateValidator(this FieldInfo fi, object host)
    {
        var constraints = fi.CreateConstraints(host);
        return constraints.HasMinMax ? new PropConstraintsValidator(constraints) : null;
    }

    public static PropDescriptor CreateDescriptor(this MemberInfo mi, Type type)
    {
        var isReadOnly = IsReadOnly(mi);
        var displayNameAttr = mi.GetCustomAttribute<DisplayNameAttribute>();
        var descAttr = mi.GetCustomAttribute<DescriptionAttribute>();
        return new(type, mi.Name, isReadOnly, displayNameAttr?.DisplayName, descAttr?.Description, mi.GetCustomAttributes().ToList());
    }

    public static PropDescriptor CreateDescriptor(this MemberInfo mi)
        => mi is FieldInfo fi ? CreateDescriptor(fi)
            : mi is PropertyInfo pi ? CreateDescriptor(pi) : null;

    public static PropDescriptor CreateDescriptor(this FieldInfo fi)
        => CreateDescriptor(fi, fi.FieldType);

    public static PropDescriptor CreateDescriptor(this PropertyInfo pi)
        => CreateDescriptor(pi, pi.PropertyType);

    public static bool IsReadOnly(this MemberInfo mi)
        => mi is FieldInfo fi
            ? fi.IsInitOnly
            : mi is not PropertyInfo pi || (pi.CanWrite || pi.GetSetMethod(false) != null);

    public static IPropAccessor CreatePropAccessor(
        this IPropValidator validator,
        Type targetType, Type valueType,
        Delegate getterRef, Delegate? setterRef)
    {
        var open = typeof(PropAccessor<,>);
        var closed = open.MakeGenericType(targetType, valueType);
        return (IPropAccessor)Activator.CreateInstance(closed, validator, getterRef, setterRef)!;
    }

    public static IPropAccessor CreatePropAccessor(this PropertyInfo pi, object hostObj)
    {
        if (!pi.CanRead)
            return null;
        if (pi.GetIndexParameters().Length != 0)
            return null;

        var setMeth = pi.GetSetMethod(false);
        var isReadOnly = !pi.CanWrite || setMeth == null || setMeth.IsPrivate;

        var getter = pi.GetFastGetter();
        var setter = !isReadOnly ? pi.GetFastSetter() : null;

        return pi.CreateValidator(hostObj).CreatePropAccessor(pi.DeclaringType, pi.PropertyType, getter, setter);
    }

    public static IPropAccessor CreatePropAccessor(this FieldInfo fi, object hostObj)
    {
        var isReadOnly = fi.IsInitOnly;

        var getter = fi.GetFastGetter();
        var setter = !isReadOnly ? fi.GetFastSetter() : null;

        return fi.CreateValidator(hostObj).CreatePropAccessor(fi.DeclaringType, fi.FieldType, getter, setter);
    }
}