using System.Text.Json;

namespace Ara3D.DoorClearance.Tests;

public sealed record RuleCitation(string Code, string Clause, string Text)
{
    public override string ToString()
        => $"{Code} {Clause}";
}

/// <summary>Which elements a provision speaks to. A null storey means every storey.</summary>
public sealed record RuleApplicability(string EntityType, string? Storey);

public sealed record RuleRequirement(string Kind, string? Source, double? MinWidthMm, double? ToleranceMm, double? ZoneDepthFactor);

public sealed record ComplianceRule(
    string Id,
    RuleCitation Citation,
    RuleApplicability Applicability,
    RuleRequirement Requirement,
    string VerdictSemantics);

public sealed record RuleSet(string Description, IReadOnlyList<ComplianceRule> Rules)
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public static RuleSet Load(string path)
        => JsonSerializer.Deserialize<RuleSet>(File.ReadAllText(path), Options)
           ?? throw new InvalidDataException($"Empty rule file: {path}");
}
