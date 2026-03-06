using CatoriCity2025WPF.Objects.DragDrop;
using System.Windows.Input;
using System.Xml.Linq;

public static class DragManager_original
{
    private static UIElement _dragged;
    private static Canvas _canvas;

    private static PhysicsDragController _physics;
    private static Point _clickOffset;

    private static readonly Dictionary<Type, IDropHandler> _dropHandlers = new();

    public static void RegisterDropHandler<T>(IDropHandler handler) where T : UIElement
        => _dropHandlers[typeof(T)] = handler;

    public static void StartDrag(UIElement element, Canvas canvas, Point clickOffset)
    {
        _dragged = element;
        _canvas = canvas;
        _clickOffset = clickOffset;

        var mouse = Mouse.GetPosition(_canvas);

        var startPos = new Point(
            mouse.X - _clickOffset.X,
            mouse.Y - _clickOffset.Y
        );

        _physics = new PhysicsDragController(p =>
        {
            Canvas.SetLeft(_dragged, p.X);
            Canvas.SetTop(_dragged, p.Y);
        });

        _physics.Start(startPos);
    }

    public static void UpdateDrag()
    {
        if (_dragged == null || _canvas == null)
            return;

        var mouse = Mouse.GetPosition(_canvas);

        var target = new Point(
            mouse.X - _clickOffset.X,
            mouse.Y - _clickOffset.Y
        );

        _physics.SetTarget(target);
    }
    public static void EndDrag()
    {
        if (_dragged == null || _canvas == null)
            return;

        _physics?.Stop();
        _physics = null;

        var mouse = Mouse.GetPosition(_canvas);

        // NEW: correct drop target detection
        //var target = HitTestForDropTarget(mouse);
        var directTarget = GetControlUnderMouse(mouse);
        if (directTarget != null &&
            _dropHandlers.TryGetValue(directTarget.GetType(), out var handler))
        {
            handler.OnDrop(_dragged, directTarget);
        }

        _dragged = null;
        _canvas = null;
    }
    private static  UIElement GetControlUnderMouse(Point position)
    {
        UIElement uIElement = null;
        var element = Mouse.DirectlyOver as DependencyObject;
        
        uIElement = HitTestForDropTarget(position);
        return uIElement;
    }
  
    private static bool IsGoodChild(DependencyObject obj)
    {
        bool isGood = false;
        string typeName = obj.GetType().ToString().ToLower();
        if (typeName.Contains("canvas"))
            isGood = true; // Skip AdornerLayer
        return isGood;
    }
    private static UIElement HitTestForDropTarget(Point position)
    {
        UIElement found = null;
        HitTestResult hit = VisualTreeHelper.HitTest(_canvas, position);
        DependencyObject current = hit.VisualHit;
        while (current != null && !(current is UserControl))
        {
            current = VisualTreeHelper.GetParent(current);
        }
        if (current is UIElement uiElement)
        {
            found = uiElement;
        }
      

        return found;
    }

    public static T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        while (child != null)
        {
            if (child is T t)
                return t;

            child = VisualTreeHelper.GetParent(child);
        }
        return null;
    }

}
