using System.Diagnostics;
using System.Text;
using Ara3D.Parakeet;
using Ara3D.Parakeet.Grammars;
using Ara3D.Parakeet.Tests;
using Ara3D.Utils;

namespace Ara3D.IfcTypeGen;

public interface IIfcTypeDecl;
public record IfcAttribute(string Name, IfcTypeExpr TypeExpr);
public record IfcTypeExpr(string Name, IReadOnlyList<IfcTypeExpr> GenericParameters);
public record IfcEnumeration(string Name, IReadOnlyList<string> Options) : IIfcTypeDecl;
public record IfcSelect(string Name, IReadOnlyList<string> Options) : IIfcTypeDecl;
public record IfcTypeAlias(string Name, IfcTypeExpr TypeExpr) : IIfcTypeDecl;
public record IfcEntity(string Name, string SubType, IReadOnlyList<IfcAttribute> Attributes);
public record IfcSchema(IReadOnlyList<IfcEntity> Entities, IReadOnlyList<IIfcTypeDecl> Decls);

public static class IfcCodeGenTest
{
    [Test, Explicit]
    public static void TestGenerate()
    {
        var pi2x3 = ExpressTests.GetIfc2x3ExpressFileAsParserInput();
        var pi4 = ExpressTests.GetIfc4ExpressFileAsParserInput();
        var pi4x3 = ExpressTests.GetIfc4x3ExpressFileAsParserInput();

        Generate(pi2x3, "Ifc2x3");
        Generate(pi4, "Ifc4");
        Generate(pi4x3, "Ifc4x3");
    }

    public static IEnumerable<string> GetExpressTypes(this ParserInput pi)
    {
        var rule = ExpressGrammar.Instance.TypeBlocks;
        var ps = rule.Parse(pi);
        Debug.Assert(ps.AtEnd());
        return ps.AllEndNodes().Select(n => n.Contents);
    }

    public static IEnumerable<string> GetExpressEntities(this ParserInput pi)
    {
        var rule = ExpressGrammar.Instance.EntityBlocks;
        var ps = rule.Parse(pi);
        Debug.Assert(ps.AtEnd());
        return ps.AllEndNodes().Select(n => n.Contents);
    }

    public static IEnumerable<IIfcTypeDecl> GetTypeDecls(this ParserInput pi)
        => GetExpressTypes(pi).Select(ToTypeDecl);

    public static IEnumerable<IfcEntity> GetEntities(this ParserInput pi)
        => GetExpressEntities(pi).Select(ToEntity);

    public static IfcSchema GetSchema(this ParserInput pi)
        => new(GetEntities(pi).ToList(), GetTypeDecls(pi).ToList());

    public static (string TypeName, int Rank) GetTypeDetails(IfcTypeExpr typeExpr)
    {
        var typeStr = typeExpr.Name;
        var isList = typeExpr.GenericParameters.Count > 0;
        if (isList)
        {
            var gp = typeExpr.GenericParameters[0];
            if (typeExpr.GenericParameters.Count > 1)
                throw new Exception("Only a maximum of 1 generic parameter is supported");
            var (name, rank) = GetTypeDetails(gp);
            return (name, rank + 1);
        }

        return (typeStr, 0);
    }

    public static string GetTypeDetailStr(IfcTypeExpr type, Dictionary<string, IfcTypeKind> typeKinds)
    {
        var (typeStr, rank) = GetTypeDetails(type);
        var kind = typeKinds.GetValueOrDefault(typeStr, IfcTypeKind.Unknown);
        return $"new(typeof({typeStr}), IfcTypeKind.{kind}, {rank})";
    }
    
