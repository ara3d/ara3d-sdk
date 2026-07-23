using Point = System.Windows.Point;

namespace Ara3D.Utils.Wpf.Tests;

/// <summary>
/// Pins the drag-to-scrub orientation: WPF's Y axis grows downward, so dragging up
/// must increase the value (studio-199).
/// </summary>
public static class LabeledFloatDragDirectionTests
{
    private const float PixelToAmount = 0.1f;

    private static float Delta(double startY, double nowY)
        => LabeledFloatUserControl.PixelsToValueDelta(new Point(0, startY), new Point(0, nowY), PixelToAmount);

    [Test]
    public static void DraggingUpIncreasesTheValue()
        => Assert.That(100f + Delta(100, 50), Is.GreaterThan(100f));

    [Test]
    public static void DraggingDownDecreasesTheValue()
        => Assert.That(100f + Delta(100, 150), Is.LessThan(100f));

    [Test]
    public static void MagnitudeScalesWithPixelDistance()
        => Assert.That(Delta(100, 50), Is.EqualTo(50 * PixelToAmount).Within(1e-5));

    [Test]
    public static void NoMovementIsNoChange()
        => Assert.That(Delta(100, 100), Is.EqualTo(0f));
}
