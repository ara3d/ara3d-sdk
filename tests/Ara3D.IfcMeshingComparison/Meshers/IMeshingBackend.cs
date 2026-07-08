using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Meshers;

/// <summary>Common interface for IFC meshing backends under comparison.</summary>
public interface IMeshingBackend
{
    string Name { get; }
    string Description { get; }
    MeshingResult Build(FilePath ifcPath);
}
