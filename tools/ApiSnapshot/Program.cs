// ApiSnapshot: deterministic public-API surface dump of an assembly.
//
// Usage: ApiSnapshot <assembly-path> [output-path]
//
// Loads the assembly (resolving dependencies from the same directory), walks all
// public types whose namespace starts with "Ara3D", and writes one line per type
// and per public member, sorted ordinally by type full name then member signature.
// Compiler-generated members are excluded. Output has no timestamps or absolute
// paths, so two runs over identical assemblies produce byte-identical files.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: ApiSnapshot <assembly-path> [output-path]");
    return 1;
}

var assemblyPath = Path.GetFullPath(args[0]);
if (!File.Exists(assemblyPath))
{
    Console.Error.WriteLine($"Assembly not found: {assemblyPath}");
    return 1;
}

// Resolve dependencies (Ara3D.Collections, Ara3D.Memory, Ara3D.Utils, ...) from
// the target assembly's own directory.
var assemblyDir = Path.GetDirectoryName(assemblyPath)!;
AppDomain.CurrentDomain.AssemblyResolve += (_, e) =>
{
    var name = new AssemblyName(e.Name).Name;
    if (name is null)
        return null;
    var candidate = Path.Combine(assemblyDir, name + ".dll");
    return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
};

var assembly = Assembly.LoadFrom(assemblyPath);

var lines = new List<string>();
foreach (var type in assembly.GetExportedTypes())
{
    if (type.Namespace is null || !type.Namespace.StartsWith("Ara3D", StringComparison.Ordinal))
        continue;
    if (IsCompilerGenerated(type))
        continue;

    var typeName = TypeName(type);
    lines.Add($"{typeName} | {TypeHeader(type)}");
    foreach (var member in PublicMembers(type))
        lines.Add($"{typeName} | {member}");
}

lines.Sort(StringComparer.Ordinal);

var output = new StringBuilder();
foreach (var line in lines)
    output.Append(line).Append('\n');

if (args.Length >= 2)
{
    var outPath = Path.GetFullPath(args[1]);
    Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
    File.WriteAllText(outPath, output.ToString(), new UTF8Encoding(false));
    Console.WriteLine($"Wrote {lines.Count} lines to {outPath}");
}
else
{
    Console.Write(output.ToString());
}

return 0;

// --- helpers ---

static bool IsCompilerGenerated(MemberInfo member)
    => member.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false);

static string TypeHeader(Type type)
{
    var kind = type.IsInterface ? "interface"
        : type.IsEnum ? "enum"
        : type.IsValueType ? "struct"
        : type.IsSubclassOf(typeof(MulticastDelegate)) ? "delegate"
        : "class";

    var sb = new StringBuilder("type ").Append(kind);

    if (type.BaseType is not null
        && type.BaseType != typeof(object)
        && type.BaseType != typeof(ValueType)
        && type.BaseType != typeof(Enum)
        && type.BaseType != typeof(MulticastDelegate))
        sb.Append(" : ").Append(TypeName(type.BaseType));

    var interfaces = type.GetInterfaces()
        .Select(TypeName)
        .OrderBy(n => n, StringComparer.Ordinal)
        .ToList();
    if (interfaces.Count > 0)
        sb.Append(" implements ").Append(string.Join(", ", interfaces));

    return sb.ToString();
}

