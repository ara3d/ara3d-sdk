namespace Ara3D.Geometry
{
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

    public partial struct PolyLine3D : IDeformable3D<PolyLine3D>
    {
        public PolyLine3D Transform(Transform3D t)
            => Deform(t.TransformPoint);
    }

    public static class Polygons
    {
        public static PolyLine3D ToPolyLine3D(this IReadOnlyList<Point2D> points, bool closed)
            => new(points.To3D(), closed);

        public static PolyLine3D ToPolyLine3D(this Polygon polygon)
            => new(polygon.Points.To3D(), polygon.Closed);

        public static PolyLine3D ToPolyLine3D(this RegularPolygon regularPolygon)
            => new(regularPolygon.Points.To3D(), true);

        public static IReadOnlyList<T> SelectEveryNth<T>(this IReadOnlyList<T> self, Integer n)
            => self.Count.MapRange(i => self[(i * n) % self.Count]);

        public static Polygon ToPolygon(this IReadOnlyList<Point2D> points) 
            => new(points);

        public static IReadOnlyList<Point2D> CirclePoints(this Integer n)
            => n.LinearSpace.Map(t => t.Circle());

        public static Polygon RegularPolygon(int n)
            => CirclePoints(n).ToPolygon();
        
        public static readonly Polygon Triangle = RegularPolygon(3);
        public static readonly Polygon Square = RegularPolygon(4);
        public static readonly Polygon Pentagon = RegularPolygon(5);
        public static readonly Polygon Hexagon = RegularPolygon(6);
        public static readonly Polygon Heptagon = RegularPolygon(7);
        public static readonly Polygon Septagon = RegularPolygon(7);
        public static readonly Polygon Octagon = RegularPolygon(8);
        public static readonly Polygon Nonagon = RegularPolygon(9);
        public static readonly Polygon Decagon = RegularPolygon(10);
        public static readonly Polygon Dodecagon = RegularPolygon(12);
        public static readonly Polygon Icosagon = RegularPolygon(20);
        public static readonly Polygon Centagon = RegularPolygon(100);

        public static Polygon RegularStarPolygon(int p, int q)
            => CirclePoints(p).SelectEveryNth(q).ToPolygon();

        /* TODO: would be nice
        public static IPolyLine2D StarFigure(int p, int q)
        {
            if (p <= 1) throw new Exception()
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
        */

        // https://en.wikipedia.org/wiki/Pentagram
        public static readonly Polygon Pentagram 
            = RegularStarPolygon(5, 2);

        // https://en.wikipedia.org/wiki/Heptagram
        public static readonly Polygon Heptagram2
            = RegularStarPolygon(7, 2);

        // https://en.wikipedia.org/wiki/Heptagram
        public static readonly Polygon Heptagram3
            = RegularStarPolygon(7, 3);

        // https://en.wikipedia.org/wiki/Octagram
        public static readonly Polygon Octagram
            = RegularStarPolygon(8, 3);

        // https://en.wikipedia.org/wiki/Enneagram_(geometry)
        public static readonly Polygon Nonagram2
            = RegularStarPolygon(9, 2);

        // https://en.wikipedia.org/wiki/Enneagram_(geometry)
        public static readonly Polygon Nonagram4
            = RegularStarPolygon(9, 4);

        // https://en.wikipedia.org/wiki/Decagram_(geometry)
        public static readonly Polygon Decagram
            = RegularStarPolygon(10, 3);

    }
}