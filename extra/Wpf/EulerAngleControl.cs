using System;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Ara3D.Mathematics;

namespace Ara3D.Utils.Wpf
{
    public class EulerAngleControl : MathControl<Quaternion>
    {
        public EulerAngleControl()
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

        public Vector3 EulerAngles
        {

            get => Value.ToEulerAngles() * 360f / MathF.PI * 2;
            set => Value = Quaternion.CreateFromEulerAngles(value * MathF.PI * 2 / 360f); 
        }

        public float X { get => EulerAngles.X; set => EulerAngles = EulerAngles.SetX(value); }
        public float Y { get => EulerAngles.Y; set => EulerAngles = EulerAngles.SetY(value); }
        public float Z { get => EulerAngles.Z; set => EulerAngles = EulerAngles.SetZ(value); }

        public static EulerAngleControl CreateBound(object source, string propName, BindingMode mode = BindingMode.TwoWay)
            => BindTo(new EulerAngleControl(), source, propName, mode);
    }
}
