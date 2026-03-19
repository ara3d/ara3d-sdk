using System;
using System.Windows.Input;
using System.Windows.Threading;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;

namespace Ara3D.Utils.Wpf;

public sealed class WpfWaitContext : IDisposable
{
    private readonly Dispatcher? _dispatcher;
    private Cursor? _previousCursor;

    public WpfWaitContext()
    {
        var app = System.Windows.Application.Current;
        if (app == null)
        {
            // No WPF Application (e.g. unit tests) – act as a no-op.
            return;
        }

        _dispatcher = app.Dispatcher;

        _dispatcher.Invoke(() =>
        {
            _previousCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
        });
    }

    public void Dispose()
    {
        if (_dispatcher == null)
            return; // no-op context

        _dispatcher.Invoke(() =>
        {
            // Restore whatever cursor was there before this context.
            Mouse.OverrideCursor = _previousCursor;
        });
    }
}