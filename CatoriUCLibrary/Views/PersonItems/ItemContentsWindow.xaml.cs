using System.Windows;
using CatoriUCLibrary.Objects.PersonItems;

namespace CatoriUCLibrary.Views;

public partial class ItemContentsWindow : Window
{
    private readonly PersonItem _container;
    public ItemContentsWindow(PersonItem container)
    {
        InitializeComponent();
        _container = container;
        DataContext = container;
        Title = container.ItemName + " contents";
    }
    private void Contents_DragOver(object sender, DragEventArgs e)
    {
        PersonItem? item = PersonItemDrag.Read(e.Data);
        e.Effects = item != null && _container.Contents.CanReceive(item) ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }
    private void Contents_Drop(object sender, DragEventArgs e)
    {
        PersonItem? item = PersonItemDrag.Read(e.Data);
        e.Effects = DragDropEffects.None;
        if (item != null && _container.Contents.CanReceive(item))
        {
            _container.Contents.Receive(item);
            e.Effects = DragDropEffects.Move;
        }
        e.Handled = true;
    }
}

