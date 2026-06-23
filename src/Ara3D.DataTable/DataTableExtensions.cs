using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Ara3D.Collections;
using Ara3D.PropKit;

namespace Ara3D.DataTable;

public static class DataTableExtensions
{
    
    public static IReadOnlyList<object> GetRowValues(this IDataTable table, int row)
        => table.Columns.Select(c => c[row]).ToList();

    public static ReadOnlyDataSet AddTable(this IDataSet self, IDataTable table)
        => new(self.Tables.Append(table).ToList());

    public static IDataTable? GetTable(this IDataSet self, string name)
        => self.Tables.FirstOrDefault(t => t.Name == name);

    public static IDataColumn? GetColumn(this IDataTable self, string name)
        => self.Columns.FirstOrDefault(c => c.Descriptor.Name == name);

    public static T[] GetTypedValues<T>(this IDataColumn column)
    {
        if (typeof(T) != column.GetDataType())
            throw new Exception($"Type {typeof(T)} does not match {column.GetDataType()}");

        if (column is DataColumnWithValues dcwv)
        {
            var vals = dcwv.Values;
            if (vals is T[] r)
                return r;
        }

        var xs = new T[column.Count];
        for (var i = 0; i < column.Count; i++)
            xs[i] = (T)column[i];
        return xs;
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
        foreach (var row in table.Rows)
            r.Rows.Add(row.Values.ToArray()); 
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
    
    public static IDataTable ToDataTable<T>(this IReadOnlyList<T> values, string name = "")
    {
        if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
        {
            return new ReadOnlyListSingleColumnDataAdapter<T>(name, values);
        }

        var props = typeof(T).GetPropProvider();
        var columns = props.Accessors.Select(
                (acc, i) => new DataColumnFromAccessorAndList<T>(i, acc, values))
            .ToList();
        return new DataTable(name, columns, (col, row) => columns[col][row]);
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
            if (!acc.HasSetter) 
                throw new Exception($"Could not find setter for {acc.Descriptor.Name}");

            var name = acc.Descriptor.Name;
            if (!columns.TryGetValue(name, out var column))
                throw new Exception($"Could not find column {name}");

            var typedAcc = acc as IPropAccessor<T>;
            if (typedAcc == null)
                throw new Exception($"Expected accessor to be a {typeof(IProgress<T>)} but was a {typedAcc.GetType()}");
            for (var i = 0; i < r.Length; i++)
                typedAcc.SetValue(ref r[i], column[i]);
        }

        return r;
    }

    public static IEnumerable<IDataRecord> GetDataRecords(this IDataTable table)
    {
        foreach (var row in table.Rows)
            yield return new DataRecordAdapter(table, row);
    }

    public static IDataTable ToDataTable(this IReadOnlyList<IDictionary<string, string>> rows)
    {
        var d = new Dictionary<string, SparseColumn>();
        var dtb = new DataTableBuilder("");
        var numRows = rows.Count;
        var rowIndex = 0;
        foreach (var row in rows)
        {
            foreach (var kv in row)
            {
                var key = kv.Key;
                var val = kv.Value;
                if (!d.ContainsKey(key))
                {
                    var col = new SparseColumn(key, typeof(string), numRows, d.Count, "");
                    dtb.AddColumn(col);
                    d.Add(key, col);
                }

                var sc = d[key];
                sc.Dictionary.Add(rowIndex, val);
            }

            rowIndex++;
        }

        return dtb.Build();
    }
}