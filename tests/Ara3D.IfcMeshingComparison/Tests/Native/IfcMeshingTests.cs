using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.Native;

[TestFixture]
public sealed class IfcMeshingTests
{
    static readonly Lazy<IfcMeshingSnapshot> SampleSnapshot = new(CreateSnapshot);

    static FilePath SampleFile => TestFiles.Ac20FzkHaus;

    static IfcMeshingSnapshot Sample
        => SampleSnapshot.Value;

    [Test]
    public void IfcSampleFileExists()
    {
        TestFiles.RequireExists(SampleFile);
        Assert.That(SampleFile.GetFileSize(), Is.GreaterThan(0));
    }

    [Test]
    public void NonGeometryLoadReadsSchemaAndEntitiesWithoutNativeMeshing()
    {
        using var file = new IfcFile(SampleFile, includeGeometry: false);

        Assert.Multiple(() =>
        {
            Assert.That(file.SchemaEnum.ToString(), Is.Not.Empty);
            Assert.That(file.EntityResolver.EntityLookup, Is.Not.Empty);
            Assert.That(file.GeometryDataLoaded, Is.False);
        });
    }

    [Test]
    [Category("Slow")]
    public void NativeGeometryLoadProducesModel()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Sample.GeometryDataLoaded, Is.True);
            Assert.That(Sample.ApiPointerLoaded, Is.True);
            Assert.That(Sample.ModelPointerLoaded, Is.True);
            Assert.That(Sample.NativeGeometryCount, Is.GreaterThan(0));
        });
    }

    [Test]
    [Category("Slow")]
    public void GeometryEnumerationMatchesNativeCount()
        => Assert.That(Sample.Geometries, Has.Count.EqualTo(Sample.NativeGeometryCount));

    [Test]
    [Category("Slow")]
    public void GeometryIdsRoundTripThroughModelLookup()
        => Assert.That(Sample.RoundTrippedGeometryIds, Is.EqualTo(Sample.Geometries.Count));

    [Test]
    [Category("Slow")]
    public void NativeGeometryIdsAreNonZeroAndUnique()
    {
        var ids = Sample.Geometries.Select(g => g.GeometryId).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(ids, Has.All.GreaterThan(0));
            Assert.That(ids.Distinct().ToList(), Has.Count.EqualTo(ids.Count));
        });
    }

    [Test]
    [Category("Slow")]
    public void GeometryMeshCountsMatchEnumeration()
        => Assert.That(Sample.Geometries, Has.All.Matches<IfcGeometrySnapshot>(g =>
            g.NativeMeshCount >= 0 && g.Meshes.Count == g.NativeMeshCount));

    [Test]
    [Category("Slow")]
    public void AtLeastOneGeometryHasNativeMeshes()
        => Assert.That(Sample.Geometries, Has.Some.Matches<IfcGeometrySnapshot>(g => g.NativeMeshCount > 0));

    [Test]
    [Category("Slow")]
    public void EveryNativeMeshHasRequiredBuffers()
        => Assert.That(Sample.Meshes, Has.All.Matches<IfcMeshSnapshot>(m =>
            m.HasVertexBuffer && m.HasIndexBuffer && m.HasColorBuffer && m.HasTransformBuffer));

    [Test]
    [Category("Slow")]
    public void EveryNativeMeshHasTriangleIndexCount()
        => Assert.That(Sample.Meshes, Has.All.Matches<IfcMeshSnapshot>(m =>
            m.NativeIndexCount > 0 && m.NativeIndexCount % 3 == 0));

    [Test]
    [Category("Slow")]
    public void EveryNativeMeshHasVertices()
        => Assert.That(Sample.Meshes, Has.All.Matches<IfcMeshSnapshot>(m => m.NativeVertexCount > 0));

    [Test]
    [Category("Slow")]
    public void MeshColorsAreNormalized()
        => Assert.That(Sample.Meshes, Has.All.Matches<IfcMeshSnapshot>(m =>
            IsNormalized(m.Color.R) &&
            IsNormalized(m.Color.G) &&
            IsNormalized(m.Color.B) &&
            IsNormalized(m.Color.A)));

    [Test]
    [Category("Slow")]
    public void MeshTransformsAreFiniteAffineMatrices()
        => Assert.That(Sample.Meshes, Has.All.Matches<IfcMeshSnapshot>(m =>
            m.Transform.Count == 16 &&
            Enumerable.All(m.Transform, double.IsFinite) &&
            Math.Abs(m.Transform[15] - 1.0) < 1e-9));

    [Test]
    [Category("Slow")]
    public void TriangleMeshConversionPreservesNativeCounts()
        => Assert.That(Sample.Meshes, Has.All.Matches<IfcMeshSnapshot>(m =>
            m.Points.Count == m.NativeVertexCount &&
            m.FaceIndices.Count == m.NativeIndexCount / 3));

    [Test]
    [Category("Slow")]
    public void TriangleIndicesReferenceExistingVertices()
        => Assert.That(Sample.Meshes, Has.All.Matches<IfcMeshSnapshot>(m =>
            Enumerable.All(m.FaceIndices, f => IsIndexInRange(f.A, m.Points.Count) &&
                                               IsIndexInRange(f.B, m.Points.Count) &&
                                               IsIndexInRange(f.C, m.Points.Count))));

    [Test]
    [Category("Slow")]
    public void TriangleVerticesAreFinite()
        => Assert.That(Sample.Meshes, Has.All.Matches<IfcMeshSnapshot>(m =>
            Enumerable.All(m.Points, p => float.IsFinite(p.X) && float.IsFinite(p.Y) && float.IsFinite(p.Z))));

    [Test]
    [Category("Slow")]
    public void ConvertedModelContainsMeshesAndInstances()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Sample.ConvertedModel.Meshes, Has.Count.GreaterThan(0));
            Assert.That(Sample.ConvertedModel.Instances, Has.Count.GreaterThan(0));
        });
    }

    [Test]
    [Category("Slow")]
    public void ConvertedModelContainsOneInstancePerNativeMeshInstance()
        => Assert.That(Sample.ConvertedModel.Instances, Has.Count.EqualTo(Sample.Meshes.Count));

    [Test]
    [Category("Slow")]
    public void ConvertedModelDeduplicatesMeshesByNativeMeshId()
        => Assert.That(Sample.ConvertedModel.Meshes, Has.Count.EqualTo(Sample.Meshes.Select(m => m.MeshId).Distinct().Count()));

    [Test]
    [Category("Slow")]
    public void ConvertedModelMeshesAreNonEmptyTriangleMeshes()
        => Assert.That(Sample.ConvertedModel.Meshes, Has.All.Matches<TriangleMesh3D>(m =>
            m.Points.Count > 0 && m.FaceIndices.Count > 0));

    [Test]
    [Category("Slow")]
    public void ConvertedModelMeshIndicesReferenceExistingMeshes()
        => Assert.That(Sample.ConvertedModel.Instances, Has.All.Matches<InstanceStruct>(i =>
            i.MeshIndex >= 0 && i.MeshIndex < Sample.ConvertedModel.Meshes.Count));

    [Test]
    [Category("Slow")]
    public void ConvertedModelInstanceTransformsAreFinite()
        => Assert.That(Sample.ConvertedModel.Instances, Has.All.Matches<InstanceStruct>(i => IsFinite(i.Matrix4x4)));

    [Test]
    [Category("Slow")]
    public void ConvertedModelInstanceEntityIdsComeFromGeometryIds()
    {
        var geometryIds = Sample.Geometries.Select(g => (int)g.GeometryId).ToHashSet();

        Assert.That(Sample.ConvertedModel.Instances, Has.All.Matches<InstanceStruct>(i => geometryIds.Contains(i.EntityIndex)));
    }

    [Test]
    [Category("Slow")]
    public void ConvertedModelHasFinitePositiveBounds()
    {
        var allPoints = Sample.ConvertedModel.Meshes.SelectMany(m => m.Points).ToList();

        var minX = allPoints.Min(p => p.X.Value);
        var maxX = allPoints.Max(p => p.X.Value);
        var minY = allPoints.Min(p => p.Y.Value);
        var maxY = allPoints.Max(p => p.Y.Value);
        var minZ = allPoints.Min(p => p.Z.Value);
        var maxZ = allPoints.Max(p => p.Z.Value);

        Assert.Multiple(() =>
        {
            Assert.That(new[] { minX, maxX, minY, maxY, minZ, maxZ }, Has.All.Matches<float>(float.IsFinite));
            Assert.That(maxX - minX, Is.GreaterThan(0));
            Assert.That(maxY - minY, Is.GreaterThan(0));
            Assert.That(maxZ - minZ, Is.GreaterThan(0));
        });
    }

    [Test]
    [Category("Slow")]
    public void MedianCenterIsFinite()
        => Assert.That(IsFinite(Sample.MedianCenter), Is.True);

    [Test]
    [Category("Slow")]
    public void BuildingSampleProducesSubstantialTriangleCount()
        => Assert.That(Sample.TotalTriangleCount, Is.GreaterThan(100));

    static unsafe IfcMeshingSnapshot CreateSnapshot()
    {
        TestFiles.RequireExists(SampleFile);
        using var file = new IfcFile(SampleFile, includeGeometry: true);

        var geometries = new List<IfcGeometrySnapshot>();
        var roundTrippedGeometryIds = 0;

        foreach (var geometry in file.Model.GetGeometries())
        {
            if (file.Model.GetGeometry(geometry.Id) is not null)
                roundTrippedGeometryIds++;

            var meshes = new List<IfcMeshSnapshot>();
            foreach (var mesh in geometry.GetMeshes())
            {
                var triangleMesh = mesh.ToTriangleMesh();
                meshes.Add(new IfcMeshSnapshot(
                    geometry.Id,
                    mesh.Id,
                    mesh.NumVertices,
                    mesh.NumIndices,
                    mesh.Vertices != IntPtr.Zero,
                    mesh.Indices != IntPtr.Zero,
                    mesh.Color != IntPtr.Zero,
                    mesh.Transform != IntPtr.Zero,
                    ReadColor(mesh.Color),
                    ReadDoubles(mesh.Transform, 16),
                    triangleMesh.Points,
                    triangleMesh.FaceIndices));
            }

            geometries.Add(new IfcGeometrySnapshot(geometry.Id, geometry.NumMeshes, meshes));
        }

        return new IfcMeshingSnapshot(
            true,
            file.ApiPtr != IntPtr.Zero,
            file.Model.ModelPtr != IntPtr.Zero,
            file.Model.GetNumGeometries(),
            geometries,
            roundTrippedGeometryIds,
            file.ToModel3D(),
            file.ComputeMedianCenter());
    }

    static unsafe double[] ReadDoubles(IntPtr ptr, int count)
    {
        if (ptr == IntPtr.Zero)
            return [];

        var source = (double*)ptr;
        var result = new double[count];
        for (var i = 0; i < count; i++)
            result[i] = source[i];
        return result;
    }

    static unsafe IfcColorSnapshot ReadColor(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
            return new IfcColorSnapshot(double.NaN, double.NaN, double.NaN, double.NaN);

        var color = (IfcColor*)ptr;
        return new IfcColorSnapshot(color->R, color->G, color->B, color->A);
    }

    static bool IsIndexInRange(int index, int count)
        => index >= 0 && index < count;

    static bool IsNormalized(double value)
        => double.IsFinite(value) && value >= 0.0 && value <= 1.0;

    static bool IsFinite((double X, double Y, double Z) point)
        => double.IsFinite(point.X) && double.IsFinite(point.Y) && double.IsFinite(point.Z);

    static bool IsFinite(Matrix4x4 matrix)
        => float.IsFinite(matrix.M11) &&
           float.IsFinite(matrix.M12) &&
           float.IsFinite(matrix.M13) &&
           float.IsFinite(matrix.M14) &&
           float.IsFinite(matrix.M21) &&
           float.IsFinite(matrix.M22) &&
           float.IsFinite(matrix.M23) &&
           float.IsFinite(matrix.M24) &&
           float.IsFinite(matrix.M31) &&
           float.IsFinite(matrix.M32) &&
           float.IsFinite(matrix.M33) &&
           float.IsFinite(matrix.M34) &&
           float.IsFinite(matrix.M41) &&
           float.IsFinite(matrix.M42) &&
           float.IsFinite(matrix.M43) &&
           float.IsFinite(matrix.M44);

    sealed record IfcMeshingSnapshot(
        bool GeometryDataLoaded,
        bool ApiPointerLoaded,
        bool ModelPointerLoaded,
        int NativeGeometryCount,
        IReadOnlyList<IfcGeometrySnapshot> Geometries,
        int RoundTrippedGeometryIds,
        Model3D ConvertedModel,
        (double X, double Y, double Z) MedianCenter)
    {
        public IReadOnlyList<IfcMeshSnapshot> Meshes { get; } = Geometries.SelectMany(g => g.Meshes).ToList();

        public int TotalTriangleCount { get; } = Geometries
            .SelectMany(g => g.Meshes)
            .Sum(m => m.FaceIndices.Count);
    }

    sealed record IfcGeometrySnapshot(
        uint GeometryId,
        int NativeMeshCount,
        IReadOnlyList<IfcMeshSnapshot> Meshes);

    sealed record IfcMeshSnapshot(
        uint GeometryId,
        uint MeshId,
        int NativeVertexCount,
        int NativeIndexCount,
        bool HasVertexBuffer,
        bool HasIndexBuffer,
        bool HasColorBuffer,
        bool HasTransformBuffer,
        IfcColorSnapshot Color,
        IReadOnlyList<double> Transform,
        IReadOnlyList<Point3D> Points,
        IReadOnlyList<Integer3> FaceIndices);

    sealed record IfcColorSnapshot(double R, double G, double B, double A);
}
