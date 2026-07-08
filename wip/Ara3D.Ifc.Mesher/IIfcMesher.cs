using Ara3D.IfcLoader;
using Ara3D.Utils;

namespace Ara3D.Ifc.Mesher;

/// <summary>Common entry point for IFC meshing approaches under evaluation.</summary>
public interface IIfcMesher
{
    string Name { get; }
    string Description { get; }
    IfcMeshingResult Build(IfcFile file);
    IfcMeshingResult Build(FilePath ifcPath);
}
