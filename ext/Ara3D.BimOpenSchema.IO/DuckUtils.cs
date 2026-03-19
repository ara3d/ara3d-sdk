using System;
using System.IO;
using System.Linq;
using Ara3D.Utils;
using DuckDB.NET.Data;

namespace Ara3D.BimOpenSchema.IO;

public static class DuckUtils
{
    public static void BosToDuckDB(this FilePath bosPath, FilePath duckDbPath)
    {
        var folder = bosPath.ToTempFolderName();
        folder.CreateAndClearDirectory();
        bosPath.UnzipAll(folder);
        ParquetToDuckDB(folder, duckDbPath);
    }

    public static void ParquetToDuckDB(DirectoryPath folderPath, FilePath databasePath, bool includeGeometry = false)
    {
        using var conn = new DuckDBConnection($"DataSource={databasePath}");
        conn.Open();

        var geometryTables = Enum.GetNames(typeof(BimGeometryTableName)).ToHashSet();

        foreach (var file in Directory.EnumerateFiles(folderPath, "*.parquet"))
        {
            var tableName = Path.GetFileNameWithoutExtension(file);

            if (!includeGeometry && geometryTables.Contains(tableName))
                continue;

            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
                CREATE OR REPLACE TABLE {tableName} AS
                SELECT *
                FROM read_parquet('{file.Replace("\\", "/")}');
            ";
            cmd.ExecuteNonQuery();
        }
    }
}