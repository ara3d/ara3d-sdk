using System;

namespace Ara3D.IO.G3D
{
    /// <summary>
    /// Provides information about identifying the role and parsing the data within an attribute data buffer.
    /// This is encoded using a string in a particular URN form. 
    /// </summary>
    public class AttributeDescriptor : IEquatable<AttributeDescriptor>
    {
        public Association Association { get; }
        public string Semantic { get; }
        public DataType DataType { get; }
        public int DataArity { get; }
        public int Index { get; }

        public int DataElementSize { get; }
        public int DataTypeSize { get; }
        public string Name { get; }

        /// <summary>
        /// The original association substring seen during Parse. Preserved so that an unknown
        /// association (which maps to <see cref="Association.assoc_none"/>) round-trips back to its
        /// original text instead of being silently rewritten to "none". Null for directly-constructed
        /// descriptors and for known associations.
        /// </summary>
        private readonly string _associationOverride;

        public AttributeDescriptor(Association association, string semantic, DataType dataType, int dataArity, int index = 0)
            : this(association, semantic, dataType, dataArity, index, null)
        { }

        private AttributeDescriptor(Association association, string semantic, DataType dataType, int dataArity, int index, string associationOverride)
        {
            Association = association;
            if (semantic.Contains(":"))
                throw new Exception("The semantic must not contain a colon");
            Semantic = semantic;
            DataType = dataType;
            DataArity = dataArity;
            Index = index;
            _associationOverride = associationOverride;
            DataTypeSize = GetDataTypeSize(DataType);
            DataElementSize = DataTypeSize * DataArity;
            Name = $"g3d:{AssociationString}:{Semantic}:{Index}:{DataTypeString}:{DataArity}";
        }

        /// <summary>
        /// Generates a URN representation of the attribute descriptor
        /// </summary>
        public override string ToString()
            => Name;

        /// <summary>
        /// Returns true if the attribute descriptor has been successfully parsed.
        /// </summary>
        public static bool TryParse(string urn, out AttributeDescriptor attributeDescriptor)
        {
            attributeDescriptor = null;
            try
            {
                attributeDescriptor = Parse(urn);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to parse attribute descriptor URN '{urn}': {e}");
            }

            return attributeDescriptor != null;
        }

        /// <summary>
        /// Parses a URN representation of the attribute descriptor to generate an actual attribute descriptor 
        /// </summary>
        public static AttributeDescriptor Parse(string urn)
        {
            var vals = urn.Split(':');
            if (vals.Length != 6) throw new Exception("Expected 6 parts to the attribute descriptor URN");
            if (vals[0] != "g3d") throw new Exception("First part of URN must be g3d");
            var association = ParseAssociation(vals[1]);
            // Preserve the original text for an unknown association so it round-trips instead of becoming "none".
            var associationOverride = association == Association.assoc_none && vals[1] != "none" ? vals[1] : null;
            return new AttributeDescriptor(
                association,
                vals[2],
                ParseDataType(vals[4]),
                int.Parse(vals[5]),
                int.Parse(vals[3]),
                associationOverride
            );
        }

        public bool Validate()
        {
            var urn = ToString();
            var tmp = Parse(urn);
            if (!Equals(tmp))
                throw new Exception("Invalid attribute descriptor (or internal error in the parsing/string conversion");
            return true;
        }

        public bool Equals(AttributeDescriptor other)
            => other != null && Name == other.Name;

        public override bool Equals(object obj)
            => Equals(obj as AttributeDescriptor);

        public override int GetHashCode()
            => Name.GetHashCode();

        public static int GetDataTypeSize(DataType dt)
        {
            switch (dt)
            {
                case DataType.dt_uint8:
                case DataType.dt_int8:
                    return 1;
                case DataType.dt_uint16:
                case DataType.dt_int16:
                    return 2;
                case DataType.dt_uint32:
                case DataType.dt_int32:
                    return 4;
                case DataType.dt_uint64:
                case DataType.dt_int64:
                    return 8;
                case DataType.dt_float32:
                    return 4;
                case DataType.dt_float64:
                    return 8;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dt), dt, null);
            }
        }

        public string AssociationString
            => _associationOverride ?? Association.ToString().Substring("assoc_".Length);

        public static Association ParseAssociation(string s)
        {
            switch (s)
            {
                case "all":
                    return Association.assoc_all;
                case "corner":
                    return Association.assoc_corner;
                case "edge":
                    return Association.assoc_edge;
                case "face":
                    return Association.assoc_face;
                case "instance":
                    return Association.assoc_instance;
                case "vertex":
                    return Association.assoc_vertex;
                case "shapevertex":
                    return Association.assoc_shapevertex;
                case "shape":
                    return Association.assoc_shape;
                case "material":
                    return Association.assoc_material;
                case "mesh":
                    return Association.assoc_mesh;
                case "submesh":
                    return Association.assoc_submesh;

                // Anything else we just treat as unknown 
                default:
                    return Association.assoc_none;
            }
        }

        public string DataTypeString
            => DataType.ToString()?.Substring("dt_".Length) ?? null;

        public static DataType ParseDataType(string s)
            => (DataType)Enum.Parse(typeof(DataType), "dt_" + s);

        public AttributeDescriptor SetIndex(int index)
            => new AttributeDescriptor(Association, Semantic, DataType, DataArity, index, _associationOverride);
    }
}
