using Ara3D.Geometry;

[TestFixture]
public class PolyhedraTests
{
    [Test]
    public void Platonic_FaceAndVertexCounts()
    {
        AssertCounts(Polyhedra.Tetrahedron, vertices: 4, faces: 4);
        AssertCounts(Polyhedra.Cube, vertices: 8, faces: 6);
        AssertCounts(Polyhedra.Octahedron, vertices: 6, faces: 8);
        AssertCounts(Polyhedra.Dodecahedron, vertices: 20, faces: 12);
        AssertCounts(Polyhedra.Icosahedron, vertices: 12, faces: 20);
    }

    [Test]
    public void Platonic_DualPairs_SwapVertexFaceCounts()
    {
        AssertDualPair(Polyhedra.Cube, Polyhedra.Octahedron);
        AssertDualPair(Polyhedra.Dodecahedron, Polyhedra.Icosahedron);
        AssertCounts(Polyhedra.Tetrahedron.Dual(), vertices: 4, faces: 4);
    }

    [Test]
    public void Ambo_Cube_IsCuboctahedron()
    {
        // Cuboctahedron: 12 verts (cube edges), 8 triangles + 6 squares.
        var mesh = Polyhedra.Cuboctahedron;
        AssertCounts(mesh, vertices: 12, faces: 14);
        Assert.That(CountFacesOfArity(mesh, 3), Is.EqualTo(8));
        Assert.That(CountFacesOfArity(mesh, 4), Is.EqualTo(6));
    }

    [Test]
    public void Truncate_Cube_IsTruncatedCube()
    {
        // Truncated cube: 24 verts, 8 triangles + 6 octagons.
        var mesh = Polyhedra.TruncatedCube;
        AssertCounts(mesh, vertices: 24, faces: 14);
        Assert.That(CountFacesOfArity(mesh, 3), Is.EqualTo(8));
        Assert.That(CountFacesOfArity(mesh, 8), Is.EqualTo(6));
    }

    [Test]
    public void Dual_Cuboctahedron_IsRhombicDodecahedron()
    {
        // Rhombic dodecahedron: 14 verts, 12 rhombi.
        var mesh = Polyhedra.RhombicDodecahedron;
        AssertCounts(mesh, vertices: 14, faces: 12);
        Assert.That(CountFacesOfArity(mesh, 4), Is.EqualTo(12));
    }

    [Test]
    public void ToTriangleMesh_PreservesClosedEulerCharacteristic()
    {
        foreach (var mesh in new[]
                 {
                     Polyhedra.Tetrahedron,
                     Polyhedra.Cube,
                     Polyhedra.Octahedron,
                     Polyhedra.Dodecahedron,
                     Polyhedra.Icosahedron,
                     Polyhedra.Cuboctahedron,
                     Polyhedra.TruncatedCube,
                 })
        {
            var tris = mesh.ToTriangleMesh();
            Assert.That(tris.FaceIndices.Count, Is.GreaterThan(0));
            Assert.That(Euler(mesh), Is.EqualTo(2));
        }
    }

    static void AssertDualPair(PolygonMesh3D a, PolygonMesh3D b)
    {
        var dualA = a.Dual();
        Assert.That(dualA.Points.Count, Is.EqualTo(b.Points.Count));
        Assert.That(dualA.FaceCount, Is.EqualTo(b.FaceCount));
    }

    static void AssertCounts(PolygonMesh3D mesh, int vertices, int faces)
    {
        Assert.That(mesh.Points.Count, Is.EqualTo(vertices));
        Assert.That(mesh.FaceCount, Is.EqualTo(faces));
        Assert.That(mesh.FaceOffsets.Count, Is.EqualTo(faces + 1));
        Assert.That(mesh.FaceOffsets[faces], Is.EqualTo(mesh.FaceVertices.Count));
    }

    static int CountFacesOfArity(PolygonMesh3D mesh, int arity)
    {
        var n = 0;
        for (var f = 0; f < mesh.FaceCount; ++f)
            if (mesh.FaceArity(f) == arity)
                ++n;
        return n;
    }

    static int Euler(PolygonMesh3D mesh)
    {
        var edgeKeys = new HashSet<(int, int)>();
        for (var f = 0; f < mesh.FaceCount; ++f)
        {
            var vs = mesh.FaceVerticesOf(f);
            for (var i = 0; i < vs.Count; ++i)
            {
                var a = vs[i];
                var b = vs[(i + 1) % vs.Count];
                edgeKeys.Add(a < b ? (a, b) : (b, a));
            }
        }
        return mesh.Points.Count - edgeKeys.Count + mesh.FaceCount;
    }
}
