using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ara3D.Buffers;
using Ara3D.Serialization.BFAST;
using Ara3D.Serialization.G3D;

namespace Ara3D.Serialization.VIM
{
    public static class VimSerializer
    {
        public static List<INamedBuffer> ToBuffers(this SerializableEntityTable table)
        {
            var r = new List<INamedBuffer>();
            r.AddRange(table.DataColumns);
            r.AddRange(table.IndexColumns);
            r.AddRange(table.StringColumns);
            return r;
        }

        public static readonly Regex TypePrefixRegex = new Regex(@"(\w+:).*");

        public static string GetTypePrefix(this string name)
        {
            var match = TypePrefixRegex.Match(name);
            return match.Success ? match.Groups[1].Value : "";
        }

        /// <summary>
        /// Returns the named buffer prefix, or null if no prefix was found.
        /// </summary>
        public static string GetTypePrefix(this INamedBuffer namedBuffer)
            => namedBuffer.Name.GetTypePrefix();

        /// <summary>
        /// Returns a NamedBuffer representing to an entity table column
        /// </summary>
        public static NamedBuffer<T> ReadEntityTableColumn<T>(
            string name, MemoryMappedView view) where T : unmanaged
            => view.ReadArray<T>().ToNamedBuffer(name);

        /// <summary>
        /// Returns a SerializableEntityTable based on the given buffer reader.
        /// </summary>
        public static SerializableEntityTable ReadEntityTable(string name, MemoryMappedView view)
        {
            var et = new SerializableEntityTable { Name = name };

            void ReadColumn(string columnName, MemoryMappedView columnView, int index) 
            {
                var typePrefix = columnName.GetTypePrefix();

                switch (typePrefix)
                {
                    case VimConstants.IndexColumnNameTypePrefix:
                            et.IndexColumns.Add(ReadEntityTableColumn<int>(columnName, columnView));
                            break;
                    case VimConstants.StringColumnNameTypePrefix:
                            et.StringColumns.Add(ReadEntityTableColumn<int>(columnName, columnView));
                            break;
                    case VimConstants.IntColumnNameTypePrefix:
                            et.DataColumns.Add(ReadEntityTableColumn<int>(columnName, columnView));
                            break;
                    case VimConstants.LongColumnNameTypePrefix:
                            et.DataColumns.Add(ReadEntityTableColumn<long>(columnName, columnView));
                            break;
                    case VimConstants.DoubleColumnNameTypePrefix:
                            et.DataColumns.Add(ReadEntityTableColumn<double>(columnName, columnView));
                            break;
                    case VimConstants.FloatColumnNameTypePrefix:
                            et.DataColumns.Add(ReadEntityTableColumn<float>(columnName, columnView));
                            break;
                    case VimConstants.ByteColumnNameTypePrefix:
                            et.DataColumns.Add(ReadEntityTableColumn<byte>(columnName, columnView));
                            break;
                    // For flexibility, we ignore the columns which do not contain a recognized prefix.
                }
            }

            BFastReader.Read(view, ReadColumn);

            return et;
        }

        public static SerializableEntityTable[] ReadEntityTables(MemoryMappedView view)
            => view.ReadBFastBuffers(ReadEntityTable);

        public static void ReadBuffer(this SerializableDocument doc, string name, MemoryMappedView view, int index)
        {
            switch (name)
            {
                case BufferNames.Header:
                    break;

                case BufferNames.Assets:
                    break;

                case BufferNames.Strings:
                    doc.StringTable = ReadStrings(view);
                    break;

                case BufferNames.Geometry:
                    doc.Geometry = G3D.G3D.Read(view);
                    break;

                case BufferNames.Entities:
                    doc.EntityTables = ReadEntityTables(view).ToList();
                    break;
            }
        }

        public static string[] ReadStrings(MemoryMappedView view)
            => view.ReadString().Split('\0');

        public static SerializableDocument Deserialize(string filePath, LoadOptions loadOptions = null)
        {
            var doc = new SerializableDocument { Options = loadOptions };
            BFastReader.Read(filePath, doc.ReadBuffer);
            return doc;
        }
    }
}
