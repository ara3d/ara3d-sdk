using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// A project/shared parameter's definition paired with the category binding that exposes it
/// in the model (<see cref="InstanceBinding"/> or <see cref="TypeBinding"/>).
/// </summary>
public readonly record struct ProjectParameterBinding(Definition Definition, ElementBinding Binding);

public static class ProjectParameterExtensions
{
    public static IReadOnlyList<Definition> GetProjectParameters(this Document document)
        => document.GetProjectParameterBindings().Select(b => b.Definition).ToList();

    public static IReadOnlyList<ProjectParameterBinding> GetProjectParameterBindings(this Document document)
    {
        RequireProjectDocument(document);

        var result = new List<ProjectParameterBinding>();
        var iterator = document.ParameterBindings.ForwardIterator();
        while (iterator.MoveNext())
        {
            if (iterator.Current is ElementBinding binding)
                result.Add(new ProjectParameterBinding(iterator.Key, binding));
        }

        return result;
    }

    public static bool TryGetProjectParameterBinding(this Document document, string name, out ProjectParameterBinding binding)
    {
        foreach (var candidate in document.GetProjectParameterBindings())
        {
            if (candidate.Definition.Name != name)
                continue;
            binding = candidate;
            return true;
        }

        binding = default;
        return false;
    }

    /// <summary>
    /// Extends an existing project parameter's binding to cover an additional category.
    /// requires an open transaction
    /// </summary>
    public static void BindParameterToCategory(this Document document, ProjectParameterBinding binding, Category category)
    {
        RequireProjectDocument(document);

        var categories = binding.Binding.Categories;
        if (categories.Contains(category))
            throw new InvalidOperationException($"'{binding.Definition.Name}' is already bound to '{category.Name}'.");

        categories.Insert(category);
        var newBinding = binding.Binding is InstanceBinding
            ? (ElementBinding)document.Application.Create.NewInstanceBinding(categories)
            : document.Application.Create.NewTypeBinding(categories);

        if (!document.ParameterBindings.ReInsert(binding.Definition, newBinding))
            throw new InvalidOperationException($"Failed to rebind '{binding.Definition.Name}'.");
    }

    private static void RequireProjectDocument(Document document)
    {
        if (document.IsFamilyDocument)
            throw new ArgumentException("Project parameters are not available in family documents.", nameof(document));
    }
}
