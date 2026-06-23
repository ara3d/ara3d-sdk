using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Window = System.Windows.Window;

namespace Ara3D.Studio.Samples.Lakehouse;

public class DataTableWindow : Window
{
    public DataTableWindow(IDataTable table)
    {
        Title = table.ToString();
        Width = 1200;
        Height = 800;

        var dataGrid = new DataGrid
        {
            AutoGenerateColumns = false,
            IsReadOnly = true,
            CanUserAddRows = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };

        foreach (var c in table.Columns)
        {
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = c.Descriptor.Name,
                Binding = new Binding($"[{c.ColumnIndex}]")
                {
                    Mode = BindingMode.OneWay
                }
            });
        }

        dataGrid.ItemsSource = table.Rows;

        Content = dataGrid;
    }

    public static DataTableWindow CreateAndShow(IDataTable table)
    {
        var w = new DataTableWindow(table);
        w.Show();
        return w;
    }
}