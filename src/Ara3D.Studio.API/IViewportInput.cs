using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// Live pointer state for the viewport, for scripts that react to the mouse (in-canvas widgets).
/// Mark the script class <c>[PointerTracking]</c> so the host re-evaluates it while the pointer
/// moves (and when the camera moves, since that changes the cursor ray). Retrieved via
/// <c>EvalContext.Services.ViewportInput</c>; null on hosts without a viewport (headless runners),
/// so scripts must keep a fallback path.
/// </summary>
public interface IViewportInput
{
    /// <summary>Cursor position normalized to the viewport: (0,0) top-left, (1,1) bottom-right.</summary>
    Vector2 CursorUV { get; }

    /// <summary>World-space ray from the camera through the cursor.</summary>
    Ray3D CursorRay { get; }

    /// <summary>Object id under the cursor from the GPU pick pass, or -1 over the background.</summary>
    int HoverObjectId { get; }

    /// <summary>True while the primary (left) button is held.</summary>
    bool PrimaryDown { get; }

    /// <summary>
    /// True if a primary click (press and release without dragging — drags orbit the camera)
    /// happened since the last consuming call. Consuming clears the latch, so exactly one
    /// eval observes each click; with several pointer-tracking nodes, first consumer wins.
    /// </summary>
    bool ConsumePrimaryClick();
}
