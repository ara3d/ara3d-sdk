using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Connector-graph and MEP-system queries shared by ducts, pipes, and MEP family instances.
/// </summary>
public static class MepSystemQueries
{
    /// <summary>Materializes a connector manager's connectors (untyped <c>ConnectorSet</c>) into a typed list.</summary>
    public static IReadOnlyList<Connector> GetConnectors(this ConnectorManager manager)
        => manager.Connectors.Cast<Connector>().ToList();

    public static bool TryGetConnectors(this Element element, out IReadOnlyList<Connector> connectors)
    {
        var manager = element switch
        {
            MEPCurve curve => curve.ConnectorManager,
            FamilyInstance { MEPModel: { } model } => model.ConnectorManager,
            _ => null
        };
        connectors = manager?.GetConnectors() ?? [];
        return manager != null;
    }

    public static IReadOnlyList<Connector> GetConnectors(this Element element)
        => element.TryGetConnectors(out var connectors)
            ? connectors
            : throw new ArgumentException($"Element {element.Id} is not an MEP curve or MEP family instance.", nameof(element));

    /// <summary>The elements at the far end of every reference on this connector (excludes the connector's own owner).</summary>
    public static IReadOnlyList<Element> GetConnectedElements(this Connector connector)
        => connector.AllRefs.Cast<Connector>()
            .Where(c => c.Owner.Id != connector.Owner.Id)
            .Select(c => c.Owner)
            .ToList();

    /// <summary>True for a mechanical or piping system flagged well-connected.</summary>
    public static bool IsWellConnected(this MEPSystem system)
        => system is MechanicalSystem { IsWellConnected: true } or PipingSystem { IsWellConnected: true };

    /// <summary>The best-connected (most elements) well-connected mechanical or piping system touching this element's connectors, or null if none.</summary>
    public static MEPSystem? FindMepSystem(this Element element)
    {
        if (!element.TryGetConnectors(out var connectors))
            return null;

        MEPSystem? best = null;
        var bestElementCount = 0;
        foreach (var connector in connectors)
        {
            var system = connector.MEPSystem;
            if (system is null || !system.IsWellConnected() || system.Elements.Size <= bestElementCount)
                continue;
            best = system;
            bestElementCount = system.Elements.Size;
        }
        return best;
    }

    public static IReadOnlyList<MEPSystem> GetMepSystems(this Document doc)
        => doc.GetElements<MEPSystem>();
}
