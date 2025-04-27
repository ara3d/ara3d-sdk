using System.Windows.Controls;
using System.Windows.Data;
using Ara3D.Mathematics;

namespace Ara3D.Utils.Wpf
{
    public class AABoxControl : MathControl<AABox>
    {
        public AABoxControl()
        {
            var panel = new StackPanel();
            panel.Children.Add(new TextBlock() { Text = "Center" });
            panel.Children.Add(Vector3Control.CreateBound(this, "Center"));
            panel.Children.Add(new TextBlock() { Text = "Extent" });
            panel.Children.Add(Vector3Control.CreateBound(this, "Extent"));
            Content = panel;
        }

        public Vector3 Center { get => Value.Center; set => Value = Value.SetCenter(value); }
        public Vector3 Extent { get => Value.Extent; set => Value = Value.SetExtent(value); }

        public static AABoxControl CreateBound(object source, string propName, BindingMode mode = BindingMode.TwoWay)
            => BindTo(new AABoxControl(), source, propName, mode);
    }
}