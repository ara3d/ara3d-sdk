using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using UserControl = System.Windows.Controls.UserControl;

namespace Ara3D.Utils.Wpf
{
    /// <summary>
    /// Interaction logic for LabeledFloatUserControl.xaml
    /// </summary>
    public partial class LabeledFloatUserControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty
            .Register(nameof(Label),
                typeof(string),
                typeof(LabeledFloatUserControl),
                new FrameworkPropertyMetadata("Unnamed Label"));

        public static readonly DependencyProperty ValueProperty = DependencyProperty
            .Register(nameof(Value),
                typeof(float),
                typeof(LabeledFloatUserControl),
                new FrameworkPropertyMetadata(0.0f, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LabeledFloatUserControl lfuc)
                lfuc.OnPropertyChanged();
        }

        public LabeledFloatUserControl()
        {
            InitializeComponent();
            
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public float Value
        {
            get => (float)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public float UpDownAmount { get; set; } = 0.05f;
        public float PixelToAmount { get; set; } = 0.1f;
        
        private float _captureValue { get; set; }
        private Point _capturePoint { get; set; } 

        private void UpButton_Click(object sender, RoutedEventArgs e)
            => Value += UpDownAmount;

        private void DownButton_Click(object sender, RoutedEventArgs e)
            => Value -= UpDownAmount;

        private void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                StartCapture();
            }
        }

        private void StartCapture()
        {
            _capturePoint = Mouse.GetPosition(this);
            _captureValue = Value;
            CaptureMouse();
        }

        /// <summary>
        /// Converts a vertical mouse drag into a value delta.
        /// WPF's Y axis grows downward, so the sign is inverted here: dragging up must
        /// increase the value, matching the Up/Down buttons. This is the one place the
        /// screen-space-to-value-space sign conversion lives.
        /// </summary>
        internal static float PixelsToValueDelta(Point start, Point now, float pixelToAmount)
            => (float)(start.Y - now.Y) * pixelToAmount;

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.Captured == this)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    var delta = PixelsToValueDelta(_capturePoint, e.GetPosition(this), PixelToAmount);
                    if (System.Math.Abs(delta) > 0.001f)
                    {
                        Value = _captureValue + delta;
                    }
                }
                if (e.RightButton == MouseButtonState.Released)
                    ReleaseMouseCapture();
            }
        }

        private void HandleMouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }
}
