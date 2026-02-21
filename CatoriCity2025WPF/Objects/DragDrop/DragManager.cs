using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

public static class DragManager
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
        var target = HitTestForDropTarget(mouse);

        if (target != null &&
            _dropHandlers.TryGetValue(target.GetType(), out var handler))
        {
            handler.HandleDrop(_dragged, target);
        }

        _dragged = null;
        _canvas = null;
    }


    private static UIElement HitTestForDropTarget(Point position)
    {
        UIElement found = null;

        VisualTreeHelper.HitTest(
            _canvas,
            // Filter: skip the dragged element
            (DependencyObject obj) =>
            {
                if (obj == _dragged)
                    return HitTestFilterBehavior.ContinueSkipSelf;

                return HitTestFilterBehavior.Continue;
            },
            // Result callback
            (HitTestResult result) =>
            {
                var current = result.VisualHit;

                while (current != null)
                {
                    if (current is UIElement ui &&
                        _dropHandlers.ContainsKey(ui.GetType()))
                    {
                        found = ui;
                        return HitTestResultBehavior.Stop;
                    }

                    current = VisualTreeHelper.GetParent(current);
                }

                return HitTestResultBehavior.Continue;
            },
            new PointHitTestParameters(position)
        );

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
