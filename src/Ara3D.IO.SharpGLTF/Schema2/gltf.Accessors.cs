using System;
using System.Collections.Generic;
using System.Numerics;

using SharpGLTF.Memory;

namespace SharpGLTF.Schema2
{
    // https://github.com/KhronosGroup/glTF/issues/827#issuecomment-277537204

    public sealed partial class Accessor
    {
        #region lifecycle

        internal Accessor()
        {
            _min = new List<double>();
            _max = new List<double>();
        }

        #endregion

        #region data

        /// <summary>
        /// This must be null, or always in sync with <see cref="_type"/>
        /// </summary>
        private DimensionType? _CachedType;

        #endregion

        #region properties

        internal int _SourceBufferViewIndex => this._bufferView.AsValue(-1);

        /// <summary>
        /// Gets the <see cref="BufferView"/> buffer that contains the items as an encoded byte array.
        /// </summary>
        public BufferView SourceBufferView => this._bufferView.HasValue ? this.LogicalParent.LogicalBufferViews[this._bufferView.Value] : null;

        /// <summary>
        /// Gets the number of items.
        /// </summary>
        public int Count => this._count;

        /// <summary>
        /// Gets the starting byte offset within <see cref="SourceBufferView"/>.
        /// </summary>
        public int ByteOffset => this._byteOffset.AsValue(0);

        /// <summary>
        /// Gets the number of bytes, starting at <see cref="ByteOffset"/> use by this <see cref="Accessor"/>
        /// </summary>
        public int ByteLength => SourceBufferView.GetAccessorByteLength(Format, Count);

        /// <summary>
        /// Gets the <see cref="DimensionType"/> of an item.
        /// </summary>
        public DimensionType Dimensions => _GetDimensions();

        /// <summary>
        /// Gets the <see cref="EncodingType"/> of an item.
        /// </summary>
        public EncodingType Encoding => this._componentType;

        /// <summary>
        /// Gets a value indicating whether the items values are normalized.
        /// </summary>
        public Boolean Normalized => this._normalized.AsValue(false);

        /// <summary>
        /// Gets a value indicating whether this <see cref="Accessor"/> has a sparse structure.
        /// </summary>
        public Boolean IsSparse => this._sparse != null;

        public AttributeFormat Format => new AttributeFormat(this.Dimensions, _componentType, this._normalized.AsValue(false));

        /// <summary>
        /// Gets the bounds of this accessor.
        /// </summary>
        /// <remarks>
        /// Bounds may not be available or up to date, call <see cref="UpdateBounds"/> for update.
        /// </remarks>
        public (IReadOnlyList<double> Min, IReadOnlyList<double> Max) Bounds
        {
            get
            {
                IReadOnlyList<double> min = _min;
                min ??= Array.Empty<double>();

                IReadOnlyList<double> max = _max;
                max ??= Array.Empty<double>();

                return (min, max);
            }
        }

        #endregion

        #region API

        private DimensionType _GetDimensions()
        {
            if (_CachedType.HasValue)
            {
                #if DEBUG
                var parsedType = Enum.TryParse<DimensionType>(this._type, out var rr) ? rr : DimensionType.CUSTOM;
                System.Diagnostics.Debug.Assert(_CachedType.Value == parsedType);
                #endif

                return _CachedType.Value;
            }

            _CachedType = Enum.TryParse<DimensionType>(this._type, out var r) ? r : DimensionType.CUSTOM;

            return _CachedType.Value;
        }

        public MemoryAccessor _GetMemoryAccessor(string name = null)
        {
            var view = SourceBufferView;
            var info = new MemoryAccessInfo(name, ByteOffset, Count, view.ByteStride, Format);
            return new MemoryAccessor(view.Content, info);
        }

        internal KeyValuePair<IntegerArray, MemoryAccessor>? _GetSparseMemoryAccessor()
        {
            return this._sparse == null
                ? (KeyValuePair<IntegerArray, MemoryAccessor>?)null
                : this._sparse._CreateMemoryAccessors(this);
        }

