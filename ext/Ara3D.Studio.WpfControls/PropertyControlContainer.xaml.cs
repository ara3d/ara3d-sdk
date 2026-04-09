using System.Windows;
using System.Windows.Controls;
using Ara3D.Studio.API;

namespace Ara3D.Studio.WpfControls
{
    /// <summary>
    /// This property Control is not used directly in ARa 3D Studio. 
    /// </summary>
    public partial class PropertyControlContainer : UserControl
    {
        public GeneratorWrapper Wrapper { get; private set; }

        public PropertyControlContainer()
        {
            InitializeComponent();
        }

        public void ConnectToModel(IGenerator g)
        {
            Wrapper = new GeneratorWrapper(g);
            var propSet = Wrapper.Props;
            var editorPanel = PropertyControlGenerator.CreatePropertyEditorPanel(propSet);

            Content = new ScrollViewer
            {
                Content = editorPanel,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Padding = new Thickness(10)
            };
        }
    }
}
