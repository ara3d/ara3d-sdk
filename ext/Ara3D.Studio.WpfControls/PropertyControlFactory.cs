// Existing code above remains unchanged...

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Ara3D.PropKit;

namespace Ara3D.Studio.WpfControls;

public static class PropertyControlGenerator
{
    public static StackPanel CreatePropertyEditorPanel(IBoundPropContainer props)
    {
        var panel = new StackPanel
        {
            Margin = new Thickness(10),
            Background = Brushes.WhiteSmoke
        };

        foreach (var desc in props.GetDescriptors())
        {
            var control = CreateControl(desc, props);
            // We get a null control for unsupported properties 
            if (control != null)
                panel.Children.Add(control);
        }

        return panel;
    }

    public static FrameworkElement? CreateControl(PropDescriptor descriptor, IBoundPropContainer props)
    {
        var editor = descriptor switch
        {
                PropDescriptorInt intDesc => CreateSlider(intDesc, props),
                PropDescriptorLong longDesc => CreateSlider(longDesc, props),
                PropDescriptorFloat floatDesc => CreateSlider(floatDesc, props),
                PropDescriptorBool boolDesc => CreateCheckBox(boolDesc, props),
                PropDescriptorStringList stringListDesc => CreateComboBox(stringListDesc, props),
                PropDescriptorDynamicStringList dynStringListDesc => CreateComboBox(dynStringListDesc, props),
                PropDescriptorString stringDesc => CreateTextBox(stringDesc, props),
                PropDescriptorAction actionDesc => CreateButton(actionDesc, props),
            _ => null
        };

        if (editor == null)
            return null;

        var panel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(5) };

        var headerGrid = new Grid
        {
            Margin = new Thickness(0, 0, 0, 2)
        };
        headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var createHeader = descriptor is not PropDescriptorAction;

