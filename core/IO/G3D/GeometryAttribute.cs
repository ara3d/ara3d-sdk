using System;
using System.Numerics;
using Ara3D.Serialization.BFAST;

namespace Ara3D.Serialization.G3D
{
    /// <summary>
    /// A mesh attribute is an array of data associated with some component of a mesh.
    /// It could be vertices, corners, faces, face groups, sub-meshes, instances or the entire mesh.
    /// This is the base class of a typed MeshAttribute.
    /// It provides two core operations we are the foundation for mesh manipulation:
    /// 1. concatenation with like-typed attributes
    /// 2. remapping    
    /// </summary>
    public abstract class GeometryAttribute
    {
        /// <summary>
        /// The descriptor contains information about the data contained in the attribute:
        /// * the primitive data type
        /// * the arity
        /// * the association
        /// * the semantic 
        /// </summary>
        public AttributeDescriptor Descriptor { get; }

        /// <summary>
        /// A "name" is a string encoding of the attribute descriptor. 
        /// </summary>
        public string Name
            => Descriptor.Name;

        /// <summary>
        /// This is the number of data elements in the attribute. This is equal to
        /// the number of primitives times the arity. All mesh attributes associated
        /// with the same mesh component (e.g. vertices) must have the same element count.
        /// </summary>
        public int ElementCount { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected GeometryAttribute(AttributeDescriptor descriptor, int count)
            => (Descriptor, ElementCount) = (descriptor, count);

        /// <summary>
        /// Convenience function to check if this object is a mesh attribute of the given type.
        /// </summary>
        public bool IsType<T>() where T : unmanaged
            => this is GeometryAttribute<T>;

        /// <summary>
        /// Convenience function to check if this object is a mesh attribute of the given type, and the association matches.
        /// </summary>
        public bool IsTypeAndAssociation<T>(Association assoc) where T : unmanaged
            => Descriptor.Association == assoc && this is GeometryAttribute<T>;

        /// <summary>
        /// Convenience function to cast this object into a mesh attribute of the given type, throwing an exception if not possible, 
        /// </summary>
        public GeometryAttribute<T> AsType<T>() where T : unmanaged
            => this as GeometryAttribute<T> ?? throw new Exception($"The type of the attribute is {GetType()} not MeshAttribute<{typeof(T)}>");

        /// <summary>
        /// Loads the correct typed data from a Stream.
        /// </summary>
        public abstract GeometryAttribute Read(MemoryMappedView view);

        /// <summary>
        /// Creates a new GeometryAttribute with the same data, but with a different index. Useful when constructing attributes 
        /// </summary>
        public abstract GeometryAttribute SetIndex(int index);
    }

    /// <summary>
    /// This is a typed attribute associated with some part of the mesh.
    /// The underlying data is an IArray which means that it can be
    /// computed on demand. 
    /// </summary>
    public class GeometryAttribute<T> : GeometryAttribute where T : unmanaged
    {
        public T[] Data;

        public GeometryAttribute(T[] data, AttributeDescriptor descriptor)
            : base(descriptor, data.Length)
        {
            Data = data;
            int arity;
            DataType dataType;
            // TODO: TECH DEBT - Support unsigned tuples in Math3d
            if (typeof(T) == typeof(byte))
                (arity, dataType) = (1, DataType.dt_uint8);
            else if (typeof(T) == typeof(sbyte))
                (arity, dataType) = (1, DataType.dt_int8);
            else if (typeof(T) == typeof(ushort))
                (arity, dataType) = (1, DataType.dt_uint16);
            else if (typeof(T) == typeof(short))
                (arity, dataType) = (1, DataType.dt_int16);
            else if (typeof(T) == typeof(uint))
                (arity, dataType) = (1, DataType.dt_uint32);
            else if (typeof(T) == typeof(int))
                (arity, dataType) = (1, DataType.dt_int32);
            else if (typeof(T) == typeof(ulong))
                (arity, dataType) = (1, DataType.dt_uint64);
            else if (typeof(T) == typeof(long))
                (arity, dataType) = (1, DataType.dt_int64);
            else if (typeof(T) == typeof(float))
                (arity, dataType) = (1, DataType.dt_float32);
            else if (typeof(T) == typeof(Vector2))
                (arity, dataType) = (2, DataType.dt_float32);
            else if (typeof(T) == typeof(Vector3))
                (arity, dataType) = (3, DataType.dt_float32);
            else if (typeof(T) == typeof(Vector4))
                (arity, dataType) = (4, DataType.dt_float32);
            else if (typeof(T) == typeof(Matrix4x4))
                (arity, dataType) = (16, DataType.dt_float32);
            else if (typeof(T) == typeof(double))
                (arity, dataType) = (1, DataType.dt_float64);
            else
                throw new Exception($"Unsupported data type {typeof(T)}");

            // Check that the computed data type is consistent with the descriptor
            if (dataType != Descriptor.DataType)
                throw new Exception($"DataType was {dataType} but expected {Descriptor.DataType}");

            // Check that the computed data arity is consistent with the descriptor
            if (arity != Descriptor.DataArity)
                throw new Exception($"DatArity was {arity} but expected {Descriptor.DataArity}");
        }

        public override GeometryAttribute Read(MemoryMappedView view)
        {
            var data = view.ReadArray<T>();
            return new GeometryAttribute<T>(data, Descriptor);
        }

        public override GeometryAttribute SetIndex(int index)
            => index == Descriptor.Index ? this : new GeometryAttribute<T>(Data, Descriptor.SetIndex(index));
    }
}
