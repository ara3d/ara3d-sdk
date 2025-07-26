namespace Ara3D.IO.StepParser;

public class StepValueEntity : StepValue
{
    public readonly StepEntityName EntityName;
    public readonly StepValueList Attributes;

    public StepValueEntity(StepEntityName entityName, StepValueList attributes)
    {
        EntityName = entityName;
        Attributes = attributes;
    }

    public override string ToString()
        => $"{EntityName}{Attributes}";
}