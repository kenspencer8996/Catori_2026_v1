using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CatoriUCLibrary.Objects.PersonItems;

namespace CatoriUCLibrary.Views;

public partial class ItemSceneSurface : UserControl
{
    public System.Windows.Media.Geometry? ShelfDropGeometry { get; set; }
    public PersonItemInventory Inventory { get; } = new();
    public ObservableCollection<SceneDroppedItem> Items { get; } = new();

    public ItemSceneSurface()
    {
        InitializeComponent();
        Inventory.Items.CollectionChanged += (_, _) => RemoveTransferredItems();
    }

    private void RemoveTransferredItems()
    {
        foreach (var entry in Items.Where(entry => !Inventory.Items.Contains(entry.Item)).ToList())
        {
            Items.Remove(entry);
        }
    }

    public bool TryReceive(PersonItem item, Point position, Size size, Point? pointerAnchor = null)
    {
        if (!Inventory.CanReceive(item))
        {
            return false;
        }
        bool onShelf = ShelfDropGeometry?.FillContains(position) == true;
        size = ItemDisplaySizing.Resolve(item, size, onShelf);
        Point anchor = pointerAnchor ?? new Point(.5, .5);
        double width = double.IsFinite(size.Width) && size.Width > 0 ? size.Width : 48;
        double height = double.IsFinite(size.Height) && size.Height > 0 ? size.Height : 64;
        if (!ItemPurchase.TryAcquire(item, this))
        {
            return false;
        }
        Inventory.Receive(item);
        foreach (var previous in Items.Where(entry => ReferenceEquals(entry.Item, item)).ToList())
        {
            Items.Remove(previous);
        }
        Items.Add(new SceneDroppedItem
        {
            Item = item,
            X = Math.Clamp(position.X - width * anchor.X, 0, Math.Max(0, ActualWidth - width)),
            Y = Math.Clamp(position.Y - height * anchor.Y, 0, Math.Max(0, ActualHeight - height)),
            Width = width,
            Height = height
        });
        return true;
    }
}
