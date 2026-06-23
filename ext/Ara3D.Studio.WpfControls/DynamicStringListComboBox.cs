using System.Windows.Controls;
using System.Windows.Threading;
using Ara3D.PropKit;

namespace Ara3D.Studio.WpfControls;

public sealed class DynamicStringListComboBox : ComboBox, IRefreshableEditor
{
    private readonly PropDescriptorDynamicStringList _desc;
    private readonly IBoundPropContainer _props;
    private bool _isRefreshing;

    public DynamicStringListComboBox(
        PropDescriptorDynamicStringList desc,
        IBoundPropContainer props)
    {
        _desc = desc;
        _props = props;

        IsEditable = false;
        IsEnabled = !desc.IsReadOnly;

        SelectionChanged += OnSelectionChanged;

        RefreshEditor();
    }

    public void RefreshEditor()
    {
        Dispatcher.BeginInvoke(() =>
        {
            _isRefreshing = true;

            try
            {
                var selectedIndex = GetSelectedIndex();

                var options = _desc.OptionsFunc()?.ToArray()
                              ?? Array.Empty<string>();

                ItemsSource = options;

                SelectedIndex =
                    selectedIndex >= 0 && selectedIndex < options.Length
                        ? selectedIndex
                        : 0;
            }
            finally
            {
                _isRefreshing = false;
            }
        }, DispatcherPriority.DataBind);
    }

    private int GetSelectedIndex()
    {
        var value = _props.GetValue(_desc.Name);
        return value is int i ? i : -1;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isRefreshing)
            return;

        if (_desc.IsReadOnly)
            return;

        var oldIndex = GetSelectedIndex();
        var newIndex = SelectedIndex;

        if (oldIndex == newIndex)
            return;

        _props.TrySetValue(_desc.Name, newIndex);
    }
}