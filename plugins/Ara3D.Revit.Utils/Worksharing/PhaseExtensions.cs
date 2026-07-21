using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class PhaseExtensions
{
    public static IReadOnlyList<Phase> GetPhases(this Document doc)
        => doc.GetElements<Phase>();

    public static int GetPhaseSequenceNumber(this Phase phase)
        => phase.GetParameter(BuiltInParameter.PHASE_SEQUENCE_NUMBER).AsInteger();

    /// <summary>Throws if the document has no phases (Get verb; contract violation).</summary>
    public static Phase GetLastPhase(this Document doc)
        => doc.GetPhases().OrderBy(GetPhaseSequenceNumber).Last();

    /// <summary>The element's creation phase. Throws if <c>CreatedPhaseId</c> doesn't resolve.</summary>
    public static Phase GetPhase(this Element element)
        => (Phase)element.Document.GetRequiredElement(element.CreatedPhaseId);

    /// <summary>The element's creation phase, or null if it doesn't carry one (P8).</summary>
    public static Phase? GetCreatedPhase(this Element element)
        => element.Document.GetElement(element.CreatedPhaseId) as Phase;

    /// <summary>The element's demolition phase, or null if it was never demolished (P8).</summary>
    public static Phase? GetDemolishedPhase(this Element element)
        => element.Document.GetElement(element.DemolishedPhaseId) as Phase;

    /// <summary>
    /// True if the element exists at <paramref name="phase"/>: created on or before it and,
    /// if demolished, demolished strictly after it. The reusable phase-existence test.
    /// </summary>
    public static bool ExistsInPhase(this Element element, Phase phase)
    {
        var target = phase.GetPhaseSequenceNumber();
        var created = element.GetCreatedPhase()?.GetPhaseSequenceNumber() ?? 0;
        var demolished = element.GetDemolishedPhase()?.GetPhaseSequenceNumber();
        return created <= target && (demolished is null || demolished > target);
    }
}
