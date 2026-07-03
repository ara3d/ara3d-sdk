using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class ExportProgressForm : Form
{
    // Public surface
    public bool IsCancelRequested => _cancelRequested;
    public CancellationToken Token => _cts.Token;

    // UI
    private readonly Label _title;
    private readonly Label _status;
    private readonly Label _counts;
    private readonly Label _eta;
    private readonly Button _cancelButton;
    private readonly ModernProgressBar _bar;

    // State
    private readonly int _total;
    private int _processed;
    private volatile bool _cancelRequested;
    private readonly CancellationTokenSource _cts = new();
    private readonly Stopwatch _sw = Stopwatch.StartNew();

    public ExportProgressForm(string title, int totalElements)
    {
        _total = Math.Max(0, totalElements);

        // Form
        Text = "Ara 3D Export";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        TopMost = true;
        DoubleBuffered = true;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9f);
        Padding = new Padding(18);
        ClientSize = new Size(520, 190);

        // Header
        _title = new Label
        {
            AutoSize = false,
            Text = title,
            Font = new Font("Segoe UI Semibold", 12f),
            ForeColor = Color.FromArgb(25, 25, 25),
            Dock = DockStyle.Top,
            Height = 32
        };

        _status = new Label
        {
            AutoSize = false,
            Text = "Starting…",
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(70, 70, 70),
            Dock = DockStyle.Top,
            Height = 22
        };

        // Progress bar
        _bar = new ModernProgressBar
        {
            Dock = DockStyle.Top,
            Height = 14,
            Margin = new Padding(0, 10, 0, 0),
            Maximum = Math.Max(1, _total),
            Value = 0,
            AccentColor = Color.FromArgb(0, 120, 212), // Windows-ish blue
        };

        // Details row
        var detailsPanel = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(0, 10, 0, 0) };

        _counts = new Label
        {
            AutoSize = false,
            Text = $"0 / {_total:N0}",
            ForeColor = Color.FromArgb(60, 60, 60),
            Dock = DockStyle.Left,
            Width = 220
        };

        _eta = new Label
        {
            AutoSize = false,
            TextAlign = ContentAlignment.TopRight,
            Text = "ETA: —",
            ForeColor = Color.FromArgb(60, 60, 60),
            Dock = DockStyle.Fill
        };

        detailsPanel.Controls.Add(_eta);
        detailsPanel.Controls.Add(_counts);

        // Buttons
        var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 52 };
        _cancelButton = new Button
        {
            Text = "Cancel",
            Width = 100,
            Height = 30,
            Anchor = AnchorStyles.Right | AnchorStyles.Top,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(245, 245, 245),
            ForeColor = Color.FromArgb(30, 30, 30),
        };
        _cancelButton.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
        _cancelButton.FlatAppearance.BorderSize = 1;
        _cancelButton.Location = new Point(ClientSize.Width - Padding.Right - _cancelButton.Width, 10);
        _cancelButton.Click += (_, __) => RequestCancel();

        bottomPanel.Controls.Add(_cancelButton);

        // Add controls (top-to-bottom)
        Controls.Add(bottomPanel);
        Controls.Add(detailsPanel);
        Controls.Add(_bar);
        Controls.Add(_status);
        Controls.Add(_title);

        // Closing should also cancel
        FormClosing += (_, __) => RequestCancel();
    }

    public void RequestCancel()
    {
        if (_cancelRequested) return;
        _cancelRequested = true;
        _cancelButton.Enabled = false;
        _cancelButton.Text = "Cancelling…";
        _status.Text = "Cancelling…";
        _cts.Cancel();
    }

    /// <summary>
    /// Thread-safe progress update. You can call this from the exporter loop.
    /// </summary>
    public void Report(int processed, string? status = null)
    {
        if (IsDisposed) return;

        if (InvokeRequired)
        {
            BeginInvoke(() => Report(processed, status));
            return;
        }

        _processed = Math.Max(0, Math.Min(_total, processed));
        _bar.Value = Math.Max(0, Math.Min(_bar.Maximum, _processed));
        if (!string.IsNullOrWhiteSpace(status))
            _status.Text = status;

        _counts.Text = $"{_processed:N0} / {_total:N0}";

        // ETA
        if (_processed <= 0 || _sw.Elapsed.TotalSeconds < 0.25)
        {
            _eta.Text = "ETA: —";
        }
        else
        {
            var rate = _processed / _sw.Elapsed.TotalSeconds; // elems / sec
            var remaining = Math.Max(0, _total - _processed);
            var secondsLeft = rate > 0.0001 ? remaining / rate : double.PositiveInfinity;
            _eta.Text = double.IsFinite(secondsLeft)
                ? $"ETA: {TimeSpan.FromSeconds(secondsLeft):mm\\:ss}"
                : "ETA: —";
        }
    }
}