using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Ara3D.Mathematics;

namespace Ara3D.Utils.Wpf
{
    public class QuaternionControl : MathControl<Quaternion>
    {
        public QuaternionControl()
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

        public float X { get => Value.X; set => Value = Value.SetX(value); }
        public float Y { get => Value.Y; set => Value = Value.SetY(value); }
        public float Z { get => Value.Z; set => Value = Value.SetZ(value); }
        public float W { get => Value.W; set => Value = Value.SetW(value); }

        public static QuaternionControl CreateBound(object source, string propName, BindingMode mode = BindingMode.TwoWay)
            => BindTo(new QuaternionControl(), source, propName, mode);
    }
}