using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ara3D.BFAST;
using Ara3D.Buffers;
using Ara3D.Logging;
using Ara3D.Serialization.BFAST;
using Ara3D.Utils;

namespace Ara3D.NarwhalDB
{
    public class DB
    {
        public readonly Dictionary<string, Table> TableLookup
            = new Dictionary<string, Table>();

        public int NumTables
            => TableLookup.Count;

        public IEnumerable<Table> GetTables()
            => TableLookup.Values;

        public Table AddTable(TableSchema tableSchema)
            => AddTable(new Table(tableSchema));

        public Table AddTable(Table table)
        {
            TableLookup.Add(table.Name, table);
            return table;
        }

        public Table AddTable<T>() where T: IBinarySerializable, new()
            => AddTable(new TableSchema(typeof(T).Name, new T()));

        public Table GetTable(string name)
            => TableLookup[name];

        public const string _STRINGS_ = nameof(_STRINGS_);

        public FilePath WriteToFile(FilePath fp, ILogger logger)
        {
            logger.Log($"Saving file to {fp}");
            var stringTable = new IndexedSet<string>();
            logger.Log($"Creating buffers");
            var buffers = GetTables().Select(t => t.ToBuffer(stringTable)).ToList();
            logger.Log($"Creating string table buffers");
            var st = stringTable.OrderedMembers().PackStrings().ToNamedBuffer(_STRINGS_);
            buffers.Add(st);
            var bldr = new BFastBuilder();
            bldr.Add(buffers);
            bldr.Write(fp);
            logger.Log("Completed writing file");
            return fp;
        }

        public static DB ReadFile(FilePath fp, IReadOnlyList<Type> types, ILogger logger)
        {
            logger.Log($"Loading file from {fp}");
            var buffers = BFastReader.Read(fp, logger);
            logger.Log($"Read {buffers.Count} buffers");
            return Create(buffers, types, logger);
        }

        public static DB Create(IReadOnlyList<ByteSpanBuffer> buffers, IReadOnlyList<Type> types, ILogger logger)
        {
            logger.Log($"Creating database from {buffers.Count} buffers, and {types.Count} types");
            if (buffers.Count != types.Count + 1)
                throw new Exception($"Expected {types.Count + 1} buffers not {buffers.Count}");
            var db = new DB();
            var stringsBuffer = buffers.Single(b => b.Name == _STRINGS_);
                
            // TODO: 
            var strings = stringsBuffer.ByteSpan.UnpackStrings().Select(bs => bs.ToString()).ToList();
            
            logger.Log($"Found {strings.Count} strings");
            foreach (var t in types)
            {
                logger.Log($"Searching for buffer {t.Name}");
                var buffer = buffers.Single(b => b.Name == t.Name);
                logger.Log($"Creating table from buffer {buffer.Name}");
                var table = Table.Create(buffer.ByteSpan, t, strings);
                db.AddTable(table);
                logger.Log($"Created table {table.Name} with {table.Objects.Count} objects");
            }

            logger.Log("Completed creating database");
            return db;
        }

        public static int WriteString(byte[] bytes, ref int offset, string value, IndexedSet<string> strings)
            => WriteInt(bytes, ref offset, strings.Add(value));

        public static int WriteInt(byte[] bytes, ref int offset, int value)
        {
            // Convert the integer value to a byte array
            var intBytes = BitConverter.GetBytes(value);

            // Copy the integer bytes to the specified byte array at the given offset
            Array.Copy(intBytes, 0, bytes, offset, intBytes.Length);

            // Update the offset
            var r = intBytes.Length;
            offset += r;
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(byte[] bytes, ref int offset, IReadOnlyList<string> strings)
            => strings[ReadInt(bytes, ref offset)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt(byte[] bytes, ref int offset)
            => bytes[offset++] 
               | (bytes[offset++] << 8) 
               | (bytes[offset++] << 16) 
               | (bytes[offset++] << 24);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(ref IntPtr ptr, IReadOnlyList<string> strings)
            => strings[ReadInt(ref ptr)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadInt(ref IntPtr ptr)
        {
            var r = *(int*)ptr.ToPointer();
            ptr += 4;
            return r;
        }
    }
}