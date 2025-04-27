using System.Windows.Controls;
using System.Windows.Data;
using Ara3D.Mathematics;

namespace Ara3D.Utils.Wpf
{
    public class TransformControl : MathControl<Pose>
    {
        public TransformControl()
        {
            var panel = new StackPanel();
            panel.Children.Add(new TextBlock() { Text = "Position" });
            panel.Children.Add(Vector3Control.CreateBound(this, "Position"));
            panel.Children.Add(new TextBlock() { Text = "Orientation" });
            panel.Children.Add(QuaternionControl.CreateBound(this, "Orientation"));
            Content = panel;
        }

        public Vector3 Position { get => Value.Position; set => Value = Value.SetPosition(value); }
        public Quaternion Orientation { get => Value.Orientation; set => Value = Value.SetOrientation(value); }

        public static TransformControl CreateBound(object source, string propName, BindingMode mode = BindingMode.TwoWay)
            => BindTo(new TransformControl(), source, propName, mode);
    }
}