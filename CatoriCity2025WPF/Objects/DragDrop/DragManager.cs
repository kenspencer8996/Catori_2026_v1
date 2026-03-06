using CatoriCity2025WPF.Objects.DragDrop;
using System.Diagnostics;
using System.Windows.Input;

public class DragManager
{
    private readonly Canvas _canvas;
    private PhysicsDragController _physics;

    private List<IDropTarget> _dropTargets = new();

    private UIElement _dragged;
    private Point _grabOffset;

    public DragManager(Canvas canvas)
    {
        _canvas = canvas;

        _canvas.MouseLeftButtonDown += OnMouseDown;
        _canvas.MouseMove += OnMouseMove;
        _canvas.MouseLeftButtonUp += OnMouseUp;

        _physics = new PhysicsDragController(_canvas, SetElementPosition);
    }
    private void CreatePhysicsController()
    {
        _physics = new PhysicsDragController(_canvas, SetElementPosition);
    }
    public void ResetDragState() 
    { 
        CreatePhysicsController();
        _dragged = null; 
        _grabOffset = new Point();
       
    }
    // ---------------------------------------------------------
    // RegisterDropTarget
    // ---------------------------------------------------------
    public void RegisterDropTarget(IDropTarget target)
    {
        if (!_dropTargets.Contains(target))
            _dropTargets.Add(target);
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is UIElement element && _canvas.Children.Contains(element))
        {
            _dragged = element;

            var mouse = e.GetPosition(_canvas);
            var left = Canvas.GetLeft(_dragged);
            var top = Canvas.GetTop(_dragged);

            _grabOffset = new Point(mouse.X - left, mouse.Y - top);

            //_physics.Start(new Point(left, top));
            _physics.Start(new Point(left,top));
            _canvas.CaptureMouse();
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_dragged == null) return;

        var mouse = e.GetPosition(_canvas);
        var target = new Point(mouse.X - _grabOffset.X, mouse.Y - _grabOffset.Y);

        _physics.SetTarget(target);
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_dragged == null) return;

        _physics.Stop();
        _canvas.ReleaseMouseCapture();

        TryDrop(_dragged);
        _dragged = null;
        ResetDragState();
    }

    private void SetElementPosition(Point p)
    {
        if (_dragged == null) return;

        Canvas.SetLeft(_dragged, p.X);
        Canvas.SetTop(_dragged, p.Y);
    }

    private void TryDrop(UIElement element)
    {
        var fe = (FrameworkElement)element;

        var elementBounds = new Rect(
            Canvas.GetLeft(element),
            Canvas.GetTop(element),
            fe.ActualWidth,
            fe.ActualHeight);

        foreach (var target in _dropTargets)
        {
            if (target is FrameworkElement feTarget)
            {
                var targetBounds = new Rect(
                    Canvas.GetLeft(feTarget),
                    Canvas.GetTop(feTarget),
                    feTarget.ActualWidth,
                    feTarget.ActualHeight);
                Debug.WriteLine($"Element={elementBounds} Target={targetBounds}");
                if (elementBounds.IntersectsWith(targetBounds) &&
                    target.CanDrop(element))
                {
                    target.OnDrop(element);
                    return;
                }
            }
        }

    }
}
