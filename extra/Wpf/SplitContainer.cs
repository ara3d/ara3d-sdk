using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ara3D.Utils.Wpf
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Ara3D.Utils.Wpf"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Ara3D.Utils.Wpf;assembly=Ara3D.Utils.Wpf"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:SplitContainer/>
    ///
    /// </summary>
    public class SplitContainer : Control
    {
        static SplitContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitContainer), new FrameworkPropertyMetadata(typeof(SplitContainer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //Apply bindings and events
            var leftContent = GetTemplateChild("LeftContent") as ContentPresenter;
            leftContent?.SetBinding(ContentPresenter.ContentProperty, new Binding(nameof(Left)) { Source = this });

            var  rightContent = GetTemplateChild("RightContent") as ContentPresenter;
            rightContent?.SetBinding(ContentPresenter.ContentProperty, new Binding(nameof(Right)) { Source = this });

            var leftColumnDef = GetTemplateChild("LeftColumnDefinition") as ColumnDefinition;
            leftColumnDef?.SetBinding(ColumnDefinition.WidthProperty, new Binding(nameof(LeftColumnWidth)) { Source = this });
        }

        public static readonly DependencyProperty LeftContentProperty = DependencyProperty.Register(
            nameof(Left), typeof(UIElement), typeof(HorizontalSplitContainer), new PropertyMetadata(null));

        public static readonly DependencyProperty RightContentProperty = DependencyProperty.Register(
            nameof(Right), typeof(UIElement), typeof(HorizontalSplitContainer), new PropertyMetadata(null));

        public UIElement? Left
        {
            get => GetValue(LeftContentProperty) as UIElement;
            set => SetValue(LeftContentProperty, value);
        }

        public UIElement? Right
        {
            get => GetValue(RightContentProperty) as UIElement;
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
