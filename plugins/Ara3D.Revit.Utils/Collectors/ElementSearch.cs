using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Predicate search over a quick-filtered collection (§3.3 <c>Find…</c> — zero or more
/// results, never throws; <c>Get…</c> — succeed or throw). The predicate runs in memory
/// over the materialized set.
/// </summary>
public static class ElementSearch
{
    public static IReadOnlyList<T> FindElements<T>(this Document doc, Func<T, bool> predicate) where T : Element
        => doc.GetElements<T>().Where(predicate).ToList();

    public static T? FindFirst<T>(this Document doc, Func<T, bool> predicate) where T : Element
        => doc.GetElements<T>().FirstOrDefault(predicate);

    /// <summary>The first instance of <typeparamref name="T"/> with an exact name match, or null (P8).</summary>
    public static T? FindByName<T>(this Document doc, string name) where T : Element
        => doc.FindFirst<T>(e => e.Name == name);

    /// <summary>Throwing form of <see cref="FindByName{T}"/> (§3.3 <c>Get…</c> contract).</summary>
    public static T GetByName<T>(this Document doc, string name) where T : Element
        => doc.FindByName<T>(name)
           ?? throw new ArgumentException($"No {typeof(T).Name} named '{name}' in the document.", nameof(name));

    /// <summary>The first element type of <typeparamref name="T"/> with an exact name match, or null (P8).</summary>
    public static T? FindTypeByName<T>(this Document doc, string name) where T : ElementType
        => doc.GetElementTypes<T>().FirstOrDefault(t => t.Name == name);

    /// <summary>Throwing form of <see cref="FindTypeByName{T}"/> (§3.3 <c>Get…</c> contract).</summary>
    public static T GetTypeByName<T>(this Document doc, string name) where T : ElementType
        => doc.FindTypeByName<T>(name)
           ?? throw new ArgumentException($"No {typeof(T).Name} named '{name}' in the document.", nameof(name));
}
