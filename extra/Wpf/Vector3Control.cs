using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Ara3D.Mathematics;

namespace Ara3D.Utils.Wpf
{
    public class Vector3Control : MathControl<Vector3>
    {
        public Vector3Control()
        {
            var grid = new UniformGrid
            {
                Rows = 1,
                Columns = 3
            };
            grid.Children.Add(CreateFloatControl("X"));
            grid.Children.Add(CreateFloatControl("Y"));
            grid.Children.Add(CreateFloatControl("Z"));
            Content = grid;
        }

        public float X { get => Value.X; set => Value = Value.SetX(value); }
        public float Y { get => Value.Y; set => Value = Value.SetY(value); }
        public float Z { get => Value.Z; set => Value = Value.SetZ(value); }

        public static Vector3Control CreateBound(object source, string propName, BindingMode mode = BindingMode.TwoWay)
            => BindTo(new Vector3Control(), source, propName, mode);
    }
}