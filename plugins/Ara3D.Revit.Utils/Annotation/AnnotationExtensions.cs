using System.Collections.Generic;
using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class AnnotationExtensions
{
    public static IReadOnlyList<Dimension> GetDimensions(this View view)
        => view.GetElementsInView<Dimension>();

    public static IReadOnlyList<IndependentTag> GetTags(this View view)
        => view.GetElementsInView<IndependentTag>();

    /// <summary>
    /// Creates a text note at a point in the view, using the document's default text note
    /// type. Requires an open transaction.
    /// </summary>
    public static TextNote CreateTextNote(this View view, Point3D at, string text)
        => TextNote.Create(view.Document, view.Id, at.ToXyz(), text,
            view.Document.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));

    /// <summary>
    /// Tags an element by category at a point in the view. Requires an open transaction.
    /// </summary>
    public static IndependentTag CreateTag(this View view, Element element, Point3D at, bool addLeader = true)
        => IndependentTag.Create(view.Document, view.Id, element.GetReference(), addLeader,
            TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal, at.ToXyz());

    /// <summary>
    /// Creates a linear dimension along the line from start to end, constrained to the given
    /// geometry references (faces, edges, or element references). Requires an open transaction.
    /// </summary>
    public static Dimension CreateDimension(this View view, Point3D start, Point3D end, IReadOnlyList<Reference> references)
    {
        var line = Line.CreateBound(start.ToXyz(), end.ToXyz());
        var referenceArray = new ReferenceArray();
        foreach (var reference in references)
            referenceArray.Append(reference);
        return view.Document.Create.NewDimension(view, line, referenceArray);
    }
}
