using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace CatoriUCLibrary.Views;

public partial class ItemDragPreviewWindow : Window
{
    private readonly FrameworkElement source;

    public ItemDragPreviewWindow(ImageSource? image, FrameworkElement source)
    {
        InitializeComponent();
        this.source = source;
        DraggedItemImage.Source = image;
        var transform = PresentationSource.FromVisual(source)?.CompositionTarget?.TransformFromDevice ?? Matrix.Identity;
        Point top = transform.Transform(source.PointToScreen(new Point()));
        Point bottom = transform.Transform(source.PointToScreen(new Point(source.ActualWidth, source.ActualHeight)));
        Width = Math.Max(1, bottom.X - top.X);
        Height = Math.Max(1, bottom.Y - top.Y);
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
            Left = position.X + 12;
            Top = position.Y + 12;
        }
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out ItemCursorPoint point);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW")]
    private static extern int GetWindowLong(IntPtr handle, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW")]
    private static extern int SetWindowLong(IntPtr handle, int index, int value);
}
