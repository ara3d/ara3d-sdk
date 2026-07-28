namespace Ara3D.Ifc.Mcp.Tests;

[TestFixture]
public sealed class IfcSessionCacheTests
{
    [Test]
    public void Get_ReturnsTheSameSessionForTheSameFile()
    {
        using var cache = new IfcSessionCache();
        var path = TestModel.RequirePath(TestModel.FzkHaus);
        Assert.That(cache.Get(path), Is.SameAs(cache.Get(path)));
    }

    [Test]
    public void Get_MissingFile_Throws()
    {
        using var cache = new IfcSessionCache();
        Assert.Throws<FileNotFoundException>(() => cache.Get("C:/no-such-model.ifc"));
    }

    [Test]
    public void Get_BlankPath_Throws()
    {
        using var cache = new IfcSessionCache();
        Assert.Throws<ArgumentException>(() => cache.Get("  "));
    }

    [Test]
    public void Close_RemovesTheSession()
    {
        using var cache = new IfcSessionCache();
        var path = TestModel.RequirePath(TestModel.FzkHaus);
        cache.Get(path);

        Assert.That(cache.IsOpen(path), Is.True);
        Assert.That(cache.Close(path), Is.True);
        Assert.That(cache.IsOpen(path), Is.False);
        Assert.That(cache.Close(path), Is.False);
    }

    [Test]
    public void Capacity_EvictsTheLeastRecentlyUsed()
    {
        using var cache = new IfcSessionCache(capacity: 1);
        var first = TestModel.RequirePath(TestModel.FzkHaus);
        var second = TestModel.RequirePath("schependomlaan.ifc");

        cache.Get(first);
        cache.Get(second);

        Assert.That(cache.IsOpen(second), Is.True);
        Assert.That(cache.IsOpen(first), Is.False);
    }

    [Test]
    public void Capacity_MustBePositive()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new IfcSessionCache(0));
}
