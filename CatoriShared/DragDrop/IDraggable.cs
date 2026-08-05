namespace CatoriApp.Core.Objects.DragDrop
{
    public interface IDraggable
    {
        public UIElement Visual { get; }
        public Point OriginalPosition { get; }
        public bool IsDragEnabled => true;

        // ? New optional callback
        void OnDragMouseup();
    }
}

