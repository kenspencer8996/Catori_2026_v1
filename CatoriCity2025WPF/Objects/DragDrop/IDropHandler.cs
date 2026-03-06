namespace CatoriCity2025WPF.Objects.DragDrop
{
    public interface IDropHandler
    {
        public bool CanDrop(object drag, object target);
        public void OnDrop(object drag, object target);
    }
}
