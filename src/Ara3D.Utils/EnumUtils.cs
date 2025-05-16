using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Utils
{
    public static class EnumUtils
    {
        /// <summary>
        /// Type-safe helper for getting all enum values. 
        /// </summary>
        public static IEnumerable<T> GetEnumValues<T>()
            => Enum.GetValues(typeof(T)).Cast<T>();

        /// <summary>
        /// The integer index of an enum type. The "value" is assumed to be an enum value.
        /// Note that enums values are different from their index. 
        /// </summary>
        public static int GetEnumIndex(this Type type, object value)
        {
            if (!type.IsEnum)
                throw new Exception("Required an enum type");
            var vals = Enum.GetValues(type);
            for (var i = 0; i < vals.Length; i++)
            {
                if (vals.GetValue(i).Equals(value))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Given the type of an enum, returns the nth associated value. 
        /// </summary>
        public static object GetEnumValue(this Type type, int index)
        {
            if (!type.IsEnum)
                throw new Exception("Required an enum type");
            var vals = Enum.GetValues(type);
            return vals.GetValue(index);
        }
    }
}