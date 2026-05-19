namespace CatoriApp.Objects.DragDrop
{
    public interface IDropTarget
    {
        public bool CanDrop(IDraggable element);
        public void OnDrop(IDraggable element);
        public void HighlightOn();
        public void HighlightOff();
        public Point GetSnapPoint(IDraggable dragged);
    }
}

