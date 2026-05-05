using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Ara3D.Geometry;

namespace Ara3D.Utils.Wpf
{
    public class Vector3Control : MathControl<Vector3>
    {
        public Vector3Control()
        {
            Loaded += (_, _) =>
            {
                if (Content != null)
                    return;

                var grid = new UniformGrid
                {
                    Rows = 1,
                    Columns = 3
                };

                grid.Children.Add(CreateFloatControl(nameof(X)));
                grid.Children.Add(CreateFloatControl(nameof(Y)));
                grid.Children.Add(CreateFloatControl(nameof(Z)));

                Content = grid;
            };
        }

        public float X { get => Value.X; set => Value = new(value, Y, Z); }
        public float Y { get => Value.Y; set => Value = new(X, value, Z); }
        public float Z { get => Value.Z; set => Value = new(X, Y, value); }

        public static Vector3Control CreateBound(object source, string propName, BindingMode mode = BindingMode.TwoWay)
            => BindTo(new Vector3Control(), source, propName, mode);

        protected override void OnValueChanged(Vector3 oldValue, Vector3 newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            RaisePropertyChanged(nameof(X));
            RaisePropertyChanged(nameof(Y));
            RaisePropertyChanged(nameof(Z));
        }
    }
}