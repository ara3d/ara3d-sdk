using System.Windows;

namespace Ara3D.Studio.Tools
{
    public partial class OverlayWindow : Window
    {
        public OverlayWindow()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            LabelText.Text = text;
        }

        public static OverlayWindow Instance { get; private set; }

        private bool _requestedVisible = true;

        public void ShowOverlay()
        {
            _requestedVisible = true;
            SyncToOwner();
        }

        public void HideOverlay()
        {
            _requestedVisible = false;
            Hide();
        }

        private void SyncToOwner()
        {
            var parent = Owner;
            if (parent == null)
                return;

            if (!_requestedVisible || !parent.IsVisible || parent.WindowState == WindowState.Minimized)
            {
                if (IsVisible)
                    Hide();
                return;
            }

            if (!IsVisible)
                Show();

            Left = parent.Left + 25;
            Top = parent.Top + 75;
        }

        public static OverlayWindow Create(Window parent)
        {
            var overlayWindow = new OverlayWindow
            {
                Owner = parent,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                AllowsTransparency = true,
                Topmost = true,
                ShowInTaskbar = false
            };

            Instance = overlayWindow;
            Instance.SetText("");

            void Sync() => overlayWindow.SyncToOwner();

            parent.LocationChanged += (_, _) => Sync();
            parent.SizeChanged += (_, _) => Sync();
            parent.IsVisibleChanged += (_, _) => Sync();
            parent.StateChanged += (_, _) => Sync();

            parent.Closed += (_, _) =>
            {
                if (overlayWindow.IsLoaded)
                    overlayWindow.Close();

                if (ReferenceEquals(Instance, overlayWindow))
                    Instance = null;
            };

            overlayWindow.Loaded += (_, _) => Sync();

            overlayWindow.Show();
            Sync();

            return overlayWindow;
        }
    }
}