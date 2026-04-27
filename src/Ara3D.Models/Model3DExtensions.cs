using Ara3D.Collections;
using Ara3D.Geometry;
using Ara3D.Memory;
using System.Diagnostics;
using Ara3D.Utils;

namespace Ara3D.Models;

public static class Model3DExtensions
{
    public static TriangleMesh3D EmptyMesh = new([], []);

    public static Integer3 Offset(Integer3 self, Integer offset)
        => (self.A + offset, self.B + offset, self.C + offset);

    public static TriangleMesh3D GetMesh(this IModel3D self, InstanceStruct node)
        => node.MeshIndex < 0 ? EmptyMesh : self.Meshes[node.MeshIndex];

    public static TriangleMesh3D ToMesh(this IModel3D self)
    {
        var points = new List<Point3D>();
        var indices = new List<Integer3>();
        var indexOffset = 0;

        foreach (var node in self.Instances)
        {
            var mesh = self.GetMesh(node);
            var mat = node.Matrix4x4;

            if (!mat.Equals(Matrix4x4.Identity))
            {
                foreach (var p in mesh.Points)
                    points.Add(p.Transform(mat));
            }
            else
            {
                // Fast path
                points.AddRange(mesh.Points);
            }

            if (indexOffset != 0)
            {
                foreach (var f in mesh.FaceIndices)
                    indices.Add(Offset(f, indexOffset));
            }
            else
            {
                // Fast path
                indices.AddRange(mesh.FaceIndices);
            }

            indexOffset = points.Count;
        }

        // TODO: we need  to be able to work more efficiently with buffers 
        return new TriangleMesh3D(points, indices);
    }

    public static IReadOnlyList<Point3D> TransformedPoints(this IModel3D self)
    {
        var points = new UnmanagedList<Point3D>();

        foreach (var node in self.Instances)
        {
            var mesh = self.GetMesh(node);
            var mat = node.Matrix4x4;

            if (!mat.Equals(Matrix4x4.Identity))
            {
                foreach (var p in mesh.Points)
                    points.Add(mat.Transform(p));
            }
            else
            {
                points.AddRange(mesh.Points);
            }
        }

        return points;
    }

    public static IReadOnlyList<Bounds3D> GetMeshBounds(this IModel3D self)
        => self.Meshes.Map(m => m.Bounds).ToArray();

    public static Bounds3D GetBounds(this IModel3D self)
    {
        var r = Bounds3D.Empty;
        var meshBounds = self.Meshes.Map(m => m.Bounds).ToArray();
        foreach (var node in self.Instances)
        {
            var meshIndex = node.MeshIndex;
            if (meshIndex < 0) continue;
            if (self.Meshes[meshIndex].FaceIndices.Count == 0) continue;
            var rawBounds = meshBounds[node.MeshIndex];
            var mat = node.Matrix4x4;
            var lclBounds = rawBounds.Transform(mat);
            r = r.Include(lclBounds);
        }
        return r;
    }

    public static IModel3D WithMeshes(this IModel3D self, Func<TriangleMesh3D, TriangleMesh3D> f)
        => self.WithMeshes(self.Meshes.Select(f));

    public static IModel3D WithInstances(this IModel3D self, Func<InstanceStruct, InstanceStruct> f)
        => self.WithInstances(self.Instances.Select(f));

    public static FilteredModel3D Where(this IModel3D self, Func<InstanceStruct, bool> f)
        => new(self, f);

    public static IModel3D Where(this IModel3D self, Func<TriangleMesh3D, bool> f)
        => self.WithInstances(self.Instances.Where(i => f(self.GetMesh(i))).ToList());

    public static IModel3D Where(this IModel3D self, Func<InstanceStruct, int, bool> f)
        => self.WithInstances(self.Instances.Where(f).ToList());

    public static IModel3D Clone(this IModel3D model, IReadOnlyList<Vector3> positions)
        => model.Clone(positions.Map(Matrix4x4.CreateTranslation));

    public static IModel3D Clone(this IModel3D model, IReadOnlyList<Matrix4x4> matrices)
        => model.WithInstances(
            model.Instances.SelectMany(
                node => matrices.Select(m => node.Transform(m))).ToList());
    
    public static IModel3D Clone(this TriangleMesh3D mesh, Material material, IReadOnlyList<Point3D> points)
        => mesh.Clone(material, points.Map(p => Matrix4x4.CreateTranslation(p.Vector3)));

    public static IModel3D Clone(this TriangleMesh3D mesh, Material material, IReadOnlyList<Matrix4x4> transforms)
        => Model3D.Create(mesh, material, transforms);

    public static IModel3D CloneAlong(this TriangleMesh3D mesh, Func<Number, Point3D> curveFunc, Integer count)
    {
        var transforms = count.LinearSpaceExclusive.Map(curveFunc).Map(p => Matrix4x4.CreateTranslation(p));
        return Clone(mesh, Material.Default, transforms);
    }

    public static Material FirstOrDefaultMaterial(this IModel3D self)
        => self.Instances.Count > 0 ? self.Instances[0].Material : Material.Default;

    public static Model3D WithMeshes(this IModel3D self, IReadOnlyList<TriangleMesh3D> meshes)
        => new(meshes, self.Instances);

    public static Model3D WithInstances(this IModel3D self, IReadOnlyList<InstanceStruct> instances)
        => new(self.Meshes, instances);

    public static Model3D Transform(this IModel3D self, Transform3D transform)
        => self.WithInstances(self.Instances.Select(i => i.Transform(transform)));

