using Autodesk.Revit.DB;
using System;

namespace Ara3D.Bowerbird.RevitSamples;

public static class VisibilityExtensions
{
    /// <summary>
    /// Returns true if this element's category is:
    ///   - Non-null,
    ///   - Not analytical, and
    ///   - Not hidden in the host view's VG.
    /// 
    /// Uses host view's CanCategoryBeHidden / GetCategoryHidden.
    /// Category.Id values are stable across documents, so link
    /// categories can be tested against the host view.
    /// </summary>
    public static bool IsElementCategoryVisibleInHostView(View hostView, Element e)
    {
        var cat = e.Category;
        if (cat == null)
            return false;

        // Drop analytical categories explicitly.
        if (cat.CategoryType == CategoryType.AnalyticalModel)
            return false;

        var catId = cat.Id;

        // If the view can't hide the category, treat it as always visible.
        if (!hostView.CanCategoryBeHidden(catId))
            return true;

        // Otherwise respect VG:
        return !hostView.GetCategoryHidden(catId);
    }

    /// <summary>
    /// Gets the view's Phase (if any) using the VIEW_PHASE built-in parameter.
    /// Works for model views that support phasing (plans, sections, 3D, etc.).
    /// </summary>
    public static Phase GetViewPhase(View view)
    {
        var phaseParam = view.get_Parameter(BuiltInParameter.VIEW_PHASE);
        if (phaseParam == null || phaseParam.StorageType != StorageType.ElementId)
            return null;

        var phaseId = phaseParam.AsElementId();
        if (phaseId == ElementId.InvalidElementId)
            return null;

        return view.Document.GetElement(phaseId) as Phase;
    }

    /// <summary>
    /// Maps the host phase to a phase in the linked document by NAME.
    /// This is a common pattern when phases are kept consistent between models.
    /// If mapping fails, returns null (meaning: don't phase-filter that link).
    /// </summary>
    public static Phase MapPhaseByName(Phase hostPhase, Document linkDoc)
    {
        if (hostPhase == null || linkDoc == null)
            return null;

        foreach (Phase p in linkDoc.Phases)
        {
            if (string.Equals(p.Name, hostPhase.Name, StringComparison.OrdinalIgnoreCase))
                return p;
        }

        return null;
    }

    /// <summary>
    /// Returns true if the element "exists" in the given viewPhase:
    ///   - If the element does not support phasing: returns true.
    ///   - Otherwise:
    ///       created.sequence <= view.sequence
    ///       AND (demolished is null OR demolished.sequence > view.sequence)
    ///
    /// This approximates: element is present at the time of the view's phase,
    /// independent of the view's Phase Filter.
    /// </summary>
    public static bool IsElementInPhaseRange(Element e, Phase viewPhase)
    {
        if (viewPhase == null)
            return true; // no phasing context – treat as always OK

        // If element does not support phasing, don't cull it.
        if (!e.ArePhasesModifiable())  // Element.ArePhasesModifiable() is an Element method. 
            return true;

        var doc = e.Document;

        var createdId = e.CreatedPhaseId;
        var demolishedId = e.DemolishedPhaseId;

        var created = doc.GetElement(createdId) as Phase;
        var demolished = doc.GetElement(demolishedId) as Phase;

        var viewSeq = viewPhase.GetPhaseSequenceNumber();     // Phase.SequenceNumber describes its order. 
        var createdSeq = created?.GetPhaseSequenceNumber() ?? 0; // if null, treat as very early
        var demoSeq = demolished?.GetPhaseSequenceNumber();

        var createdOnOrBefore = createdSeq <= viewSeq;
        var notDemolishedYet = !demoSeq.HasValue || demoSeq.Value > viewSeq;

        return createdOnOrBefore && notDemolishedYet;
    }
}
