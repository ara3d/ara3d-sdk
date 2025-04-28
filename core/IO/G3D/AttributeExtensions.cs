using System;
using System.Diagnostics;
using System.Numerics;
using Ara3D.Collections;

namespace Ara3D.Serialization.G3D
{
    public static class AttributeExtensions
    {
        public static GeometryAttribute<T> CheckArity<T>(this GeometryAttribute<T> self, int arity) where T : unmanaged
            => self?.Descriptor?.DataArity == arity ? self : null;

        public static GeometryAttribute<T> CheckAssociation<T>(this GeometryAttribute<T> self, Association assoc) where T : unmanaged
            => self?.Descriptor?.Association == assoc ? self : null;

        public static GeometryAttribute<T> CheckArityAndAssociation<T>(this GeometryAttribute<T> self, int arity, Association assoc) where T : unmanaged
            => self?.CheckArity(arity)?.CheckAssociation(assoc);

        public static GeometryAttribute<T> ToAttribute<T>(this IArray<T> self, AttributeDescriptor desc) where T : unmanaged
            => new GeometryAttribute<T>(self.ToArray(), desc);

        public static GeometryAttribute<T> ToAttribute<T>(this IArray<T> self, string desc) where T : unmanaged
            => self.ToAttribute(AttributeDescriptor.Parse(desc));

        public static GeometryAttribute<T> ToAttribute<T>(this IArray<T> self, string desc, int index) where T : unmanaged
            => self.ToAttribute(AttributeDescriptor.Parse(desc).SetIndex(index));

        public static Vector4[] AttributeToColors(this GeometryAttribute attr)
        {
            var desc = attr.Descriptor;
            if (desc.DataType == DataType.dt_float32)
            {
                if (desc.DataArity == 4)
                    return attr.AsType<Vector4>().Data;
                /*
                if (desc.DataArity == 3)
                    return attr.AsType<Vector3>().Data.Select(vc => new Vector4(vc, 1f));
                if (desc.DataArity == 2)
                    return attr.AsType<Vector2>().Data.Select(vc => new Vector4(vc.X, vc.Y, 0, 1f));
                if (desc.DataArity == 1)
                    return attr.AsType<float>().Data.Select(vc => new Vector4(vc, vc, vc, 1f));
                */
            }
            Debug.WriteLine($"Failed to recognize color format {attr.Descriptor}");
            return null;
        }

        public static GeometryAttribute ToDefaultAttribute(this AttributeDescriptor desc, int count)
        {
            switch (desc.DataType)
            {
                // TODO: TECH DEBT - Add unsigned tuple objects to Math3d
                case DataType.dt_uint8:
                    if (desc.DataArity == 1)
                        return default(byte).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_uint16:
                    if (desc.DataArity == 1)
                        return default(ushort).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_int16:
                    if (desc.DataArity == 1)
                        return default(short).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_uint32:
                    if (desc.DataArity == 1)
                        return default(uint).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_int32:
                    if (desc.DataArity == 1)
                        return default(int).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_uint64:
                    if (desc.DataArity == 1)
                        return default(ulong).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_int64:
                    if (desc.DataArity == 1)
                        return default(long).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_float32:
                    if (desc.DataArity == 1)
                        return default(float).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 2)
                        return default(Vector2).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 3)
                        return default(Vector3).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 4)
                        return default(Vector4).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 16)
                        return default(Matrix4x4).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_float64:
                    if (desc.DataArity == 1)
                        return default(double).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 2)
                        return default(Vector2).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 3)
                        return default(Vector3).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 4)
                        return default(Vector4).Repeat(count).ToAttribute(desc);
                    break;
            }

            throw new Exception($"Could not create a default attribute for {desc}");
        }

        public static long GetByteSize(this GeometryAttribute attribute)
            => (long)attribute.ElementCount * attribute.Descriptor.DataElementSize;
    }
}
