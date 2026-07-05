using System.Collections.Generic;
using System.Linq;
using Ara3D.IO.BFAST;

namespace Ara3D.IO.VIM
{
    /// <summary>
    /// Tracks all of the data for a particular entity type in a conceptual table.
    /// A column maybe a relation to another entity table (IndexColumn)
    /// a data value stored as a double (DataColumn) or else
    /// it is string data, stored as an index into the global lookup table (StringColumn).
    /// </summary>
    public class SerializableEntityTable
    {
        /// <summary>
        /// Name of 
        /// </summary>
        public string Name;

        /// <summary>
        /// Relation to another entity table. For example surface to element. 
        /// </summary>
        public List<NamedBuffer<int>> IndexColumns = [];

        /// <summary>
        /// Data encoded as strings in the global string table
        /// </summary>
        public List<NamedBuffer<int>> StringColumns = [];

        /// <summary>
        /// Numeric data encoded as byte, int, float, or doubles 
        /// </summary>
        public List<TypedNamedBuffer> DataColumns = [];

        public IEnumerable<string> ColumnNames
            => IndexColumns.Select(c => c.Name)
                .Concat(StringColumns.Select(c => c.Name))
                .Concat(DataColumns.Select(c => c.Name));
    }

    /// <summary>
    /// Controls what parts of the VIM file are loaded
    /// </summary>
    public class LoadOptions
    {
        public bool SkipGeometry = false;
        public bool SkipAssets = false;
    }

    /// <summary>
    /// The low-level representation of a VIM data file.
    /// </summary>
    public class SerializableDocument
    {
        /// <summary>
        /// Controls how the file is read and loaded into memory
        /// </summary>
        public LoadOptions Options = new();

        /// <summary>
        /// A collection of endline terminated <key>=<value> pairs information about the file
        /// </summary>
        public SerializableHeader Header;

        /// <summary>
        /// A an array of tables, one for each entity 
        /// </summary>
        public List<SerializableEntityTable> EntityTables = [];

        /// <summary>
        /// Used for looking up property strings and entity string fields by Id
        /// </summary>
        public string[] StringTable = [];

        /// <summary>
        /// A list of named buffers each representing a different asset in the file 
        /// </summary>
        public NamedBuffer[] Assets = [];

        /// <summary>
        /// The uninstanced / untransformed geometry
        /// </summary>
        public G3D.G3D Geometry;

        /// <summary>
        /// The originating file name (if provided)
        /// </summary>
        public string FileName;
    }
}
