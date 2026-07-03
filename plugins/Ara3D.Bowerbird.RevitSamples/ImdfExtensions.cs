using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Architecture;

namespace Ara3D.Bowerbird.RevitSamples;

public static class ImdfExtensions
{
    public static GeoJsonPolygon BoundaryAsGeoJson(this Room room)
        => room.GetBoundaryLoops().Select(loop => loop.Select(pt => pt.ToAra3D())).ToGeoJson();

    public static ImdfUnit ToImdf(this Room room)
        => ImdfUnit.Create(
            room.Id.ToString(),
            room.BoundaryAsGeoJson(),
            new ImdfUnitProperties
            {
                name = room.Name,
                alt_name = room.Number,
                level_elevation = room.Level?.Elevation,
                level_id = room.LevelId.ToString(),
                area = room.Area,
                perimeter = room.Perimeter
            });

    public static GeoJsonDocument ToGeoJson(this IEnumerable<Room> rooms)
    {
        var r = new GeoJsonDocument();
        r.features.Add(rooms.Select(r => r.ToImdf()));
        return r;
    }
}