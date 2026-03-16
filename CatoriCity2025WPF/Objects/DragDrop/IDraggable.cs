namespace CatoriCity2025WPF.Objects.DragDrop
{
    public interface IDraggable
    {
        public UIElement Visual { get; }
        public Point OriginalPosition { get; }
    }
}
