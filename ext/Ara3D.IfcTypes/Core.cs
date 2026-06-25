using System.Diagnostics;
using System.Reflection;
using Ara3D.Utils;

namespace Ara3D.IfcTypes;

public enum IfcSchemaEnum
{
    Ifc2x3,
    Ifc4,
    Ifc4x3,
}

public enum IfcTypeKind
{
    Enum,
    Select,
    Entity,
    Alias,
    System,
    Unknown,
}

public interface IEntity
{
    uint EntityTypeCode { get; }
    ReadOnlySpan<byte> EntityTypeName { get; }
    IfcAttribute[] Attributes { get; }
}

public interface ISelectInterface { }

public abstract class EntityBaseClass : IEntity
{
    public abstract uint EntityTypeCode { get; }
    public abstract ReadOnlySpan<byte> EntityTypeName { get; }
    public abstract IfcAttribute[] Attributes { get; }
}

public class TypeAliasBaseClass { }

public readonly record struct TypeDetails
(
    Type Type, 
    IfcTypeKind Kind, 
    int Rank
);

public record IfcAttribute
(
    int Index, 
    TypeDetails Type
);

public record IfcAttribute<T>(
    int Index,
    IfcTypeKind Kind,
    int Rank
) : IfcAttribute(Index, new(typeof(T), Kind, Rank))
{
    public string TypeName 
        => Type.Type.Name;

    public override string ToString()
        => $"Index={Index} Kind={Kind} Rank={Rank} Type={TypeName}";
}

public class IfcSchema
{
    public IfcSchema(IfcSchemaEnum schemaEnum, IEntity[] entities)
    {
        Enum = schemaEnum;
        Entities = entities.ToDictionary(e => e.EntityTypeCode, e => e);
    }

    public IfcSchemaEnum Enum;
    public Dictionary<uint, IEntity> Entities = new();
}

public static class EntityExtensions
{
    public static string GetEntityName(this IEntity e)
    {
        return System.Text.Encoding.UTF8.GetString(e.EntityTypeName);
    }
}
