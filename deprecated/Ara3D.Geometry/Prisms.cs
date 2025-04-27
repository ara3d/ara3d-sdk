namespace Ara3D.Geometry
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Prism_(geometry)
    /// </summary>
    public static class Prisms
    {
        public static GridMesh TriangularPrism = Polygons.Triangle.ToPrism();
        public static GridMesh SquarePrism = Polygons.Square.ToPrism();
        public static GridMesh PentagonalPrism = Polygons.Pentagon.ToPrism();
        public static GridMesh HexagonalPrism = Polygons.Hexagon.ToPrism();
        public static GridMesh HeptagonalPrism = Polygons.Heptagon.ToPrism();
        public static GridMesh OctagonalPrism = Polygons.Octagon.ToPrism();
        public static GridMesh NonagonalPrism = Polygons.Nonagon.ToPrism();
        public static GridMesh DecagonalPrism = Polygons.Decagon.ToPrism();
        public static GridMesh IcosagonalPrism = Polygons.Icosagon.ToPrism();
        public static GridMesh PentagramalPrism = Polygons.Pentagram.ToPrism();
        public static GridMesh OctagramalPrism = Polygons.Octagram.ToPrism();
        public static GridMesh DecagramalPrism = Polygons.Decagram.ToPrism();

        public static GridMesh ToPrism(this CommonPolygonsEnum polygonEnum, float extrusion = 1.0f)
            => polygonEnum.ToPolygon().Extrude(extrusion);

        public static GridMesh ToPrism(this IPolyLine2D polyLine, float extrusion = 1.0f)
            => polyLine.Extrude(extrusion);
    }
}