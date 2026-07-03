using System.Windows;
using Ara3D.DataTable;
using System.Windows.Controls;
using System.Windows.Data;
using Binding = System.Windows.Data.Binding;
using DataGrid = System.Windows.Controls.DataGrid;

namespace Ara3D.BimOpenSchema.Browser;

public static class DataGridUtils
{
    public static void AssignDataTable(this DataGrid grid, IDataTable t)
    {
        grid.Columns.Clear();
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Index",
            Binding = new Binding(nameof(IDataRow.RowIndex)) { Mode = BindingMode.OneTime }
        });
        for (var c = 0; c < t.Columns.Count; c++)
        {
            var descriptor = t.Columns[c].Descriptor;
            var binding = descriptor.Type == typeof(float) || descriptor.Type == typeof(double)
                ? new Binding($"[{c}]") { Mode = BindingMode.OneTime, StringFormat = "F3" }
                : new Binding($"[{c}]") { Mode = BindingMode.OneTime };
            grid.Columns.Add(new DataGridTextColumn
            {
                Header = descriptor.Name, Binding = binding
            });
        }
        grid.ItemsSource = t.Rows;
    }

    /// <summary>
    /// Adds a new tab containing an empty DataGrid and returns that DataGrid.
    /// Call from the UI thread, or any thread – the method marshals to Dispatcher.
    /// </summary>
    public static DataGrid AddDataGridTab(this System.Windows.Controls.TabControl host, string headerText)
    {
        if (!host.Dispatcher.CheckAccess())
            return host.Dispatcher.Invoke(() => AddDataGridTab(host, headerText));

        var dg = new DataGrid
        {
            AutoGenerateColumns = false,   
            CanUserAddRows = false,
            CanUserDeleteRows = false,
            IsReadOnly = false,
            Margin = new Thickness(6)
        };

        var tab = new TabItem
        {
            Header = headerText,
            Content = dg
        };

        host.Items.Add(tab);
        host.SelectedItem = tab;

        return dg;
    }
}
