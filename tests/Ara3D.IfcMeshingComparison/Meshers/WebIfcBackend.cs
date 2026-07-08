using System.Diagnostics;
using Ara3D.IfcLoader;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Meshers;

/// <summary>Native web-ifc tessellation via <see cref="IfcFile"/> with <c>includeGeometry: true</c>.</summary>
public sealed class WebIfcBackend : IMeshingBackend
{
    public string Name => "WebIfcDll";
    public string Description =>
        "Native web-ifc DLL: IfcFile(includeGeometry:true).ToModel3D(). " +
        "Same path exercised by Tests/Native/IfcMeshingTests native geometry tests.";

    public MeshingResult Build(FilePath ifcPath)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            using var file = new IfcFile(ifcPath, includeGeometry: true);
            var model = file.ToModel3D();
            sw.Stop();
            return MeshingResult.FromModel(Name, ifcPath, sw.ElapsedMilliseconds, model);
        }
        catch (Exception ex)
        {
            sw.Stop();
            return MeshingResult.Failed(Name, ifcPath, sw.ElapsedMilliseconds, ex.Message);
        }
    }
}
