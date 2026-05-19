using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CatoriApp.Objects.DragDrop;

public class DebugOverlay
{
    private readonly Canvas _canvas;
    private readonly Canvas _overlay;

    private readonly Dictionary<IDropTarget, Rectangle> _targetRects = new();
    private readonly Dictionary<IDropTarget, Ellipse> _snapPoints = new();

    private Rectangle _dragRect;

    public DebugOverlay(Canvas canvas)
    {
        _canvas = canvas;

        _overlay = new Canvas
        {
            IsHitTestVisible = false,
            Background = Brushes.Transparent
        };

        Panel.SetZIndex(_overlay, 9999);
        _canvas.Children.Add(_overlay);

        _dragRect = CreateRect(Brushes.LimeGreen);
        _overlay.Children.Add(_dragRect);
    }

    private Rectangle CreateRect(Brush brush)
    {
        return new Rectangle
        {
            Stroke = brush,
            StrokeThickness = 2,
            Fill = Brushes.Transparent
        };
    }

    private Ellipse CreatePoint(Brush brush)
    {
        return new Ellipse
        {
            Width = 10,
            Height = 10,
            Fill = brush
        };
    }

    // ---------------------------------------------------------
    // REGISTER TARGETS
    // ---------------------------------------------------------
    public void RegisterTarget(IDropTarget target)
    {
        if (target is FrameworkElement fe)
        {
            var rect = CreateRect(Brushes.Cyan);
            _targetRects[target] = rect;
            _overlay.Children.Add(rect);

            var snap = CreatePoint(Brushes.Yellow);
            _snapPoints[target] = snap;
            _overlay.Children.Add(snap);
        }
    }

    // ---------------------------------------------------------
    // UPDATE DRAGGED ELEMENT BOUNDS
    // ---------------------------------------------------------
    public void UpdateDragged(UIElement dragged)
    {
        if (dragged is FrameworkElement fe)
        {
            double x = Canvas.GetLeft(fe);
            double y = Canvas.GetTop(fe);

            Canvas.SetLeft(_dragRect, x);
            Canvas.SetTop(_dragRect, y);

            _dragRect.Width = fe.ActualWidth;
            _dragRect.Height = fe.ActualHeight;
        }
    }

    // ---------------------------------------------------------
    // UPDATE TARGET BOUNDS + SNAP POINTS
    // ---------------------------------------------------------
    public void UpdateTargets(IEnumerable<IDropTarget> targets)
    {
        foreach (var target in targets)
        {
            if (target is FrameworkElement fe)
            {
                double x = Canvas.GetLeft(fe);
                double y = Canvas.GetTop(fe);

                var rect = _targetRects[target];
                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
                rect.Width = fe.ActualWidth;
                rect.Height = fe.ActualHeight;

                // Snap point
                var snap = target.GetSnapPoint(null);
                var dot = _snapPoints[target];
                Canvas.SetLeft(dot, snap.X - 5);
                Canvas.SetTop(dot, snap.Y - 5);
            }
        }
    }

    // ---------------------------------------------------------
    // HIGHLIGHT ACTIVE TARGET
    // ---------------------------------------------------------
    public void SetActiveTarget(IDropTarget target)
    {
        foreach (var kv in _targetRects)
        {
            kv.Value.Stroke = kv.Key == target ? Brushes.Red : Brushes.Cyan;
        }
    }
}

