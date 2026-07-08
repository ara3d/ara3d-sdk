using System.Diagnostics;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Meshers;

/// <summary>Pure C# meshing via <see cref="ModelAssembler"/> (Approach1 modular pipeline).</summary>
public sealed class Approach1Backend : IMeshingBackend
{
    public string Name => "Approach1";
    public string Description =>
        "Modular pure C# meshing: IfcFile(includeGeometry:false) + ModelAssembler (Ara3D.Ifc.Mesher.Approach1).";

    public MeshingResult Build(FilePath ifcPath)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            using var file = new IfcFile(ifcPath, includeGeometry: false);
            var (model, diagnostics) = ModelAssembler.BuildModel(file);
            sw.Stop();
            return MeshingResult.FromModel(Name, ifcPath, sw.ElapsedMilliseconds, model, diagnostics);
        }
        catch (Exception ex)
        {
            sw.Stop();
            return MeshingResult.Failed(Name, ifcPath, sw.ElapsedMilliseconds, ex.Message);
        }
    }
}