    public static StringBuilder OutputTypeAliases(IfcSchema schema, string ns, Dictionary<string, IfcTypeExpr> typeAliases, Dictionary<string, IfcTypeKind> typeKinds)
    {
        var sb = new StringBuilder();

        var selectLookup = schema.GetSelectsPerEntity();

        sb.AppendLine($"namespace Ara3D.IfcTypes.{ns};");
        foreach (var kv in typeAliases)
        {
            sb.AppendLine();
            sb.AppendLine($"public class {kv.Key}");

            var inherits = new List<string>();
            inherits.Add("TypeAliasBaseClass");

            if (selectLookup.ContainsKey(kv.Key))
                inherits.AddRange(selectLookup[kv.Key].Select(s => s.Name));

            if (inherits.Count > 0)
                sb.AppendLine("    : " + inherits.JoinStringsWithComma());
            
            sb.AppendLine("{");
            sb.AppendLine($"    public static TypeDetails Type = {GetTypeDetailStr(kv.Value, typeKinds)};");
            sb.AppendLine("}");
        }

        return sb;
    }

    public static StringBuilder OutputEntities(IfcSchema schema, string ns, Dictionary<string, IfcTypeKind> typeKinds, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();
        var selectLookup = schema.GetSelectsPerEntity();
        var entities = schema.Entities.OrderBy(e => e.Name).ToList();
        
        var entityLookup = entities.ToDictionary(e => e.Name, e => e);
        sb.AppendLine("#pragma warning disable CS0108");
        sb.AppendLine($"namespace Ara3D.IfcTypes.{ns};");

        var entityIndex = 0;
        foreach (var e in entities)
        {
            sb.AppendLine();
            sb.AppendLine($"public partial class {e.Name}");
            var inherits = new List<string>();

            if (!e.SubType.IsNullOrWhiteSpace())
                inherits.Add(e.SubType);
            else 
                inherits.Add("EntityBaseClass");
            
            if (selectLookup.TryGetValue(e.Name, out var value))
            {
                var ifaces = value.OrderBy(s => s.Name);
                foreach (var i in ifaces)
                {
                    inherits.Add(i.Name);
                }
            }

            var code = e.Name.ToUpperInvariant().Fnv1a32bit();

            if (inherits.Count > 0)
                sb.AppendLine($"   : {inherits.JoinStringsWithComma()}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static {e.Name} Instance = new();");
            sb.AppendLine($"    public static ReadOnlySpan<byte> NAME => \"{e.Name.ToUpperInvariant()}\"u8;");
            sb.AppendLine($"    public const uint ENTITY_CODE = {code};");
            sb.AppendLine($"    public override uint EntityTypeCode => ENTITY_CODE;");
            sb.AppendLine($"    public override ReadOnlySpan<byte> EntityTypeName => NAME;");

            var inheritedAttrs = GetInheritedAttributes(e, entityLookup).ToList();
            var attrIndex = inheritedAttrs.Count;

            foreach (var attr in e.Attributes)
            {
                var (typeStr, rank) = GetTypeDetails(attr.TypeExpr);
                var kind = typeKinds.GetValueOrDefault(typeStr, IfcTypeKind.Unknown);
                sb.AppendLine($"    public readonly IfcAttribute<{typeStr}> {attr.Name} = new(\"{attr.Name}\", {attrIndex++}, IfcTypeKind.{kind}, {rank});");
            }

            var attrNames = inheritedAttrs.Concat(e.Attributes).Select(attr => attr.Name).JoinStringsWithComma();
            sb.AppendLine($"    public override IfcAttribute[] Attributes => [ {attrNames} ];");
            sb.AppendLine("}");
        }

        return sb;
    }

    public static StringBuilder OutputSchema(IfcSchema schema, string ns, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();
        sb.AppendLine($"using Ara3D.IfcTypes.{ns};");
        sb.AppendLine();
        sb.AppendLine("namespace Ara3D.IfcTypes;");
        sb.AppendLine();
        sb.AppendLine("public static partial class IfcSchemas");
        sb.AppendLine("{");
        sb.AppendLine($"    public static IfcSchema {ns} = new(IfcSchemaEnum.{ns}, [");

        var first = true;
        foreach (var e in schema.Entities)
        {
            if (!first)
                sb.AppendLine(",");
            else
                first = false;
            sb.Append($"        {e.Name}.Instance");
        }

        sb.AppendLine("]);");
        sb.AppendLine("}");
        return sb;
    }

