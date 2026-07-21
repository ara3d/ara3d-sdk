using System;
using System.Linq;
using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// View creation. Every member requires an open transaction; none is opened here (P1).
/// </summary>
public static class ViewCreation
{
    /// <summary>requires an open transaction</summary>
    public static View CreateFloorPlan(this Document doc, Level level)
    {
        var viewFamilyType = doc.GetElementTypes<ViewFamilyType>()
            .FirstOrDefault(t => t.ViewFamily == ViewFamily.FloorPlan)
            ?? throw new InvalidOperationException("Document has no floor plan ViewFamilyType.");
        return ViewPlan.Create(doc, viewFamilyType.Id, level.Id);
    }

    /// <summary>Creates a perspective (camera) 3D view. requires an open transaction</summary>
    public static View3D CreatePerspectiveView(this Document doc, Point3D eye, Vector3 upDirection, Vector3 forwardDirection)
    {
        var viewFamilyType = doc.GetElementTypes<ViewFamilyType>()
            .FirstOrDefault(t => t.ViewFamily == ViewFamily.ThreeDimensional)
            ?? throw new InvalidOperationException("Document has no 3D ViewFamilyType.");
        var view = View3D.CreatePerspective(doc, viewFamilyType.Id);
        view.SetOrientation(new ViewOrientation3D(eye.ToXyz(), upDirection.ToXyz(), forwardDirection.ToXyz()));
        return view;
    }
}
