public class DragManager
{
    private readonly List<IDropTarget> _targets = new();
    private object _dragged;
    private IDropTarget _currentHover;

    public void RegisterDropTarget(IDropTarget target) => _targets.Add(target);

    public void StartDrag(object draggable)
    {
        _dragged = draggable;
        _currentHover = null;
    }

    public void UpdateDrag(Point cursor)
    {
        if (_dragged == null)
            return;

        IDropTarget newHover = HitTest(cursor);

        if (newHover != _currentHover)
        {
            // Leaving old target
            _currentHover?.OnHoverLeave(_dragged);

            // Entering new target
            newHover?.OnHoverEnter(_dragged);

            _currentHover = newHover;
        }
    }

    public void EndDrag(Point cursor)
    {
        if (_dragged == null)
            return;

        var target = HitTest(cursor);

        if (target != null && target.CanDrop(_dragged))
            target.OnDrop(_dragged);

        // Clear hover state
        _currentHover?.OnHoverLeave(_dragged);
        _currentHover = null;
        _dragged = null;
    }

    private IDropTarget HitTest(Point cursor)
    {
        foreach (var t in _targets)
        {
            if (t is IHitTestable hit && hit.Contains(cursor))
                return t;
        }
        return null;
    }
}
