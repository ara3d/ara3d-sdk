using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ara3D.Models;
using Ara3D.Studio.API;
using Plato;

namespace Ara3D.Studio.Samples;

public class PlatonicSolidDemos : IModelGenerator
{
    [Range(1, 100), DisplayName("# Rows")]
    public int NumRows { get; set; } = 5;

    [Range(1, 100), DisplayName("# Columns")]
    public int NumCols { get; set; } = 5;

    [Range(1, 100), DisplayName("# Layers")]
    public int NumLayers { get; set; } = 1;

    [Range(0f, 20f)]
    public float Spacing { get; set; } = 2f;

    [Range(0f, 1f)]
    public float Red { get; set; } = 0.7f;

    [Range(0f, 1f)]
    public float Green { get; set; } = 0.4f;

    [Range(0f, 1f)]
    public float Blue { get; set; } = 0.8f;

    [Range(0f, 1f)]
    public float Alpha { get; set; } = 1f;

    public Model Eval()
    {
        var mesh = PlatonicSolids.Tetrahedron.ToNode();
        var nodes = new List<ModelNode>();
        for (var k = 0; k < NumLayers; k++)
        {
            for (var j = 0; j < NumRows; j++)
            {
                for (var i = 0; i < NumCols; i++)
                {
                    var x = i * Spacing;
                    var y = j * Spacing;
                    var z = k * Spacing;
                    var node = mesh
                        .Translate((x, y, z))
                        .WithColor((Red, Green, Blue, Alpha));
                    nodes.Add(node);
                }
            }
        }

        return nodes;
    }
}