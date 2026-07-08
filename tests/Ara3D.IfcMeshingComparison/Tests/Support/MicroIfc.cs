using System.Text;
using Ara3D.IfcLoader;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IO.StepParser;
using Ara3D.Memory;

namespace Ara3D.IfcMeshingComparison.Tests.Support;

/// <summary>Helper for hand-written micro IFC snippets in tests.</summary>
public static class MicroIfc
{
    public const string Header = """
        ISO-10303-21;
        HEADER;
        FILE_DESCRIPTION(('ViewDefinition [CoordinationView]'),'2;1');
        FILE_NAME('micro.ifc','2026-01-01T00:00:00',(''),(''),'','','');
        FILE_SCHEMA(('IFC4'));
        ENDSEC;
        DATA;
        """;

    public const string Footer = """
        ENDSEC;
        END-ISO-10303-21;
        """;

    public static string WrapData(string dataLines)
        => $"{Header}\n{dataLines.Trim()}\n{Footer}";

    public static MicroIfcModel Parse(string dataLines, int circleSegments = 32, double? lengthScaleOverride = null)
        => ParseContent(WrapData(dataLines), circleSegments, lengthScaleOverride);

    public static MicroIfcModel ParseContent(string ifcContent, int circleSegments = 32, double? lengthScaleOverride = null)
    {
        var document = new StepDocument(Encoding.ASCII.GetBytes(ifcContent).Fix());
        var ctx = new MeshingContext(document, lengthScaleOverride, circleSegments);
        return new MicroIfcModel(ctx);
    }

    public static MicroIfcModel WriteTemp(string dataLines)
    {
        var path = Path.Combine(Path.GetTempPath(), $"micro-ifc-{Guid.NewGuid():N}.ifc");
        File.WriteAllText(path, WrapData(dataLines));
        var file = new IfcFile(new Ara3D.Utils.FilePath(path), includeGeometry: false);
        return new MicroIfcModel(new MeshingContext(file));
    }
}

public sealed class MicroIfcModel : IDisposable
{
    public MicroIfcModel(MeshingContext context)
    {
        Context = context;
    }

    public MeshingContext Context { get; }
    public IfcEntityResolver Resolver => Context.Resolver;

    public IfcEntity Entity(int id) => Context.GetEntity(id);

    public void Dispose() => Context.Dispose();
}
