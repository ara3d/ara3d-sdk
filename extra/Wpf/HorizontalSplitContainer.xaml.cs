using System.Windows;

namespace Ara3D.Utils.Wpf
{
    public partial class HorizontalSplitContainer 
    {
        public HorizontalSplitContainer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LeftContentProperty = DependencyProperty.Register(
            nameof(LeftContent), typeof(UIElement), typeof(HorizontalSplitContainer), new PropertyMetadata(null));

        public static readonly DependencyProperty RightContentProperty = DependencyProperty.Register(
            nameof(RightContent), typeof(UIElement), typeof(HorizontalSplitContainer), new PropertyMetadata(null));

        public UIElement LeftContent
        {
            get => (UIElement)GetValue(LeftContentProperty);
            set => SetValue(LeftContentProperty, value);
        }

        public UIElement RightContent
        {
            get => (UIElement)GetValue(RightContentProperty);
            set => SetValue(RightContentProperty, value);
        }

        public GridLength LeftColumnWidth
        {
            get => (GridLength)GetValue(LeftColumnWidthProperty);
            set => SetValue(LeftColumnWidthProperty, value);
        }

        public static readonly DependencyProperty LeftColumnWidthProperty =
            DependencyProperty.Register(
                nameof(LeftColumnWidth),
                typeof(GridLength),
                typeof(HorizontalSplitContainer),
                new PropertyMetadata(new GridLength(1, GridUnitType.Star)));
    }
}
