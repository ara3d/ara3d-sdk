using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.Bowerbird.RevitSamples;

/// <summary>
/// A simple custom-drawn flat progress bar.
/// </summary>
internal sealed class ModernProgressBar : Control
{
    public int Maximum { get; set; } = 100;
    private int _value;
    public int Value { 
        get => _value;
        set { _value = value; Invalidate(); }
    } 
    public Color AccentColor { get; set; } = Color.FromArgb(0, 120, 212);

    public ModernProgressBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint, true);

        Height = 14;
        BackColor = Color.FromArgb(235, 235, 235);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.Clear(BackColor);

        var rect = ClientRectangle;
        rect.Inflate(-1, -1);

        // Background track
        using (var track = new SolidBrush(Color.FromArgb(235, 235, 235)))
            e.Graphics.FillRectangle(track, rect);

        // Progress
        var max = Math.Max(1, Maximum);
        var v = Math.Max(0, Math.Min(Value, max));
        var w = (int)Math.Round(rect.Width * (v / (double)max));
        if (w > 0)
        {
            var prog = new Rectangle(rect.X, rect.Y, w, rect.Height);
            using var brush = new SolidBrush(AccentColor);
            e.Graphics.FillRectangle(brush, prog);
        }

        // Subtle border
        using var pen = new Pen(Color.FromArgb(210, 210, 210));
        e.Graphics.DrawRectangle(pen, rect);
    }
}