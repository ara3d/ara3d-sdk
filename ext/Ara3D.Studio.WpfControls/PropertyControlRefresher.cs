using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ara3D.Studio.WpfControls;

public static class PropertyControlRefresher
{

    public static void RefreshPropertyEditors(DependencyObject parent)
    {
        if (parent == null)
            return;

        var dispatcher = parent.Dispatcher;

        if (!dispatcher.CheckAccess())
        {
            dispatcher.Invoke(
                () => RefreshPropertyEditors(parent),
                DispatcherPriority.DataBind);
            return;
        }

        foreach (var element in TraverseVisualTree(parent))
        {
            // Custom dynamic editor refresh.
            // Example: dynamic ComboBox re-queries OptionsFunc().
            if (element is IRefreshableEditor refreshable)
                refreshable.RefreshEditor();

            // Generic WPF binding refresh.
            // This pulls values from props into the UI.
            // It does NOT push UI values back into props.
            UpdateAllBindingTargets(element);
        }
    }

    private static void UpdateAllBindingTargets(DependencyObject element)
    {
        var localValues = element.GetLocalValueEnumerator();

        while (localValues.MoveNext())
        {
            var dp = localValues.Current.Property;

            var binding = BindingOperations.GetBindingExpressionBase(element, dp);
            binding?.UpdateTarget();
        }
    }

    private static IEnumerable<DependencyObject> TraverseVisualTree(DependencyObject root)
    {
        yield return root;

        var visualChildCount = VisualTreeHelper.GetChildrenCount(root);

        for (var i = 0; i < visualChildCount; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);

            foreach (var descendant in TraverseVisualTree(child))
                yield return descendant;
        }
    }
}