    public static Model3D FilterAndRemoveUnusedMeshes(this IModel3D self, Func<InstanceStruct, bool> f)
        => new Model3D(self.Meshes, self.Instances.Where(f).ToList()).RemoveUnusedMeshes();

    public static Model3D RemoveUnusedMeshes(this IModel3D self)
    {
        var newMeshIndices = new IndexedSet<int>();
        var newInstances = new List<InstanceStruct>();
        var newMeshes = new List<TriangleMesh3D>();
        foreach (var inst in self.Instances)
        {
            if (inst.MeshIndex < 0)
            {
                newInstances.Add(inst);
                continue;
            }

            if (!newMeshIndices.Contains(inst.MeshIndex))
            {
                var mesh = self.Meshes[inst.MeshIndex];
                var newMeshIndex = newMeshIndices.Add(inst.MeshIndex);
                newMeshes.Add(mesh);
                Debug.Assert(newMeshIndex == newMeshIndices.Count - 1);
                newInstances.Add(inst.WithMeshIndex(newMeshIndex));
            }
            else
            {
                var newMeshIndex = newMeshIndices[inst.MeshIndex];
                newInstances.Add(inst.WithMeshIndex(newMeshIndex));
            }
        }

        return new(newMeshes, newInstances);
    }

    public static IModel3D ToModel3D(this TriangleMesh3D self)
        => Model3D.Create(self);

    public static IModel3D ToModel3D(this TriangleMesh3D self, Material mat)
        => Model3D.Create(self, mat);

    public static Model3D Merge(this IEnumerable<IModel3D> models)
    {
        var meshes = new List<TriangleMesh3D>();
        var instances = new List<InstanceStruct>();
        foreach (var model in models)
        {
            var meshOffset = meshes.Count;
            meshes.AddRange(model.Meshes);
            foreach (var inst in model.Instances)
                instances.Add(inst.WithMeshIndex(inst.MeshIndex + meshOffset));
        }

        return new(meshes, instances);
    }

    public static (Matrix4x4 Transform, int EntityIndex, Material Material, byte Flags)
    GetInstanceGroupKey(InstanceStruct i)
    {
        return (i.Matrix4x4, i.EntityIndex, i.Material, i.Flags);
    }

    public sealed class SequenceEqualityComparer<T>
        : IEqualityComparer<IReadOnlyList<T>>
    {
        public bool Equals(IReadOnlyList<T>? x, IReadOnlyList<T>? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null || x.Count != y.Count) return false;
            return x.SequenceEqual(y);
        }

        public int GetHashCode(IReadOnlyList<T> obj)
        {
            unchecked
            {
                int hash = 17;
                foreach (var v in obj)
                    hash = hash * 31 + v!.GetHashCode();
                return hash;
            }
        }
    }

    // TODO: maybe should be in a geometry extensions class. 
    public static TriangleMesh3D Merge(this IReadOnlyList<TriangleMesh3D> meshes)
    {
        if (meshes.Count == 0) return TriangleMesh3D.Default;
        if (meshes.Count == 1) return meshes[0];

        var points = new List<Point3D>();
        var indices = new List<Integer3>();
        foreach (var m in meshes)
        {
            var offset = points.Count;
            points.AddRange(m.Points);
            foreach (var i in m.FaceIndices)
            {
                indices.Add(
                    new Integer3(
                        i.A + offset,
                        i.B + offset,
                        i.C + offset));
            }
        }

        return new(points, indices);
    }

    public static TriangleMesh3D MergeMeshes(this Model3D model, IReadOnlyList<int> indices)
    {
        var meshes = new List<TriangleMesh3D>();
        foreach (var i in indices)
        {
            if (i >= 0)
                meshes.Add(model.Meshes[i]);
        }

        return meshes.Merge();
    }

    public static Model3D MergeInstances(this Model3D model)
    {
        var groups = model.Instances.GroupBy(GetInstanceGroupKey).ToList();

        var meshes = new List<TriangleMesh3D>();
        var instances = new List<InstanceStruct>();

        var meshIndexGroups = new Dictionary<IReadOnlyList<int>, int>(
            new SequenceEqualityComparer<int>());

        var groupMeshIndices = new List<int>();
        foreach (var g in groups)
        {
            var meshIndexList = g.Select(i => i.MeshIndex).ToList();

            if (!meshIndexGroups.TryGetValue(meshIndexList, out var meshIndex))
            {
                var index = meshes.Count;
                var mergedMesh = model.MergeMeshes(meshIndexList);
                meshes.Add(mergedMesh);
                meshIndexGroups.Add(meshIndexList, index);
                groupMeshIndices.Add(index);
            }
            else
            {
                groupMeshIndices.Add(meshIndex);
            }
        }

        for (var i = 0; i < groups.Count; i++)
        {
            var g = groups[i];
            var meshIndex = groupMeshIndices[i];
            var inst = new InstanceStruct(
                g.Key.EntityIndex,
                g.Key.Transform,
                meshIndex,
                g.Key.Material,
                g.Key.Flags);
            instances.Add(inst);
        }

        return new(meshes, instances);
    }

    public static IModel3D Combine(this IModel3D model, IModel3D other)
    {
        var meshes = model.Meshes.Concat(other.Meshes);
        var n = model.Meshes.Count;
        var instances = model.Instances.Concat(other.Instances.Map(i => i.WithMeshIndex(i.MeshIndex + n)));
        return new Model3D(meshes, instances);
    }
}