static IEnumerable<string> PublicMembers(Type type)
{
    const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static
        | BindingFlags.DeclaredOnly;

    foreach (var field in type.GetFields(flags))
    {
        if (IsCompilerGenerated(field))
            continue;
        var mod = field.IsLiteral ? "const " : field.IsStatic ? "static " : "";
        var ro = !field.IsLiteral && field.IsInitOnly ? "readonly " : "";
        yield return $"field {mod}{ro}{TypeName(field.FieldType)} {field.Name}";
    }

    foreach (var ctor in type.GetConstructors(flags))
    {
        if (IsCompilerGenerated(ctor))
            continue;
        yield return $"ctor .ctor({Parameters(ctor)})";
    }

    foreach (var method in type.GetMethods(flags))
    {
        if (IsCompilerGenerated(method))
            continue;
        if (method.IsSpecialName && !method.Name.StartsWith("op_", StringComparison.Ordinal))
            continue; // property/event accessors are reported via their property/event
        var kind = method.Name.StartsWith("op_", StringComparison.Ordinal) ? "operator" : "method";
        var mod = method.IsStatic ? "static " : "";
        var generics = method.IsGenericMethodDefinition
            ? "<" + string.Join(", ", method.GetGenericArguments().Select(a => a.Name)) + ">"
            : "";
        yield return $"{kind} {mod}{TypeName(method.ReturnType)} {method.Name}{generics}({Parameters(method)})";
    }

    foreach (var prop in type.GetProperties(flags))
    {
        if (IsCompilerGenerated(prop))
            continue;
        var accessors = new List<string>();
        if (prop.GetGetMethod() is not null)
            accessors.Add("get");
        if (prop.GetSetMethod() is not null)
            accessors.Add("set");
        if (accessors.Count == 0)
            continue; // no public accessors
        var isStatic = (prop.GetGetMethod() ?? prop.GetSetMethod())!.IsStatic;
        var mod = isStatic ? "static " : "";
        var indexParams = prop.GetIndexParameters();
        var name = indexParams.Length > 0
            ? $"this[{string.Join(", ", indexParams.Select(FormatParameter))}]"
            : prop.Name;
        yield return $"prop {mod}{TypeName(prop.PropertyType)} {name} {{ {string.Join("; ", accessors)}; }}";
    }

    foreach (var evt in type.GetEvents(flags))
    {
        if (IsCompilerGenerated(evt) || evt.EventHandlerType is null)
            continue;
        if (evt.GetAddMethod() is null)
            continue;
        var mod = evt.GetAddMethod()!.IsStatic ? "static " : "";
        yield return $"event {mod}{TypeName(evt.EventHandlerType)} {evt.Name}";
    }
}

static string Parameters(MethodBase method)
    => string.Join(", ", method.GetParameters().Select(FormatParameter));

static string FormatParameter(ParameterInfo p)
{
    var type = p.ParameterType;
    var prefix = "";
    if (type.IsByRef)
    {
        prefix = p.IsOut ? "out " : p.IsIn ? "in " : "ref ";
        type = type.GetElementType()!;
    }
    if (p.IsDefined(typeof(ParamArrayAttribute), false))
        prefix = "params ";
    return $"{prefix}{TypeName(type)} {p.Name}";
}

static string TypeName(Type type)
{
    if (type.IsByRef)
        return "ref " + TypeName(type.GetElementType()!);
    if (type.IsArray)
        return TypeName(type.GetElementType()!) + "[" + new string(',', type.GetArrayRank() - 1) + "]";
    if (type.IsPointer)
        return TypeName(type.GetElementType()!) + "*";
    if (type.IsGenericParameter)
        return type.Name;

    var nullableUnderlying = Nullable.GetUnderlyingType(type);
    if (nullableUnderlying is not null)
        return TypeName(nullableUnderlying) + "?";

    var baseName = type.IsNested && type.DeclaringType is not null
        ? DeclaringPrefixedName(type)
        : (type.Namespace is null ? type.Name : type.Namespace + "." + type.Name);

    if (!type.IsGenericType)
        return baseName;

    // Strip the arity suffix (`2) and append formatted type arguments.
    var tick = baseName.IndexOf('`');
    if (tick >= 0)
        baseName = baseName[..tick];
    var typeArgs = type.GetGenericArguments().Select(TypeName);
    return $"{baseName}<{string.Join(", ", typeArgs)}>";
}

static string DeclaringPrefixedName(Type type)
{
    // Nested type: Declaring.Nested (declaring type formatted without its own generic args duplicated).
    var declaring = type.DeclaringType!;
    var declaringName = declaring.Namespace is null
        ? declaring.Name
        : declaring.Namespace + "." + declaring.Name;
    return declaringName + "." + type.Name;
}
