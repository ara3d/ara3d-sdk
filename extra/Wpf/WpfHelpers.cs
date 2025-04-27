using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ara3D.Mathematics;
using static System.Net.Mime.MediaTypeNames;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListBox = System.Windows.Controls.ListBox;
using TabControl = System.Windows.Controls.TabControl;

namespace Ara3D.Utils.Wpf
{
    public static class WpfHelpers
    {
        // TODO: this stuff could be interesting if ported to 
        /*
        public static void Bind<TState>(this IDataService service, string field, DependencyObject obj,
            DependencyProperty prop, IDisposing disposing, BindingMode mode = BindingMode.TwoWay)
            where TState : class, IState

        {
            var bindableState = service.GetBindableState(service.GetStoreFromState<TState>());
            Bind(bindableState, field, obj, prop, mode);
            var pi = typeof(TState).GetProperty(field);
            if (pi == null)
                return;
            service.RegisterObserver<TState>(state =>
            {
                var newVal = pi.GetValue(state);
                obj.Dispatcher.Invoke(() => obj.SetCurrentValue(prop, newVal));
            }, disposing);
        }

        public static void Bind<TState>(TState state, string field, DependencyObject obj, DependencyProperty prop, BindingMode mode = BindingMode.TwoWay)
            where TState : class, IState
            => BindingOperations.SetBinding(obj, prop, new Binding(field) { Source = state, Mode = mode });

        public class TempViewModel<TValue>
        {
            public TempViewModel(Func<TValue> getter)
                => _getter = getter;
            Func<TValue> _getter;
            public TValue Value { get => _getter(); }
        }

        public static void BindDerivedValue<TState, TValue>(this IDataService service, Func<TState, TValue> getter, DependencyObject obj, DependencyProperty prop, IDisposing disposing)
            where TState : class, IState
            => service.RegisterObserver<TState>(state =>
                BindingOperations.SetBinding(obj, prop, new Binding("Value")
                {
                    Source = new TempViewModel<TValue>(() => getter(state)),
                    Mode = BindingMode.OneWay
                }), disposing);
        */

        public static bool ShiftDown
            => (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

        public static bool CtrlDown
            => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

        public static bool AltDown
            => (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;

        public static DockPanel Add(this DockPanel self, UIElement element, Dock dock)
        {
            DockPanel.SetDock(element, dock);
            return self.Add(element);
        }

        public static DockPanel Add(this DockPanel self, UIElement element)
        {
            self.Children.Add(element);
            return self;
        }

        public static TextBlock TextBlock(string text) => new() { Text = text };

        public static TextBlock AddLabel(this UIElementCollection self, string text)
        {
            var r = TextBlock(text);
            self.Add(r);
            return r;
        }

        public static MenuItem AddMenuItem(this ItemCollection self, string text, Action action = null)
        {
            var r = new MenuItem()
            {
                Header = text,
                // Complicated hack: https://stackoverflow.com/questions/14510931/is-there-a-way-to-determine-where-a-wpf-binding-is-declared-created
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Top
            };
            if (action != null)
                r.Click += (_, _) => action();
            self.Add(r);
            return r;
        }

        public static MenuItem? FindMenuItem(this Menu menu, string text)
            => menu.Items.FindMenuItem(text);

        public static bool HasText(this MenuItem menu, string text)
            => menu.Header.ToString().RemoveChars("_").ToLowerInvariant() == text.ToLowerInvariant();

        public static MenuItem? FindMenuItem(this MenuItem menu, string text)
            => menu.HasText(text) ? menu :  menu.Items.FindMenuItem(text);

        public static MenuItem? FindMenuItem(this ItemCollection items, string text)
            => items.OfType<MenuItem>().FirstOrDefault(x => FindMenuItem(x, text) != null);

        public static MenuItem AddMenuItem(this MenuItem menu, string text, Action action = null)
            => menu.Items.AddMenuItem(text, action);

        public static MenuItem AddMenuItem(this Menu menu, string text, Action action = null)
            => menu.Items.AddMenuItem(text, action);

        public static MenuItem AddMenuItem(this ContextMenu menu, string text, Action action = null)
            => menu.Items.AddMenuItem(text, action);

        public static int GetItemIndex(this ListBox self, object value)
            => self.ItemContainerGenerator.IndexFromContainer(self.ItemContainerGenerator.ContainerFromItem(value));

        public static List<int> SelectedIndices(this ListBox self)
        {
            var r = new List<int>();
            foreach (var x in self.SelectedItems)
                r.Add(self.GetItemIndex(x));
            return r;
        }

        public static Grid Add(this Grid self, UIElement element, int row, int col)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, col);
            self.Children.Add(element);
            return self;
        }

        const int SplitterSize = 5;

        public static Grid AddControlRow(this Grid grid, UIElement element)
        {
            var nRow = grid.RowDefinitions.Count;
            grid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(element, nRow);
            return grid;
        }

        public static Grid TwoColumnGridWithSplitter(UIElement left, UIElement right, int width)
        {
            var g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(width) });
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(SplitterSize) });
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.Add(left, 0, 0);
            g.Add(new GridSplitter() { VerticalAlignment = VerticalAlignment.Stretch, Width = SplitterSize }, 0, 1);
            g.Add(right, 0, 2);
            return g;
        }

        public static Grid TwoRowGridWithSplitter(UIElement top, UIElement bottom, int height)
        {
            var g = new Grid();
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(height) });
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(SplitterSize) });
            g.RowDefinitions.Add(new RowDefinition());
            g.Add(top, 0, 0);
            g.Add(new GridSplitter() { HorizontalAlignment = HorizontalAlignment.Stretch, Height = SplitterSize }, 1,
                0);
            g.Add(bottom, 2, 0);
            return g;
        }

        public static TabItem AddTab(this TabControl self, string header, object content)
        {
            var r = new TabItem() { Header = header, Content = content };
            self.Items.Add(r);
            return r;
        }

        public static ContextMenu ShowAtMousePoint(this ContextMenu self)
        {
            self.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            self.IsOpen = true;
            return self;
        }

        public static Vector2 ToVector(this Point pt)
            => ((float)pt.X, (float)pt.Y);

        public static Vector2 GetMouseVector(this Control control)
            => Mouse.GetPosition(control).ToVector();
    }
}
