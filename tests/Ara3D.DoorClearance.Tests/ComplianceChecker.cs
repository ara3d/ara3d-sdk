using System.Globalization;

namespace Ara3D.DoorClearance.Tests;

public enum Verdict
{
    Pass,
    Fail,
    NotApplicable,
    Inconclusive,
}

/// <summary>One (door, rule) evaluation with the evidence that produced it.</summary>
public sealed record VerdictRecord(string GlobalId, string RuleId, Verdict Verdict, string Evidence, string Citation);

/// <summary>
/// Evaluates every rule against every door, producing exactly Doors x Rules verdicts, sorted by
/// (GlobalId, RuleId) so output is deterministic regardless of file order.
/// </summary>
public static class ComplianceChecker
{
    public static IReadOnlyList<VerdictRecord> Evaluate(ModelFacts facts, RuleSet rules)
        => facts.Doors
            .SelectMany(door => rules.Rules.Select(rule => Evaluate(door, rule, facts)))
            .OrderBy(v => v.GlobalId, StringComparer.Ordinal)
            .ThenBy(v => v.RuleId, StringComparer.Ordinal)
            .ToList();

    public static VerdictRecord Evaluate(DoorFact door, ComplianceRule rule, ModelFacts facts)
    {
        var storeyFilter = rule.Applicability.Storey;
        if (storeyFilter != null && !string.Equals(door.Storey, storeyFilter, StringComparison.Ordinal))
            return Record(door, rule, Verdict.NotApplicable,
                $"storey '{door.Storey}' is outside the rule's storey filter '{storeyFilter}'");

        return rule.Requirement.Kind switch
        {
            "property-threshold" => PropertyThreshold(door, rule),
            "measured-vs-declared" => MeasuredVsDeclared(door, rule),
            "zone-unobstructed" => ZoneUnobstructed(door, rule, facts),
            _ => Record(door, rule, Verdict.Inconclusive, $"unknown requirement kind '{rule.Requirement.Kind}'"),
        };
    }

    private static VerdictRecord PropertyThreshold(DoorFact door, ComplianceRule rule)
    {
        var min = rule.Requirement.MinWidthMm ?? 0;
        var (value, how) = rule.Requirement.Source switch
        {
            "attribute:OverallWidth" => (door.DeclaredWidthMm, "IFCDOOR.OverallWidth attribute"),
            "pset:Pset_DoorCommon.ClearWidth" => (door.PsetClearWidthMm, "Pset_DoorCommon.ClearWidth property"),
            _ => ((double?)null, $"unknown source '{rule.Requirement.Source}'"),
        };
        if (value == null)
            return Record(door, rule, Verdict.Inconclusive, $"{how} not present in the model; required >= {Mm(min)}");
        return Record(door, rule, value >= min ? Verdict.Pass : Verdict.Fail,
            $"{how} = {Mm(value.Value)}; required >= {Mm(min)}");
    }

    private static VerdictRecord MeasuredVsDeclared(DoorFact door, ComplianceRule rule)
    {
        var tolerance = rule.Requirement.ToleranceMm ?? 0;
        if (door.LeafWidthMm == null)
            return Record(door, rule, Verdict.Inconclusive,
                $"leaf width could not be read from ObjectType '{door.ObjectType}'");
        if (door.DeclaredWidthMm == null)
            return Record(door, rule, Verdict.Inconclusive, "IFCDOOR.OverallWidth attribute not present");
        var delta = Math.Abs(door.LeafWidthMm.Value - door.DeclaredWidthMm.Value);
        return Record(door, rule, delta <= tolerance ? Verdict.Pass : Verdict.Fail,
            $"leaf width {Mm(door.LeafWidthMm.Value)} (from ObjectType '{door.ObjectType}') vs declared OverallWidth {Mm(door.DeclaredWidthMm.Value)}; delta {Mm(delta)} tolerance {Mm(tolerance)}");
    }

    /// <summary>
    /// Clearance zone as a world axis-aligned box: door placement origin expanded horizontally by
    /// one door width and vertically by the door height. An obstruction is a furnishing element
    /// whose placement origin falls inside the box. AABB-and-origin level only; no meshes.
    /// </summary>
    private static VerdictRecord ZoneUnobstructed(DoorFact door, ComplianceRule rule, ModelFacts facts)
    {
        if (door.WorldPosition == null || door.DeclaredWidthMm == null || door.DeclaredHeightMm == null)
            return Record(door, rule, Verdict.Inconclusive, "door placement or size could not be resolved from STEP data");

        var p = door.WorldPosition.Value;
        var reach = door.DeclaredWidthMm.Value / 1000.0 * (rule.Requirement.ZoneDepthFactor ?? 1.0);
        var height = door.DeclaredHeightMm.Value / 1000.0;
        var min = new Vec3(p.X - reach, p.Y - reach, p.Z - 0.1);
        var max = new Vec3(p.X + reach, p.Y + reach, p.Z + height);

        var blockers = new List<string>();
        foreach (var o in facts.Obstacles)
            if (o.WorldPosition is { } q && Inside(q, min, max))
                blockers.Add($"#{o.Id}");

        var zone = $"zone [{Pt(min)}..{Pt(max)}] m from placement {Pt(p)} m, reach {Mm(reach * 1000)}";
        return blockers.Count == 0
            ? Record(door, rule, Verdict.Pass, $"{zone}; no furnishing origin inside")
            : Record(door, rule, Verdict.Fail, $"{zone}; obstructed by {string.Join(" ", blockers)}");
    }

    private static bool Inside(Vec3 q, Vec3 min, Vec3 max)
        => q.X >= min.X && q.X <= max.X && q.Y >= min.Y && q.Y <= max.Y && q.Z >= min.Z && q.Z <= max.Z;

    private static VerdictRecord Record(DoorFact door, ComplianceRule rule, Verdict verdict, string evidence)
        => new(door.GlobalId, rule.Id, verdict, evidence, rule.Citation.ToString());

    private static string Mm(double value)
        => value.ToString("0.###", CultureInfo.InvariantCulture) + "mm";

    private static string Pt(Vec3 v)
        => FormattableString.Invariant($"({v.X:0.###},{v.Y:0.###},{v.Z:0.###})");
}
