using CatoriCity2025WPF.Objects.DragDrop;
using System.Diagnostics;
using System.Windows.Input;
using System.Xml.Linq;

public class DragManager2
{
    private readonly Canvas _canvas;
    private PhysicsDragController _physics;

    private List<IDropTarget> _dropTargets = new();

    private IDraggable _dragged;
    private Point _grabOffset;
    double _draggedelementWidth;
    double _draggedelementheight;

    public DragManager2(Canvas canvas)
    {
        _canvas = canvas;
        cLogger.Log("DragManager2 initialized with canvas: " + canvas.Name);
        _canvas.MouseLeftButtonDown += OnMouseDown;
        _canvas.MouseMove += OnMouseMove;
        _canvas.MouseLeftButtonUp += OnMouseUp;

        CreatePhysicsController();
    }
    private void CreatePhysicsController()
    {
        cLogger.Log("Creating PhysicsDragController for DragManager2.");
        _physics = new PhysicsDragController(_canvas, SetElementPosition);
    }
    public void ResetDragState() 
    { 
        cLogger.Log("Resetting drag state in DragManager2.");
        CreatePhysicsController();
        _dragged = null; 
        _grabOffset = new Point();
       
    }
    // ---------------------------------------------------------
    // RegisterDropTarget
    // ---------------------------------------------------------
    public void RegisterDropTarget(IDropTarget target)
    {
        cLogger.Log("Registering drop target: " + target.GetType().Name);
        if (!_dropTargets.Contains(target))
            _dropTargets.Add(target);

    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
       cLogger.Log("Mouse down at position: " + e.GetPosition(_canvas));
        if (e.Source is UIElement element && _canvas.Children.Contains(element))
        {
            cLogger.Log("Starting drag for element: " + element.GetType().Name);
            _dragged = element as IDraggable;
            var uiitem = _dragged as UIElement;
            var mouse = e.GetPosition(_canvas);
            var left = Canvas.GetLeft(uiitem);
            var top = Canvas.GetTop(uiitem);

            _grabOffset = new Point(mouse.X - left, mouse.Y - top);

            //_physics.Start(new Point(left, top));
            _physics.Start(new Point(left,top));
            //_physics.Start();
            _canvas.CaptureMouse();
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_dragged == null) return;
        cLogger.Log("Mouse move at position: " + e.GetPosition(_canvas));
        var mouse = e.GetPosition(_canvas);
        var target = new Point(mouse.X - _grabOffset.X, mouse.Y - _grabOffset.Y);
        target = ClampToCanvas(target);
        _physics.SetTarget(target);
    }
    private Point ClampToCanvas(Point p)
    {
        cLogger.Log("Clamping position to canvas: " + p);
        double x = Math.Max(0, Math.Min(_canvas.ActualWidth - _draggedelementWidth, p.X));
        double y = Math.Max(0, Math.Min(_canvas.ActualHeight - _draggedelementheight, p.Y));
        return new Point(x, y);
    }
    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_dragged == null) return;
        cLogger.Log("Mouse up at position: " + e.GetPosition(_canvas) + " for element: " + _dragged.GetType().Name);
        _physics.Stop();
        _canvas.ReleaseMouseCapture();

        TryDrop(_dragged);
        _dragged = null;
        ResetDragState();
    }

    private void SetElementPosition(Point p)
    {
        cLogger.Log("Setting element position to: " + p);
        if (_dragged == null) return;
        var uiitem = _dragged as UIElement;

        Canvas.SetLeft(uiitem, p.X);
        Canvas.SetTop(uiitem, p.Y);
    }

    private void TryDrop(IDraggable element)
    {
        cLogger.Log("Trying to drop element: " + element.GetType().Name);
        var fe = (FrameworkElement)element;
        var uiitem = element as UIElement;
        var elementBounds = new Rect(
            Canvas.GetLeft(uiitem),
            Canvas.GetTop(uiitem),
            fe.ActualWidth,
            fe.ActualHeight);
        cLogger.Log($"Target bounds: {fe.ActualWidth} x {fe.ActualHeight}");
        foreach (var target in _dropTargets)
        {
            cLogger.Log("Checking drop target: " + target.GetType().Name);
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
