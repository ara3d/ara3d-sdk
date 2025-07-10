using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Ara3D.DataSetBrowser.WPF;

public static class MessagePackDynamicConverter
{
    /// <summary>
    /// Entry‐point: convert a typeless MessagePack object into T.
    /// </summary>
    public static T ConvertTo<T>(object input)
        => (T)ConvertTo(typeof(T), input);

    private static object ConvertTo(Type targetType, dynamic input)
    {
        // 1) null ⇒ default (handles Nullable<T> as well)
        if (input == null)
        {
            if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null)
                return FormatterServices.GetUninitializedObject(targetType);
            return null;
        }

        // 2) already the right type?
        if (targetType.IsInstanceOfType(input))
            return input;

        // 3) Nullable<T> ⇒ unwrap and re-convert
        var nullableUnderlying = Nullable.GetUnderlyingType(targetType);
        if (nullableUnderlying != null)
            return ConvertTo(nullableUnderlying, input);

        // 4) enums
        if (targetType.IsEnum)
            return Enum.Parse(targetType, input.ToString(), ignoreCase: true);

        // 5) Guid, DateTime
        if (targetType == typeof(Guid))
            return Guid.Parse(input.ToString());
        if (targetType == typeof(DateTime))
            return DateTime.Parse(input.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        // 6) primitive, decimal, string, bool, double, float
        if (IsSimpleType(targetType))
            return System.Convert.ChangeType(input, targetType, CultureInfo.InvariantCulture);

        // for convenience below
        dynamic dyn = input;
        var inputType = input.GetType();

        // 7) T[] via .Length + indexer
        if (targetType.IsArray && HasProperty(inputType, "Length"))
        {
            var elemType = targetType.GetElementType();
            int length = (int)GetPropertyValue(input, "Length");
            var arr = Array.CreateInstance(elemType, length);
            for (int i = 0; i < length; i++)
                arr.SetValue(ConvertTo(elemType, dyn[i]), i);
            return arr;
        }

        // 8) IList<T>
        if (IsGenericList(targetType))
        {
            var itemType = targetType.GetGenericArguments()[0];
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
            foreach (var item in dyn)
                list.Add(ConvertTo(itemType, item));
            return list;
        }


        // 10) Fallback: instantiate POCO and map public writable props
        var instance = FormatterServices.GetUninitializedObject(targetType);
        // include both normal setters and init-only setters
        var props = targetType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p =>
            p.CanWrite ||
            (p.SetMethod != null)
            );

        foreach (var prop in props)
        {
            try
            {
                if (!input.ContainsKey(prop.Name))
                    continue;
                var raw = input[prop.Name];
                var converted = ConvertTo(prop.PropertyType, raw);
                prop.SetValue(instance, converted);
            }
            catch (Exception ex)
            {
                // swallow or replace with your logging
                Debug.WriteLine(ex);
            }
        }

        return instance;
    }

    private static bool IsSimpleType(Type t)
        => t.IsPrimitive
           || t == typeof(string)
           || t == typeof(decimal)
           || t == typeof(double)
           || t == typeof(float)
           || t == typeof(bool);

    private static bool HasProperty(Type type, string name)
        => type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) != null;

    private static object GetPropertyValue(object obj, string propName)
        => obj.GetType().GetProperty(propName).GetValue(obj);

    private static bool TryGetIndexerValue(object obj, string key, out object value)
    {
        value = null;
        var type = obj.GetType();
        // find default indexer that takes a string
        var indexer = type.GetProperties()
            .FirstOrDefault(p =>
                p.GetIndexParameters().Length == 1 &&
                p.GetIndexParameters()[0].ParameterType == typeof(string));
        if (indexer == null) return false;
        try
        {
            value = indexer.GetValue(obj, new object[] { key });
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static object GetIndexerValue(object obj, object key)
    {
        var type = obj.GetType();
        var keyType = key.GetType();
        var indexer = type.GetProperties()
            .FirstOrDefault(p =>
            {
                var idx = p.GetIndexParameters();
                return idx.Length == 1 && idx[0].ParameterType.IsAssignableFrom(keyType);
            });
        if (indexer == null)
            throw new InvalidOperationException($"No indexer found for key type {keyType.Name}");
        return indexer.GetValue(obj, new[] { key });
    }

    private static bool IsGenericList(Type t)
        => t.IsGenericType
           && typeof(IList<>).MakeGenericType(t.GetGenericArguments()[0]).IsAssignableFrom(t);

    private static bool IsGenericDictionary(Type t)
        => t.IsGenericType
           && typeof(IDictionary<,>)
               .MakeGenericType(t.GetGenericArguments())
               .IsAssignableFrom(t);
}