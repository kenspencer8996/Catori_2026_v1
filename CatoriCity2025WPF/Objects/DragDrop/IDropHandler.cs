using System.Windows;

public interface IDropHandler
{
    void HandleDrop(UIElement dragged, UIElement target);
}
