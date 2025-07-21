using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Ara3D.Collections;
using Ara3D.PropKit;

namespace Ara3D.DataTable;

public static class DataTableExtensions
{
    public static IDataTable ToDataTable(this DbDataReader ddr, string name = "")
    {
        var n = ddr.FieldCount;
        var r = new DataTableBuilder(name);
        for (var i = 0; i < n; i++)
            r.AddColumn(ddr.GetName(i), ddr.GetFieldType(i));
        while (ddr.Read())
            for (var i = 0; i < n; i++)
                r.ColumnBuilders[i].Values.Add(ddr[i]);
        return r;
    }

    public static DataRow GetRow(this IDataTable self, int rowIndex)
        => new(self, rowIndex);

    public static IReadOnlyList<object> GetRowValues(this IDataTable table, int row)
        => table.Columns.Select(c => c[row]).ToList();

    public static ReadOnlyDataSet AddColumns(this IDataSet self, IDataTable table, params IDataColumn[] columns)
        => self.AddTable(new ReadOnlyDataTable(table.Name, table.Columns.Concat(columns)));

    public static ReadOnlyDataSet AddTable(this IDataSet self, IDataTable table)
        => new(self.Tables.Append(table).ToList());

    public static IDataTable? GetTable(this IDataSet self, string name)
        => self.Tables.FirstOrDefault(t => t.Name == name);

    public static IDataColumn? GetColumn(this IDataTable self, string name)
        => self.Columns.FirstOrDefault(c => c.Descriptor.Name == name);

    public static ReadOnlyDataSet AddColumnsToTable(this IDataSet self, string tableName,
        IReadOnlyList<IDataColumn> columns)
    {
        var table = self.GetTable(tableName);
        if (table == null)
        { 
            table = new ReadOnlyDataTable(tableName, columns);
            return self.AddTable(table);
        }
        return self.AddColumns(table, columns.ToArray());
    }

    public static IReadOnlyList<object> GetValues(this IDataColumn column)
        => column.Count.Select(i => column[i]);

    public static IDataSet ToDataSet(this IReadOnlyList<IDataTable> tables)
        => new ReadOnlyDataSet(tables);

    public static string GetName(this IDataColumn self)
        => self.Descriptor.Name;

    public static Type GetDataType(this IDataColumn self)
        => self.Descriptor.Type;

    public static DataSet ToSystemDataSet(this IDataSet set, string name = "")
    {
        var r = new DataSet(name);
        foreach (var t in set.Tables)
            r.Tables.Add(t.ToSystemDataTable());
        return r;
    }

    public static System.Data.DataTable ToSystemDataTable(this IDataTable table)
    {
        var r = new System.Data.DataTable(table.Name);
        foreach (var c in table.Columns)
            r.Columns.Add(c.GetName(), c.GetDataType());
        return r;
    }

    public static IReadOnlyList<long> AsIndexColumn(this IDataColumn c)
    {
        var elementType = c.GetDataType();
        var r = new long[c.Count];
        if (elementType == typeof(int))
        {
            for (var i=0; i < c.Count; i++)
                r[i] = (int)c[i];
        }
        else if (elementType == typeof(long))
        {
            for (var i = 0; i < c.Count; i++)
                r[i] = (long)c[i];
        }
        else if (elementType == typeof(short))
        {
            for (var i = 0; i < c.Count; i++)
                r[i] = (short)c[i];
        }
        else if (elementType == typeof(sbyte))
        {
            for (var i = 0; i < c.Count; i++)
                r[i] = (sbyte)c[i];
        }
        else if (elementType == typeof(uint))
        {
            for (var i = 0; i < c.Count; i++)
                r[i] = (uint)c[i];
        }
        else if (elementType == typeof(ulong))
        {
            for (var i = 0; i < c.Count; i++)
                r[i] = (long)(ulong)c[i];
        }
        if (elementType == typeof(ushort))
        {
            for (var i = 0; i < c.Count; i++)
                r[i] = (ushort)c[i];
        }
        if (elementType == typeof(byte))
        {
            for (var i = 0; i < c.Count; i++)
                r[i] = (byte)c[i];
        }
        else
        {
            throw new Exception($"Only columns containing integer types can be used as index column, data type was {elementType}");
        }

        return r;
    }

