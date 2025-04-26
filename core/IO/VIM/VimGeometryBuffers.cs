using Ara3D.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Ara3D.Serialization.VIM
{
    public interface IBufferGroup
    {
        IEnumerable<INamedBuffer> GetBuffers();
    }

    public class Vector3BufferGroup : IBufferGroup
    {
        public INamedBuffer<float> X { get; set; }
        public INamedBuffer<float> Y { get; set; }
        public INamedBuffer<float> Z { get; set; }

        public IEnumerable<INamedBuffer> GetBuffers()
            => [X, Y, Z];

        public static Vector3BufferGroup Create(string name, IReadOnlyList<Vector3> vectors)
            => new()
            {
                X = vectors.Select(v => v.X).ToArray().ToNamedBuffer(name + "." + nameof(X)),
                Y = vectors.Select(v => v.Y).ToArray().ToNamedBuffer(name + "." + nameof(Y)),
                Z = vectors.Select(v => v.Z).ToArray().ToNamedBuffer(name + "." + nameof(Z)),
            };

        public Vector3[] ToVector3s()
        {
            var r = new Vector3[X.Count];
            for (var i = 0; i < X.Count; i++)
                r[i] = new Vector3(X[i], Y[i], Z[i]);
            return r;
        }
    }

    public class Vector4BufferGroup : IBufferGroup
    {
        public INamedBuffer<float> X { get; set; }
        public INamedBuffer<float> Y { get; set; }
        public INamedBuffer<float> Z { get; set; }
        public INamedBuffer<float> W { get; set; }

        public IEnumerable<INamedBuffer> GetBuffers()
            => [X, Y, Z, W];

        public static Vector4BufferGroup Create(string name, IReadOnlyList<Vector4> vectors)
            => new()
            {
                X = vectors.Select(v => v.X).ToArray().ToNamedBuffer(name + "." + nameof(X)),
                Y = vectors.Select(v => v.Y).ToArray().ToNamedBuffer(name + "." + nameof(Y)),
                Z = vectors.Select(v => v.Z).ToArray().ToNamedBuffer(name + "." + nameof(Z)),
                W = vectors.Select(v => v.W).ToArray().ToNamedBuffer(name + "." + nameof(W)),
            };

        public static Vector4BufferGroup Create(string name, IReadOnlyList<Quaternion> vectors)
            => new()
            {
                X = vectors.Select(v => v.X).ToArray().ToNamedBuffer(name + "." + nameof(X)),
                Y = vectors.Select(v => v.Y).ToArray().ToNamedBuffer(name + "." + nameof(Y)),
                Z = vectors.Select(v => v.Z).ToArray().ToNamedBuffer(name + "." + nameof(Z)),
                W = vectors.Select(v => v.W).ToArray().ToNamedBuffer(name + "." + nameof(W)),
            };

        public Vector4[] ToVector4s()
        {
            var r = new Vector4[X.Count];
            for (var i = 0; i < X.Count; i++)
                r[i] = new Vector4(X[i], Y[i], Z[i], W[i]);
            return r;
        }

        public Quaternion[] ToQuaternions()
        {
            var r = new Quaternion[X.Count];
            for (var i = 0; i < X.Count; i++)
                r[i] = new Quaternion(X[i], Y[i], Z[i], W[i]);
            return r;
        }
    }

    public class TransformBufferGroup : IBufferGroup
    {
        public Vector3BufferGroup Position { get; set; }
        public Vector4BufferGroup Rotation { get; set; }
        public Vector3BufferGroup Scale { get; set; }

        public IEnumerable<INamedBuffer> GetBuffers()
            => Position.GetBuffers()
                .Concat(Rotation.GetBuffers())
                .Concat(Scale.GetBuffers());

        public static TransformBufferGroup Create(string name,IReadOnlyList<Matrix4x4> transforms)
        {
            var positions = new List<Vector3>();
            var rotations = new List<Quaternion>();
            var scales = new List<Vector3>();
            foreach (var t in transforms)
            {
                Matrix4x4.Decompose(t, out var scale, out var rotation, out var translation);
                positions.Add(translation);
                rotations.Add(rotation);
                scales.Add(scale);
            }
            return new TransformBufferGroup
            {
                Position = Vector3BufferGroup.Create(name + "." + nameof(Position), positions),
                Rotation = Vector4BufferGroup.Create(name + "." + nameof(Rotation), rotations),
                Scale = Vector3BufferGroup.Create(name + "." + nameof(Scale), scales),
            };
        }

        public Matrix4x4[] ToMatrices()
        {
            var positions = Position.ToVector3s();
            var rotations = Rotation.ToQuaternions();
            var scales = Scale.ToVector3s();
            var r = new Matrix4x4[positions.Length];
            for (var i = 0; i < positions.Length; i++)
            {
                var p = positions[i];
                var q = rotations[i];
                var s = scales[i];
                r[i] = Matrix4x4.CreateScale(s) 
                       * Matrix4x4.CreateFromQuaternion(q) 
                       * Matrix4x4.CreateTranslation(p);
            }
            return r;
        }
    }

    public class VimGeometryBuffers : IBufferGroup
    {
        public int NumVertices => Vertices.Count;
        public int NumFaces => NumCorners / 3;
        public int NumCorners => Indices.Count;
        public int NumMeshes => MeshSubMeshOffset.Count;
        public int NumInstances => InstanceTransforms.Count;
        public int NumMaterials => MaterialColors.Count;
        public int NumSubMeshes => SubMeshMaterials.Count;

        public INamedBuffer<int> Indices { get; set; }
        public INamedBuffer<Matrix4x4> InstanceTransforms { get; set; }
        public INamedBuffer<int> InstanceMeshes { get; set; }
        public INamedBuffer<Vector4> MaterialColors { get; set; }
        public INamedBuffer<float> MaterialGlossiness { get; set; }
        public INamedBuffer<float> MaterialSmoothness { get; set; }
        public INamedBuffer<int> MeshSubMeshOffset { get; set; }
        public INamedBuffer<int> SubMeshIndexOffsets { get; set; }
        public INamedBuffer<int> SubMeshMaterials { get; set; }
        public INamedBuffer<Vector3> Vertices { get; set; }

        public IEnumerable<INamedBuffer> GetBuffers()
            => [Indices, 
                InstanceTransforms, 
                InstanceMeshes, 
                MaterialColors, 
                MaterialGlossiness, 
                MaterialSmoothness, 
                MeshSubMeshOffset, 
                SubMeshIndexOffsets, 
                SubMeshMaterials, 
                Vertices];

        public static VimGeometryBuffers Create(G3D.G3D g3d)
            => Create(
            [
                g3d.Indices.ToNamedBuffer(nameof(Indices)),
                g3d.InstanceTransforms.ToNamedBuffer(nameof(InstanceTransforms)),
                g3d.InstanceMeshes.ToNamedBuffer(nameof(InstanceMeshes)),
                g3d.MaterialColors.ToNamedBuffer(nameof(MaterialColors)),
                g3d.MaterialGlossiness.ToNamedBuffer(nameof(MaterialGlossiness)),
                g3d.MaterialSmoothness.ToNamedBuffer(nameof(MaterialSmoothness)),
                g3d.MeshSubmeshOffset.ToNamedBuffer(nameof(MeshSubMeshOffset)),
                g3d.SubmeshIndexOffsets.ToNamedBuffer(nameof(SubMeshIndexOffsets)),
                g3d.SubmeshMaterials.ToNamedBuffer(nameof(SubMeshMaterials)),
                g3d.Vertices.ToNamedBuffer(nameof(Vertices)),
            ]);

        public static VimGeometryBuffers Create(IEnumerable<INamedBuffer> buffers)
        {
            var t = typeof(VimGeometryBuffers);
            var r = new VimGeometryBuffers();
            foreach (var b in buffers)
            {
                var p = t.GetProperty(b.Name);
                if (p.PropertyType.Name.StartsWith("INamedBuffer"))
                    p.SetValue(r, b);
            }
            return r;
        }

        public VimGeometryBuffers2 ToVimGeometryBuffers2()
            => VimGeometryBuffers2.Create(this);
    }

    public class VimGeometryBuffers2 : IBufferGroup
    {
        public int NumVertices => Vertices.X.Count;
        public int NumFaces => NumCorners / 3;
        public int NumCorners => Indices.Count;
        public int NumMeshes => MeshSubMeshOffset.Count;
        public int NumInstances => InstanceMeshes.Count;
        public int NumMaterials => MaterialGlossiness.Count;
        public int NumSubMeshes => SubMeshMaterials.Count;

        public INamedBuffer<int> Indices { get; set; }
        public TransformBufferGroup InstanceTransforms { get; set; }
        public INamedBuffer<int> InstanceMeshes { get; set; }
        public Vector4BufferGroup MaterialColors { get; set; }
        public INamedBuffer<float> MaterialGlossiness { get; set; }
        public INamedBuffer<float> MaterialSmoothness { get; set; }
        public INamedBuffer<int> MeshSubMeshOffset { get; set; }
        public INamedBuffer<int> SubMeshIndexOffsets { get; set; }
        public INamedBuffer<int> SubMeshMaterials { get; set; }
        public Vector3BufferGroup Vertices { get; set; }

        public IEnumerable<INamedBuffer> GetBuffers()
            => InstanceTransforms.GetBuffers().Concat(Vertices.GetBuffers()).Concat(MaterialColors.GetBuffers()).Concat([Indices,
                InstanceMeshes,
                MaterialGlossiness,
                MaterialSmoothness,
                MeshSubMeshOffset,
                SubMeshIndexOffsets,
                SubMeshMaterials]);

        public static VimGeometryBuffers2 Create(VimGeometryBuffers g)
            => new()
            {
                Indices = g.Indices,
                InstanceMeshes = g.InstanceMeshes,
                MaterialGlossiness = g.MaterialGlossiness,
                MaterialSmoothness = g.MaterialSmoothness,
                MeshSubMeshOffset = g.MeshSubMeshOffset,
                SubMeshIndexOffsets = g.SubMeshIndexOffsets,
                SubMeshMaterials = g.SubMeshMaterials,
                Vertices = Vector3BufferGroup.Create(nameof(Vertices), g.Vertices),
                MaterialColors = Vector4BufferGroup.Create(nameof(MaterialColors), g.MaterialColors),
                InstanceTransforms = TransformBufferGroup.Create(nameof(InstanceTransforms), g.InstanceTransforms),
            };

        public static VimGeometryBuffers2 Create(IEnumerable<INamedBuffer> buffers)
        {
            var r = new VimGeometryBuffers2();
            var d = buffers.ToDictionary(b => b.Name, b => b);
            r.Indices = (INamedBuffer<int>)d[nameof(Indices)];
            r.InstanceMeshes = (INamedBuffer<int>)d[nameof(InstanceMeshes)];
            r.MaterialGlossiness = (INamedBuffer<float>)d[nameof(MaterialGlossiness)];
            r.MaterialSmoothness = (INamedBuffer<float>)d[nameof(MaterialSmoothness)];
            r.MeshSubMeshOffset = (INamedBuffer<int>)d[nameof(MeshSubMeshOffset)];
            r.SubMeshIndexOffsets = (INamedBuffer<int>)d[nameof(SubMeshIndexOffsets)];
            r.SubMeshMaterials = (INamedBuffer<int>)d[nameof(SubMeshMaterials)];
            r.Vertices = new ()
            {
                X = (INamedBuffer<float>)d["Vertices.X"], 
                Y = (INamedBuffer<float>)d["Vertices.Y"],
                Z = (INamedBuffer<float>)d["Vertices.Z"],
            };
            r.MaterialColors = new()
            {
                X = (INamedBuffer<float>)d["MaterialColors.X"],
                Y = (INamedBuffer<float>)d["MaterialColors.Y"],
                Z = (INamedBuffer<float>)d["MaterialColors.Z"],
                W = (INamedBuffer<float>)d["MaterialColors.W"],
            };
            r.InstanceTransforms = new()
            {
                Position = new()
                {
                    X = (INamedBuffer<float>)d["InstanceTransforms.Position.X"],
                    Y = (INamedBuffer<float>)d["InstanceTransforms.Position.Y"],
                    Z = (INamedBuffer<float>)d["InstanceTransforms.Position.Z"],
                },
                Rotation = new()
                {
                    X = (INamedBuffer<float>)d["InstanceTransforms.Rotation.X"],
                    Y = (INamedBuffer<float>)d["InstanceTransforms.Rotation.Y"],
                    Z = (INamedBuffer<float>)d["InstanceTransforms.Rotation.Z"],
                    W = (INamedBuffer<float>)d["InstanceTransforms.Rotation.W"],
                },
                Scale = new()
                {
                    X = (INamedBuffer<float>)d["InstanceTransforms.Scale.X"],
                    Y = (INamedBuffer<float>)d["InstanceTransforms.Scale.Y"],
                    Z = (INamedBuffer<float>)d["InstanceTransforms.Scale.Z"],
                }
            };
            return r;
        }

        public VimGeometryBuffers ToVimGeometryBuffers()
            => new ()
            {
                Indices = Indices,
                InstanceMeshes = InstanceMeshes,
                MaterialGlossiness = MaterialGlossiness,
                MaterialSmoothness = MaterialSmoothness,
                MeshSubMeshOffset = MeshSubMeshOffset,
                SubMeshIndexOffsets = SubMeshIndexOffsets,
                SubMeshMaterials = SubMeshMaterials,
                Vertices = Vertices.ToVector3s().ToNamedBuffer(nameof(Vertices)),
                MaterialColors = MaterialColors.ToVector4s().ToNamedBuffer(nameof(MaterialColors)),
                InstanceTransforms = InstanceTransforms.ToMatrices().ToNamedBuffer(nameof(InstanceTransforms))
            };
    }
}
