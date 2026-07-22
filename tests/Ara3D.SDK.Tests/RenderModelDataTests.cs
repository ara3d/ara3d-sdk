using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.SDK.Tests
{
    /// <summary>
    /// Regression coverage for RenderModelData.Update(IEnumerable) (ara3d-128): merged
    /// slices must be rebased by the accumulated vertex/index counts, or every slice
    /// after the first points into the first model's data (shredded geometry).
    /// </summary>
    public static class RenderModelDataTests
    {
        private static RenderModelData MakeTriangles(int numTriangles, float offset)
        {
            var points = new List<Point3D>();
            var indices = new List<uint>();
            for (var i = 0; i < numTriangles; i++)
            {
                var x = offset + i * 10f;
                points.Add(new Point3D(x, 0, 0));
                points.Add(new Point3D(x + 1, 0, 0));
                points.Add(new Point3D(x, 1, 0));
                indices.Add((uint)(i * 3));
                indices.Add((uint)(i * 3 + 1));
                indices.Add((uint)(i * 3 + 2));
            }

            var r = new RenderModelData(3);
            r.Update(points, indices, 3, Material.Default, Matrix4x4.Identity);
            return r;
        }

        [Test]
        public static void MergeRebasesSlicesAndRemapsInstances()
        {
            using var a = MakeTriangles(1, 0f);
            using var b = MakeTriangles(2, 100f);
            using var merged = new RenderModelData(3);
            merged.Update(new[] { a, b });

            Assert.That(merged.VertexData.Count, Is.EqualTo(a.VertexData.Count + b.VertexData.Count));
            Assert.That(merged.IndexData.Count, Is.EqualTo(a.IndexData.Count + b.IndexData.Count));
            Assert.That(merged.MeshSliceData.Count, Is.EqualTo(2));
            Assert.That(merged.InstanceData.Count, Is.EqualTo(2));

            var sliceA = merged.MeshSliceData[0];
            Assert.That(sliceA.BaseVertex, Is.EqualTo(0));
            Assert.That(sliceA.FirstIndex, Is.EqualTo(0u));
            Assert.That(sliceA.VertexCount, Is.EqualTo(3));
            Assert.That(sliceA.IndexCount, Is.EqualTo(3u));

            var sliceB = merged.MeshSliceData[1];
            Assert.That(sliceB.BaseVertex, Is.EqualTo(3));
            Assert.That(sliceB.FirstIndex, Is.EqualTo(3u));
            Assert.That(sliceB.VertexCount, Is.EqualTo(6));
            Assert.That(sliceB.IndexCount, Is.EqualTo(6u));

            Assert.That(merged.InstanceData[0].MeshIndex, Is.EqualTo(0));
            Assert.That(merged.InstanceData[1].MeshIndex, Is.EqualTo(1));

            // The slice must resolve to the second model's points, not the first's.
            var pointsB = merged.GetPoints(sliceB);
            Assert.That((float)pointsB[0].X, Is.EqualTo(100f));

            // Indices stay slice-local: every index addresses a vertex inside its slice.
            for (var i = 0; i < merged.MeshSliceData.Count; i++)
            {
                var slice = merged.MeshSliceData[i];
                for (var j = 0u; j < slice.IndexCount; j++)
                    Assert.That(merged.IndexData[(int)(slice.FirstIndex + j)], Is.LessThan((uint)slice.VertexCount));
            }
        }

        [Test]
        public static void MergeSkipsModelsWithMismatchedLayout()
        {
            using var triangles = MakeTriangles(1, 0f);

            using var lines = new RenderModelData(2);
            lines.Update(
                new List<Point3D> { new(0, 0, 0), new(1, 0, 0) },
                new List<uint> { 0, 1 },
                2, Material.Default, Matrix4x4.Identity);

            using var colored = new RenderModelData(3);
            colored.Update(
                new List<Point3D> { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0) },
                new List<uint> { 0, 1, 2 },
                new List<Vector3> { new(1, 0, 0), new(0, 1, 0), new(0, 0, 1) },
                3, Material.Default, Matrix4x4.Identity);

            using var merged = new RenderModelData(3);
            merged.Update(new[] { lines, colored, triangles });

            Assert.That(merged.MeshSliceData.Count, Is.EqualTo(1));
            Assert.That(merged.VertexData.Count, Is.EqualTo(triangles.VertexData.Count));
            Assert.That(merged.MeshSliceData[0].BaseVertex, Is.EqualTo(0));
        }
    }
}
