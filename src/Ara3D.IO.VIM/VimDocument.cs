using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Utils;

namespace Ara3D.IO.VIM
{
    public class VimDocument
    {
        public SerializableDocument Document { get; }
        public Dictionary<long, Element> Elements = new();
        public List<string> CategoryNames = new();
        public List<int> NodeElements = new();

        public static VimDocument Load(FilePath filePath) => new(VimSerializer.Deserialize(filePath));

        public VimDocument(SerializableDocument doc)
        {
            Document = doc;
            foreach (var t in Document.EntityTables)
                Add(new VimTable(this, t));

            var parameterTable = FindTable("Parameter");
            var descriptorTable = FindTable("ParameterDescriptor");
            var elementTable = FindTable("Element");
            var categoryTable = FindTable("Category");
            var nodeTable = FindTable("Node");

            for (var i = 0; i < categoryTable.Count; ++i)
            {
                var row = (VimRow)categoryTable[i];
                var nameIndex = (int)row["Name"];
                var name = GetString(nameIndex);
                CategoryNames.Add(name);
            }

            for (var i = 0; i < elementTable.Count; ++i)
            {
                var row = (VimRow)elementTable[i];

                var id = row[0] is int ? (long)(int)row[0] : (long)row[0];
                if (id < 0)
                    continue;
                if (Elements.ContainsKey(id))
                    continue;
                var typeIndex = (int)row["Type"];
                var nameIndex = (int)row["Name"];
                var categoryIndex = (int)row["Category"];
                var cat = categoryIndex >= 0 ? CategoryNames[categoryIndex] : "";
                var type = GetString(typeIndex);
                var name = GetString(nameIndex);

                var element = new Element
                {
                    Id = id,
                    Type = type,
                    Name = name,
                    Category = cat,
                };
                Elements.Add(id, element);
            }

            for (var i = 0; i < parameterTable.Count; ++i)
            {
                var row = (VimRow)parameterTable[i];
                var elementIndex = (int)row[2];
                if (elementIndex < 0)
                    continue;
                if (!Elements.TryGetValue(elementIndex, out var element))
                    continue;

                var valueIndex = (int)row["Value"];
                var descriptorIndex = (int)row["ParameterDescriptor"];
                var value = GetString(valueIndex);
                var values = value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                value = values.Length > 1 ? values[1] : values.Length == 1 ? values[0] : "";
                var descriptor = descriptorIndex >= 0
                    ? (VimRow)descriptorTable[descriptorIndex]
                    : null;
                var nameIndex = descriptor == null ? -1 : (int)descriptor["Name"];
                var name = GetString(nameIndex);
                var p = new Parameter(name, value);
                element.Parameters.Add(p);
            }

            for (var i = 0; i < nodeTable.Count; ++i)
            {
                var row = (VimRow)nodeTable[i];
                var elementIndex = (int)row[0];
                NodeElements.Add(elementIndex);
            }
        }

        public void Add(VimTable table)
            => _tables.Add(table);

        private List<VimTable> _tables = new();
        public IReadOnlyList<VimTable> Tables => _tables;

        public VimTable FindTable(string name)
            => string.IsNullOrEmpty(name) ? null : Tables.FirstOrDefault(table => table.Name == name);

        public string GetString(int index)
            => index >= 0 ? Document.StringTable[index] : string.Empty;
    }
}