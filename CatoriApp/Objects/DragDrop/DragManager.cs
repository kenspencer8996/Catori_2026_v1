using CatoriApp.Objects.DragDrop;
using System.Windows.Input;

public class DragManager
{
    private readonly Canvas _canvas;
    private readonly Canvas _root;
    private DebugOverlay _debug;

    private PhysicsDragController _physics;

    private readonly List<IDropTarget> _dropTargets = new();

    private IDraggable _currentDraggable;
    private UIElement _dragged;

    private bool _mouseDown;
    private bool _isDragging;
    private Point _mouseDownPos;
    private Vector _grabOffset;

    private IDropTarget _activeTarget;
    private IDropTarget _previousTarget;
    private bool _debugvisualizer = false;
    private const double DragThreshold = 4;

    public DragManager(Canvas canvas)
    {
        _canvas = canvas;
        _root = canvas;
        _debug = new DebugOverlay(canvas);

        _canvas.MouseLeftButtonDown += OnMouseDown;
        _canvas.MouseMove += OnMouseMove;
        _canvas.MouseLeftButtonUp += OnMouseUp;

        CreatePhysicsController();
    }

    private void CreatePhysicsController()
    {
        _physics = new PhysicsDragController(_canvas, SetElementPosition);
    }

    public void RegisterDropTarget(IDropTarget target)
    {
        if (!_dropTargets.Contains(target))
            _dropTargets.Add(target);
        _debug.RegisterTarget(target);
    }

    // ---------------------------------------------------------
    // MOUSE DOWN
    // ---------------------------------------------------------
    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is IDraggable draggable &&
            draggable.Visual is UIElement element &&
            _canvas.Children.Contains(element))
        {
            _currentDraggable = draggable;
            _dragged = draggable.Visual;

            _mouseDown = true;
            _isDragging = false;

            _mouseDownPos = e.GetPosition(_root);

            var elementPos = _dragged.TranslatePoint(new Point(0, 0), _root);
            _grabOffset = _mouseDownPos - elementPos;

            _activeTarget = null;
            _previousTarget = null;
        }
    }

    // ---------------------------------------------------------
    // MOUSE MOVE
    // ---------------------------------------------------------
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_dragged == null || !_mouseDown)
            return;

        var mouse = e.GetPosition(_root);

        if (!_isDragging)
        {
            double dx = mouse.X - _mouseDownPos.X;
            double dy = mouse.Y - _mouseDownPos.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            if (dist < DragThreshold)
                return;

            // ⭐ Start dragging
            _isDragging = true;

            var left = Canvas.GetLeft(_dragged);
            var top = Canvas.GetTop(_dragged);

            _physics.Start(new Point(left, top));
            _canvas.CaptureMouse();
        }

        // ⭐ Update physics target
        var target = new Point(mouse.X - _grabOffset.X, mouse.Y - _grabOffset.Y);
        target = ClampToCanvas(target);
        _physics.SetTarget(target);
    }

    // ---------------------------------------------------------
    // MOUSE UP
    // ---------------------------------------------------------
    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (!_mouseDown)
            return;

        _mouseDown = false;

        bool wasDragging = _isDragging;
        _isDragging = false;
        _currentDraggable.OnDragMouseup();

        _canvas.ReleaseMouseCapture();
        _physics.Stop();

        if (wasDragging && _currentDraggable != null && _activeTarget != null)
        {
            if (_activeTarget.CanDrop(_currentDraggable))
            {
                SnapToTarget(_currentDraggable, _activeTarget);
                _activeTarget.OnDrop(_currentDraggable);
            }
            else
            {
                (_activeTarget as dynamic)?.RejectDrop(_currentDraggable);
            }
        }

        ClearHighlight();
        ResetDragState();
    }


  

    // ---------------------------------------------------------
    // SNAP TO TARGET (Option 3)
    // ---------------------------------------------------------
    private void SnapToTarget(IDraggable draggable, IDropTarget target)
    {
        Point snap = target.GetSnapPoint(draggable);

        Canvas.SetLeft(draggable.Visual, snap.X);
        Canvas.SetTop(draggable.Visual, snap.Y);
    }

    // ---------------------------------------------------------
    // RESET STATE
    // ---------------------------------------------------------
    private void ResetDragState()
    {
        CreatePhysicsController();
        _currentDraggable = null;
        _dragged = null;
        _activeTarget = null;
        _previousTarget = null;
        _mouseDown = false;
        _isDragging = false;
    }

    // ---------------------------------------------------------
    // CLAMP
    // ---------------------------------------------------------
    private Point ClampToCanvas(Point p)
    {
        if (_dragged is FrameworkElement fe)
        {
            double w = fe.ActualWidth;
            double h = fe.ActualHeight;

            double x = Math.Max(0, Math.Min(_canvas.ActualWidth - w, p.X));
            double y = Math.Max(0, Math.Min(_canvas.ActualHeight - h, p.Y));

            return new Point(x, y);
        }

        return p;
    }

    // ---------------------------------------------------------
    // PHYSICS CALLBACK
    // ---------------------------------------------------------
    private void SetElementPosition(Point p)
    {
        if (_dragged == null) return;

        Canvas.SetLeft(_dragged, p.X);
        Canvas.SetTop(_dragged, p.Y);

        UpdateActiveTarget();

        if (_debugvisualizer)
        {
            _debug.UpdateDragged(_dragged);
            _debug.UpdateTargets(_dropTargets);
            _debug.SetActiveTarget(_activeTarget);
        }

    }

    // ---------------------------------------------------------
    // ACTIVE TARGET DETECTION
    // ---------------------------------------------------------
    private void UpdateActiveTarget()
    {
        if (_dragged == null) return;

        var fe = (FrameworkElement)_dragged;

        var elementBounds = new Rect(
            Canvas.GetLeft(_dragged),
            Canvas.GetTop(_dragged),
            fe.ActualWidth,
            fe.ActualHeight);

        IDropTarget newTarget = null;

        foreach (var target in _dropTargets)
        {
            if (target is FrameworkElement feTarget)
            {
                var targetBounds = new Rect(
                    Canvas.GetLeft(feTarget),
                    Canvas.GetTop(feTarget),
                    feTarget.ActualWidth,
                    feTarget.ActualHeight);

                if (elementBounds.IntersectsWith(targetBounds))
                {
                    newTarget = target;

                    break;
                }
            }
        }

        if (newTarget != _previousTarget)
        {
            _previousTarget?.HighlightOff();
            newTarget?.HighlightOn();
            _previousTarget = newTarget;
        }

        _activeTarget = newTarget;
    }

    private void ClearHighlight()
    {
        _previousTarget?.HighlightOff();
        _previousTarget = null;
    }
}

