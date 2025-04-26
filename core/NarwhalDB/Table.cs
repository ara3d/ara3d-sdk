using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ara3D.Buffers;
using Ara3D.Utils;

namespace Ara3D.NarwhalDB
{
    public class Table
    {
        public readonly TableSchema TableSchema;

        public string Name 
            => TableSchema.Name;

        public readonly List<object> Objects
            = new List<object>();

        public Table(TableSchema tableSchema)
            => TableSchema = tableSchema;

        public int Add(object obj)
        {
            Objects.Add(obj);
            return Objects.Count - 1;
        }

        public INamedBuffer ToBuffer(IndexedSet<string> stringTable)
        {
            var data = new byte[Objects.Count * TableSchema.Size];
            var offset = 0;
            foreach (IBinarySerializable obj in Objects)
                obj.Write(data, ref offset, stringTable);
            Debug.Assert(offset == data.Length);
            return data.ToNamedBuffer(TableSchema.Name);
        }

        public static Table Create(ByteSpan span, Type type, IReadOnlyList<string> strings)
            => Create(span, new TableSchema(type), strings);

        public static unsafe Table Create(ByteSpan span, TableSchema schema, IReadOnlyList<string> strings)
        {
            var t = new Table(schema);
            var ptr = new IntPtr(span.Ptr);
            var end = span.End();
            while (ptr.ToPointer() < end)
            {
                var obj = schema.ReadObject(ref ptr, strings);
                t.Add(obj);
            }

            return t;
        }
    }
}