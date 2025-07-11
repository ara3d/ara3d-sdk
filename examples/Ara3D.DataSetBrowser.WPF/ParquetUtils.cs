using System.IO;
using Ara3D.DataTable;
using Ara3D.Utils;
using Parquet;
using Parquet.Data;
using Parquet.Schema;

namespace Ara3D.DataSetBrowser.WPF;

public static class ParquetUtils
{
    public static async Task WriteParquetAsync(
        this IDataTable table,
        FilePath filePath)
    {
        var dataFields = table.Columns.Select(c => new DataField(c.Descriptor.Name, c.Descriptor.Type)).ToList();
        var schema = new ParquetSchema(dataFields);
        
        await using var fs = File.Create(filePath);
        await using var writer = await ParquetWriter.CreateAsync(schema, fs);

        using var rg = writer.CreateRowGroup();
        foreach (var c in table.Columns)
        {
            var df = dataFields[c.ColumnIndex];
            var array = Array.CreateInstance(c.Descriptor.Type, c.Values.Count);
            for (int i = 0; i < c.Values.Count; i++)
                array.SetValue(c.Values[i], i);
            var dc = new DataColumn(df, array);
            await rg.WriteColumnAsync(dc);
        }
    }
}