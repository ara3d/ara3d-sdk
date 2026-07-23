using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.Studio.API;

public static class FlowObjectConverters
{
    public static object Convert(FlowObject flowObject, Type outputType)
    {
        if (outputType == typeof(FlowObject))
            return flowObject;

        var input = flowObject.Content;
        if (input == null)
            return null;

        if (flowObject.Type == outputType)
            return input;

        return ConvertImpl(input, flowObject.Type, outputType);
    }

    public static object ConvertImpl(object input, Type inputType, Type outputType)
    {
        if (outputType == typeof(object))
        {
            return input;
        }
        else if (outputType.IsAssignableTo(typeof(IModel3D)))
        {
            if (input is TriangleMesh3D tri)
            {
                return Model3D.Create(tri);
            }
            else if (input is ColoredTriangleMesh3D coloredTri)
            {
                return Model3D.Create(coloredTri.Mesh);
            }
            else if (input is QuadMesh3D quad)
            {
                return Model3D.Create(quad.Triangulate());
            }
            else if (input is QuadGrid3D grid)
            {
                return Model3D.Create(grid.Triangulate());
            }
            else if (input is Model3D || input is IModel3D)
            {
                return input;
            }
            else if (input is RenderModelData rmd)
            {
                return rmd.ToModel3D();
            }
        }
        else if (outputType == typeof(TriangleMesh3D))
        {
            if (input is QuadMesh3D quad)
            {
                return quad.Triangulate();
            }
            else if (input is QuadGrid3D grid)
            {
                return grid.Triangulate();
            }
            else if (input is ColoredTriangleMesh3D mesh)
            {
                return mesh.Mesh;
            }
            else if (input is IModel3D m)
            {
                return m.ToMesh();
            }
        }

        throw new Exception($"Unable to convert from {input} of type {inputType} to object of type {outputType}");
    }
}