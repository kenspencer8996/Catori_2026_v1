using System.Windows;
using CatoriUCLibrary.Objects.PersonItems;

namespace CatoriUCLibrary.Views;

public partial class PersonUC
{
    public PersonItemInventory CarriedItems { get; } = new();
    public event EventHandler? CarriedItemsChanged;
    private void InitializeCarriedItems()
    {
        CarriedItems.Items.CollectionChanged += (_, _) => CarriedItemsChanged?.Invoke(this, EventArgs.Empty);
    }
    public void HoldItem(PersonItem item)
    {
        Dispatcher.VerifyAccess();
        if (ItemPurchase.TryAcquire(item, this))
        {
            CarriedItems.Receive(item);
        }
    }
}
