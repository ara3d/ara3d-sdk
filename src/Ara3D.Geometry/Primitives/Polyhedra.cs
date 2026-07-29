namespace Ara3D.Geometry;

/// <summary>
/// Hybrid polyhedra catalog: Platonic seeds as <see cref="PolygonMesh3D"/>,
/// Conway operators for Archimedean/Catalan aliases, triangle/polygon export.
/// </summary>
public static class Polyhedra
{
    static readonly float Phi = (1f + MathF.Sqrt(5)) / 2f;
    static readonly float InvPhi = 1f / Phi;
    static readonly float Sqrt2 = MathF.Sqrt(2);

    public static PolygonMesh3D Tetrahedron { get; } =
        Points(
                (1f, 0f, -1f / Sqrt2),
                (-1f, 0f, -1f / Sqrt2),
                (0f, 1f, 1f / Sqrt2),
                (0f, -1f, 1f / Sqrt2))
            .Normalize()
            .ToPoints()
            .ToPolygonMesh(Faces(
                [0, 1, 2],
                [1, 0, 3],
                [0, 2, 3],
                [1, 3, 2]));

    public static PolygonMesh3D Cube { get; } =
        Points(
                (-0.5f, -0.5f, -0.5f),
                (-0.5f, 0.5f, -0.5f),
                (0.5f, 0.5f, -0.5f),
                (0.5f, -0.5f, -0.5f),
                (-0.5f, -0.5f, 0.5f),
                (-0.5f, 0.5f, 0.5f),
                (0.5f, 0.5f, 0.5f),
                (0.5f, -0.5f, 0.5f))
            .ToPoints()
            .ToPolygonMesh(Faces(
                [0, 1, 2, 3],
                [1, 5, 6, 2],
                [7, 6, 5, 4],
                [4, 0, 3, 7],
                [4, 5, 1, 0],
                [3, 2, 6, 7]))
            .WithNormalizedPoints();

    public static PolygonMesh3D Octahedron { get; } =
        Points(
                (1, 0, 0), (-1, 0, 0), (0, 1, 0),
                (0, -1, 0), (0, 0, 1), (0, 0, -1))
            .Normalize()
            .ToPoints()
            .ToPolygonMesh(Faces(
                [0, 2, 4], [0, 4, 3], [0, 3, 5], [0, 5, 2],
                [1, 2, 5], [1, 5, 3], [1, 3, 4], [1, 4, 2]));

    public static PolygonMesh3D Dodecahedron { get; } =
        Points(
                (-1, -1, -1), (-1, -1, 1), (-1, 1, -1), (-1, 1, 1),
                (1, -1, -1), (1, -1, 1), (1, 1, -1), (1, 1, 1),
                (0, -InvPhi, -Phi), (0, -InvPhi, Phi),
                (0, InvPhi, -Phi), (0, InvPhi, Phi),
                (-InvPhi, -Phi, 0), (-InvPhi, Phi, 0),
                (InvPhi, -Phi, 0), (InvPhi, Phi, 0),
                (-Phi, 0, -InvPhi), (Phi, 0, -InvPhi),
                (-Phi, 0, InvPhi), (Phi, 0, InvPhi))
            .Normalize()
            .ToPoints()
            .ToPolygonMesh(Faces(
                [11, 7, 15, 13, 3],
                [19, 17, 6, 15, 7],
                [4, 8, 10, 6, 17],
                [0, 16, 2, 10, 8],
                [12, 1, 18, 16, 0],
                [10, 2, 13, 15, 6],
                [16, 18, 3, 13, 2],
                [1, 9, 11, 3, 18],
                [14, 12, 0, 8, 4],
                [9, 5, 19, 7, 11],
                [5, 14, 4, 17, 19],
                [12, 14, 5, 9, 1]));

    public static PolygonMesh3D Icosahedron { get; } =
        Points(
                (-1f, Phi, 0f), (1f, Phi, 0f), (-1f, -Phi, 0f), (1f, -Phi, 0f),
                (0f, -1f, Phi), (0f, 1f, Phi), (0f, -1f, -Phi), (0f, 1f, -Phi),
                (Phi, 0f, -1f), (Phi, 0f, 1f), (-Phi, 0f, -1f), (-Phi, 0f, 1f))
            .Normalize()
            .ToPoints()
            .ToPolygonMesh(Faces(
                [0, 11, 5], [0, 5, 1], [0, 1, 7], [0, 7, 10], [0, 10, 11],
                [1, 5, 9], [5, 11, 4], [11, 10, 2], [10, 7, 6], [7, 1, 8],
                [3, 9, 4], [3, 4, 2], [3, 2, 6], [3, 6, 8], [3, 8, 9],
                [4, 9, 5], [2, 4, 11], [6, 2, 10], [8, 6, 7], [9, 8, 1]));

    // Archimedean aliases (Conway programs on Platonic seeds)
    public static PolygonMesh3D Cuboctahedron
        => Cube.Ambo();

    public static PolygonMesh3D Icosidodecahedron
        => Dodecahedron.Ambo();

    public static PolygonMesh3D TruncatedTetrahedron
        => Tetrahedron.Truncate();

    public static PolygonMesh3D TruncatedCube
        => Cube.Truncate();

    public static PolygonMesh3D TruncatedOctahedron
        => Octahedron.Truncate();

    public static PolygonMesh3D TruncatedDodecahedron
        => Dodecahedron.Truncate();

    public static PolygonMesh3D TruncatedIcosahedron
        => Icosahedron.Truncate();

    // Catalan aliases (duals of Archimedean)
    public static PolygonMesh3D RhombicDodecahedron
        => Cuboctahedron.Dual();

    public static PolygonMesh3D RhombicTriacontahedron
        => Icosidodecahedron.Dual();

    public static PolygonMesh3D TriakisTetrahedron
        => TruncatedTetrahedron.Dual();

    public static PolygonMesh3D GetPlatonic(PlatonicSolidsEnum kind)
        => kind switch
        {
            PlatonicSolidsEnum.Tetrahedron => Tetrahedron,
            PlatonicSolidsEnum.Cube => Cube,
            PlatonicSolidsEnum.Octahedron => Octahedron,
            PlatonicSolidsEnum.Dodecahedron => Dodecahedron,
            PlatonicSolidsEnum.Icosahedron => Icosahedron,
            _ => Cube,
        };

    static IReadOnlyList<Vector3> Points(params Vector3[] xs)
        => xs;

    static IReadOnlyList<Point3D> ToPoints(this IReadOnlyList<Vector3> xs)
        => xs.Map(v => (Point3D)v);

    static IReadOnlyList<IReadOnlyList<int>> Faces(params int[][] faces)
        => faces;
}
