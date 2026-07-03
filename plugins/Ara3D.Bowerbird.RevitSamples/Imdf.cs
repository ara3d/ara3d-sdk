using System.Collections.Generic;
using Ara3D.Utils;

/*
 * Indoor Mapping Data Format (IMDF)
 * https://docs.ogc.org/cs/20-094/index.html
 * A GeoJson compatible standard for expressing indoor maps:
 * https://datatracker.ietf.org/doc/html/rfc7946
 */

namespace Ara3D.Bowerbird.RevitSamples;

//==
// These are convenience objects 

// https://docs.ogc.org/cs/20-094/Unit/index.html#unit

// https://docs.ogc.org/cs/20-094/Opening/index.html


//==

// https://docs.ogc.org/cs/20-094/Opening/index.html
// https://www.essential.co.uk/blog/articles/places-imdf-map-orientation/

public class GeoJsonDocument
{
    public string type { get; set; } = "FeatureCollection";
    public List<object> features { get; set; } = [];
}

// https://docs.ogc.org/cs/20-094/Unit/index.html
// This is a room, zone, area, what have you. 
public class ImdfUnit
{
    public string type { get; set; } = "Feature";
    public string feature_type { get; set; } = "unit";
    public string id { get; set; }
    public Dictionary<string, object> properties { get; set; } 
    public GeoJsonPolygon geometry { get; set; }

    public ImdfUnit SetProperties(ImdfUnitProperties props)
    {
        if (props == null)
            return this;
        properties ??= new();
        foreach (var kv in props.PropertiesToDictionary())
            properties.Add(kv.Key, kv.Value);
        return this;
    }

    public static ImdfUnit Create(string id, GeoJsonPolygon geometry, ImdfUnitProperties props = null)
        => new ImdfUnit()
        {
            id = id,
            geometry = geometry
        }.SetProperties(props);
}

public class ImdfUnitProperties
{
    public string level_id { get; set; }
    public string category { get; set; }
    public string restriction { get; set; }
    public string accessibility { get; set; }
    public string name { get; set; }
    public string alt_name { get; set; }
    public GeoJsonPoint DisplayGeoJsonPoint { get; set; } 

    // Additional non-standard properties
    public double? level_elevation { get; set; }
    public double? area { get; set; }
    public double? perimeter { get; set; }
}

// https://docs.ogc.org/cs/20-094/Opening/index.html
public class ImdfOpening
{
    public string type { get; set; } = "Feature";
    public string feature_type { get; set; } = "opening";
    public string id { get; set; }
    public List<double> display_point { get; set; }
    public Dictionary<string, object> properties { get; set; }
    public GeoJsonLineString geometry { get; set; }

    public ImdfOpening SetProperties(ImdfOpeningProperties props)
    {
        if (props == null)
            return this;
        properties ??= new();
        foreach (var kv in props.PropertiesToDictionary())
            properties.Add(kv.Key, kv.Value);
        return this;
    }

    public static ImdfOpening Create(string id, GeoJsonLineString geometry, ImdfOpeningProperties props = null)
        => new ImdfOpening()
        {
            id = id,
            geometry = geometry
        }.SetProperties(props);
}

public class ImdfOpeningProperties
{
    public string level_id { get; set; }
    public string category { get; set; }
    public string access_control { get; set; }
    public string accessibility { get; set; }
    public string name { get; set; }
    public string door { get; set; }
    public string alt_name { get; set; }
    public GeoJsonPoint DisplayGeoJsonPoint { get; set; }

    // Additional non-standard properties
    public string level_elevation { get; set; }
    public string from_room_id { get; set; }
    public string to_room_id { get; set; }
    public string family_name { get; set; }
}

//==
// Geometry 

public class GeoJsonLineString
{
    public string type { get; set; } = "LineString";
    // For type "LineString", the "coordinates" member must be an array of two or more positions.
    public List<List<double>> coordinates { get; set; } = [];
}

public class GeoJsonPolygon
{
    public string type { get; set; } = "Polygon";
    // For type "Polygon", the "coordinates" member must be an array of LinearRing coordinate arrays. For Polygons with multiple rings, the first must be the exterior ring and any others must be interior rings or holes.
    public List<List<List<double>>> coordinates { get; set; } = [];
}

public class GeoJsonPoint
{
    public string type { get; set; } = "Point";
    // For type "Point", the "coordinates" member must be a single position.
    public List<double> coordinates { get; set; } = [];
}

//==