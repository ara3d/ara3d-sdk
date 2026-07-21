using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Project location and shared-coordinate queries/mutations (P1: reads never open a
/// transaction; the one mutation documents that it needs one).
/// </summary>
public static class ProjectLocationExtensions
{
    public static ProjectLocation GetActiveProjectLocation(this Document doc)
        => doc.ActiveProjectLocation;

    /// <summary>
    /// The transform from project (internal) coordinates to this location's shared
    /// (world) coordinates, derived from the ProjectPosition reported at the origin.
    /// </summary>
    public static Transform GetProjectToWorld(this Document doc)
    {
        var position = doc.ActiveProjectLocation.GetProjectPosition(XYZ.Zero);
        var rotation = Transform.CreateRotation(XYZ.BasisZ, position.Angle);
        var translation = Transform.CreateTranslation(new XYZ(position.EastWest, position.NorthSouth, position.Elevation));
        return translation.Multiply(rotation);
    }

    /// <summary>
    /// Sets the project's geographic site location (latitude/longitude in radians,
    /// optional time zone offset). Requires an open transaction.
    /// </summary>
    public static void SetProjectLocation(this Document doc, double latitude, double longitude, double? timeZone = null)
    {
        var site = doc.SiteLocation;
        site.Latitude = latitude;
        site.Longitude = longitude;
        if (timeZone.HasValue)
            site.TimeZone = timeZone.Value;
    }
}
