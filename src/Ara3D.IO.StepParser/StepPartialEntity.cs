using System.Collections.Generic;
using System.Linq;

namespace Ara3D.IO.StepParser;

/// <summary>One entity of a definition: the whole of a simple instance <c>#1=A(...)</c>, or one part of a
/// complex instance <c>#1=(A(...)B(...))</c>, whose attribute list holds only the attributes A declares.</summary>
public readonly record struct StepPartialEntity(StepToken NameToken, StepToken AttributesToken);

public static class StepDefinitionExtensions
{
    public static bool IsComplex(this StepDefinition def)
        => def.NameToken.Length == 0;

    /// <summary>The definition's entities in file order; a simple instance has one.</summary>
    public static IReadOnlyList<StepPartialEntity> PartialEntities(this StepDefinition def, StepDocument doc)
        => def.IsComplex()
            ? def.AttributesToken.AsList(doc).Select(name => new StepPartialEntity(name, name.NextToken(doc))).ToList()
            : [new(def.NameToken, def.AttributesToken)];
}
