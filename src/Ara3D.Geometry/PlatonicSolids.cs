namespace Ara3D.Geometry
{
    public enum PlatonicSolidsEnum
    {
        Tetrahedron,
        Cube,
        Octahedron,
        Dodecahedron,
        Icosahedron,
    }

    /// <summary>
    /// These are the five unit-sized platonic-solids.
    /// Note that the face definitions were taken from Three.JS which uses counter clock-wise winding order.
    /// </summary>
    public static class PlatonicSolids
    {
        private static readonly float _t = (1f + MathF.Sqrt(5)) / 2f;
        private static readonly float _rt = 1 / _t;
        public static readonly float Sqrt2 = MathF.Sqrt(2);

        public static TriangleMesh3D ToTriangleMesh(this IReadOnlyList<Vector3> self, params (int, int, int)[] faces)
            => new(self.Map(v => (Point3D)v), faces.Map(xs => (Integer3)xs));

        public static QuadMesh3D ToQuadMesh(this IReadOnlyList<Vector3> self, params (int, int, int, int)[] faces)
            => new(self.Map(v => (Point3D)v), faces.Map(xs => (Integer4)xs));

        public static TriangleMesh3D FlipFaces(this TriangleMesh3D mesh)
            => new(mesh.Points, mesh.FaceIndices.Map(f => new Integer3(f.C, f.B, f.A)));

        public static Integer3 QuadFaceToTriFace(this Integer4 self, bool firstOrSecond)
            => firstOrSecond ? (self.A, self.B, self.C) : (self.C, self.D, self.A);

        public static IReadOnlyList<Integer3> QuadFacesToTriFaces(this IReadOnlyList<Integer4> self)
            => (self.Count * 2).MapRange(i => QuadFaceToTriFace(self[i / 2], i % 2 == 0));

        public static TriangleMesh3D Triangulate(this QuadMesh3D mesh)
            => new(mesh.Points, mesh.FaceIndices.QuadFacesToTriFaces());

        public static TriangleMesh3D Triangulate(this GridMesh mesh)
            => new(mesh.Points, mesh.FaceIndices.QuadFacesToTriFaces());

        // https://mathworld.wolfram.com/RegularTetrahedron.html
        // https://github.com/mrdoob/three.js/blob/master/src/geometries/TetrahedronGeometry.js
        public static readonly TriangleMesh3D Tetrahedron
            = new Vector3[]
                {
                    (1f, 0f, -1f / Sqrt2),
                    (-1f, 0f, -1f / Sqrt2),
                    (0f, 1f, 1f / Sqrt2),
                    (0f, -1f, 1f / Sqrt2)
                }
                .Normalize()
                .ToTriangleMesh(
                    (0, 1, 2), (1, 0, 3),
                    (0, 2, 3), (1, 3, 2));

        // https://mathworld.wolfram.com/Cube.html
        public static readonly QuadMesh3D Cube
            = new Vector3[] {
                (-0.5f, -0.5f, -0.5f),
                (-0.5f, 0.5f, -0.5f),
                (0.5f, 0.5f, -0.5f),
                (0.5f, -0.5f, -0.5f),
                (-0.5f, -0.5f, 0.5f),
                (-0.5f, 0.5f, 0.5f),
                (0.5f, 0.5f, 0.5f),
                (0.5f, -0.5f, 0.5f)
                }
                .Normalize()
                .ToQuadMesh(
                    (0, 1, 2, 3),
                    (1, 5, 6, 2),
                    (7, 6, 5, 4),
                    (4, 0, 3, 7),
                    (4, 5, 1, 0),
                    (3, 2, 6, 7));

        // https://mathworld.wolfram.com/RegularOctahedron.html
        // https://github.com/mrdoob/three.js/blob/master/src/geometries/OctahedronGeometry.js
        public static readonly TriangleMesh3D Octahedron
            = new Vector3[] { 
                    (1, 0, 0), (-1, 0, 0), (0, 1, 0),
                    (0, -1, 0), (0, 0, 1), (0, 0, -1) }
                .Normalize()
                .ToTriangleMesh(
                    (0, 2, 4), (0, 4, 3), (0, 3, 5),
                    (0, 5, 2), (1, 2, 5), (1, 5, 3),
                    (1, 3, 4), (1, 4, 2));

        // https://mathworld.wolfram.com/RegularDodecahedron.html
        // https://github.com/mrdoob/three.js/blob/master/src/geometries/DodecahedronGeometry.js
        public static readonly TriangleMesh3D Dodecahedron
            = new Vector3[] {
                    // (±1, ±1, ±1)
                    (-1, -1, -1), (-1, -1, 1),
                    (-1, 1, -1), (-1, 1, 1),
                    (1, -1, -1), (1, -1, 1),
                    (1, 1, -1), (1, 1, 1),

                    // (0, ±1/φ, ±φ)
                    (0, -_rt, -_t), (0, -_rt, _t),
                    (0, _rt, -_t), (0, _rt, _t),

                    // (±1/φ, ±φ, 0)
                    (-_rt, -_t, 0), (-_rt, _t, 0),
                    (_rt, -_t, 0), (_rt, _t, 0),

                    // (±φ, 0, ±1/φ)
                    (-_t, 0, -_rt), (_t, 0, -_rt),
                    (-_t, 0, _rt), (_t, 0, _rt) }
            .Normalize()
            .ToTriangleMesh(
                (3, 11, 7), (3, 7, 15), (3, 15, 13),
                (7, 19, 17), (7, 17, 6), (7, 6, 15),
                (17, 4, 8), (17, 8, 10), (17, 10, 6),
                (8, 0, 16), (8, 16, 2), (8, 2, 10),
                (0, 12, 1), (0, 1, 18), (0, 18, 16),
                (6, 10, 2), (6, 2, 13), (6, 13, 15),
                (2, 16, 18), (2, 18, 3), (2, 3, 13),
                (18, 1, 9), (18, 9, 11), (18, 11, 3),
                (4, 14, 12), (4, 12, 0), (4, 0, 8),
                (11, 9, 5), (11, 5, 19), (11, 19, 7),
                (19, 5, 14), (19, 14, 4), (19, 4, 17),
                (1, 12, 14), (1, 14, 5), (1, 5, 9));

        // https://mathworld.wolfram.com/RegularIcosahedron.html
        // https://github.com/mrdoob/three.js/blob/master/src/geometries/IcosahedronGeometry.js
        public static readonly TriangleMesh3D Icosahedron
            = new Vector3[] { 
                (-1f, _t, 0.0f),
                (1f, _t, 0.0f),
                (-1f, -_t, 0.0f),
                (1f, -_t, 0.0f),
                (0.0f, -1f, _t),
                (0.0f, 1f, _t),
                (0.0f, -1f, -_t),
                (0.0f, 1f, -_t),
                (_t, 0.0f, -1f),
                (_t, 0.0f, 1f),
                (-_t, 0.0f, -1f),
                (-_t, 0.0f, 1f) }
            .Normalize()
            .ToTriangleMesh(
                (0, 11, 5), (0, 5, 1), (0, 1, 7), (0, 7, 10), (0, 10, 11),
                (1, 5, 9), (5, 11, 4), (11, 10, 2), (10, 7, 6), (7, 1, 8),
                (3, 9, 4), (3, 4, 2), (3, 2, 6), (3, 6, 8), (3, 8, 9),
                (4, 9, 5), (2, 4, 11), (6, 2, 10), (8, 6, 7), (9, 8, 1));

        public static readonly TriangleMesh3D TriangulatedCube
            = Cube.Triangulate();

        public static TriangleMesh3D GetMesh(int n)
            => GetMesh((PlatonicSolidsEnum)n);

        public static TriangleMesh3D GetMesh(PlatonicSolidsEnum n)
        {
            switch (n)
            {
                case PlatonicSolidsEnum.Tetrahedron: return Tetrahedron;
                case PlatonicSolidsEnum.Cube: return TriangulatedCube;
                case PlatonicSolidsEnum.Octahedron: return Octahedron;
                case PlatonicSolidsEnum.Dodecahedron: return Dodecahedron;
                case PlatonicSolidsEnum.Icosahedron: return Icosahedron;
            }

            return TriangulatedCube;
        }

    }
}
