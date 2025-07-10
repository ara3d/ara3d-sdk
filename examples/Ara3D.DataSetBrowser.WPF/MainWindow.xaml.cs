using System.Text;
using System.Windows;
using Ara3D.Utils;

namespace Ara3D.DataSetBrowser.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var fp = new FilePath(
                @"C:\Users\cdigg\AppData\Local\Temp\477d45ba-22d8-44c3-bb1a-1d5daadf8e59\parameters.mpz");
            var mp = Serialization.LoadBimDataFromMessagePack(fp);
            var dataSet = mp.ToDataSet();
            foreach (var t in dataSet.Tables)
            {
                var dataGrid = TabControl.AddDataGridTab(t.Name);
                dataGrid.AssignDataTable(t);
            }
        }
    }
}