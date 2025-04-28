using System;
using System.Collections.Generic;

namespace Ara3D.NarwhalDB
{
    /// <summary>
    /// Stores information about the name of the table, and the types of each
    /// column. This is always created from a type, the type name is the
    /// name of table, and the fields of the type type make up the columns. 
    /// </summary>
    public class TableSchema
    {
        public readonly string Name;
        public int Size => Archetype.Size();
        public readonly IBinarySerializable Archetype;

        public TableSchema(Type type)
        {
            Name = type.Name;
            Archetype = (IBinarySerializable)Activator.CreateInstance(type);
        }

        public TableSchema(string name, IBinarySerializable archetype)
        {
            Name = name;
            Archetype = archetype;
        }

        public override string ToString()
            => $"{Name}";

        public object ReadObject(ref IntPtr ptr, IReadOnlyList<string> strings)
            => Archetype.Read(ref ptr, strings);
    }
}