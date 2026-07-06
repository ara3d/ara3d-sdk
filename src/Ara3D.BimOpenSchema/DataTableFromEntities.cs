using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.DataTable;
using Ara3D.Utils;

namespace Ara3D.BimOpenSchema;

public class DataTableFromEntities : IDataTable
{
    public class Row : IDataRow
    {
        public DataTableFromEntities Parent;
        public int RowIndex { get; init; }
        public IDataTable DataTable => Parent;
        public IReadOnlyList<object> Values => Parent.Columns.Select(c => c[RowIndex]).ToList().AsReadOnly();
        public object this[int index] => Parent.Columns[index][RowIndex];

        public Row(DataTableFromEntities parent, int index)
        {
            Parent = parent;
            RowIndex = index;
        }
    }

    public class Column : IDataColumn
    {
        public object DefaultValue { get; }
        public string Name => Descriptor.Name;
        public Type Type => Descriptor.Type;
        public List<object> Values { get; } = new();
        public Array AsArray() => Values.ToArray();
        public int ColumnIndex { get; }
        public IDataDescriptor Descriptor { get; }
        public int Count => Values.Count;
        public object this[int n] => Values[n];

        public Column(string name, Type type, int index)
        {
            DefaultValue = type.GetDefaultValue();
            ColumnIndex = index;
            Descriptor = new DataDescriptor(name, type);
        }
    }

    private IReadOnlyList<EntityModel> Entities { get; }
    public Dictionary<string, Column> ColumnLookup { get; } = new();

    public Column AddColumn(string name, Type type)
    {
        if (ColumnLookup.TryGetValue(name.ToLowerInvariant(), out var column))
            return column;
        var r = new Column(name, type, ColumnLookup.Count);
        ColumnLookup.Add(name.ToLowerInvariant(), r);
        return r;
    }

    public static Type GetColumnDotNetType(ParameterType pt)
        => pt switch
        {
            ParameterType.String => typeof(string),
            ParameterType.Number => typeof(float),
            ParameterType.Entity => typeof(int),
            ParameterType.Int => typeof(int),
            _ => null
        };

    public DataTableFromEntities(
        IReadOnlyList<EntityModel> entities,
        string name,
        bool includeParameters,
        IReadOnlyList<string>? parameterNames = null)
    {
        Entities = entities;
        ColumnLookup.Clear();
        var nameColumn = AddColumn("Name", typeof(string));
        var localIdColumn = AddColumn("LocalId", typeof(long));
        var documentColumn = AddColumn("Document", typeof(string));
        var categoryColumn = AddColumn("Category", typeof(string));
        var categoryTypeColumn = AddColumn("CategoryType", typeof(string));
        var classNameColumn = AddColumn("ClassName", typeof(string));
        var levelColumn = AddColumn("Level", typeof(string));
        var groupColumn = AddColumn("Group", typeof(string));
        var roomColumn = AddColumn("Room", typeof(string));
        var familyTypeColumn = AddColumn("Type", typeof(string));

        var nonParameterColumnCount = ColumnLookup.Count;
        var parameterFilter = parameterNames is { Count: > 0 }
            ? new HashSet<string>(parameterNames, StringComparer.OrdinalIgnoreCase)
            : null;

        foreach (var e in Entities)
        {
            if (!includeParameters)
                continue;

            foreach (var pm in e.Parameters)
            {
                var paramName = pm.Descriptor.Name;
                if (parameterFilter != null && !parameterFilter.Contains(paramName))
                    continue;

                var paramType = GetColumnDotNetType(pm.Descriptor.ParameterType);
                if (paramType == null)
                    continue;

                AddColumn(paramName, paramType);
            }
        }

        foreach (var e in Entities)
        {
            nameColumn.Values.Add(e.Name);
            localIdColumn.Values.Add(e.LocalId);
            documentColumn.Values.Add(e.DocumentTitle);
            categoryColumn.Values.Add(e.Category);
            categoryTypeColumn.Values.Add(e.CategoryType);
            classNameColumn.Values.Add(e.ClassName);
            levelColumn.Values.Add(e.LevelName);
            groupColumn.Values.Add(e.GroupName);
            roomColumn.Values.Add(e.RoomName);
            familyTypeColumn.Values.Add(e.TypeName);

            foreach (var column in ColumnLookup.Values)
            {
                if (column.ColumnIndex < nonParameterColumnCount)
                    continue;

                if (e.ParameterValues.TryGetValue(column.Name, out var val) && val != null)
                {
                    if (val is EntityModel em)
                        column.Values.Add((int)em.Index);
                    else if (val.GetType() == column.Type)
                        column.Values.Add(val);
                    else
                        column.Values.Add(column.DefaultValue);
                }
                else
                {
                    column.Values.Add(column.DefaultValue);
                }
            }
        }

        Columns = ColumnLookup.Values.OrderBy(c => c.ColumnIndex).ToList().AsReadOnly();
        Rows = entities
            .Select((_, index) => new Row(this, index))
            .ToList()
            .AsReadOnly();
        Name = name;
    }

    public string Name { get; }
    public IReadOnlyList<IDataRow> Rows { get; }
    public IReadOnlyList<IDataColumn> Columns { get; }

    public object this[int column, int row] => Columns[column][row];
}
