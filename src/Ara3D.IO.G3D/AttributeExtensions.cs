using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Ara3D.Collections;
using Ara3D.Memory;

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

        public static GeometryAttribute<T> ToAttribute<T>(this IMemoryOwner<T> self, AttributeDescriptor desc) where T : unmanaged
            => new GeometryAttribute<T>(self, desc);

        public static GeometryAttribute<T> ToAttribute<T>(this IMemoryOwner<T> self, string desc) where T : unmanaged
            => self.ToAttribute(AttributeDescriptor.Parse(desc));

        public static GeometryAttribute<T> ToAttribute<T>(this IMemoryOwner<T> self, string desc, int index) where T : unmanaged
            => self.ToAttribute(AttributeDescriptor.Parse(desc).SetIndex(index));

        public static IBuffer<Vector4> AttributeToColors(this GeometryAttribute attr)
        {
            var desc = attr.Descriptor;
            if (desc.DataType == DataType.dt_float32)
            {
                if (desc.DataArity == 4)
                    return attr.AsType<Vector4>().Data;
            }
            Debug.WriteLine($"Failed to recognize color format {attr.Descriptor}");
            return null;
        }


        public static long GetByteSize(this GeometryAttribute attribute)
            => (long)attribute.ElementCount * attribute.Descriptor.DataElementSize;
    }
}
