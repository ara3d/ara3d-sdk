using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ara3D.Models;
using Ara3D.SceneEval;
using Ara3D.Studio.API;
using Plato;
using Plato.Geometry;

namespace Ara3D.Studio.Samples;

public class CloningDemo : IModelGenerator
{
    [Range(0, 4)]
    public int Shape { get; set; }

    [Range(1, 100), DisplayName("# Rows")]
    public int NumRows { get; set; } = 5;

    [Range(1, 100), DisplayName("# Columns")]
    public int NumCols { get; set; } = 5;

    [Range(1, 100), DisplayName("# Layers")]
    public int NumLayers { get; set; } = 1;

    [Range(0f, 20f)]
    public float Spacing = 2f;

    [Range(0f, 1f)]
    public float Red = 0.2f;

    [Range(0f, 1f)]
    public float Green = 0.8f;

    [Range(0f, 1f)]
    public float Blue = 0.1f;

    [Range(0f, 1f)]
    public float Alpha = 1f;

    public Color Color => (Red, Green, Blue, Alpha);

    public Model3D Eval(EvalContext context)
    {
        var mesh = PlatonicSolids.GetMesh(Shape);
        var elements = new List<Element>();
        var mat = new Material(Color, 0.5f, 0.5f);

        for (var k = 0; k < NumLayers; k++)
        {
            for (var j = 0; j < NumRows; j++)
            {
                for (var i = 0; i < NumCols; i++)
                {
                    var vec = new Vector3(i,j,k) * Spacing;
                    var trans = Matrix4x4.CreateTranslation(vec);
                    var elem = new Element(mesh, mat, trans);
                    elements.Add(elem);
                }
            }
        }

        return Model3D.Create(elements);
    }
}