namespace Ara3D.Ifc.Tests;

public static class IfcGuidTests
{
    [Test]
    public static void RoundTripsRandomGuids()
    {
        for (var i = 0; i < 100; i++)
        {
            var g = Guid.NewGuid();
            var text = g.ToIfcGuid();
            Assert.That(text, Has.Length.EqualTo(IfcGuid.Length));
            Assert.That(IfcGuid.IsValid(text), Is.True);
            Assert.That(IfcGuid.FromIfcGuid(text), Is.EqualTo(g));
        }
    }

    [Test]
    public static void KnownIdsFromTestDataAreValid()
    {
        Assert.That(IfcGuid.IsValid("2OrWItJ6zAwBNp0OUxK_l8"), Is.True);
        Assert.That(IfcGuid.IsValid("1xS3BCk291UvhgP2dvNMKI"), Is.True);
        Assert.That(IfcGuid.IsValid("3bXiCStxP6Fgxdej$yc5T8"), Is.True);
        Assert.That(IfcGuid.IsValid("not-a-guid"), Is.False);
    }

    [Test]
    public static void KnownIdRoundTripsThroughGuid()
    {
        const string text = "2OrWItJ6zAwBNp0OUxK_l8";
        Assert.That(IfcGuid.FromIfcGuid(text).ToIfcGuid(), Is.EqualTo(text));
    }

    [Test]
    public static void DeterministicGuidIsStable()
    {
        var first = IfcGuid.Deterministic("key").ToIfcGuid();
        Assert.That(IfcGuid.Deterministic("key").ToIfcGuid(), Is.EqualTo(first));
        Assert.That(IfcGuid.Deterministic("a").ToIfcGuid(), Is.Not.EqualTo(IfcGuid.Deterministic("b").ToIfcGuid()));
    }
}
