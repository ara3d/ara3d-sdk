using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Ara3D.DataTable;
using Ara3D.Utils;
using Microsoft.Data.Sqlite;

namespace Ara3D.BimOpenSchema.IO;

public static class DataTableExportUtils
{
    public static void WriteCsv(this IDataTable table, FilePath filePath)
    {
        filePath.GetDirectory().Create();
        using var writer = new StreamWriter(filePath.ToString(), false, Encoding.UTF8);
        writer.WriteLine(string.Join(",", table.Columns.Select(c => EscapeCsv(c.Descriptor.Name))));

        foreach (var row in table.Rows)
        {
            var values = table.Columns.Select(c => EscapeCsv(FormatValue(row[c.ColumnIndex])));
            writer.WriteLine(string.Join(",", values));
        }
    }

    public static void WriteMarkdownTable(this IDataTable table, FilePath filePath)
    {
        filePath.GetDirectory().Create();
        using var writer = new StreamWriter(filePath.ToString(), false, Encoding.UTF8);
        var headers = table.Columns.Select(c => c.Descriptor.Name).ToList();

        writer.WriteLine("| " + string.Join(" | ", headers) + " |");
        writer.WriteLine("| " + string.Join(" | ", headers.Select(_ => "---")) + " |");

        foreach (var row in table.Rows)
        {
            var values = table.Columns.Select(c => FormatValue(row[c.ColumnIndex]).Replace("|", "\\|"));
            writer.WriteLine("| " + string.Join(" | ", values) + " |");
        }
    }

    public static void WriteHtmlTable(this IDataTable table, FilePath filePath)
    {
        filePath.GetDirectory().Create();
        using var writer = new StreamWriter(filePath.ToString(), false, Encoding.UTF8);
        writer.WriteLine("<table>");
        writer.WriteLine("<thead><tr>");
        foreach (var column in table.Columns)
            writer.WriteLine($"<th>{EscapeHtml(column.Descriptor.Name)}</th>");
        writer.WriteLine("</tr></thead>");
        writer.WriteLine("<tbody>");

        foreach (var row in table.Rows)
        {
            writer.WriteLine("<tr>");
            foreach (var column in table.Columns)
                writer.WriteLine($"<td>{EscapeHtml(FormatValue(row[column.ColumnIndex]))}</td>");
            writer.WriteLine("</tr>");
        }

        writer.WriteLine("</tbody></table>");
    }

    public static void WriteSqlite(this IDataTable table, FilePath filePath)
    {
        if (filePath.Exists())
            filePath.Delete();

        filePath.GetDirectory().Create();
        var systemTable = table.ToSystemDataTable();
        var tableName = SanitizeSqliteIdentifier(systemTable.TableName);

        using var connection = new SqliteConnection($"Data Source={filePath}");
        connection.Open();

        var columnDefs = string.Join(", ",
            systemTable.Columns.Cast<System.Data.DataColumn>()
                .Select(c => $"\"{SanitizeSqliteIdentifier(c.ColumnName)}\" TEXT"));

        using (var create = connection.CreateCommand())
        {
            create.CommandText = $"CREATE TABLE \"{tableName}\" ({columnDefs});";
            create.ExecuteNonQuery();
        }

        var columnNames = string.Join(", ",
            systemTable.Columns.Cast<System.Data.DataColumn>()
                .Select(c => $"\"{SanitizeSqliteIdentifier(c.ColumnName)}\""));

        foreach (System.Data.DataRow row in systemTable.Rows)
        {
            using var insert = connection.CreateCommand();
            var parameters = new List<string>();
            for (var i = 0; i < systemTable.Columns.Count; i++)
            {
                var name = $"@p{i}";
                parameters.Add(name);
                insert.Parameters.AddWithValue(name, row[i]?.ToString() ?? (object)DBNull.Value);
            }

            insert.CommandText =
                $"INSERT INTO \"{tableName}\" ({columnNames}) VALUES ({string.Join(", ", parameters)});";
            insert.ExecuteNonQuery();
        }

        SqliteConnection.ClearAllPools();
    }

    static string FormatValue(object? value)
        => value switch
        {
            null => "",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? "",
            _ => value.ToString() ?? ""
        };

    static string EscapeCsv(string value)
    {
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    static string EscapeHtml(string value)
        => value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;");

    static string SanitizeSqliteIdentifier(string name)
        => string.IsNullOrWhiteSpace(name) ? "table" : name.Replace("\"", "\"\"");
}
