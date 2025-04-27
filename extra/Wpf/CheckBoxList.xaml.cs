using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CheckBox = System.Windows.Controls.CheckBox;
using UserControl = System.Windows.Controls.UserControl;

namespace Ara3D.Utils.Wpf
{
    /// <summary>
    /// Interaction logic for VisibilityControl.xaml
    /// </summary>
    public partial class CheckBoxList
        : UserControl, INotifyPropertyChanged
    {
        public CheckBoxList()
        {
            InitializeComponent();
            ListBox.ItemsSource = _values;
        }

        private void OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                using (BulkUpdate())
                {
                    foreach (var x in _values)
                    {
                        x.Item2 = cb.Content.Equals(x.Item1);
                    }

                    e.Handled = true;
                }
            }
        }

        private bool dontNotify = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ObservablePair<string, bool>> _values { get; } = new();

        private IEnumerable<ObservablePair<string, bool>> _selection 
        { 
            get
            {
                foreach (var x in ListBox.SelectedItems)
                    yield return x as ObservablePair<string, bool>;
            }
        }

        public void SetValues(IEnumerable<(string, bool)> values)
        {
            dontNotify = true;
            Dispatcher.Invoke(() =>
            {
                try
                {
                    foreach (var _value in _values)
                        _value.PropertyChanged -= Pair_PropertyChanged;
                    _values.Clear();
                    foreach (var value in values)
                    {
                        var pair = new ObservablePair<string, bool>();
                        pair.Item1 = value.Item1;
                        pair.Item2 = value.Item2;
                        pair.PropertyChanged += Pair_PropertyChanged;
                        _values.Add(pair);
                    }
                }
                finally
                {
                    dontNotify = false;
                }
            });
        }

        public IEnumerable<(string, bool)> GetValues()
            => _values.Select(pair => (pair.Item1, pair.Item2));

        private void Pair_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!dontNotify)
                PropertyChanged?.Invoke(sender, e);
        }

        public bool AnySelected
            => ListBox.SelectedItem != null;

        public Disposer BulkUpdate()
        {
            dontNotify = true;
            return new Disposer(() =>
            {
                dontNotify = false;
                PropertyChanged?.Invoke(null, null);
            });
        }

        public void Isolate()
        {
            if (!AnySelected)
                return;
            using (BulkUpdate())
            {
                foreach (var x in _values)
                    x.Item2 = false;
                foreach (var x in _selection)
                    x.Item2 = true;
            }
        }

        public void CheckAll(bool flag)
        {
            using (BulkUpdate())
            {
                foreach (var x in _values)
                    x.Item2 = flag;
            }
        }

        public void Invert()
        {
            using (BulkUpdate())
            {
                foreach (var x in _values)
                    x.Item2 = !x.Item2;
            }
        }

        public void CheckSelection(bool flag = true)
        {
            using (BulkUpdate())
            { 
                foreach (var x in _selection)
                    x.Item2 = flag;
            }
        }
    }
}
