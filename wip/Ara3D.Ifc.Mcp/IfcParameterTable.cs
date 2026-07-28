using Ara3D.IfcLoader;

namespace Ara3D.Ifc.Mcp;

/// <summary>Builds a rectangle of elements against parameters — the shape a caller actually wants
/// when comparing many elements, and the one that otherwise costs an <c>ifc_properties</c> call per
/// element. Every column is a dictionary built from the index, so the whole table costs one pass
/// over the requested parameters rather than one pass per row.</summary>
public static class IfcParameterTable
{
    public static (IReadOnlyList<string> Columns, IReadOnlyList<IfcParameterRow> Rows) Build(
        IfcParameterIndex index,
        IfcEntityResolver resolver,
        IReadOnlyList<string> tokens,
        string? propertySet,
        string? type,
        IReadOnlyList<int>? ids)
    {
        var columns = new Dictionary<int, string>[tokens.Count];
        for (var i = 0; i < tokens.Count; i++)
            columns[i] = Column(index, tokens[i], propertySet);

        var rowIds = RowIds(resolver, columns, type, ids);
        var rows = new List<IfcParameterRow>(rowIds.Count);
        foreach (var id in rowIds)
        {
            var entity = resolver.GetEntityOrDefault(id);
            if (entity is not { } found)
                continue;

            var values = new string?[columns.Length];
            for (var i = 0; i < columns.Length; i++)
                values[i] = columns[i].TryGetValue(id, out var text) ? text : null;

            rows.Add(new IfcParameterRow(found.Summarize(), values));
        }

        return (tokens, rows);
    }

    /// <summary>A token naming no parameter is a caller mistake, and a column of nulls would hide
    /// it, so it fails with the name that did not resolve.</summary>
    private static Dictionary<int, string> Column(IfcParameterIndex index, string token, string? propertySet)
    {
        var keys = index.Resolve(token, propertySet);
        if (keys.Count == 0)
            throw new KeyNotFoundException(
                $"No parameter named '{token}' in this model. Call ifc_parameters to see what exists.");

        var column = new Dictionary<int, string>();
        foreach (var key in keys)
            foreach (var value in index.Values(key))
                if (value.Text.Length > 0 && !column.ContainsKey(value.ElementId))
                    column[value.ElementId] = value.Text;

        return column;
    }

    /// <summary>Explicit ids win. A type filter lists every element of that type, including ones
    /// carrying none of the columns, because an absent value is itself an answer. With neither, the
    /// rows are the elements that carry at least one requested parameter.</summary>
    private static IReadOnlyList<int> RowIds(
        IfcEntityResolver resolver,
        IReadOnlyList<Dictionary<int, string>> columns,
        string? type,
        IReadOnlyList<int>? ids)
    {
        if (ids != null)
            return ids;

        var result = new List<int>();
        if (type != null)
        {
            foreach (var entity in resolver.GetEntities())
                if (entity.GetEntityName().Equals(type, StringComparison.OrdinalIgnoreCase))
                    result.Add(entity.Id);
        }
        else
        {
            var union = new HashSet<int>();
            foreach (var column in columns)
                foreach (var id in column.Keys)
                    union.Add(id);
            result.AddRange(union);
        }

        result.Sort();
        return result;
    }
}
