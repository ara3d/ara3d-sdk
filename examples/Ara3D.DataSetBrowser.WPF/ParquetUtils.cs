using System.IO;
using Parquet;
using Parquet.Data;
using Parquet.Schema;

namespace Ara3D.DataSetBrowser.WPF;

public static class ParquetUtils
{
    public static async Task WriteParquetAsync<T>(
        IReadOnlyList<T> rows,
        ParquetSchema schema,
        string filePath,
        params Func<T, Array>[] getters)
    {
        using var fs = File.Create(filePath);
        using var writer = await ParquetWriter.CreateAsync(schema, fs);

        // one big row group; split if you hit memory limits
        using var rg = writer.CreateRowGroup();
        for (int c = 0; c < getters.Length; ++c)
        {
            var data = getters[c](default!);         // probe for element type
            var vector = Array.CreateInstance(data.GetType().GetElementType()!, rows.Count);

            for (int i = 0; i < rows.Count; ++i)
                vector.SetValue(getters[c](rows[i]).GetValue(0), i);

            await rg.WriteColumnAsync(new DataColumn(schema.DataFields[c], vector));
        }
    }
}