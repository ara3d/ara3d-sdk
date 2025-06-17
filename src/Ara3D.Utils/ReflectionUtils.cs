using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Ara3D.Utils
{
    public static class ReflectionUtils
    {
        /// <summary>
        /// Returns true if the type self is an instance of the given generic type
        /// </summary>
        public static bool InstanceOfGenericType(this Type self, Type genericType)
            => self.IsGenericType && self.GetGenericTypeDefinition() == genericType;

        /// <summary>
        /// Returns true if the type self is an instance of the given generic interface, or implements the interface
        /// </summary>
        public static bool InstanceOfGenericInterface(this Type self, Type ifaceType)
            => self.InstanceOfGenericType(ifaceType)
               || self.GetInterfaces().Any(i => InstanceOfGenericType(i, ifaceType));

        /// <summary>
        /// https://stackoverflow.com/questions/4963160/how-to-determine-if-a-type-implements-an-interface-with-c-sharp-reflection
        /// </summary>
        public static bool ImplementsInterface(this Type self, Type ifaceType)
            => ifaceType.IsAssignableFrom(self) || self.GetInterfaces().Contains(ifaceType);

        public static bool ImplementsInterface<TInterface>(this Type self)
            => self.ImplementsInterface(typeof(TInterface));

        public static ConstructorInfo GetDefaultConstructor(this Type self)
            => self.GetConstructor(Array.Empty<Type>());

        public static bool HasDefaultConstructor(this Type self)
            => self.GetDefaultConstructor() != null;

        /// <summary>
        /// Returns true if the type implements IList with a generic parmaeter.
        /// </summary>
        public static bool ImplementsIList(this Type t)
            => t.InstanceOfGenericInterface(typeof(IList<>));

        /// <summary>
        /// Returns true if the source type can be cast to doubles.
        /// </summary>
        public static bool CanCastToDouble(this Type typeSrc)
            => typeSrc.IsPrimitive
               && typeSrc != typeof(char)
               && typeSrc != typeof(decimal)
               && typeSrc != typeof(bool);

        /// <summary>
        /// Returns true if the object is not null and can be safely cast to a double. 
        /// </summary>
        public static bool CanCastToDouble(this object o)
            => o != null && o.GetType().CanCastToDouble();

        /// <summary>
        /// Returns the associated double value, or throws an exception if not possible. 
        /// </summary>
        public static double CastToDouble(this object o)
        {
            switch (o)
            {
                case int n: return n;
                case uint un: return un;
                case long l: return l;
                case ulong ul: return ul;
                case byte b: return b;
                case sbyte sb: return sb;
                case short s: return s;
                case ushort un: return un;
                case float f: return f;
                case double d: return d;
            }
            throw new Exception($"Cannot cast object {o} to double");
        }

        /// <summary>
        /// Returns all loaded types of in the current domain. 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllTypes()
            => AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes()).Distinct();

        // https://stackoverflow.com/questions/857705/get-all-derived-types-of-a-type
        public static IEnumerable<Type> GetAllSubclassesOf(Type t)
            => GetAllTypes().Where(x => x.IsSubclassOf(t));

        public static IEnumerable<Type> GetAllSubclassesOf(Assembly asm, Type t)
            => asm.GetTypes().Where(x => x.IsSubclassOf(t));
        
        public static IEnumerable<Assembly> GetReferencedAssemblies(this Type type)
            => type.Assembly
                .GetReferencedAssemblies()
                .Select(asmName => Assembly.ReflectionOnlyLoad(asmName.FullName))
                .Append(type.Assembly);

        // https://stackoverflow.com/questions/1582510/get-paths-of-assemblies-used-in-type
        public static IEnumerable<string> GetReferencedAssemblyPaths(this Type type)
            => type.GetReferencedAssemblies().Select(asm => asm.Location);

        public static IEnumerable<MethodInfo> GetMethodsWithAttribute(this Type type, Type attrType)
            => type.GetMethods().Where(m => m.GetCustomAttributes(attrType, false).Length > 0);

        /// <summary>
        /// Converts properties to an enumerable of strings.
        /// </summary>
        public static IEnumerable<string> PropertiesToStrings(this object self)
            => self.PropertiesToDictionary().Select(kv => $"{kv.Key}: {kv.Value}");

        /// <summary>
        /// Returns true if the type is a "plain old data" type (is a struct type that contains no references).
        /// This means that we should be able to create pointers to the type, and copying
        /// arrays of them into buffers makes sense.
        /// </summary>
        public static bool ContainsNoReferences(this Type t)
            => t.IsPrimitive || t.GetAllFields().Select(f => f.FieldType).All(ContainsNoReferences);

        /// <summary>
        /// Returns the size of the managed type.
        /// </summary>
        public static int SizeOf(this Type t)
            => Marshal.SizeOf(t);

        /// <summary>
        /// Returns the size of the managed type.
        /// </summary>
        public static int SizeOf<T>()
            => SizeOf(typeof(T));

        /// <summary>
        /// Returns all instance fields, public and private.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetAllFields(this Type self)
            => self.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Converts fields to a dictionary of string/object pair.
        /// </summary>
        public static IReadOnlyDictionary<string, object> PublicFieldsToDictionary(this object self)
            => self.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(fi => fi.Name, fi => fi.GetValue(self));

        /// <summary>
        /// The convention used by the C# compiler to name the backing field.
        /// Warning this is a hack, and could change between implementations 
        /// </summary>
        public static string BackingFieldName(string name)
            => $"<{name}>k__BackingField";

        /// <summary>
        /// A hacky method based on C# compiler implementation details
        /// to retrieve the backing field for a property, if one exists. 
        /// https://stackoverflow.com/questions/8817070/is-it-possible-to-access-backing-fields-behind-auto-implemented-properties
        /// </summary>
        public static FieldInfo GetBackingField(this PropertyInfo pi)
            => pi.DeclaringType.GetField(BackingFieldName(pi.Name));

        /// <summary>
        /// Returns true if a backing field is found. Not reliable. 
        /// </summary>
        public static bool HasBackingField(this PropertyInfo pi)
            => pi.GetBackingField() != null;

        /// <summary>
        /// Converts properties to a dictionary of string/object pair.
        /// </summary>
        public static IReadOnlyDictionary<string, object> PublicFieldBackedPropertiesToDictionary(this object self)
            => self.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(fi => fi.Name, fi => fi.GetValue(self));

        /// <summary>
        /// Converts fields to a dictionary of string/object pair.
        /// </summary>
        public static IDictionary<string, object> PublicFieldsAndFieldBackedPropertiesToDictionary(this object self)
            => self.PublicFieldsToDictionary().ConcatDictionaries(self.PublicFieldBackedPropertiesToDictionary());

        /// <summary>
        /// Converts properties to a dictionary of string/object pair.
        /// </summary>
        public static IReadOnlyDictionary<string, object> PropertiesToDictionary(this object self)
            => self.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(fi => fi.Name, fi => fi.GetValue(self));

        /// <summary>
        /// Given a method info, an object to invoke it on, and args, returns a func object (lambda)
        /// </summary>
        public static Func<T> InvokableMethod<T>(this MethodInfo mi, object self, params object[] args)
            => () => (T)mi.Invoke(self, args);

        /// <summary>
        /// Given a property info, an object to invoke it on, returns a func object (lambda)
        /// </summary>
        public static Func<T> InvokableProperty<T>(this PropertyInfo pi, object self)
            => () => (T)pi.GetValue(self);

        /// <summary>
        /// Given a method info, an object to invoke it on, and args, returns an action object (lambda)
        /// </summary>
        public static Action InvokableMethod(this MethodInfo mi, object self, params object[] args)
            => () => mi.Invoke(self, args);

        
        /// <summary>
        /// Convenience method, which is the inverse of Type.IsAssignableFrom 
        /// </summary>
        public static bool IsAssignableTo(this Type type, Type other)
            => other.IsAssignableFrom(type);

        /// <summary>
        /// Returns an enumerator by calling "GetEnumerator" if present on the object.
        /// This is more or less how LINQ does it. 
        /// </summary>
        public static IEnumerator GetEnumerator(this object o)
        {
            if (o == null) return null;
            var t = o.GetType();
            var m = t.GetMethod(nameof(GetEnumerator));
            if (m != null && m.ReturnType.IsAssignableTo(typeof(IEnumerator)))
                return (IEnumerator)m.Invoke(o, Array.Empty<object>());
            return null;
        }

        
        /// <summary>
        /// Creates a stribgkt-typed delegate of type TDelegate
        /// for the given MethodInfo. TDelegate must be a delegate type whose Invoke signature
        /// matches the method’s signature (for instance methods, the first parameter of
        /// TDelegate must be the instance type).
        /// </summary>
        public static TDelegate CreateDelegate<TDelegate>(MethodInfo method)
            where TDelegate : Delegate
        {
            var delegateType = typeof(TDelegate);
            var invoke = delegateType.GetMethod("Invoke");
            var callParams = invoke.GetParameters()
                                   .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                                   .ToArray();

            Expression instance = null;
            var parametersForCall = callParams;

            // If this is an instance method, the first parameter must be the target
            if (!method.IsStatic)
            {
                instance = callParams[0];
                parametersForCall = callParams.Skip(1).ToArray();
            }

            // Build the call expression
            var call = Expression.Call(instance, method, parametersForCall);

            // If return types differ (e.g. void → object), convert
            Expression body = call;
            if (invoke.ReturnType == typeof(void))
            {
                // For void methods, wrap in a block that returns default(TReturn)
                var @return = Expression.Label(invoke.ReturnType);
                body = Expression.Block(call, Expression.Label(@return, Expression.Default(invoke.ReturnType)));
            }
            else if (call.Type != invoke.ReturnType)
            {
                body = Expression.Convert(call, invoke.ReturnType);
            }

            // Compile to a delegate of the right type
            var lambda = Expression.Lambda<TDelegate>(body, callParams);
            return lambda.Compile();
        }

        public static object GetFieldOrPropOrInvokeMethod(this object obj, string name)
        {
            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var type = obj.GetType();
            var mi = type.GetMethod(name, flags);
            if (mi != null)
            {
                if (mi.GetParameters().Length != 0)
                    throw new Exception($"Method {name} must not take any parameters.");
                return mi.Invoke(obj, null);
            }
            var fi = type.GetField(name, flags);
            if (fi != null)
            {
                return fi.GetValue(obj);
            }
            var pi = type.GetProperty(name, flags);
            if (pi != null)
            {
                if (!pi.CanRead)
                    throw new Exception($"Property {name} is not readable.");
                return pi.GetValue(obj);
            }
            throw new Exception($"No method, field or property named {name} found in {type.Name}.");
        }
    }
}