using Autodesk.Revit.DB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ara3D.Bowerbird.RevitSamples;

/// <summary>
/// For each non-type element decides whether it is should be included.
/// Used in conjunction with FilteredElementCollector(doc).WhereElementIsNotElementType()
/// We explicitly include only element types that are referenced by elements.
/// This avoids a lot of noise. 
/// </summary>
public sealed class ExportDecider
{
    public BimOpenSchemaExportSettings Cfg { get; }

    public readonly ConcurrentDictionary<(BuiltInCategory bic, CategoryType ct, Type t), bool> Lookup = new();
    public readonly ConcurrentDictionary<Type, bool> LookupDotNetType = new();
    public readonly ConcurrentDictionary<BuiltInCategory, bool> LookupBuiltInCategory = new();

    public ExportDecider(BimOpenSchemaExportSettings cfg)
        => Cfg = cfg ?? new BimOpenSchemaExportSettings();

    public bool ShouldExport(Element e)
    {
        if (e == null) return false;

        var cat = e?.Category;
        var ct = cat?.CategoryType ?? CategoryType.Invalid;
        var bic = cat?.BuiltInCategory ?? BuiltInCategory.INVALID;
        var t = e.GetType();

        return Lookup.GetOrAdd((bic, ct, t), static (key, self) =>
            self.ComputeShouldExport(key.bic, key.ct, key.t),
            this);
    }

    private bool ComputeShouldExport(BuiltInCategory bic, CategoryType ct, Type dotNetType)
    {
        // 1) Explicit include overrides win globally
        if (IsDotNetTypeExplicitlyIncluded(dotNetType) || IsBuiltInCategoryExplicitlyIncluded(bic))
            return true;

        // 2) CategoryType gating (after overrides)
        if (!ShouldExportCategoryType(ct))
            return false;

        // 3) Apply skips (both must pass)
        if (!ShouldExportDotNetType(dotNetType))
            return false;

        if (!ShouldExportBuiltInCategory(bic))
            return false;

        return true;
    }

    private bool ShouldExportCategoryType(CategoryType ct) =>
        ct switch
        {
            CategoryType.Annotation => Cfg.IncludeAnnotationElements,
            _ => true,  // permissive by default
        };

    // -------------------------
    // DotNet type decisions
    // -------------------------

    private bool IsDotNetTypeExplicitlyIncluded(Type type)
    {
        var name = type.Name;
        return MatchesAnyNormalized(Cfg.IncludedClassNamesOverride, name);
    }

    public bool ShouldExportDotNetType(Type type)
    {
        return LookupDotNetType.GetOrAdd(type, t =>
        {
            var className = t.Name;

            // explicit include overrides local skips
            if (MatchesAnyNormalized(Cfg.IncludedClassNamesOverride, className))
                return true;

            if (MatchesAnyNormalized(Cfg.SkippedClassNames, className))
                return false;

            return true;
        });
    }

    // -------------------------
    // BuiltInCategory decisions
    // -------------------------

    private bool IsBuiltInCategoryExplicitlyIncluded(BuiltInCategory bic)
    {
        var bicName = (Enum.GetName(typeof(BuiltInCategory), bic) ?? "");
        var bicNoOst = TrimOstPrefix(bicName);

        return MatchesAnyCategory(Cfg.IncludedCategoriesOverride, bicName, bicNoOst);
    }

    public bool ShouldExportBuiltInCategory(BuiltInCategory bic)
    {
        return LookupBuiltInCategory.GetOrAdd(bic, b =>
        {
            var bicName = (Enum.GetName(typeof(BuiltInCategory), b) ?? "");
            var bicNoOst = TrimOstPrefix(bicName);

            // explicit include overrides local skips
            if (MatchesAnyCategory(Cfg.IncludedCategoriesOverride, bicName, bicNoOst))
                return true;

            if (MatchesAnyCategory(Cfg.SkippedCategories, bicName, bicNoOst))
                return false;

            return true;
        });
    }

    private static string TrimOstPrefix(string s)
        => s.StartsWith("OST_", StringComparison.OrdinalIgnoreCase) ? s.Substring(4) : s;

    private static bool MatchesAnyCategory(IReadOnlyList<string>? patterns, string bicName, string bicNoOst)
    {
        if (patterns == null || patterns.Count == 0) return false;

        // Try matching against both forms of the category name:
        //   - "OST_Walls"
        //   - "Walls"
        //
        // And also treat patterns similarly (normalize OST_ prefix away during match).
        return MatchesAnyNormalized(patterns, bicName) || MatchesAnyNormalized(patterns, bicNoOst);
    }

    // -------------------------
    // Wildcard matching with OST_ normalization
    // -------------------------

    // Cache compiled regex patterns to avoid allocations in big exports
    private static readonly ConcurrentDictionary<string, Regex> _regexCache =
        new(StringComparer.OrdinalIgnoreCase);

    private static bool MatchesAnyNormalized(IReadOnlyList<string>? patterns, string value)
    {
        if (patterns == null || patterns.Count == 0) return false;
        if (string.IsNullOrEmpty(value)) return false;

        // Also try matching against a value with OST_ trimmed,
        // but without allocating unless needed.
        var valueNoOst = TrimOstPrefix(value);

        for (int i = 0; i < patterns.Count; i++)
        {
            var p = patterns[i];
            if (string.IsNullOrWhiteSpace(p)) continue;

            // Normalize pattern too (allow users to omit OST_)
            var pNorm = TrimOstPrefix(p.Trim());

            if (IsMatchWildcard(value, pNorm) || IsMatchWildcard(valueNoOst, pNorm))
                return true;
        }

        return false;
    }

    private static bool IsMatchWildcard(string value, string pattern)
    {
        if (string.IsNullOrEmpty(pattern)) return false;

        // Fast path: no wildcards => case-insensitive equals
        if (pattern.IndexOf('*') < 0 && pattern.IndexOf('?') < 0)
            return string.Equals(value, pattern, StringComparison.OrdinalIgnoreCase);

        var rx = _regexCache.GetOrAdd(pattern, static p =>
        {
            var escaped = Regex.Escape(p);
            var regex = "^" + escaped.Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        });

        return rx.IsMatch(value);
    }
}