    public static StringBuilder OutputInterfaces(IfcSchema schema, string ns, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();
        var selects = schema.GetSelects();
        sb.AppendLine($"namespace Ara3D.IfcTypes.{ns};");
        foreach (var s in selects)
        {
            sb.AppendLine();
            sb.AppendLine($"public interface {s.Name} : ISelectInterface {{ }}");
        }

        return sb;
    }

    public static StringBuilder OutputEnums(IfcSchema schema, string ns, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();

        var enums = schema.GetEnums();
        sb.Clear();
        sb.AppendLine($"namespace Ara3D.IfcTypes.{ns};");
        foreach (var e in enums)
        {
            sb.AppendLine();
            sb.AppendLine($"public enum {e.Name}");
            sb.AppendLine("{");
            foreach (var o in e.Options)
                sb.AppendLine($"  {o},");
            sb.AppendLine("}");
        }

        return sb;
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

    public static void Generate(ParserInput input, string ns)
    {
        var schema = GetSchema(input);
        var outputDir = PathUtil.GetCallerSourceFolder().RelativeFolder("..", "..", "ext", "Ara3D.IfcTypes");

        var typeKinds = new Dictionary<string, IfcTypeKind>();
        typeKinds.Add("double", IfcTypeKind.System);
        typeKinds.Add("bool", IfcTypeKind.System);
        typeKinds.Add("string", IfcTypeKind.System);
        typeKinds.Add("long", IfcTypeKind.System);

        var typeAliases = schema.GetTypeAliases().ToDictionary(ta => ta.Name, ta => ta.TypeExpr);
        typeAliases.Add("REAL", new("double", []));
        typeAliases.Add("NUMBER", new("double", []));
        typeAliases.Add("LOGICAL", new("bool", []));
        typeAliases.Add("BOOLEAN", new("bool", []));
        typeAliases.Add("STRING", new("string", []));
        typeAliases.Add("BINARY", new("string", []));
        typeAliases.Add("INTEGER", new("long", []));

        foreach (var kv in typeAliases)
            typeKinds.Add(kv.Key, IfcTypeKind.Alias);

        foreach (var s in schema.GetSelects())
            typeKinds.Add("I" + s.Name, IfcTypeKind.Select);

        foreach (var e in schema.GetEnums())
            typeKinds.Add(e.Name, IfcTypeKind.Enum);
        foreach (var e in schema.Entities)
            typeKinds.Add(e.Name, IfcTypeKind.Entity);

        outputDir.RelativeFile($"Entities.{ns}.g.cs").WriteAllText(OutputEntities(schema, ns, typeKinds).ToString());
        outputDir.RelativeFile($"Interfaces.{ns}.g.cs").WriteAllText(OutputInterfaces(schema, ns).ToString());
        outputDir.RelativeFile($"Enums.{ns}.g.cs").WriteAllText(OutputEnums(schema, ns).ToString());
        outputDir.RelativeFile($"Aliases.{ns}.g.cs").WriteAllText(OutputTypeAliases(schema, ns, typeAliases, typeKinds).ToString());
        outputDir.RelativeFile($"Schema.{ns}.g.cs").WriteAllText(OutputSchema(schema, ns).ToString());
    }

    public static IEnumerable<IfcAttribute> GetInheritedAttributes(this IfcEntity e,
        Dictionary<string, IfcEntity> entityLookup)
    {
        var subType = e.SubType;
        if (subType.IsNullOrWhiteSpace())
            yield break;
        if (entityLookup.TryGetValue(subType, out var subTypeEntity))
        {
            var ancestors = GetInheritedAttributes(subTypeEntity, entityLookup);
            foreach (var a in ancestors)
                yield return a;
            foreach (var attr in subTypeEntity.Attributes)
                yield return attr;
        }
    }

    public static IEnumerable<IfcSelect> GetSelects(this IfcSchema self)
        => self.Decls.OfType<IfcSelect>().OrderBy(s => s.Name);

    public static IEnumerable<IfcEnumeration> GetEnums(this IfcSchema self)
        => self.Decls.OfType<IfcEnumeration>().OrderBy(s => s.Name);

    public static IEnumerable<IfcTypeAlias> GetTypeAliases(this IfcSchema self)
        => self.Decls.OfType<IfcTypeAlias>().OrderBy(s => s.Name);

    public static MultiDictionary<string, IfcSelect> GetSelectsPerEntity(this IfcSchema self)
    {
        var r = new MultiDictionary<string, IfcSelect>();
        foreach (var s in self.GetSelects())
        foreach (var o in s.Options)
            r.Add(o, s);
        return r;
    }

    public static ParserTreeNode GetEntityBody(ParserTreeNode node)
    {
        Debug.Assert("Entity" == node.Node.Name);
        return node.Children.FirstOrDefault(c => c.Node.Name == "EntityBody");
    }

    public static string GetEntityName(ParserTreeNode node)
    {
        Debug.Assert("Entity" == node.Node.Name);
        return node.Children[0].Children[0].Contents;
    }

    public static IfcTypeExpr ToTypeExpr(ParserTreeNode node)
    {
        Debug.Assert("TypeExpr" == node.Node.Name);
        var partTwo = node.Children[0];

        if (partTwo.Node.Name == "AggregationType")
        {
            Debug.Assert(1 == partTwo.Children.Count);
            var innerType = ToTypeExpr(partTwo.Children[0]);
            return new("List", [innerType]);
        }
        
        if (partTwo.Node.Name == "Identifier")
            return new(partTwo.Node.Contents, []);

        throw new Exception($"Unexpected type: {node}");
    }

    public static string GetEntitySubType(ParserTreeNode node)
    {
        Debug.Assert("Entity" == node.Node.Name);
        var header = node.Children[0];
        var subTypeHeader = header.Children.FirstOrDefault(c => c.Node.Name == "SubtypeHeader");
        if (subTypeHeader == null) return null;
        var identList = subTypeHeader.Children[0];
        return identList.Children[0].Contents;
    }

    public static IfcEntity ToEntity(string text)
    {
        var rule = ExpressGrammar.Instance.Entity;
        var ps = rule.Parse(text);
        if (ps == null || !ps.AtEnd()) throw new Exception("Failed to parse");
        return ToEntity(ps.Node.ToParseTree());
    }

    public static IIfcTypeDecl ToTypeDecl(string text)
    {
        var typeRule = ExpressGrammar.Instance.TypeDecl;

        var ps = typeRule.Parse(text);
        if (ps == null)
            throw new Exception("Parsing failed");
        var tree = ps.Node.ToParseTree();
        var name = tree.Children[0].Node.Contents;
        var typeDef = tree.Children[1].Children[0];

        if (typeDef.Node.Name == "EnumerationType")
            return ToEnum(typeDef, name);
        if (typeDef.Node.Name == "SelectType")
            return ToSelect(typeDef, name);
        if (typeDef.Node.Name == "TypeExpr")
            return new IfcTypeAlias(name, ToTypeExpr(typeDef));
        
        throw new Exception($"Unrecognized type {typeDef}");
    }

    public static IfcEntity ToEntity(ParserTreeNode node)
    {
        var name = GetEntityName(node);
        var subType = GetEntitySubType(node);
        var body = GetEntityBody(node);
        var attrs = body.Children.Select(ToAttribute).ToList();
        return new(name, subType, attrs);
    }

    public static IfcAttribute ToAttribute(ParserTreeNode node)
    {
        Debug.Assert("AttributeDecl" == node.Node.Name);
        var name = node.Children[0].Contents;
        var type = ToTypeExpr(node.Children[1]);
        return new(name, type);
    }

    public static IfcSelect ToSelect(ParserTreeNode node, string name)
    {
        Debug.Assert("SelectType" == node.Node.Name);
        var options = node.Children[0].Children.Select(c => c.Contents);
        return new(name, options.ToList());
    }

    public static IfcEnumeration ToEnum(ParserTreeNode node, string name)
    {
        Debug.Assert("EnumerationType" == node.Node.Name);
        var options = node.Children[0].Children.Select(c => c.Contents);
        return new(name, options.ToList());
    }



}