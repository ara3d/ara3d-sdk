using Ara3D.BimOpenSchema;

namespace Ara3D.BIMOpenSchema.Tests;

public static class BimDataBuilderTests
{
    public static BimData CreateSimpleData()
    {
        var bdb = new BimDataBuilder();
        bdb.Manifest.GeneratorApplication = "Test";
        var doc = bdb.AddDocument("doc", "path");
        var cat = bdb.AddEntity(-1, "", doc, "Walls", BimDataBuilder.InvalidEntityIndex, BimDataBuilder.InvalidEntityIndex);
        var e = bdb.AddEntity(1, "guid-1", doc, "Wall 1", cat, BimDataBuilder.InvalidEntityIndex);
        bdb.AddParameter(e, 42.5, "Area", "squareFeet", "Dimensions");
        bdb.AddParameter(e, "hello", "Comment", "", "Text");
        bdb.AddParameter(e, new Point(1, 2, 3), "Location", "", "Geometry");
        bdb.AddRelation(e, cat, RelationType.ChildOf);
        return bdb.Build();
    }

    [Test]
    public static void BuildPreservesManifest()
    {
        var data = CreateSimpleData();
        Assert.That(data.Manifest, Is.Not.Null);
        Assert.That(data.Manifest.GeneratorApplication, Is.EqualTo("Test"));
    }

    [Test]
    public static void PointParameterDescriptorHasPointType()
    {
        var bdb = new BimDataBuilder();
        var doc = bdb.AddDocument("doc", "path");
        var e = bdb.AddEntity(1, "guid-1", doc, "e", BimDataBuilder.InvalidEntityIndex, BimDataBuilder.InvalidEntityIndex);
        var pi = bdb.AddPoint(new Point(1, 2, 3));
        bdb.AddParameter(e, pi, "Location", "", "Geometry");
        var data = bdb.Build();
        var desc = data.Descriptors[(int)data.Parameters[0].Descriptor];
        Assert.That(desc.Type, Is.EqualTo(ParameterType.Point));
    }

    /// <summary>
    /// Regression test: adding descriptors/points/numbers/strings after AddBimData used to
    /// return corrupt indices, because AddBimData appended to the lists without updating
    /// the deduplication dictionaries, and new indices were computed from the dictionary count.
    /// </summary>
    [Test]
    public static void AddAfterAddBimDataReturnsValidIndices()
    {
        var source = CreateSimpleData();

        var bdb = new BimDataBuilder();
        bdb.AddBimData(source, "merged", "merged-path");

        // These adds happen after the merge; each returned index must point at the right item.
        var di = bdb.AddDescriptor("Bos:Area", "m^2", "Bos", ParameterType.Number);
        var d = bdb.Get(di);
        Assert.That(bdb.Get(d.Name), Is.EqualTo("Bos:Area"));
        Assert.That(bdb.Get(d.Units), Is.EqualTo("m^2"));

        var pi = bdb.AddPoint(new Point(7, 8, 9));
        Assert.That(bdb.Get(pi), Is.EqualTo(new Point(7, 8, 9)));

        var ni = bdb.AddNumber(123.25);
        Assert.That(bdb.Get(ni), Is.EqualTo(123.25f));

        var si = bdb.AddString("some new string");
        Assert.That(bdb.Get(si), Is.EqualTo("some new string"));

        // Adding a duplicate of an item that came in through AddBimData must
        // also resolve to an index holding the same value.
        var pi2 = bdb.AddPoint(new Point(1, 2, 3));
        Assert.That(bdb.Get(pi2), Is.EqualTo(new Point(1, 2, 3)));

        var data = bdb.Build();

        // The merged tables must be internally consistent: every parameter's value index
        // must resolve within the right table.
        foreach (var p in data.Parameters)
        {
            var desc = data.Descriptors[(int)p.Descriptor];
            switch (desc.Type)
            {
                case ParameterType.String:
                    Assert.That(p.Value, Is.InRange(0, data.Strings.Length - 1));
                    break;
                case ParameterType.Number:
                    Assert.That(p.Value, Is.InRange(0, data.Numbers.Length - 1));
                    break;
                case ParameterType.Point:
                    Assert.That(p.Value, Is.InRange(0, data.Points.Length - 1));
                    break;
            }
        }

        // Original parameter values survive the merge.
        var strings = data.Strings.ToHashSet();
        Assert.That(strings, Does.Contain("hello"));
        Assert.That(data.Numbers, Does.Contain(42.5f));
        Assert.That(data.Points, Does.Contain(new Point(1, 2, 3)));
    }
}