        if (createHeader)
        {
            // display name (left)
            var nameText = new TextBlock
            {
                Text = descriptor.DisplayName,
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(nameText, 0);
            headerGrid.Children.Add(nameText);

            // bound value (right)
            var valueText = new TextBlock
            {
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var format = descriptor switch
            {
                PropDescriptorFloat => "{0:0.##}",
                PropDescriptorInt => "{0:N0}",
                PropDescriptorLong => "{0:N0}",
                _ => "{0}"
            };

            valueText.SetBinding(
                TextBlock.TextProperty,
                new Binding(descriptor.Name)
                {
                    Source = props,
                    Mode = BindingMode.OneWay,
                    StringFormat = format
                });
            Grid.SetColumn(valueText, 1);
            headerGrid.Children.Add(valueText);

            panel.Children.Add(headerGrid);
        }

        // Don't bother creating an editor if the whole thing is read-only
        if (descriptor.IsReadOnly)
            return panel;

        editor.ToolTip = descriptor.Description;
        panel.Children.Add(editor);
        return panel;
    }

    public static Binding CreateBinding(PropDescriptor desc, IBoundPropContainer props)
        => new Binding(desc.Name)
        {
            Source = props,
            Mode = desc.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        };

    public static FrameworkElement CreateNumericUpDown(PropDescriptorInt desc, IBoundPropContainer props)
    {
        var numeric = new TextBox
        {
            MinWidth = 60,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
            Background = Brushes.White,
            BorderBrush = Brushes.LightGray
        };
        numeric.SetBinding(TextBox.TextProperty, CreateBinding(desc, props));
        return numeric;
    }

    public static FrameworkElement CreateSlider(PropDescriptorFloat desc, IBoundPropContainer props)
    {
        var slider = new Slider
        {
            Minimum = desc.MinValue,
            Maximum = desc.MaxValue,
            TickFrequency = 0.001f,
            IsSnapToTickEnabled = false,
            Height = 24,
            Margin = new Thickness(0, 2, 0, 2),
            IsEnabled = !desc.IsReadOnly
        };
        slider.SetBinding(RangeBase.ValueProperty, CreateBinding(desc, props));
        return slider;
    }

    public static FrameworkElement CreateSlider(PropDescriptorInt desc, IBoundPropContainer props)
    {
        var slider = new Slider
        {
            Minimum = desc.MinValue,
            Maximum = desc.MaxValue,
            TickFrequency = 1,
            IsSnapToTickEnabled = true,
            Height = 24,
            Margin = new Thickness(0, 2, 0, 2),
            IsEnabled = !desc.IsReadOnly
        };
        slider.SetBinding(RangeBase.ValueProperty, CreateBinding(desc, props));
        return slider;
    }

    public static FrameworkElement CreateSlider(PropDescriptorLong desc, IBoundPropContainer props)
    {
        var slider = new Slider
        {
            Minimum = desc.MinValue,
            Maximum = desc.MaxValue,    
            TickFrequency = 1,
            IsSnapToTickEnabled = true,
            Height = 24,
            Margin = new Thickness(0, 2, 0, 2),
            IsEnabled = !desc.IsReadOnly
        };
        slider.SetBinding(RangeBase.ValueProperty, CreateBinding(desc, props));
        return slider;
    }

    public static FrameworkElement CreateComboBox(PropDescriptorStringList desc, IBoundPropContainer props)
    {
        var comboBox = new ComboBox
        {
            ItemsSource = desc.Options,
            IsEditable = false,
            IsEnabled = !desc.IsReadOnly
        };
        comboBox.SetBinding(Selector.SelectedIndexProperty, CreateBinding(desc, props));
        return comboBox;
    }

    public static FrameworkElement CreateComboBox(PropDescriptorDynamicStringList desc, IBoundPropContainer props)
    {
        var comboBox = new ComboBox
        {
            ItemsSource = desc.OptionsFunc(),
            IsEditable = false,
            IsEnabled = !desc.IsReadOnly
        };
        comboBox.SetBinding(Selector.SelectedIndexProperty, CreateBinding(desc, props));

        void OnPropsOnPropertyChanged(object? o, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var curSource = comboBox.ItemsSource;
            var newSource = desc.OptionsFunc();
            if (ReferenceEquals(curSource, newSource)) return;
            comboBox.Dispatcher.BeginInvoke(() => comboBox.ItemsSource = newSource);
        }

        PropertyChangedEventManager.AddHandler(
            props,
            OnPropsOnPropertyChanged,
            string.Empty); 
        
        return comboBox;
    }

    public static FrameworkElement CreateCheckBox(PropDescriptorBool desc, IBoundPropContainer props)
    {
        var checkBox = new CheckBox
        {
            Content = desc.DisplayName,
            VerticalAlignment = VerticalAlignment.Center,
            FontWeight = FontWeights.Medium,
            Padding = new Thickness(4),
            IsEnabled = !desc.IsReadOnly
        };
        checkBox.SetBinding(ToggleButton.IsCheckedProperty, CreateBinding(desc, props));
        return checkBox;
    }

    public static FrameworkElement CreateTextBox(PropDescriptorString desc, IBoundPropContainer props)
    {
        var textBox = new TextBox()
        {
            VerticalAlignment = VerticalAlignment.Center,
            FontWeight = FontWeights.Medium,
            Padding = new Thickness(4),
            IsEnabled = !desc.IsReadOnly
        };
        textBox.SetBinding(TextBox.TextProperty, CreateBinding(desc, props));
        return textBox;
    }

    public static FrameworkElement CreateButton(PropDescriptorAction desc, IBoundPropContainer props)
    {
        var action = props.GetValue(desc.Name) as Action;
        if (action == null)
            return null;
        var button = new Button()
        {
            Content = desc.DisplayName,
            IsEnabled = action != null,
            VerticalAlignment = VerticalAlignment.Center,
        };
        button.Click += (_, _) => action?.Invoke();
        return button;
    }
}