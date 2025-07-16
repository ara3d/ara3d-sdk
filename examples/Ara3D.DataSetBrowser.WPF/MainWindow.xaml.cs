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
            var fp = new FilePath(@"C:\Users\cdigg\AppData\Local\Temp\6428f1c7-28b6-4b4c-bf8f-9292ca00c754\bimdata.mpz");
            var mp = Serialization.ReadBimDataFromMessagePack(fp);
            var dataSet = mp.ToDataSet();
            foreach (var t in dataSet.Tables)
            {
                var dataGrid = TabControl.AddDataGridTab(t.Name);
                dataGrid.AssignDataTable(t);
            }
        }
    }
}   