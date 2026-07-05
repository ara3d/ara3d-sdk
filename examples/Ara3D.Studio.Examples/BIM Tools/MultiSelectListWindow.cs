using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListBox = System.Windows.Controls.ListBox;
using SelectionMode = System.Windows.Controls.SelectionMode;

public static class MultiSelectListWindow
{
    /// <summary>
    /// Shows a modeless WPF window containing a multi-select list of strings.
    /// Calls <paramref name="onSelectionChanged"/> whenever selection changes.
    /// Returns the created Window so caller can close/position it if desired.
    /// </summary>
    public static Window Show(
        IEnumerable<string> items,
        Action<IReadOnlyList<string>> onSelectionChanged,
        string title = "Select items",
        double width = 420,
        double height = 520,
        Window owner = null)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));
        if (onSelectionChanged is null) throw new ArgumentNullException(nameof(onSelectionChanged));

        // Ensure we're on the WPF UI thread
        if (Application.Current == null)
            throw new InvalidOperationException("WPF Application.Current is null. Ensure WPF is initialized.");

        Window window = null;

        void CreateAndShow()
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.Extended, // Ctrl/Shift multi-select
                Margin = new Thickness(10),
                BorderThickness = new Thickness(1),
                HorizontalContentAlignment = HorizontalAlignment.Stretch
            };

            // Populate
            foreach (var s in items)
                listBox.Items.Add(s ?? "");

            // Optional: nicer scrolling
            ScrollViewer.SetCanContentScroll(listBox, true);
            ScrollViewer.SetVerticalScrollBarVisibility(listBox, ScrollBarVisibility.Auto);
            ScrollViewer.SetHorizontalScrollBarVisibility(listBox, ScrollBarVisibility.Auto);

            // Selection changed callback
            listBox.SelectionChanged += (_, __) =>
            {
                var selected = listBox.SelectedItems.Cast<object>()
                    .Select(o => o?.ToString() ?? "")
                    .ToList()
                    .AsReadOnly();

                onSelectionChanged(selected);
            };

            // Small header bar (no XAML)
            var header = new DockPanel { LastChildFill = true, Margin = new Thickness(10, 10, 10, 0) };
            var hint = new TextBlock
            {
                Text = "Ctrl-click to toggle, Shift-click to range-select",
                Opacity = 0.75,
                VerticalAlignment = VerticalAlignment.Center
            };

            var clearBtn = new Button
            {
                Content = "Clear",
                Margin = new Thickness(10, 0, 0, 0),
                Padding = new Thickness(10, 4, 10, 4),
                MinWidth = 70
            };
            clearBtn.Click += (_, __) => listBox.UnselectAll();

            DockPanel.SetDock(clearBtn, Dock.Right);
            header.Children.Add(clearBtn);
            header.Children.Add(hint);

            var root = new DockPanel();
            DockPanel.SetDock(header, Dock.Top);
            root.Children.Add(header);
            root.Children.Add(listBox);

            window = new Window
            {
                Title = title,
                Width = width,
                Height = height,
                Content = root,
                WindowStartupLocation = owner != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen
            };

            if (owner != null)
            {
                window.Owner = owner;
                window.ShowInTaskbar = false;
            }

            // Make keyboard feel right
            window.PreviewKeyDown += (_, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    window.Close();
                    e.Handled = true;
                }
            };

            window.Show(); // modeless
        }

        var dispatcher = Application.Current.Dispatcher;
        if (dispatcher.CheckAccess()) CreateAndShow();
        else dispatcher.Invoke(CreateAndShow);

        return window;
    }
}
