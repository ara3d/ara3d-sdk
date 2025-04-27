using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Binding = System.Windows.Data.Binding;
using UserControl = System.Windows.Controls.UserControl;

namespace Ara3D.Utils.Wpf
{
    public class MathControl<T> : UserControl, INotifyPropertyChanged
    {
        public Binding CreateBinding(string propName)
            => new() { Source = this, Path = new PropertyPath(propName), Mode = BindingMode.TwoWay };

        public LabeledFloatUserControl CreateFloatControl(string propName)
        {
            var r = new LabeledFloatUserControl { Label = propName };
            BindingOperations.SetBinding(r, LabeledFloatUserControl.ValueProperty, CreateBinding(propName));
            return r;
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(T), typeof(MathControl<T>),
                new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            => (sender as MathControl<T>)?.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(""));

        public event PropertyChangedEventHandler PropertyChanged;

        public T Value
        {
            get => (T)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public MathControl<T> BindTo(object source, string propName, BindingMode mode = BindingMode.TwoWay)
        {
            BindingOperations.SetBinding(this, ValueProperty, new Binding { Source = source, Path = new PropertyPath(propName), Mode = mode });
            return this;
        }

        public static TSelf BindTo<TSelf>(TSelf self, object source, string propName, BindingMode mode = BindingMode.TwoWay)
            where TSelf: MathControl<T>
            => self.BindTo(source, propName, mode) as TSelf;
    }
}