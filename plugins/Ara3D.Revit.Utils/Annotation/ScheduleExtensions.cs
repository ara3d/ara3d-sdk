using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class ScheduleExtensions
{
    /// <summary>
    /// Reads the schedule's body section (including its header row) as a table of the
    /// display strings Revit shows in each cell. Pure read; never recalculates or mutates
    /// the schedule.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<string>> ReadAsTable(this ViewSchedule schedule)
    {
        var section = schedule.GetTableData().GetSectionData(SectionType.Body);
        var rows = new List<IReadOnlyList<string>>(section.NumberOfRows);

        for (var r = 0; r < section.NumberOfRows; r++)
        {
            var row = new string[section.NumberOfColumns];
            for (var c = 0; c < section.NumberOfColumns; c++)
                row[c] = schedule.GetCellText(SectionType.Body, r, c);
            rows.Add(row);
        }

        return rows;
    }

    public static bool IsDisplayTypeTotals(this ScheduleField field)
        => field.DisplayType == ScheduleFieldDisplayType.Totals;
}
