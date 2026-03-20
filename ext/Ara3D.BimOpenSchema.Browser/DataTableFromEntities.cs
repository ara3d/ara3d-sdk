using Ara3D.DataTable;
using Ara3D.Utils;

namespace Ara3D.BimOpenSchema.Browser;

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
            Descriptor = new DataDescriptor(name, type, index);
        }
    }

    private IReadOnlyList<EntityModel> Entities { get; }
    public Dictionary<string, Column> ColumnLookup { get; } = new();

    public Column AddColumn(string name, Type type)
    {
        // NOTE: this prevents the same column appearing twice when the parameter names are capitalized differently.
        // That should be flagged as a bug, 
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
            // TEMP: Points are skipped, for now.
            _ => null
        };

    public DataTableFromEntities(IReadOnlyList<EntityModel> entities, string name, bool includeParameters)
    {
        Entities = entities;
        ColumnLookup.Clear();
        var nameColumn = AddColumn("Name", typeof(string));
        var localIdColumn = AddColumn("LocalId", typeof(long));
        var documentColumn = AddColumn("Document", typeof(int));
        var categoryColumn = AddColumn("Category", typeof(string));
        var categoryTypeColumn = AddColumn("CategoryType", typeof(string));
        var classNameColumn = AddColumn("ClassName", typeof(string));
        var levelColumn = AddColumn("Level", typeof(string));
        var groupColumn = AddColumn("Group", typeof(string));
        var roomColumn = AddColumn("Room", typeof(string));
        var familyTypeColumn = AddColumn("Type", typeof(string));

        //var assemblyColumn = AddColumn("Assembly", typeof(string));
        //var worksetColumn = AddColumn("Workset", typeof(int));
        //var globalIdColumn = AddColumn("GlobalId", typeof(string));
        //var categoryTypeColumn = AddColumn("CategoryType", typeof(string));

        var nonParameterColumnCount = ColumnLookup.Count;
        
        // Create the parameter columns
        foreach (var e in Entities)
        {
            if (includeParameters)
            {
                foreach (var pm in e.Parameters)
                {
                    var pt = pm.Descriptor.ParameterType;
                    var paramType = GetColumnDotNetType(pt);
                    if (paramType == null)
                        continue;
                    var paramName = pm.Descriptor.Name;
                    AddColumn(paramName, paramType);
                }
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
            familyTypeColumn.Values.Add(e.Type);

            //globalIdColumn.Values.Add(e.GlobalId);
            //assemblyColumn.Values.Add(e.AssemblyName);
            //worksetColumn.Values.Add(e.WorksetId);
            //categoryTypeColumn.Values.Add(e.CategoryT ype);

            // Add values or default values 
            foreach (var column in ColumnLookup.Values)
            {
                // Skip the first columns
                if (column.ColumnIndex < nonParameterColumnCount)
                    continue;

                if (e.ParameterValues.TryGetValue(column.Name, out var val) && val != null) 
                {
                    // EntityModel is just stored as an integer
                    if (val is EntityModel em)
                    {
                        column.Values.Add((int)em.Index);
                    }
                    // String, Int, Float
                    else if (val.GetType() == column.Type)
                    {
                        column.Values.Add(val);
                    }
                    // Default to the default value if the type is wrong (e.g., a string value for a number parameter)
                    else
                    {
                        column.Values.Add(column.DefaultValue);
                    }
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