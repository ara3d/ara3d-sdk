using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Ara3D.Mathematics;

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

            XControl.PropertyChanged += XControl_PropertyChanged;
            YControl.PropertyChanged += YControl_PropertyChanged;
            base.PropertyChanged += Vector2Control_PropertyChanged;
        }

        private void XControl_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Value = Value.SetX(X);
        }

        private void YControl_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Value = Value.SetY(Y);
        }

        public LabeledFloatUserControl XControl { get; }
        public LabeledFloatUserControl YControl { get; }

        private void Vector2Control_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            XControl.Value = Value.X;
            YControl.Value = Value.Y;
        }

        public float X { get => Value.X; set => Value = Value.SetX(value); }
        public float Y { get => Value.Y; set => Value = Value.SetY(value); }

        public static Vector2Control CreateBound(object source, string propName, BindingMode mode = BindingMode.TwoWay)
            => BindTo(new Vector2Control(), source, propName, mode);

    }
}