using System.Reflection;
using Ara3D.Utils;

namespace Ara3D.Bowerbird;

/// <summary>
/// Loads a compiled command type and verifies it implements INamedCommand.
/// </summary>
public static class CommandLoader
{
    public static INamedCommand Load(FilePath dllPath, string typeName)
    {
        if (!dllPath.Exists())
            throw new FileNotFoundException($"Command DLL not found: {dllPath}");

        var assembly = Assembly.LoadFile(dllPath.GetFullPath());
        var type = assembly.GetType(typeName, throwOnError: true);
        if (!type.ImplementsInterface<INamedCommand>())
            throw new InvalidOperationException($"Type {typeName} does not implement INamedCommand");

        return Activator.CreateInstance(type) as INamedCommand
            ?? throw new InvalidOperationException($"Failed to create instance of {typeName}");
    }
}
