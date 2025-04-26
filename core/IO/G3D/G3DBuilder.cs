using System.Collections.Generic;
using System.Numerics;
using Ara3D.Collections;

namespace Ara3D.Serialization.G3D
{
    /// <summary>
    /// This is a helper class for constructing a G3D from individual attributes
    /// </summary>
    public class G3DBuilder
    {
        public readonly List<GeometryAttribute> Attributes = new List<GeometryAttribute>();

        public G3D ToG3D(G3dHeader? header = null)
            => new G3D(Attributes, header ?? G3dHeader.Default);

        public G3DBuilder Add(GeometryAttribute attr)
        {
            Attributes.Add(attr);
            return this;
        }

        public G3DBuilder AddIndices(int[] indices)
            => Add(indices.ToIndexAttribute());

        public G3DBuilder AddIndices(IArray<int> indices)
            => Add(indices.ToIndexAttribute());

        public G3DBuilder SetObjectFaceSize(int objectFaceSize)
            => Add(new[] { objectFaceSize }.ToIArray().ToObjectFaceSizeAttribute());

        public G3DBuilder AddVertices(IArray<Vector3> vertices)
            => Add(vertices.ToPositionAttribute());

        public IGeometryAttributes ToIGeometryAttributes()
            => new GeometryAttributes(Attributes);
    }
}