        public int DimCount(DimensionType dt)
        {
            switch (dt)
            {
                case DimensionType.SCALAR:
                    return 1;
                case DimensionType.VEC2:
                    return 2;
                case DimensionType.VEC3:
                    return 3;
                case DimensionType.VEC4:
                    return 4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dt), dt, null);
            }
        }
        public void UpdateBounds()
        {
            this._min.Clear();
            this._max.Clear();

            if (this.Count == 0) return;

            // With the current limitations of the serializer, we can only handle floating point values.
            if (this.Encoding != EncodingType.FLOAT) return;

            // https://github.com/KhronosGroup/glTF-Validator/issues/79

            var dimensions = DimCount(this.Dimensions);

            for (int i = 0; i < dimensions; ++i)
            {
                this._min.Add(double.MaxValue);
                this._max.Add(double.MinValue);
            }

            var array = new MultiArray(this.SourceBufferView.Content, this.ByteOffset, this.Count, this.SourceBufferView.ByteStride, dimensions, this.Encoding, false);

            var current = new float[dimensions];

            for (int i = 0; i < array.Count; ++i)
            {
                array.CopyItemTo(i, current);

                for (int j = 0; j < current.Length; ++j)
                {
                    this._min[j] = Math.Min(this._min[j], current[j]);
                    this._max[j] = Math.Max(this._max[j], current[j]);
                }
            }
        }

        #endregion

        #region Data Buffer API

        /// <summary>
        /// Associates this <see cref="Accessor"/> with a <see cref="BufferView"/>
        /// </summary>
        /// <param name="buffer">The <see cref="BufferView"/> source.</param>
        /// <param name="bufferByteOffset">The start byte offset within <paramref name="buffer"/>.</param>
        /// <param name="itemCount">The number of items in the accessor.</param>
        /// <param name="dimensions">The <see cref="DimensionType"/> item type.</param>
        /// <param name="encoding">The <see cref="EncodingType"/> item encoding.</param>
        /// <param name="normalized">The item normalization mode.</param>
        public void SetData(BufferView buffer, int bufferByteOffset, int itemCount, DimensionType dimensions, EncodingType encoding, Boolean normalized)
        {
            this._bufferView = buffer.LogicalIndex;
            this._byteOffset = bufferByteOffset.AsNullable(_byteOffsetDefault, _byteOffsetMinimum, int.MaxValue);
            this._count = itemCount;

            this._CachedType = dimensions;
            this._type = Enum.GetName(typeof(DimensionType), dimensions);

            this._componentType = encoding;
            this._normalized = normalized.AsNullable(_normalizedDefault);

            UpdateBounds();
        }

        public IList<Matrix3x2> AsMatrix2x2Array()
        {
            return _GetMemoryAccessor().AsMatrix2x2Array();
        }

        public IList<Matrix4x4> AsMatrix3x3Array()
        {
            return _GetMemoryAccessor().AsMatrix3x3Array();
        }

        public IList<Matrix4x4> AsMatrix4x3Array()
        {
            const int dimsize = 4 * 3;

            var view = SourceBufferView;
            var stride = Math.Max(dimsize * this.Encoding.ByteLength(), view.ByteStride);
            var content = view.Content.Slice(this.ByteOffset, Count * stride);

            return new Matrix4x3Array(content, stride, this.Encoding, this.Normalized);
        }

        public IList<Matrix4x4> AsMatrix4x4Array()
        {
            return _GetMemoryAccessor().AsMatrix4x4Array();
        }

        internal IReadOnlyList<Matrix4x4> AsMatrix4x4ReadOnlyList()
        {
            return _GetMemoryAccessor().AsMatrix4x4Array();
        }

        #endregion

        #region Index Buffer API

        public void SetIndexData(MemoryAccessor src)
        {
            var bv = this.LogicalParent.UseBufferView(src.Data, src.Attribute.ByteStride, BufferMode.ELEMENT_ARRAY_BUFFER);
            SetIndexData(bv, src.Attribute.ByteOffset, src.Attribute.ItemsCount, src.Attribute.Encoding.ToIndex());
        }

        /// <summary>
        /// Associates this <see cref="Accessor"/> with a <see cref="BufferView"/>
        /// </summary>
        /// <param name="buffer">The <see cref="BufferView"/> source.</param>
        /// <param name="bufferByteOffset">The start byte offset within <paramref name="buffer"/>.</param>
        /// <param name="itemCount">The number of items in the accessor.</param>
        /// <param name="encoding">The <see cref="IndexEncodingType"/> item encoding.</param>
        public void SetIndexData(BufferView buffer, int bufferByteOffset, int itemCount, IndexEncodingType encoding)
        {
            SetData(buffer, bufferByteOffset, itemCount, DimensionType.SCALAR, encoding.ToComponent(), false);
        }

        public IntegerArray AsIndicesArray()
        {
            return new IntegerArray(SourceBufferView.Content, this.ByteOffset, this._count, this.Encoding.ToIndex());
        }

        #endregion

        #region Vertex Buffer API

        public void SetVertexData(MemoryAccessor src)
        {
            var bv = this.LogicalParent.UseBufferView(src.Data, src.Attribute.StepByteLength, BufferMode.ARRAY_BUFFER);

            SetVertexData(bv, src.Attribute.ByteOffset, src.Attribute.ItemsCount, src.Attribute.Dimensions, src.Attribute.Encoding, src.Attribute.Normalized);
        }

        /// <summary>
        /// Associates this <see cref="Accessor"/> with a <see cref="BufferView"/>
        /// </summary>
        /// <param name="buffer">The <see cref="BufferView"/> source.</param>
        /// <param name="bufferByteOffset">The start byte offset within <paramref name="buffer"/>.</param>
        /// <param name="itemCount">The number of items in the accessor.</param>
        /// <param name="dimensions">The <see cref="DimensionType"/> item type.</param>
        /// <param name="encoding">The <see cref="EncodingType"/> item encoding.</param>
        /// <param name="normalized">The item normalization mode.</param>
        public void SetVertexData(BufferView buffer, int bufferByteOffset, int itemCount, DimensionType dimensions = DimensionType.VEC3, EncodingType encoding = EncodingType.FLOAT, Boolean normalized = false)
        {
            SetData(buffer, bufferByteOffset, itemCount, dimensions, encoding, normalized);
        }

        public IList<Single> AsScalarArray()
        {
            var memory = _GetMemoryAccessor();

            if (this._sparse == null) return memory.AsScalarArray();

            var sparseKV = this._sparse._CreateMemoryAccessors(this);
            return MemoryAccessor.CreateScalarSparseArray(memory, sparseKV.Key, sparseKV.Value);
        }

        public IList<Vector2> AsVector2Array()
        {
            var memory = _GetMemoryAccessor();

            if (this._sparse == null) return memory.AsVector2Array();

            var sparseKV = this._sparse._CreateMemoryAccessors(this);
            return MemoryAccessor.CreateVector2SparseArray(memory, sparseKV.Key, sparseKV.Value);
        }

        public IList<Vector3> AsVector3Array()
        {
            var memory = _GetMemoryAccessor();

            if (this._sparse == null) return memory.AsVector3Array();

            var sparseKV = this._sparse._CreateMemoryAccessors(this);
            return MemoryAccessor.CreateVector3SparseArray(memory, sparseKV.Key, sparseKV.Value);
        }

        public IList<Vector4> AsVector4Array()
        {
            var memory = _GetMemoryAccessor();

            if (this._sparse == null) return memory.AsVector4Array();

            var sparseKV = this._sparse._CreateMemoryAccessors(this);
            return MemoryAccessor.CreateVector4SparseArray(memory, sparseKV.Key, sparseKV.Value);
        }

        public IList<Vector4> AsColorArray(Single defaultW = 1)
        {
            var memory = _GetMemoryAccessor();

            if (this._sparse == null) return memory.AsColorArray(defaultW);

            var sparseKV = this._sparse._CreateMemoryAccessors(this);
            return MemoryAccessor.CreateColorSparseArray(memory, sparseKV.Key, sparseKV.Value, defaultW);
        }

        public IList<Quaternion> AsQuaternionArray()
        {
            var memory = _GetMemoryAccessor();

            if (this._sparse == null) return memory.AsQuaternionArray();

            throw new NotImplementedException();
        }

        public IList<Single[]> AsMultiArray(int dimensions)
        {
            var memory = _GetMemoryAccessor();

            if (this._sparse == null) return memory.AsMultiArray(dimensions);

            throw new NotImplementedException();
        }

        public ArraySegment<Byte> TryGetVertexBytes(int vertexIdx)
        {
            if (_sparse != null) throw new InvalidOperationException("Can't be used on Acessors with Sparse Data");

            var itemByteSz = Encoding.ByteLength() * Dimensions.DimCount();
            var byteStride = Math.Max(itemByteSz, SourceBufferView.ByteStride);
            var byteOffset = vertexIdx * byteStride;

            return SourceBufferView.Content.Slice(this.ByteOffset + (vertexIdx * byteStride), itemByteSz);
        }

        #endregion
    }

    public partial class ModelRoot
    {
        /// <summary>
        /// Creates a new <see cref="Accessor"/> instance
        /// and adds it to <see cref="ModelRoot.LogicalAccessors"/>.
        /// </summary>
        /// <param name="name">The name of the instance.</param>
        /// <returns>A <see cref="Accessor"/> instance.</returns>
        public Accessor CreateAccessor(string name = null)
        {
            var accessor = new Accessor
            {
                Name = name
            };

            _accessors.Add(accessor);

            return accessor;
        }
    }
}
