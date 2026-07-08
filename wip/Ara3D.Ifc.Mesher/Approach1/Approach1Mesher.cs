using Ara3D.IfcLoader;
using Ara3D.Ifc.Mesher;
using Ara3D.Utils;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>
/// Modular pure C# mesher: <see cref="GeometryDispatcher"/> + <see cref="ModelAssembler"/>.
/// </summary>
public sealed class Approach1Mesher : IIfcMesher
{
    public string Name => "Approach1";
    public string Description =>
        "Modular pure C# meshing via GeometryDispatcher and ModelAssembler.";

    public IfcMeshingResult Build(IfcFile file)
    {
        try
        {
            var (model, diagnostics) = ModelAssembler.BuildModel(file);
            return IfcMeshingResult.FromModel(Name, model, diagnostics.Messages);
        }
        catch (Exception ex)
        {
            return IfcMeshingResult.Failed(Name, ex.Message);
        }
    }

    public IfcMeshingResult Build(FilePath ifcPath)
    {
        using var file = new IfcFile(ifcPath, includeGeometry: false);
        return Build(file);
    }
}
