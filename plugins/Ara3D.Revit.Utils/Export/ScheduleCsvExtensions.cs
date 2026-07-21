using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class ScheduleCsvExtensions
{
    /// <summary>
    /// Writes the schedule's body as a CSV file, via D9's ScheduleExtensions.ReadAsTable
    /// (no re-query of the schedule here). Pure read of the schedule; the only side effect
    /// is the file write.
    /// </summary>
    public static void ExportScheduleToCsv(this ViewSchedule schedule, string path)
        => File.WriteAllText(path, schedule.ReadAsTable().ToCsv());

    static string ToCsv(this IReadOnlyList<IReadOnlyList<string>> rows)
    {
        var sb = new StringBuilder();
        foreach (var row in rows)
            sb.AppendLine(string.Join(",", row.Select(EscapeCsvField)));
        return sb.ToString();
    }

    static string EscapeCsvField(string field)
        => field.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0
            ? "\"" + field.Replace("\"", "\"\"") + "\""
            : field;
}
