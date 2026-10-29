using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace CatoriUCLibrary.Views;

public partial class ItemDragPreviewWindow : Window
{
    private readonly FrameworkElement source;
    private readonly Point anchor;

    public ItemDragPreviewWindow(ImageSource? image, FrameworkElement source, Size? displaySize = null, Point? pointerAnchor = null)
    {
        InitializeComponent();
        this.source = source;
        anchor = pointerAnchor ?? new Point(.5, .5);
        DraggedItemImage.Source = image;
        var transform = PresentationSource.FromVisual(source)?.CompositionTarget?.TransformFromDevice ?? Matrix.Identity;
        Point top = transform.Transform(source.PointToScreen(new Point()));
        Point bottom = transform.Transform(source.PointToScreen(new Point(source.ActualWidth, source.ActualHeight)));
        Width = Math.Max(1, bottom.X - top.X) * (displaySize?.Width ?? source.ActualWidth) / Math.Max(1, source.ActualWidth);
        Height = Math.Max(1, bottom.Y - top.Y) * (displaySize?.Height ?? source.ActualHeight) / Math.Max(1, source.ActualHeight);
        SourceInitialized += (_, _) =>
        {
            var handle = new WindowInteropHelper(this).Handle;
            SetWindowLong(handle, -20, GetWindowLong(handle, -20) | 0x20 | 0x08000000);
        };
    }

    public void MoveToCursor()
    {
        if (GetCursorPos(out ItemCursorPoint point))
        {
            var transform = PresentationSource.FromVisual(source)?.CompositionTarget?.TransformFromDevice ?? Matrix.Identity;
            Point position = transform.Transform(new Point(point.X, point.Y));
            Left = position.X - Width * anchor.X;
            Top = position.Y - Height * anchor.Y;
        }
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out ItemCursorPoint point);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW")]
    private static extern int GetWindowLong(IntPtr handle, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW")]
    private static extern int SetWindowLong(IntPtr handle, int index, int value);
}
