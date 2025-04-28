using System.Numerics;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Ara3D.Utils.Wpf
{
    public class Vector4Control : MathControl<Vector4>
    {
        public Vector4Control()
        {
            var grid = new UniformGrid
            {
                Rows = 1,
                Columns = 4
            };
            grid.Children.Add(CreateFloatControl("X"));
            grid.Children.Add(CreateFloatControl("Y"));
            grid.Children.Add(CreateFloatControl("Z"));
            grid.Children.Add(CreateFloatControl("W"));
            Content = grid;
        }

        public float X { get => Value.X; set => Value = new(value, Y, Z, W); }
        public float Y { get => Value.Y; set => Value = new(X, value, Z, W); }
        public float Z { get => Value.Z; set => Value = new(X, Y, value, W); }
        public float W { get => Value.W; set => Value = new(X, Y, Z, value); }

        public static Vector4Control CreateBound(object source, string propName, BindingMode mode = BindingMode.TwoWay)
            => BindTo(new Vector4Control(), source, propName, mode);
    }
}