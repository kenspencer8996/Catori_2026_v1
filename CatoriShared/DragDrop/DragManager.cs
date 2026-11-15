using CatoriApp.Core.Objects.DragDrop;
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

    private bool _dropConsumed;
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

        _canvas.AddHandler(UIElement.MouseLeftButtonDownEvent,
            new MouseButtonEventHandler(OnMouseDown), true);
        _canvas.AddHandler(UIElement.MouseMoveEvent,
            new MouseEventHandler(OnMouseMove), true);
        _canvas.AddHandler(UIElement.MouseLeftButtonUpEvent,
            new MouseButtonEventHandler(OnMouseUp), true);

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

    public void ConsumeCurrentDrop()
    {
        _dropConsumed = true;
        ClearHighlight();
        _activeTarget = null;
    }

    public void ClearDropTargets()
    {
        ClearHighlight();
        _activeTarget = null;
        _dropTargets.Clear();
    }

    // ---------------------------------------------------------
    // MOUSE DOWN
    // ---------------------------------------------------------
    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DependencyObject? originalSource = e.OriginalSource as DependencyObject;
        IDraggable? draggable = FindDraggable(originalSource);
        if (draggable?.IsDragEnabled == true &&
            (draggable is not IDragStartFilter filter || filter.CanStartDrag(originalSource)) &&
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

    private static IDraggable? FindDraggable(DependencyObject? source)
    {
        DependencyObject? current = source;
        while (current != null)
        {
            if (current is IDraggable draggable)
                return draggable;

            current = current is Visual or Visual3D
                ? VisualTreeHelper.GetParent(current)
                : LogicalTreeHelper.GetParent(current);
        }

        return null;
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

            // ? Start dragging
            _isDragging = true;
            _currentDraggable.OnDragStarted();

            var left = Canvas.GetLeft(_dragged);
            var top = Canvas.GetTop(_dragged);

            _physics.Start(new Point(left, top));
            _canvas.CaptureMouse();
        }

        // ? Update physics target
        var target = new Point(mouse.X - _grabOffset.X, mouse.Y - _grabOffset.Y);
        target = ClampToCanvas(target);
        _physics.SetTarget(target);
    }

    // ---------------------------------------------------------
    // MOUSE UP
    // ---------------------------------------------------------
    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        CompleteDrag(e.GetPosition(_root));
    }

    private void CompleteDrag(Point mousePosition)
    {
        if (!_mouseDown)
        {
            return;
        }
        bool wasDragging = _isDragging;
        _mouseDown = false;
        _isDragging = false;
        _physics.Stop();
        if (wasDragging && _dragged is not null)
        {
            SetElementPosition(ClampToCanvas(new Point(mousePosition.X - _grabOffset.X, mousePosition.Y - _grabOffset.Y)));
        }
        _canvas.ReleaseMouseCapture();
        try
        {
            _currentDraggable?.OnDragMouseup();
            if (wasDragging && !_dropConsumed && _currentDraggable is not null
                && _currentDraggable.Visual is FrameworkElement element && element.IsVisible && element.IsDescendantOf(_canvas)
                && _activeTarget is FrameworkElement target && target.IsVisible && target.IsDescendantOf(_canvas)
                && _activeTarget.CanDrop(_currentDraggable))
            {
                SnapToTarget(_currentDraggable, _activeTarget);
                _activeTarget.OnDrop(_currentDraggable);
            }
        }
        finally
        {
            ClearHighlight();
            ResetDragState();
        }
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
        _dropConsumed = false;
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
            if (_dragged is ICanvasDragAnchor anchored)
            {
                double anchoredX = Math.Max(-anchored.DragAnchorX,
                    Math.Min(_canvas.ActualWidth - anchored.DragAnchorX,p.X));
                double anchoredY = Math.Max(-anchored.DragAnchorY,
                    Math.Min(_canvas.ActualHeight - anchored.DragAnchorY,p.Y));
                return new Point(anchoredX,anchoredY);
            }

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
            if (target is FrameworkElement feTarget && feTarget.IsVisible && feTarget.IsDescendantOf(_canvas)
                && _currentDraggable is not null && target.CanDrop(_currentDraggable))
            {
                var targetBounds = feTarget.TransformToAncestor(_canvas)
                    .TransformBounds(new Rect(0, 0, feTarget.ActualWidth, feTarget.ActualHeight));

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

/// <summary>
/// Allows a large, mostly transparent draggable control to be constrained by
/// its meaningful physical anchor instead of its full layout rectangle.
/// </summary>
public interface ICanvasDragAnchor
{
    double DragAnchorX { get; }
    double DragAnchorY { get; }
}

/// <summary>
/// Lets a draggable control reserve interactive children, such as an arm or
/// button, without disabling dragging from the rest of the control.
/// </summary>
public interface IDragStartFilter
{
    bool CanStartDrag(DependencyObject? originalSource);
}

