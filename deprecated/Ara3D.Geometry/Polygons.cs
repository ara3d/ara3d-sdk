using System;
using System.Collections.Generic;
using Ara3D.Collections;
using Ara3D.Mathematics;
using Ara3D.Utils;

namespace Ara3D.Geometry
{
    public class Polygon : PolyLine2D, IPolygon, IDeformable2D<Polygon>
    {
        public Polygon(IArray<Vector2> points)
            : base(points, true)
        { }

        public Polygon Deform2D(Func<Vector2, Vector2> f)
            => new Polygon(Points.Select(f));

        IPolygon IDeformable2D<IPolygon>.Deform2D(Func<Vector2, Vector2> f)
            => Deform2D(f);
    }

    public enum CommonPolygonsEnum
    {
        // Regular polygons
        Triangle, 
        Square,
        Pentagon,
        Hexagon,
        Heptagon,
        Octagon,
        Nonagon,
        Decagon, 
        Dodecagon,
        Icosagon,
        Centagon,
        
        // Star figures 
        Pentagram,
        Octagram,
        Decagram,
    }

    public static class Polygons
    {
        public static IPolygon ToPolygon(this IArray<Vector2> points) 
            => new Polygon(points);

        public static IPolygon RegularPolygon(int n)
            => Curves2D.Circle.Sample(n).ToPolygon();
        
        public static readonly IPolygon Triangle = RegularPolygon(3);
        public static readonly IPolygon Square = RegularPolygon(4);
        public static readonly IPolygon Pentagon = RegularPolygon(5);
        public static readonly IPolygon Hexagon = RegularPolygon(6);
        public static readonly IPolygon Heptagon = RegularPolygon(7);
        public static readonly IPolygon Septagon = RegularPolygon(7);
        public static readonly IPolygon Octagon = RegularPolygon(8);
        public static readonly IPolygon Nonagon = RegularPolygon(9);
        public static readonly IPolygon Decagon = RegularPolygon(10);
        public static readonly IPolygon Dodecagon = RegularPolygon(12);
        public static readonly IPolygon Icosagon = RegularPolygon(20);
        public static readonly IPolygon Centagon = RegularPolygon(100);

        public static IPolygon RegularStarPolygon(int p, int q)
            => Curves2D.Circle.Sample(p).SelectEveryNth(q).ToPolygon();

        public static IPolyLine2D StarFigure(int p, int q)
        {
            Verifier.Assert(p > 1);
            Verifier.Assert(q > 1);
            if (p.RelativelyPrime(q))
                return RegularStarPolygon(p, q);
            var points = Curves2D.Circle.Sample(p);
            var r = new List<Vector2>();
            var connected = new bool[p];
            for (var i = 0; i < p; ++i)
            {
                if (connected[i])
                    break;
                var j = i;
                while (j != i)
                {
                    r.Add(points[j]);
                    j = (j + q) % p;
                    if (connected[j])
                        break;
                    connected[j] = true;
                }
            }
            return new PolyLine2D(r.ToIArray(), false);
        }

        // https://en.wikipedia.org/wiki/Pentagram
        public static readonly IPolygon Pentagram 
            = RegularStarPolygon(5, 2);

        public static readonly IPolyLine2D Hexagram
            = StarFigure(6, 2);

        // https://en.wikipedia.org/wiki/Heptagram
        public static readonly IPolygon Heptagram2
            = RegularStarPolygon(7, 2);

        // https://en.wikipedia.org/wiki/Heptagram
        public static readonly IPolygon Heptagram3
            = RegularStarPolygon(7, 3);

        // https://en.wikipedia.org/wiki/Octagram
        public static readonly IPolygon Octagram
            = RegularStarPolygon(8, 3);

        // https://en.wikipedia.org/wiki/Enneagram_(geometry)
        public static readonly IPolygon Nonagram2
            = RegularStarPolygon(9, 2);

        // https://en.wikipedia.org/wiki/Enneagram_(geometry)
        public static readonly IPolygon Nonagram4
            = RegularStarPolygon(9, 4);

        // https://en.wikipedia.org/wiki/Decagram_(geometry)
        public static readonly IPolygon Decagram
            = RegularStarPolygon(10, 3);

        public static IPolygon ToPolygon(this CommonPolygonsEnum polygonEnum)
            => typeof(Polygons).GetField(polygonEnum.ToString()).GetValue(null) as IPolygon;
    }
}