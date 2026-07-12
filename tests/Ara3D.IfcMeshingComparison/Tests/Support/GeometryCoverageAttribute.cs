namespace Ara3D.IfcMeshingComparison.Tests.Support;

/// <summary>Links a test method to a geometry-creation entity covered by the coverage gate.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class GeometryCoverageAttribute(string entityName) : Attribute
{
    public string EntityName { get; } = entityName;
}
