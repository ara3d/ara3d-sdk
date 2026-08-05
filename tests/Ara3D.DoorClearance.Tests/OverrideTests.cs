using Ara3D.Ifc.Tests;

namespace Ara3D.DoorClearance.Tests;

/// <summary>
/// A human reviewer overrides a failing verdict: the override is recorded in the IFC file itself
/// as an appended "Ara3D_Compliance" property set, verified with an entity-level diff, then
/// removed again to restore the model byte-for-byte. The source model is never modified — all
/// edits happen on a copy in the artifacts folder.
/// </summary>
public static class OverrideTests
{
    [Test]
    public static void OverrideRoundTripsThroughAppendedPropertySet()
    {
        TestPaths.RequireTestKit();
        var workPath = Path.Combine(TestPaths.OutputFolder, "duplex-override-work.ifc");
        File.Copy(TestPaths.DuplexIfc, workPath, overwrite: true);

        using var original = IfcSourceFile.Load(workPath);
        var facts = ModelFacts.Extract(original);
        var rules = RuleSet.Load(TestPaths.RulesJson);
        var verdicts = ComplianceChecker.Evaluate(facts, rules);

        var failing = verdicts.First(v => v.RuleId == "DC-W1" && v.Verdict == Verdict.Fail);
        var door = facts.Doors.First(d => d.GlobalId == failing.GlobalId);

        var builder = new IfcPropertySetBuilder(original.MaxId + 1, original.FirstIdOfType("IFCOWNERHISTORY"));
        builder.AddPropertySet(door.Id, "Ara3D_Compliance",
            new[]
            {
                IfcPropertyValue.Label("Verdict", VerdictReport.VerdictText(failing.Verdict)),
                IfcPropertyValue.Label("OverrideVerdict", VerdictReport.VerdictText(Verdict.Pass)),
                IfcPropertyValue.Text("OverrideReason",
                    "Door serves a storage room outside the accessible path of travel; provision does not apply."),
                IfcPropertyValue.Label("ReviewedBy", "C. Diggins"),
            },
            $"override:{failing.GlobalId}:{failing.RuleId}");

        var overriddenPath = Path.Combine(TestPaths.OutputFolder, "duplex-override-applied.ifc");
        File.WriteAllBytes(overriddenPath, IfcPatcher.Append(original, builder.Lines));

        using var overridden = IfcSourceFile.Load(overriddenPath);
        var diff = IfcDiff.Compare(original, overridden);
        Assert.That(diff.Added, Is.EqualTo(builder.Ids), "exactly the pset entities must be added");
        Assert.That(diff.Deleted, Is.Empty);
        Assert.That(diff.Changed, Is.Empty);

        var restored = IfcPatcher.Remove(overridden, diff.Added);
        Assert.That(restored, Is.EqualTo(File.ReadAllBytes(TestPaths.DuplexIfc)),
            "removing the override must restore duplex.ifc byte-for-byte");
    }
}
