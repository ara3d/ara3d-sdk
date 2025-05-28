using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ara3D.Models;
using Ara3D.Studio.API;

namespace Ara3D.Studio.Samples
{
    public class Cylinder : IModelGenerator
    {
        public string Name => "Cylinder Demo";

        [Range(3, 100), Description("The number of sides of the cylinder.")]
        public int Sides { get; set; } = 32;

        [Range(0.01, 100), Description("The Radius of the cylinder.")]
        public float Radius { get; set; } = 0.5f;

        [Range(0.01, 100), Description("The height of the cylinder.")]
        public float Height { get; set; } = 2f;

        public Model Eval()
        {
            throw new NotImplementedException("In progress");
        }
    }
}
