using System.Collections.Generic;
using System.Linq;
using Ara3D.Geometry;

namespace Ara3D.Bowerbird.RevitSamples;

public static class ImdfHelpers
{
    public static List<double> ToGeoJsonCoordinates(this Vector2 v)
        => [v.X, v.Y];

    public static List<double> ToGeoJsonCoordinates(this Vector3 v)
        => [v.X, v.Y, v.Z];

    public static List<List<double>> ToGeoJsonCoordinates(this IEnumerable<Vector2> v)
        => v.Select(ToGeoJsonCoordinates).ToList();

    public static List<List<double>> ToGeoJsonCoordinates(this IEnumerable<Vector3> v)
        => v.Select(ToGeoJsonCoordinates).ToList();

    public static List<List<List<double>>> ToGeoJsonCoordinates(this IEnumerable<IEnumerable<Vector2>> v)
        => v.Select(ToGeoJsonCoordinates).ToList();

    public static List<List<List<double>>> ToGeoJsonCoordinates(this IEnumerable<IEnumerable<Vector3>> v)
        => v.Select(ToGeoJsonCoordinates).ToList();

    public static GeoJsonPoint ToGeoJson(this Vector2 v)
        => new () { coordinates = v.ToGeoJsonCoordinates() };

    public static GeoJsonPoint ToGeoJson(this Vector3 v)
        => new () { coordinates = v.ToGeoJsonCoordinates() };

    public static GeoJsonLineString ToGeoJson(this IEnumerable<Vector2> v)
        => new () { coordinates = v.ToGeoJsonCoordinates() };

    public static GeoJsonLineString ToGeoJson(this IEnumerable<Vector3> v)
        => new () { coordinates = v.ToGeoJsonCoordinates() };

    public static GeoJsonPolygon ToGeoJson(this IEnumerable<IEnumerable<Vector2>> v)
        => new() { coordinates = v.ToGeoJsonCoordinates() };

    public static GeoJsonPolygon ToGeoJson(this IEnumerable<IEnumerable<Vector3>> v)
        => new() { coordinates = v.ToGeoJsonCoordinates() };
}