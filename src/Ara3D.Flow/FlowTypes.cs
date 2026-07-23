using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.Studio.API;

public static class FlowTypes
{
    /// <summary>
    /// These are objects that can be rendered. 
    /// </summary>
    public static IReadOnlyList<Type> RenderTypes = GetRenderableTypes();

    /// <summary>
    /// These are objects that can flow through the graph 
    /// </summary>
    public static IReadOnlyList<Type> EvalTypes = GetEvalTypes();

    /// <summary>
    /// Used to compute the set of renderable types
    /// </summary>
    public static IReadOnlyList<Type> GetRenderableTypes()
        =>
        [
            typeof(TriangleMesh3D),
            typeof(LineMesh3D),
            typeof(QuadMesh3D),
            typeof(QuadGrid3D),
            typeof(IModel3D),
        ];

    /// <summary>
    /// Used to compute the set of non-renderable types, which
    /// can be converted into into render
    /// </summary>
    public static IReadOnlyList<Type> GetNonRenderableTypes()
        =>
        [
            typeof(ParametricSurface),
            typeof(Curve3D),
        ];

    public static IReadOnlyList<Type> GetEvalTypes()
        => GetRenderableTypes()
            .Concat(GetNonRenderableTypes())
            .Prepend(typeof(FlowObject)).ToList();
}