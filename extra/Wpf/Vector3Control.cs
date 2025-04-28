using System.Numerics;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

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

        public float X { get => Value.X; set => Value = new(value, Y, Z); }
        public float Y { get => Value.Y; set => Value = new(X, value, Z); }
        public float Z { get => Value.Z; set => Value = new(X, Y, value); }

        public static Vector3Control CreateBound(object source, string propName, BindingMode mode = BindingMode.TwoWay)
            => BindTo(new Vector3Control(), source, propName, mode);
    }
}