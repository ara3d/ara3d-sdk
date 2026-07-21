using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;

namespace Ara3D.Revit.Utils;

/// <summary>Duct connector-profile and sizing queries, internal units (feet, P7).</summary>
public static class DuctQueries
{
    /// <summary>The connector profile shape shared by a duct's connectors (round/rectangular/oval).</summary>
    public static ConnectorProfileType GetShape(this Duct duct)
        => duct.ConnectorManager.GetConnectors().First().Shape;

    public static IReadOnlyList<ConnectorProfileType> GetConnectorProfileTypes(this Duct duct)
        => duct.ConnectorManager.GetConnectors().Select(c => c.Shape).ToList();

    /// <summary>Duct cross-section size for the given shape: diameter when round, otherwise height.</summary>
    public static double GetSize(this Duct duct, ConnectorProfileType shape)
        => shape == ConnectorProfileType.Round ? duct.Diameter : duct.Height;

    /// <summary>Connector cross-section size: diameter when round, otherwise height.</summary>
    public static double GetSize(this Connector connector)
        => connector.Shape == ConnectorProfileType.Round ? 2 * connector.Radius : connector.Height;
}