    public static IDataTable JoinTable(this IDataTable tableA, int keyIndex, IDataTable tableB)
    {
        if (tableA.Columns.Count < keyIndex || keyIndex < 0)
            throw new Exception($"Column {keyIndex} not found");

        var keyColumn = tableA.Columns[keyIndex];
        if (keyColumn == null)
            throw new Exception($"KeyColumn {keyIndex} not found");
        var indices = keyColumn.AsIndexColumn();

        var newColumns = new List<IDataColumn>();
        foreach (var col in tableA.Columns)
        {
            if (col.ColumnIndex == keyIndex)
            {
                foreach (var col2 in tableB.Columns)
                {
                    var dcb = new DataColumnBuilder(col2.Descriptor, newColumns.Count);
                    foreach (var index in indices)
                    {
                        dcb.Values.Add(col2[(int)index]);
                    }
                    newColumns.Add(dcb);
                }
            }
            else
            {
                newColumns.Add(col);
            }
        }

        return new ReadOnlyDataTable(tableA.Name, newColumns);
    }

    public static ReadOnlyDataTable ToDataTable<T>(this IReadOnlyList<T> values, string name = "")
    {
        var props = typeof(T).GetPropProvider();

        if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            return new ReadOnlyDataTable(name, [new ReadOnlyDataColumn<T>(0, values, name)]);

        var columns = props.Accessors.Select(
                (acc, i) => new DataColumnFromAccessorAndList<T>(i, acc, values))
            .ToList();
        return new ReadOnlyDataTable(name, columns);
    }

    public static DataTableBuilder AddColumnsFromFieldsAndProperties<T>(this DataTableBuilder self, IEnumerable<T> values)
    {
        var propSet = typeof(T).GetPropProvider();
        var descriptors = propSet.GetDescriptors();
        var columns = descriptors.Count.Select(_ => new List<object>()).ToList();

        foreach (var value in values)
        {
            var row = propSet.GetPropValues(value);
            for (var i = 0; i < row.Count; i++)
            {
                var propVal = row[i];
                Debug.Assert(propVal.Descriptor.Name.Equals(descriptors[i].Name));
                columns[i].Add(propVal.Value);
            }
        }

        for (var i = 0; i < columns.Count; i++)
        {
            self.AddColumn(columns[i], descriptors[i].Name, descriptors[i].Type);
        }

        return self;
    }

    public static IReadOnlyList<T> ToArray<T>(this IDataTable self)
    {
        var r = new T[self.Rows.Count];
        if (self.Columns.Count == 1)
        {
            var c = self.Columns[0];
            if (c.Descriptor.Type == typeof(T))
            {
                for (var i = 0; i < r.Length; i++)
                    r[i] = (T)c[i];
                return r;
            }
        }

        var propSet = typeof(T).GetPropProvider();
        var descriptors = propSet.GetDescriptors();
        var d1 = descriptors.ToDictionary(d => d.Name, d => d);

        var columns = self.Columns.ToDictionary(c => c.GetName(), c => c);
        if (d1.Count != columns.Count)
            throw new Exception($"Number of columns {d1.Count} does not match number of descriptors {columns.Count}");

        for (var i = 0; i < r.Length; i++)
            r[i] = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));

        foreach (var acc in propSet.Accessors)
        {
            var setter = acc.Setter;
            if (setter == null) 
                throw new Exception($"Could not find setter for {acc.Descriptor.Name}");

            var name = acc.Descriptor.Name;
            if (!columns.TryGetValue(name, out var column))
                throw new Exception($"Could not find column {name}");

            for (var i = 0; i < r.Length; i++)
                setter.Invoke(r[i], column[i]);
        }

        return r;
    }

    public static IEnumerable<IDataRecord> GetDataRecords(this IDataTable table)
    {
        foreach (var row in table.Rows)
            yield return new DataRecordAdapter(table, row);
    }
}