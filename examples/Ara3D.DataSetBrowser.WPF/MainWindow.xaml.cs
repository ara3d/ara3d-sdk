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
            var fp = PathUtil.GetCallerSourceFolder().RelativeFile("..", "input", "bimdata.mpz");
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