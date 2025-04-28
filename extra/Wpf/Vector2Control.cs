using System.Numerics;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
namespace Ara3D.Utils.Wpf
{
    public class Vector2Control : MathControl<Vector2>
    {
        public Vector2Control()
        {
            var grid = new UniformGrid
            {
                Rows = 1,
                Columns = 2
            };
            grid.Children.Add(XControl = CreateFloatControl("X"));
            grid.Children.Add(YControl = CreateFloatControl("Y"));
            Content = grid;
        }

        public LabeledFloatUserControl XControl { get; }
        public LabeledFloatUserControl YControl { get; }


        public float X { get => Value.X; set => Value = new(value, Y); }
        public float Y { get => Value.Y; set => Value = new(X, value); }

        public static Vector2Control CreateBound(object source, string propName, BindingMode mode = BindingMode.TwoWay)
            => BindTo(new Vector2Control(), source, propName, mode);

    }
}