using Ara3D.BimOpenSchema;
using Ara3D.BimOpenSchema.IO;
using Ara3D.DataTable;
using Ara3D.IO.GltfExporter;
using Ara3D.Models;
using Ara3D.Utils;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Ara3D.Utils.Wpf;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace Ara3D.BimOpenSchema.Browser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BimData Data;
        public BimModel3D Model3D;
        public Model3D ToModel3D() => Model3D.RenderModelData.ToModel3D();
        public BimObjectModel ObjectModel => Model3D.ObjectModel;
        public IReadOnlyList<IDataTable> Tables;
        public Grouping CurrentGrouping = Grouping.None;
        public IReadOnlyList<IGrouping<string, EntityModel>> GroupedEntities = null;
        public FilePath CurrentFile;
        public OpenFileDialog OpenFileDialog = null;
        public FolderBrowserDialog FolderDialog = null;

        public enum Grouping
        {
            AlphaName,
            None,
            Document,
            Level,
            Group,
            Room,
            Class,
            Category,
            CategoryType,
            Family,
            FamilyCategoryWithParameters,
            FamilyClassWithParameters,
            CategoryWithParameters,
            ClassWithParameters,
        }

        public MainWindow()
        {
            InitializeComponent();
            UpdateGroupingMenuItems();
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                {
                    await OpenFile(args[1]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occured {ex.Message}");
            }
        }

        public static DirectoryPath DefaultSaveLocation()
            => SpecialFolders.MyDocuments.RelativeFolder("BIM Open Schema");
        
        public async Task OpenFile(FilePath fp)
        {
            if (!fp.Exists())
                return;
            using var waitContext = new WpfWaitContext();

            Model3D = null;
            CurrentFile = fp;
            Data = await fp.ReadBimDataFromParquetZipAsync().ConfigureAwait(false);
            Model3D = BimModel3D.Create(Data, true);
            await UpdateTables();
        }

        public void UpdateGroupingMenuItems()
        {
            GroupingMenuItem.Items.Clear();
            foreach (var val in Enum.GetValues(typeof(Grouping)))
            {
                if (val.Equals(Grouping.FamilyCategoryWithParameters))
                    GroupingMenuItem.Items.Add(new Separator());

                if (val.Equals(Grouping.CategoryWithParameters))
                    GroupingMenuItem.Items.Add(new Separator());

                var name = Enum.GetName(typeof(Grouping), val).SplitCamelCase();
                var tmp = new MenuItem()
                {
                    Header = name,
                    IsCheckable = true,
                };
                if (CurrentGrouping == (Grouping)val)
                {
                    tmp.IsChecked = true;
                }

                tmp.Click += (_, _) => SetGrouping((Grouping)val);
                GroupingMenuItem.Items.Add(tmp);
            }
        }

        public async Task SetGrouping(Grouping g)
        {
            if (g == CurrentGrouping)
                return;
            CurrentGrouping = g;
            UpdateGroupingMenuItems();
            await UpdateTables();
        }

        public DirectoryPath ChooseFolder()
        {
            if (FolderDialog == null)
            {
                FolderDialog = new FolderBrowserDialog();
                var startFolder = DefaultSaveLocation();
                startFolder.Create();
                FolderDialog.InitialDirectory = startFolder;
            }

            if (FolderDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return null;

            var baseName = CurrentFile.GetFileNameWithoutExtension();
            var baseFolder = new DirectoryPath(FolderDialog.SelectedPath);
            var folder = baseFolder.RelativeFolder(baseName);
            if (CurrentGrouping != Grouping.None)
            {
                var subFolder = CurrentGrouping.ToString();
                folder = folder.RelativeFolder(subFolder);
            }
            folder.Create();
            return folder;
        }

        public IEnumerable<EntityModel> GetAllEntities()
            => ObjectModel.Entities;

        public IEnumerable<EntityModel> GetInstanceEntities()
            => ObjectModel.Entities.Where(e => e.IsNotTypeOrCategory);

        public IEnumerable<EntityModel> GetTypeEntities()
            => ObjectModel.Entities.Where(e => e.IsType);

        public IEnumerable<IGrouping<string, EntityModel>> CreateGroupings()
        {
            // TODO: there is some confusion about the name "Family" versus "Type". They are used interchangeably. 
            switch (CurrentGrouping)
            {
                case Grouping.None:
                    return GetAllEntities().GroupBy(_ => "All");
                case Grouping.AlphaName:
                    return GetAllEntities().GroupBy(e => e.Name.IsNullOrEmpty() ? " " : e.Name[0].ToString());
                case Grouping.Category:
                    return GetAllEntities().GroupBy(e => e.Category);
                case Grouping.FamilyCategoryWithParameters:
                    return GetTypeEntities().GroupBy(e => e.Category);
                case Grouping.CategoryWithParameters:
                    return GetInstanceEntities().GroupBy(e => e.Category);
                case Grouping.CategoryType:
                    return GetAllEntities().GroupBy(e => e.CategoryType);
                case Grouping.Level:
                    return GetAllEntities().GroupBy(e => e.LevelName);
                case Grouping.Group:
                    return GetAllEntities().GroupBy(e => e.GroupName);
                case Grouping.Class:
                    return GetAllEntities().GroupBy(e => e.ClassName);
                case Grouping.FamilyClassWithParameters:
                    return GetTypeEntities().GroupBy(e => e.ClassName);
                case Grouping.ClassWithParameters:
                    return GetInstanceEntities().GroupBy(e => e.ClassName);
                case Grouping.Room:
                    return GetAllEntities().GroupBy(e => e.RoomName);
                case Grouping.Document:
                    return GetAllEntities().GroupBy(e => e.DocumentTitle);
                case Grouping.Family:
                    return GetAllEntities().GroupBy(e => e.TypeName);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //case Grouping.CategoryType:
        //    return ObjectModel.Entities.GroupBy(e => e.CategoryType);

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ??= new OpenFileDialog()
            {
                DefaultExt = ".bos",
                Filter = "BIM Open Schema files (*.bos)|*.bos|All files (*.*)|*.*"
            };

            if (OpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await OpenFile(OpenFileDialog.FileName);
            }
        }

        private async void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (Tables == null)
            {
                MessageBox.Show("No data loaded", "Error");
                return;
            }

            var folder = ChooseFolder();
            if (!folder.Exists())
                return;

            try
            {
                using var waitContext = new WpfWaitContext();

                foreach (var t in Tables)
                {
                    var fp = folder.RelativeFile(t.Name.ToValidFileName() + ".xlsx");
                    t.WriteToExcel(fp);
                }

                CommonDialogs.FolderExportCompleted(folder);
            }
            catch (Exception ex)
            {
                CommonDialogs.Error("Error occured when exporting excel files", ex);
            }
        }
        
        private async void ExportGLB_Click(object sender, RoutedEventArgs e)
        {
            if (Tables == null)
            {
                MessageBox.Show("No data loaded", "Error");
                return;
            }

            var folder = ChooseFolder();
            if (!folder.Exists())
                return;

            try
            {
                using var waitContext = new WpfWaitContext();

                foreach (var g in GroupedEntities)
                {
                    var fp = folder.RelativeFile(g.Key.ToValidFileName() + ".glb");
                    SaveGltf(g, fp);
                }

                CommonDialogs.FolderExportCompleted(folder);
            }
            catch (Exception ex)
            {
                CommonDialogs.Error("Error occured when exporting gltf files", ex);
            }
        }

        public void SaveGltf(IEnumerable<EntityModel> entities, FilePath fp)
        {
            var entityIndices = entities.Select(em => (int)em.Index).ToHashSet();
            var newModel = Model3D.RenderModelData.ToModel3D().FilterAndRemoveUnusedMeshes(i => entityIndices.Contains(i.EntityIndex));
            if (newModel.Instances.Count > 0 && newModel.Meshes.Count > 0)
                newModel.WriteGlb(fp);
        }

        private async void ExportParquet_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(CurrentFile))
                return;

            var folder = ChooseFolder();
            if (!folder.Exists())
                return;

            try
            {
                using var waitContext = new WpfWaitContext();
                CurrentFile.UnzipAll(folder);
                CommonDialogs.FolderExportCompleted(folder);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occured when exporting parquet files: {ex}");
            }
        }

        public bool IncludeParameters()
        {
            return CurrentGrouping.ToString().EndsWith("Parameters");
        }

        public DataTableFromEntities CreateTable(IGrouping<string, EntityModel> entities)
            => new (entities.ToList(), entities.Key, IncludeParameters());

        private async Task UpdateTables()
        {
            using var waitContext = new WpfWaitContext();

            GroupedEntities = CreateGroupings().OrderBy(g => g.Key).ToList();

            await Dispatcher.InvokeAsync(() =>
            {
                Tables = GroupedEntities.Select(CreateTable).ToList();

                TabControl.Items.Clear();
                foreach (var t in Tables)
                {
                    var grid = TabControl.AddDataGridTab(t.Name);
                    grid.AssignDataTable(t);
                }
            });
        }
        
        private void ExportDuckDB_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CurrentFile.Exists())
                {
                    MessageBox.Show("No file loaded", "Error");
                    return;
                }

                var dlg = new SaveFileDialog();
                dlg.DefaultExt = ".duckdb";
                dlg.FileName = CurrentFile.GetFileNameWithoutExtension();
                dlg.Filter = "DuckDB files (*.duckdb)|*.duckdb|All files (*.*)|*.*";
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
                var duckDbPath = new FilePath(dlg.FileName);
                using var waitContext = new WpfWaitContext();

                CurrentFile.BosToDuckDB(duckDbPath);
                CommonDialogs.FileExportCompleted(duckDbPath);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occured while exporting duck DB: " + ex.Message, "Error");
            }
        }
    }
}   