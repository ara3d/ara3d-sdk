using Ara3D.Ifc.Tests;

namespace Ara3D.DoorClearance.Tests;

public static class DoorClearanceTests
{
    private static (ModelFacts Facts, RuleSet Rules, IReadOnlyList<VerdictRecord> Verdicts) Run()
    {
        TestPaths.RequireTestKit();
        using var file = IfcSourceFile.Load(TestPaths.DuplexIfc);
        var facts = ModelFacts.Extract(file);
        var rules = RuleSet.Load(TestPaths.RulesJson);
        return (facts, rules, ComplianceChecker.Evaluate(facts, rules));
    }

    [Test]
    public static void FindsFourteenDoors()
    {
        var (facts, rules, verdicts) = Run();
        Assert.That(facts.Doors, Has.Count.EqualTo(14));
        Assert.That(verdicts, Has.Count.EqualTo(14 * rules.Rules.Count));
    }

    [Test]
    public static void NarrowDoorsFailAndWideDoorsPassTheWidthRule()
    {
        var (facts, _, verdicts) = Run();
        var widthVerdicts = verdicts.Where(v => v.RuleId == "DC-W1").ToDictionary(v => v.GlobalId, v => v.Verdict);

        var narrow = facts.Doors.Where(d => d.ObjectType.StartsWith("0762", StringComparison.Ordinal)).ToList();
        var wide = facts.Doors.Where(d => d.ObjectType.StartsWith("1250", StringComparison.Ordinal)).ToList();
        Assert.That(narrow, Has.Count.EqualTo(4));
        Assert.That(wide, Has.Count.EqualTo(2));

        foreach (var door in narrow)
            Assert.That(widthVerdicts[door.GlobalId], Is.EqualTo(Verdict.Fail), $"762mm door {door.GlobalId}");
        foreach (var door in wide)
            Assert.That(widthVerdicts[door.GlobalId], Is.EqualTo(Verdict.Pass), $"1250mm door {door.GlobalId}");
    }

    [Test]
    public static void AllFourVerdictCategoriesArePresentAcrossTheRuleSet()
    {
        var (_, _, verdicts) = Run();
        var present = verdicts.Select(v => v.Verdict).Distinct().ToList();
        Assert.That(present, Is.EquivalentTo(Enum.GetValues<Verdict>()));
    }

    [Test]
    public static void StoreyFilterMakesLevelTwoDoorsNotApplicable()
    {
        var (facts, _, verdicts) = Run();
        var zone = verdicts.Where(v => v.RuleId == "DC-Z1").ToList();
        var levelOneDoors = facts.Doors.Count(d => d.Storey == "Level 1");
        Assert.That(levelOneDoors, Is.EqualTo(6));
        Assert.That(zone.Count(v => v.Verdict == Verdict.NotApplicable), Is.EqualTo(14 - levelOneDoors));
        Assert.That(zone.Count(v => v.Verdict != Verdict.NotApplicable), Is.EqualTo(levelOneDoors));
    }

    [Test]
    public static void VerdictsAreKeyedByValidUniqueGlobalIds()
    {
        var (_, _, verdicts) = Run();
        foreach (var v in verdicts)
            Assert.That(IfcGuid.IsValid(v.GlobalId), Is.True, $"invalid GlobalId '{v.GlobalId}'");
        var keys = verdicts.Select(v => (v.GlobalId, v.RuleId)).ToList();
        Assert.That(keys, Is.Unique);
    }

    [Test]
    public static void EvaluationIsDeterministicAndWritesArtifacts()
    {
        TestPaths.RequireTestKit();
        var rules = RuleSet.Load(TestPaths.RulesJson);

        byte[] Evaluate()
        {
            using var file = IfcSourceFile.Load(TestPaths.DuplexIfc);
            return VerdictReport.ToCsvBytes(ComplianceChecker.Evaluate(ModelFacts.Extract(file), rules));
        }

        var first = Evaluate();
        var second = Evaluate();
        var hash = VerdictReport.Sha256Hex(first);
        Assert.That(VerdictReport.Sha256Hex(second), Is.EqualTo(hash), "two runs must produce identical verdicts.csv bytes");

        using var f = IfcSourceFile.Load(TestPaths.DuplexIfc);
        var facts = ModelFacts.Extract(f);
        var verdicts = ComplianceChecker.Evaluate(facts, rules);
        File.WriteAllBytes(Path.Combine(TestPaths.OutputFolder, "verdicts.csv"), first);
        File.WriteAllText(Path.Combine(TestPaths.OutputFolder, "run-log.txt"),
            VerdictReport.BuildRunLog(TestPaths.DuplexIfc, facts, rules, verdicts, hash));
    }
}
