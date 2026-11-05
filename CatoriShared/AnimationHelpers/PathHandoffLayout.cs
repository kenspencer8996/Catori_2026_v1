using System.Windows;
using System.Windows.Controls;

namespace CatoriShared.AnimationHelpers;

public static class PathHandoffLayout
{
    public static void Apply(ContentControl host, FrameworkElement part, PathAnimationOptions options)
    {
        if (!options.PreserveControlSizeOnHandoff)
        {
            return;
        }
        double width = double.IsFinite(part.Width) && part.Width > 0 ? part.Width : part.ActualWidth;
        double height = double.IsFinite(part.Height) && part.Height > 0 ? part.Height : part.ActualHeight;
        if (!double.IsFinite(width) || width <= 0 || !double.IsFinite(height) || height <= 0)
        {
            throw new InvalidOperationException("A transferred control must have valid dimensions before preserving its path size.");
        }
        host.Width = width;
        host.Height = height;
        Point anchor = options.ControlAnchorOnPath ?? new Point(0.5, 0.5);
        Canvas.SetLeft(host, options.CenterOnPath ? -width * anchor.X : 0);
        Canvas.SetTop(host, options.CenterOnPath ? -height * anchor.Y : 0);
    }
}
