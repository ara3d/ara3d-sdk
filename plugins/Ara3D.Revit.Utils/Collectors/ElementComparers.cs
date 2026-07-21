using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>Sorts wall types by name, case-insensitively.</summary>
public sealed class WallTypeComparer : IComparer<WallType>
{
    public int Compare(WallType? x, WallType? y)
        => string.Compare(x?.Name, y?.Name, StringComparison.OrdinalIgnoreCase);
}

/// <summary>Sorts views by view type then name, case-insensitively.</summary>
public sealed class ViewComparer : IComparer<View>
{
    public int Compare(View? x, View? y)
        => string.Compare($"{x?.ViewType} : {x?.Name}", $"{y?.ViewType} : {y?.Name}", StringComparison.OrdinalIgnoreCase);
}
