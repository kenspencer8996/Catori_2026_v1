namespace CatoriCity2025WPF.Objects.DragDrop
{
    public interface IDropTarget
    {
        public bool CanDrop(UIElement element);
        public void OnDrop(UIElement element);
    }

}
