using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Category tests and lookups (P1: pure queries, never mutate). Comparisons against a
/// <see cref="BuiltInCategory"/> go through <see cref="Identity"/> (C6) — the only place
/// this module reads <see cref="ElementId.Value"/>.
/// </summary>
public static class CategoryExtensions
{
    public static IReadOnlyList<Category> GetAllCategories(this Document doc, bool includeSubCategories = true)
    {
        var result = new List<Category>();

        void Collect(Category category)
        {
            result.Add(category);
            if (!includeSubCategories) return;
            foreach (Category sub in category.SubCategories)
                Collect(sub);
        }

        foreach (Category category in doc.Settings.Categories)
            Collect(category);
        return result;
    }

    /// <summary>Throws if <paramref name="e"/> has no category (§3.3 <c>Get…</c> contract: succeed or throw).</summary>
    public static Category GetCategory(this Element e)
        => e.Category ?? throw new InvalidOperationException($"Element {e.Id.ToLong()} has no category.");

    /// <summary>True when <paramref name="e"/> (or, for a <see cref="FamilyInstance"/>, its symbol) is in <paramref name="category"/>.</summary>
    public static bool IsCategory(this Element e, BuiltInCategory category)
        => CategoryMatches(e.Category, category)
           || e is FamilyInstance fi && CategoryMatches(fi.Symbol?.Category, category);

    public static bool IsModelElement(this Element e)
        => e.Category?.CategoryType == CategoryType.Model;

    public static string GetCategoryName(this Element e)
        => e.GetCategory().Name;

    static bool CategoryMatches(Category? category, BuiltInCategory builtIn)
        => category is not null && category.Id.ToLong() == (long)builtIn;
}
